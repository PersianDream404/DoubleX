import type { PageParams, PagedResult } from "@/lib/api/types/paged-result";

export interface ReceptionListItem {
  id: number;
  salonId: number;
  receptionNumber: string;
  customerId: number;
  customerName: string;
  receptionistId: number;
  serviceTariffId: number | null;
  receptionDate: string;
  status: number;
  totalAmount: number;
  paidAmount: number;
  remainingAmount: number;
  financialStatus: number;
}

export interface ReceptionServiceMaterial {
  id: number;
  materialId: number;
  quantity: number;
  unitCost: number | null;
  description: string | null;
  movementRegistered: boolean;
  movementReversed: boolean;
}

export interface ReceptionServiceItem {
  id: number;
  serviceId: number;
  serviceTariffId: number;
  personnelId: number | null;
  serviceName: string;
  quantity: number;
  unitPrice: number;
  discountAmount: number;
  finalAmount: number;
  status: number;
  materials: ReceptionServiceMaterial[];
}

export interface ReceptionPayment {
  id: number;
  amount: number;
  paymentMethod: string;
  paidAt: string;
  status: number;
  transactionReference: string | null;
  description: string | null;
}

export interface ReceptionDetails extends ReceptionListItem {
  subTotal: number;
  totalDiscountAmount: number;
  notes: string | null;
  services: ReceptionServiceItem[];
  payments: ReceptionPayment[];
}

export interface ReceptionListQuery extends PageParams {
  [key: string]: string | number | boolean | null | undefined;
  UnpaidOnly?: boolean;
}

export interface CustomerSelectItem {
  id: number;
  fullName: string;
  status: boolean;
}

export interface CreateReceptionRequest {
  salonId: number;
  customerId: number;
  receptionistId: number;
  serviceTariffId: number;
  receptionDate: string;
  notes: string | null;
  appointmentId: number | null;
}

export interface AddReceptionServiceRequest {
  salonId: number;
  serviceTariffId: number;
  serviceId: number;
  personnelId: number | null;
  quantity: number;
  discountAmount: number;
}

export interface RegisterReceptionMaterialRequest {
  salonId: number;
  materialId: number;
  quantity: number;
  idempotencyKey: string;
  description: string | null;
}

export interface RegisterReceptionPaymentRequest {
  salonId: number;
  amount: number;
  paymentMethod: string;
  paidAt: string;
  transactionReference: string | null;
  idempotencyKey: string;
  description: string | null;
}

export type ReceptionListResult = PagedResult<ReceptionListItem>;
