export type PermissionEffect = "Grant" | "Deny";

export interface ManagedPermission {
  code: string;
  module: string;
  displayName: string;
  isAssignable: boolean;
}

export interface UserPermissionAccess {
  userId: number;
  salonId: number;
  roles: string[];
  rolePermissions: string[];
  grants: string[];
  denies: string[];
  effectivePermissions: string[];
}
