export interface Nomination {
  id: number;
  shipperId: string;
  pipelineSegment: string;
  requestedVolume: number;
  scheduledVolume: number | null;
  status: string;
  createdAt: string;
}

export interface CreateNominationRequest {
  shipperId: string;
  pipelineSegment: string;
  requestedVolume: number;
}

export interface ScheduleRequest {
  capacity: number;
}

export interface ScheduleResult {
  pipelineSegment: string;
  totalRequested: number;
  capacity: number;
  outcome: string;
  nominations: Nomination[];
}
