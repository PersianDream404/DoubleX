export const ROLES = {
  ADMIN: "Admin",
  Employer: "Employer",
} as const;

export type RoleName = (typeof ROLES)[keyof typeof ROLES];

export function isRoleName(value: string): value is RoleName {
  return Object.values(ROLES).includes(value as RoleName);
}
