import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type { UploadedPersonnelFile } from "./personnel.types";

export const PersonnelUploadCategory = {
  photo: 5,
  training: 6,
  healthCourse: 7,
  healthTest: 8,
  guarantor: 9,
} as const;

export class PersonnelFileService {
  private readonly api = new ApiClient();

  upload(
    file: File,
    category: (typeof PersonnelUploadCategory)[keyof typeof PersonnelUploadCategory],
  ) {
    const body = new FormData();
    body.append("File", file);
    body.append("Category", String(category));

    return this.api.post<UploadedPersonnelFile, FormData>(
      "/api/FileStore",
      body,
    );
  }

  delete(id: number): Promise<ApiResponse<void>> {
    return this.api.delete<void>(`/api/FileStore/${id}`);
  }
}

export function getPersonnelFileUrl(path: string) {
  const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

  if (!baseUrl) {
    throw new Error("آدرس API برای نمایش فایل‌های پرسنل تنظیم نشده است.");
  }

  const apiOrigin = new URL(baseUrl).origin;
  return new URL(path.replace(/^\/+/, ""), `${apiOrigin}/`).toString();
}
