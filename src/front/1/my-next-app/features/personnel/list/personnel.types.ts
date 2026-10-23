import type { PagedResult, PageParams } from "@/lib/api/types/paged-result";

export interface PersonnelDto {
  id: number;
  fullName: string;
  nationalCode: string;
  jobTitle: string;
  cardNumber: string;
  birthDate: string;
  mobilePhone: string;
}

export interface PersonnelDetailsDto {
  id: number;
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
    skills: string[];
    workHistory: string | null;
  };
  financialInfo: {
    salaryType: string;
    cardNumber: string;
    accountNumber: string;
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
}

export type GetAllPersonnelRequest = PageParams & {
  [key: string]: string | number | boolean | null | undefined;
  Q?: string;
};

export type PersonnelListResult = PagedResult<PersonnelDto>;
