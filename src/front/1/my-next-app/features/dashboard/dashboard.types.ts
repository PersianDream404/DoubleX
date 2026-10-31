export type DashboardPeriod = 1 | 2 | 3 | 4;

export interface DashboardSummary {
  receptionCount: number;
  completedReceptionCount: number;
  inProgressReceptionCount: number;
  cancelledReceptionCount: number;
  appointmentReceptionCount: number;
  walkInReceptionCount: number;
  uniqueCustomerCount: number;
  serviceSubtotal: number;
  discountAmount: number;
  serviceFinalAmount: number;
  paidAmount: number;
  outstandingAmount: number;
  recordedExpenses: number;
  cancelledExpenses: number;
}

export interface DashboardFinancialTrend {
  date: string;
  receptionCount: number;
  serviceSubtotal: number;
  discountAmount: number;
  serviceFinalAmount: number;
  paidAmount: number;
}

export interface DashboardPopularService {
  serviceId: number;
  serviceName: string;
  completedServiceCount: number;
  serviceAmount: number;
}

export interface DashboardPersonnelPerformance {
  personnelId: number;
  personnelName: string | null;
  completedServiceCount: number;
  serviceAmount: number;
}

export interface DashboardCharts {
  financialTrend: DashboardFinancialTrend[];
  recordedExpenses: number;
  cancelledExpenses: number;
  popularServices: DashboardPopularService[];
  personnelPerformance: DashboardPersonnelPerformance[];
}

export interface DashboardAppointment {
  id: number;
  receptionId: number | null;
  customerName: string;
  serviceName: string;
  personnelName: string;
  startsAt: string;
  endsAt: string;
  finalAmount: number;
  status: string;
}

export interface DashboardTodayAppointments {
  appointments: DashboardAppointment[];
  totalCount: number;
}

export type DashboardAlertType = 1 | 2 | 3;

export interface DashboardAlert {
  type: DashboardAlertType;
  title: string;
  count: number;
  amount: number | null;
  relatedIds: number[];
}

export interface DashboardData {
  summary: DashboardSummary;
  charts: DashboardCharts;
  appointments: DashboardTodayAppointments;
  alerts: DashboardAlert[];
}
