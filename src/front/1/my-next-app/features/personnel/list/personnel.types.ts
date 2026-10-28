import type { PagedResult, PageParams } from "@/lib/api/types/paged-result";

export interface PersonnelDto {
  id: number;
  status: boolean;
  fullName: string;
  nationalCode: string;
  jobTitle: string;
  cardNumber: string;
  birthDate: string;
  mobilePhone: string;
}

export interface PersonnelContractDto {
  id: number;
  cooperationType: string;
  fixedSalaryAmount: number | null;
  defaultCommissionRate: number | null;
  effectiveFrom: string;
  effectiveTo: string | null;
  isActive: boolean;
}

export interface PersonnelDetailsDto {
  id: number;
  status: boolean;
  salonId: number;
  personalInfo: {
    fullName: string;
    nationalCode: string;
    birthDate: string;
    maritalStatus: string;
    childrenCount: number;
    mobilePhone: string;
    landlinePhone: string | null;
    addressText: string;
    isCloseToSalon: boolean;
    pictureUrl: string | null;
  };
  jobInfo: {
    jobTitle: string;
    personnelCode: string;
    cooperationStartDate: string;
    workingShiftType: string;
    jobPictures: string[];
    workHistory: string | null;
  };
  financialInfo: {
    salaryType: string;
    cardNumber: string;
    accountNumber: string;
    fixedSalaryAmount?: number | null;
    commissionRate?: number | null;
    effectiveFrom?: string | null;
    effectiveTo?: string | null;
  };
  healthInfo: {
    healthStatus: boolean;
    diseases: string | null;
    allergyStatus: boolean;
    allergies: string | null;
    pictureCourse: string[];
    pictureTest: string[];
  };
  guarantorInfo: {
    guarantorName: string;
    relationship: string;
    guarantorPhone: string;
    guarantorLandlinePhone: string | null;
    pictureGuaranties: string[];
  };
  documents: {
    documentType: number;
    fileName: string;
    url: string | null;
  }[];
  contracts?: PersonnelContractDto[];
}

export type PersonnelDocumentType = 0 | 1 | 2 | 3 | 4 | 5;

export interface PersonnelDocumentRequest {
  documentType: PersonnelDocumentType;
  fileName: string;
  url: string;
}

export interface CreatePersonnelRequest {
  status: boolean;
  personalInfo: {
    fullName: string;
    nationalCode: string;
    birthDate: string;
    maritalStatus: 0 | 1;
    childrenCount: number;
    mobilePhone: string;
    landlinePhone: string | null;
    addressText: string;
    isCloseToSalon: boolean;
    pictureUrl: string | null;
    clearPicture?: boolean;
  };
  jobInfo: {
    jobTitle: string;
    personnelCode: string;
    cooperationStartDate: string;
    workingShiftType: number;
    jobPictures: string[];
    workHistory: string | null;
  };
  financialInfo: {
    salaryType: number;
    cardNumber: string;
    accountNumber: string;
    fixedSalaryAmount?: number | null;
    commissionRate?: number | null;
    effectiveFrom?: string | null;
    effectiveTo?: string | null;
  };
  healthInfo: {
    healthStatus: string;
    allergyStatus: string;
    diseases: string | null;
    allergies: string | null;
  };
  guarantorInfo: {
    guarantorName: string;
    relationship: string;
    guarantorPhone: string;
    guarantorLandlinePhone: string;
  };
  documents: PersonnelDocumentRequest[];
}

export interface UploadedPersonnelFile {
  id: number;
  path: string;
}

export type GetAllPersonnelRequest = PageParams & {
  [key: string]: string | number | boolean | null | undefined;
  Q?: string;
};

export type PersonnelListResult = PagedResult<PersonnelDto>;

export type UpdatePersonnelRequest = Omit<CreatePersonnelRequest, "documents"> & {
  documents: {
    documentType: PersonnelDocumentType;
    fileName: string;
    url: string | null;
  }[];
};
