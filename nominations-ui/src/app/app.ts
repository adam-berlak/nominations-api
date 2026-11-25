import { Component, OnInit, Inject, PLATFORM_ID, ChangeDetectorRef } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { NominationService } from './services/nomination.service';
import { Nomination, CreateNominationRequest, ScheduleResult } from './models/nomination';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  nominations: Nomination[] = [];
  scheduleResult: ScheduleResult | null = null;
  errorMessage: string = '';
  loading = false;

  createForm: CreateNominationRequest = {
    shipperId: '',
    pipelineSegment: '',
    requestedVolume: 0
  };

  scheduleForm = {
    segment: '',
    capacity: 0
  };

  constructor(
    private nominationService: NominationService,
    private cdr: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: object
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadNominations();
    }
  }

  loadNominations(): void {
    this.loading = true;
    this.nominationService.getAll().subscribe({
      next: (data) => {
        this.nominations = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load nominations. Is the API running?';
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  createNomination(): void {
    this.errorMessage = '';
    this.nominationService.create(this.createForm).subscribe({
      next: () => {
        this.loadNominations();
        this.createForm = { shipperId: '', pipelineSegment: '', requestedVolume: 0 };
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to create nomination.';
        this.cdr.detectChanges();
      }
    });
  }

  runSchedule(): void {
    this.errorMessage = '';
    this.scheduleResult = null;
    this.nominationService
      .schedule(this.scheduleForm.segment, { capacity: this.scheduleForm.capacity })
      .subscribe({
        next: (result) => {
          this.scheduleResult = result;
          this.loadNominations();
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.errorMessage =
            err.status === 404
              ? `No pending nominations found for segment "${this.scheduleForm.segment}".`
              : `Failed to run schedule (status ${err.status}).`;
          this.cdr.detectChanges();
        }
      });
  }
}
