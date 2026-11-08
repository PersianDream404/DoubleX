import { PersonnelPermissionsPage } from "@/features/personnel/permissions/personnel-permissions-page";

export default async function AdminUserPermissionsRoute({
  searchParams,
}: {
  searchParams: Promise<{ userId?: string }>;
}) {
  const { userId: userIdParam } = await searchParams;
  const userId = Number(userIdParam);

  return (
    <PersonnelPermissionsPage
      adminMode
      initialUserId={
        Number.isSafeInteger(userId) && userId > 0 ? userId : undefined
      }
    />
  );
}
