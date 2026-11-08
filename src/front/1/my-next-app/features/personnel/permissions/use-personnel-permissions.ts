"use client";

import { useCallback, useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";

import { PersonnelPermissionsService } from "./personnel-permissions.service";
import type {
  ManagedPermission,
  ManageableUser,
  PermissionEffect,
  UserPermissionAccess,
} from "./personnel-permissions.types";

export function usePersonnelPermissions(userId: number | null) {
  const [permissions, setPermissions] = useState<ManagedPermission[]>([]);
  const [users, setUsers] = useState<ManageableUser[]>([]);
  const [loadingUsers, setLoadingUsers] = useState(true);
  const [usersError, setUsersError] = useState<string | null>(null);
  const [accessRequest, setAccessRequest] = useState<{
    userId: number;
    version: number;
    data: UserPermissionAccess | null;
    error: string | null;
  } | null>(null);
  const [loadingPermissions, setLoadingPermissions] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [pendingPermission, setPendingPermission] = useState<string | null>(
    null,
  );
  const [resetting, setResetting] = useState(false);
  const [accessVersion, setAccessVersion] = useState(0);
  const selectedUserId =
    !loadingUsers && users.some((user) => user.userId === userId)
      ? userId
      : null;
  const currentAccessRequest =
    accessRequest !== null &&
    accessRequest.userId === selectedUserId &&
    accessRequest.version === accessVersion
      ? accessRequest
      : null;
  const access = currentAccessRequest?.data ?? null;
  const loadingAccess = selectedUserId !== null && currentAccessRequest === null;

  useEffect(() => {
    const controller = new AbortController();
    const service = new PersonnelPermissionsService();

    service
      .getAvailablePermissions({ signal: controller.signal })
      .then((response) => {
        if (!response.data) {
          throw new Error(response.message ?? "فهرست مجوزها خالی است.");
        }
        setPermissions(response.data);
      })
      .catch((requestError: unknown) => {
        if (!controller.signal.aborted) {
          setError(handleApiError(requestError));
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) {
          setLoadingPermissions(false);
        }
      });

    return () => controller.abort();
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    new PersonnelPermissionsService()
      .getManageableUsers({ signal: controller.signal })
      .then((response) => {
        if (controller.signal.aborted) return;
        if (!response.data) {
          throw new Error(response.message ?? "فهرست حساب‌های قابل انتخاب دریافت نشد.");
        }
        setUsers(response.data);
      })
      .catch((requestError: unknown) => {
        if (!controller.signal.aborted) {
          setUsersError(handleApiError(requestError));
        }
      })
      .finally(() => {
        if (!controller.signal.aborted) {
          setLoadingUsers(false);
        }
      });

    return () => controller.abort();
  }, []);

  useEffect(() => {
    if (selectedUserId === null) {
      return;
    }

    const controller = new AbortController();
    new PersonnelPermissionsService()
      .getUserPermissions(selectedUserId, { signal: controller.signal })
      .then((response) => {
        if (controller.signal.aborted) return;
        if (!response.data) {
          throw new Error(response.message ?? "اطلاعات دسترسی دریافت نشد.");
        }
        setAccessRequest({
          userId: selectedUserId,
          version: accessVersion,
          data: response.data,
          error: null,
        });
      })
      .catch((requestError: unknown) => {
        if (!controller.signal.aborted) {
          setAccessRequest({
            userId: selectedUserId,
            version: accessVersion,
            data: null,
            error: handleApiError(requestError),
          });
        }
      });

    return () => controller.abort();
  }, [accessVersion, selectedUserId]);

  const displayedError = currentAccessRequest?.error ?? error;

  const setOverride = useCallback(
    async (permission: string, effect: PermissionEffect) => {
      if (selectedUserId === null || pendingPermission !== null) return;

      setPendingPermission(permission);
      setError(null);
      try {
        const service = new PersonnelPermissionsService();
        if (effect === "Grant") {
          await service.grant(selectedUserId, permission);
        } else {
          await service.deny(selectedUserId, permission);
        }
        setAccessVersion((version) => version + 1);
      } catch (requestError: unknown) {
        setError(handleApiError(requestError));
      } finally {
        setPendingPermission(null);
      }
    },
    [pendingPermission, selectedUserId],
  );

  const removeOverride = useCallback(
    async (permission: string) => {
      if (selectedUserId === null || pendingPermission !== null) return;

      setPendingPermission(permission);
      setError(null);
      try {
        const service = new PersonnelPermissionsService();
        await service.removeOverride(selectedUserId, permission);
        setAccessVersion((version) => version + 1);
      } catch (requestError: unknown) {
        setError(handleApiError(requestError));
      } finally {
        setPendingPermission(null);
      }
    },
    [pendingPermission, selectedUserId],
  );

  const resetOverrides = useCallback(async () => {
    if (selectedUserId === null || resetting) return;

    setResetting(true);
    setError(null);
    try {
      const service = new PersonnelPermissionsService();
      await service.resetOverrides(selectedUserId);
      setAccessVersion((version) => version + 1);
    } catch (requestError: unknown) {
      setError(handleApiError(requestError));
    } finally {
      setResetting(false);
    }
  }, [resetting, selectedUserId]);

  return {
    permissions,
    users,
    loadingUsers,
    usersError,
    selectedUserId,
    access,
    loadingPermissions,
    loadingAccess,
    error: displayedError,
    pendingPermission,
    resetting,
    setOverride,
    removeOverride,
    resetOverrides,
  };
}
