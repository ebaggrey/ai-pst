import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { Subscription } from 'rxjs';

import { OnboardingAssistantService } from '../../services/onboarding-assistant.service';
import { MentorInfo, OnboardingError, OnboardingFeedback, OnboardingModule, OnboardingPlan, OnboardingProfile, OnboardingProgressUpdate, OnboardingRequest, OnboardingStats, OnboardingTask } from 'src/chapter-2/models/onboarding-assistant-models';


@Component({
  selector: 'app-onboarding-assistant',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule
  ],
  templateUrl: './onboarding-assistant.component.html',
  styleUrls: ['./onboarding-assistant.component.css']
})
export class OnboardingAssistantComponent implements OnInit, OnDestroy {
  // State
  isLoading = false;
  error: OnboardingError | null = null;
  successMessage = '';

  // Data
  profile: OnboardingProfile | null = null;
  plan: OnboardingPlan | null = null;
  tasks: OnboardingTask[] = [];
  modules: OnboardingModule[] = [];
  mentor: MentorInfo | null = null;
  stats: OnboardingStats | null = null;

  // Forms
  createProfileForm: FormGroup;
  updateProgressForm: FormGroup;
  feedbackForm: FormGroup;

  // Selected IDs
  selectedProfileId = '';
  selectedModuleId = '';
  selectedTaskId = '';

  // API Base URL
  apiBaseUrl: string;

  private subscriptions: Subscription;

  constructor(
    private fb: FormBuilder,
    private onboardingService: OnboardingAssistantService
  ) {
    this.apiBaseUrl = (this.onboardingService as any)['apiUrl'] || 'http://localhost:5000/api';
    this.subscriptions = new Subscription();

    // Initialize forms
    this.createProfileForm = this.fb.group({
      userName: ['', [Validators.required, Validators.minLength(2)]],
      userEmail: ['', [Validators.required, Validators.email]],
      role: ['developer', Validators.required],
      experienceLevel: ['beginner', Validators.required],
      department: ['', Validators.required],
      manager: ['', Validators.required],
      startDate: [this.getTodayDate(), Validators.required]
    });

    this.updateProgressForm = this.fb.group({
      profileId: ['', Validators.required],
      moduleId: [''],
      taskId: [''],
      status: ['in-progress', Validators.required],
      score: [0, [Validators.min(0), Validators.max(100)]],
      timeSpentMinutes: [0, [Validators.min(0)]],
      notes: ['']
    });

    this.feedbackForm = this.fb.group({
      profileId: ['', Validators.required],
      moduleId: [''],
      taskId: [''],
      rating: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
      comments: ['', Validators.required],
      suggestions: ['']
    });
  }

  ngOnInit(): void {
    // Optional: Load initial data
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  private getTodayDate(): string {
    const today = new Date();
    return today.toISOString().split('T')[0];
  }

  clearMessages(): void {
    this.error = null;
    this.successMessage = '';
  }

  // ==================== HELPER METHODS ====================

  /**
   * Load all profile data after profile creation
   */
  private loadProfileData(profileId: string): void {
    this.getProfile(profileId);
    this.loadPlan(profileId);
    this.loadTasks(profileId);
  }

  /**
   * Mark all form controls as touched
   */
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.values(formGroup.controls).forEach(control => {
      control.markAsTouched();
      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }

  /**
   * Get CSS class for status badge
   */
  getStatusClass(status: string): string {
    const classes: Record<string, string> = {
      'pending': 'badge-pending',
      'active': 'badge-active',
      'completed': 'badge-completed',
      'in-progress': 'badge-progress',
      'locked': 'badge-locked',
      'available': 'badge-available',
      'draft': 'badge-draft',
      'archived': 'badge-archived'
    };
    return classes[status?.toLowerCase()] || 'badge-default';
  }

  // ==================== API METHODS ====================

  // 1. Create Profile
  createProfile(): void {
    if (this.createProfileForm.invalid) {
      this.markFormGroupTouched(this.createProfileForm);
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const formValue = this.createProfileForm.value;
    const request: OnboardingRequest = {
      userId: `user_${Date.now()}`,
      userName: formValue.userName,
      userEmail: formValue.userEmail,
      role: formValue.role,
      experienceLevel: formValue.experienceLevel,
      department: formValue.department,
      manager: formValue.manager,
      startDate: new Date(formValue.startDate).toISOString(),
      preferences: {
        learningStyle: 'mixed',
        communicationFrequency: 'weekly',
        preferredChannels: ['email', 'slack'],
        timezone: Intl.DateTimeFormat().resolvedOptions().timeZone,
        workHours: {
          start: '09:00',
          end: '17:00',
          timezone: Intl.DateTimeFormat().resolvedOptions().timeZone
        },
        language: 'en',
        notificationsEnabled: true
      }
    };

    this.subscriptions.add(
      this.onboardingService.createOnboardingProfile(request).subscribe({
        next: (response) => {
          // Create a basic profile object from response
          this.profile = {
            id: response.profileId,
            userId: `user_${Date.now()}`,
            userName: formValue.userName,
            userEmail: formValue.userEmail,
            role: formValue.role,
            experienceLevel: formValue.experienceLevel,
            startDate: new Date(formValue.startDate).toISOString(),
            department: formValue.department,
            manager: formValue.manager,
            status: 'active',
            progress: {
              overall: 0,
              modulesCompleted: 0,
              totalModules: response.plan?.modules?.length || 10,
              assessmentsPassed: 0,
              totalAssessments: 5,
              timeSpentMinutes: 0,
              estimatedCompletionDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
              lastActivityDate: new Date().toISOString()
            },
            preferences: request.preferences,
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
          } as OnboardingProfile;
          
          this.plan = response.plan;
          this.selectedProfileId = response.profileId;
          this.isLoading = false;
          this.successMessage = `Profile created for ${formValue.userName}!`;
          this.loadProfileData(response.profileId);
        },
        error: (error) => {
          this.isLoading = false;
          this.error = error;
        }
      })
    );
  }

  // 2. Get Profile
  getProfile(profileId: string): void {
    if (!profileId) return;

    this.isLoading = true;
    this.clearMessages();

    this.subscriptions.add(
      this.onboardingService.getOnboardingProfile(profileId).subscribe({
        next: (profile) => {
          this.profile = profile;
          this.selectedProfileId = profile.id;
          this.isLoading = false;
          this.successMessage = `Profile loaded for ${profile.userName}`;
          this.loadPlan(profile.id);
          this.loadTasks(profile.id);
        },
        error: (error) => {
          this.isLoading = false;
          this.error = error;
        }
      })
    );
  }

  // 3. Get Plan
  loadPlan(profileId: string): void {
    this.subscriptions.add(
      this.onboardingService.getOnboardingPlan(profileId).subscribe({
        next: (plan) => {
          this.plan = plan;
          this.modules = plan.modules || [];
        },
        error: (error) => {
          console.warn('Failed to load plan:', error);
          // Set default empty plan
          this.plan = {
            id: `plan_${profileId}`,
            profileId: profileId,
            name: 'Default Plan',
            description: 'Standard onboarding plan',
            modules: [],
            estimatedTotalMinutes: 0,
            startDate: new Date().toISOString(),
            targetCompletionDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
            milestones: [],
            status: 'active',
            progress: {
              overall: 0,
              modulesCompleted: 0,
              totalModules: 0,
              assessmentsPassed: 0,
              totalAssessments: 0,
              timeSpentMinutes: 0,
              estimatedCompletionDate: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
              lastActivityDate: new Date().toISOString()
            }
          };
        }
      })
    );
  }

  // 4. Get Tasks
  loadTasks(profileId: string, status?: string): void {
    this.subscriptions.add(
      this.onboardingService.getTasks(profileId, status as any).subscribe({
        next: (tasks) => this.tasks = tasks,
        error: (error) => {
          console.warn('Failed to load tasks:', error);
          this.tasks = [];
        }
      })
    );
  }

  // 5. Update Progress
  updateProgress(): void {
    if (this.updateProgressForm.invalid) {
      this.markFormGroupTouched(this.updateProgressForm);
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const formValue = this.updateProgressForm.value;
    const update: OnboardingProgressUpdate = {
      profileId: formValue.profileId,
      moduleId: formValue.moduleId || undefined,
      taskId: formValue.taskId || undefined,
      status: formValue.status,
      score: formValue.score,
      timeSpentMinutes: formValue.timeSpentMinutes,
      notes: formValue.notes
    };

    this.subscriptions.add(
      this.onboardingService.updateProgress(update).subscribe({
        next: (progress) => {
          this.isLoading = false;
          this.successMessage = 'Progress updated!';
          if (this.profile) {
            this.profile.progress = progress;
          }
          this.updateProgressForm.reset({ profileId: this.selectedProfileId, status: 'in-progress' });
        },
        error: (error) => {
          this.isLoading = false;
          this.error = error;
        }
      })
    );
  }

  // 6. Update Task
  updateTaskStatus(taskId: string, status: string): void {
    if (!taskId) return;

    this.isLoading = true;
    this.clearMessages();

    this.subscriptions.add(
      this.onboardingService.updateTaskStatus(taskId, status as any).subscribe({
        next: (task) => {
          this.isLoading = false;
          this.successMessage = `Task marked as ${task.status}`;
          if (this.selectedProfileId) {
            this.loadTasks(this.selectedProfileId);
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.error = error;
        }
      })
    );
  }

  // 7. Get Module
  loadModule(moduleId: string): void {
    if (!moduleId) return;

    this.subscriptions.add(
      this.onboardingService.getModule(moduleId).subscribe({
        next: (module) => {
          this.selectedModuleId = moduleId;
          // Update the module in the modules array
          const index = this.modules.findIndex(m => m.id === moduleId);
          if (index >= 0) {
            this.modules[index] = module;
          } else {
            this.modules.push(module);
          }
          this.successMessage = `Module loaded: ${module.title}`;
        },
        error: (error) => {
          console.warn('Failed to load module:', error);
          this.error = error;
        }
      })
    );
  }

  // 8. Assign Mentor
  assignMentor(profileId: string, mentorId: string = 'mentor_001'): void {
    if (!profileId) return;

    this.isLoading = true;
    this.clearMessages();

    this.subscriptions.add(
      this.onboardingService.assignMentor(profileId, mentorId).subscribe({
        next: (mentor) => {
          this.mentor = mentor;
          this.isLoading = false;
          this.successMessage = `Mentor ${mentor.name} assigned`;
        },
        error: (error) => {
          this.isLoading = false;
          this.error = error;
        }
      })
    );
  }

  // 9. Submit Feedback
  submitFeedback(): void {
    if (this.feedbackForm.invalid) {
      this.markFormGroupTouched(this.feedbackForm);
      return;
    }

    this.isLoading = true;
    this.clearMessages();

    const formValue = this.feedbackForm.value;
    const feedback: OnboardingFeedback = {
      id: `fb_${Date.now()}`,
      profileId: formValue.profileId,
      moduleId: formValue.moduleId || undefined,
      taskId: formValue.taskId || undefined,
      rating: formValue.rating,
      comments: formValue.comments,
      suggestions: formValue.suggestions ? [formValue.suggestions] : [],
      createdAt: new Date().toISOString()
    };

    this.subscriptions.add(
      this.onboardingService.submitFeedback(feedback).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.successMessage = 'Feedback submitted! Thanks!';
          this.feedbackForm.reset({ 
            profileId: this.selectedProfileId, 
            rating: 5,
            comments: '',
            suggestions: ''
          });
        },
        error: (error) => {
          this.isLoading = false;
          this.error = error;
        }
      })
    );
  }

  // 10. Get Stats
  loadStats(department?: string): void {
    this.isLoading = true;

    this.subscriptions.add(
      this.onboardingService.getStatistics(department).subscribe({
        next: (stats) => {
          this.stats = stats;
          this.isLoading = false;
          this.successMessage = 'Statistics loaded';
        },
        error: (error) => {
          this.isLoading = false;
          console.warn('Failed to load stats:', error);
          // Set default stats
          this.stats = {
            totalOnboardings: 0,
            activeOnboardings: 0,
            completedOnboardings: 0,
            averageCompletionDays: 0,
            averageSatisfactionScore: 0,
            moduleCompletionRates: {},
            commonChallenges: [],
            mentorEffectiveness: 0
          };
        }
      })
    );
  }

  // 11. Complete Onboarding
  completeOnboarding(profileId: string): void {
    if (!profileId) return;
    
    if (!confirm('Complete onboarding for this user?')) return;

    this.isLoading = true;
    this.clearMessages();

    this.subscriptions.add(
      this.onboardingService.completeOnboarding(profileId).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.successMessage = 'Onboarding completed!';
          this.getProfile(profileId);
        },
        error: (error) => {
          this.isLoading = false;
          this.error = error;
        }
      })
    );
  }

  // ==================== TEST ENDPOINT METHODS ====================

  testCreateProfileEndpoint(event: Event): void {
    event.preventDefault();
    this.createProfileForm.patchValue({
      userName: 'John Doe',
      userEmail: 'john.doe@example.com',
      role: 'developer',
      experienceLevel: 'intermediate',
      department: 'Engineering',
      manager: 'Jane Smith',
      startDate: this.getTodayDate()
    });
    // Auto submit after a short delay
    setTimeout(() => this.createProfile(), 100);
  }

  testGetProfileEndpoint(event: Event, profileId: string): void {
    event.preventDefault();
    if (profileId?.trim()) {
      this.getProfile(profileId.trim());
    } else {
      alert('Please enter a profile ID');
    }
  }

  testGetPlanEndpoint(event: Event, profileId: string): void {
    event.preventDefault();
    if (profileId?.trim()) {
      this.loadPlan(profileId.trim());
      this.successMessage = 'Loading plan...';
    } else {
      alert('Please enter a profile ID');
    }
  }

  testGetTasksEndpoint(event: Event, profileId: string): void {
    event.preventDefault();
    if (profileId?.trim()) {
      this.loadTasks(profileId.trim());
      this.successMessage = 'Loading tasks...';
    } else {
      alert('Please enter a profile ID');
    }
  }

  testUpdateProgressEndpoint(event: Event): void {
    event.preventDefault();
    if (this.selectedProfileId) {
      this.updateProgressForm.patchValue({ profileId: this.selectedProfileId });
      // Scroll to progress form
      document.querySelector('.progress-section')?.scrollIntoView({ behavior: 'smooth' });
    } else {
      alert('Please load a profile first');
    }
  }

  testUpdateTaskEndpoint(event: Event, taskId: string): void {
    event.preventDefault();
    if (taskId?.trim()) {
      this.updateTaskStatus(taskId.trim(), 'completed');
    } else {
      alert('Please enter a task ID');
    }
  }

  testGetModuleEndpoint(event: Event, moduleId: string): void {
    event.preventDefault();
    if (moduleId?.trim()) {
      this.loadModule(moduleId.trim());
    } else {
      alert('Please enter a module ID');
    }
  }

  testAssignMentorEndpoint(event: Event, profileId: string): void {
    event.preventDefault();
    if (profileId?.trim()) {
      this.assignMentor(profileId.trim());
    } else {
      alert('Please enter a profile ID');
    }
  }

  testSubmitFeedbackEndpoint(event: Event): void {
    event.preventDefault();
    if (this.selectedProfileId) {
      this.feedbackForm.patchValue({ 
        profileId: this.selectedProfileId,
        comments: 'Great onboarding experience!',
        suggestions: 'More interactive exercises'
      });
      // Scroll to feedback form
      document.querySelector('.feedback-section')?.scrollIntoView({ behavior: 'smooth' });
    } else {
      alert('Please load a profile first');
    }
  }

  testGetStatsEndpoint(event: Event, department?: string): void {
    event.preventDefault();
    this.loadStats(department);
  }

  testCompleteOnboardingEndpoint(event: Event, profileId: string): void {
    event.preventDefault();
    if (profileId?.trim()) {
      this.completeOnboarding(profileId.trim());
    } else {
      alert('Please enter a profile ID');
    }
  }
}