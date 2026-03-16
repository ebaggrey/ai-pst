import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HealthScoreResponse, InsightResponse,
        MetricDesignResponse, OptimizationResponse, 
        PredictionResponse } from 'src/chapter-10/models/responses/response-models';
import { HealthScoreExample, InsightExample, 
       MetricDesignExample, OptimizationExample,
        PredictionExample } from 'src/chapter-10/examples/metrics-that-matter-examples';
import { MetricsThatMatterService } from 'src/chapter-10/services/metrics-that-matter.service';

@Component({
  selector: 'app-metrics-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './metrics-dashboard.component.html',
  styleUrls: ['./metrics-dashboard.component.scss']
})
export class MetricsDashboardComponent implements OnInit {
    designResult: MetricDesignResponse | null = null;
    healthScore: HealthScoreResponse | null = null;
    predictions: PredictionResponse | null = null;
    insights: InsightResponse | null = null;
    optimization: OptimizationResponse | null = null;
    
    loading = false;
    error: string | null = null;

    constructor(private metricsService: MetricsThatMatterService) {}

    ngOnInit(): void {
        // Optional: Load initial data
    }

    designMetrics(): void {
        this.loading = true;
        this.error = null;
        
        this.metricsService.designImpactfulMetrics(MetricDesignExample)
            .subscribe({
                next: (response) => {
                    this.designResult = response;
                    this.loading = false;
                    console.log('Metrics designed:', response);
                },
                error: (error) => {
                    this.error = error.message;
                    this.loading = false;
                }
            });
    }

    calculateHealth(): void {
        this.loading = true;
        this.error = null;
        
        this.metricsService.calculateTestingHealthScore(HealthScoreExample)
            .subscribe({
                next: (response) => {
                    this.healthScore = response;
                    this.loading = false;
                    console.log('Health score:', response);
                },
                error: (error) => {
                    this.error = error.message;
                    this.loading = false;
                }
            });
    }

    predictTrends(): void {
        this.loading = true;
        this.error = null;
        
        this.metricsService.predictQualityTrends(PredictionExample)
            .subscribe({
                next: (response) => {
                    this.predictions = response;
                    this.loading = false;
                    console.log('Predictions:', response);
                },
                error: (error) => {
                    this.error = error.message;
                    this.loading = false;
                }
            });
    }

    generateInsights(): void {
        this.loading = true;
        this.error = null;
        
        this.metricsService.generateActionableInsights(InsightExample)
            .subscribe({
                next: (response) => {
                    this.insights = response;
                    this.loading = false;
                    console.log('Insights:', response);
                },
                error: (error) => {
                    this.error = error.message;
                    this.loading = false;
                }
            });
    }

    optimizeCollection(): void {
        this.loading = true;
        this.error = null;
        
        this.metricsService.optimizeMetricCollection(OptimizationExample)
            .subscribe({
                next: (response) => {
                    this.optimization = response;
                    this.loading = false;
                    console.log('Optimization:', response);
                },
                error: (error) => {
                    this.error = error.message;
                    this.loading = false;
                }
            });
    }

    runAllExamples(): void {
        this.designMetrics();
        this.calculateHealth();
        this.predictTrends();
        this.generateInsights();
        this.optimizeCollection();
    }
}