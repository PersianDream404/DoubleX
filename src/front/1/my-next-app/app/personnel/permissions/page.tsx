import { PersonnelPermissionsPage } from "@/features/personnel/permissions/personnel-permissions-page";

export default async function PersonnelPermissionsRoute({
  searchParams,
}: {
  searchParams: Promise<{ userId?: string }>;
}) {
  const { userId: userIdParam } = await searchParams;
  const userId = Number(userIdParam);

  return (
    <PersonnelPermissionsPage
      initialUserId={
        Number.isSafeInteger(userId) && userId > 0 ? userId : undefined
      }
    />
  );
}
