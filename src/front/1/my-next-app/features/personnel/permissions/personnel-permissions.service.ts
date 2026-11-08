import { ApiClient } from "@/lib/api/api-client";
import type { ApiResponse } from "@/lib/api/types/api-response";

import type {
  ManagedPermission,
  ManageableUser,
  UserPermissionAccess,
} from "./personnel-permissions.types";

export class PersonnelPermissionsService {
  private readonly api = new ApiClient();

  getAvailablePermissions(
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ManagedPermission[]>> {
    return this.api.get<ManagedPermission[]>(
      "/api/Identity/permissions",
      undefined,
      config,
    );
  }

  getUserPermissions(
    userId: number,
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<UserPermissionAccess>> {
    return this.api.get<UserPermissionAccess>(
      `/api/Identity/users/${userId}/permissions`,
      undefined,
      config,
    );
  }

  getManageableUsers(
    config?: { signal?: AbortSignal },
  ): Promise<ApiResponse<ManageableUser[]>> {
    return this.api.get<ManageableUser[]>(
      "/api/Identity/users/assignable",
      undefined,
      config,
    );
  }

  grant(userId: number, permission: string): Promise<ApiResponse<void>> {
    return this.api.put<void, undefined>(
      `/api/Identity/users/${userId}/permissions/${encodeURIComponent(permission)}/grant`,
      undefined,
    );
  }

  deny(userId: number, permission: string): Promise<ApiResponse<void>> {
    return this.api.put<void, undefined>(
      `/api/Identity/users/${userId}/permissions/${encodeURIComponent(permission)}/deny`,
      undefined,
    );
  }

  removeOverride(
    userId: number,
    permission: string,
  ): Promise<ApiResponse<void>> {
    return this.api.delete<void>(
      `/api/Identity/users/${userId}/permissions/${encodeURIComponent(permission)}`,
    );
  }

  resetOverrides(userId: number): Promise<ApiResponse<void>> {
    return this.api.delete<void>(
      `/api/Identity/users/${userId}/permissions`,
    );
  }
}
