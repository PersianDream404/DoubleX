"use client";

import { useCallback, useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";

import { PersonnelPermissionsService } from "./personnel-permissions.service";
import type {
  ManagedPermission,
  PermissionEffect,
  UserPermissionAccess,
} from "./personnel-permissions.types";

export function usePersonnelPermissions(userId: number | null) {
  const [permissions, setPermissions] = useState<ManagedPermission[]>([]);
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
  const currentAccessRequest =
    accessRequest?.userId === userId && accessRequest.version === accessVersion
      ? accessRequest
      : null;
  const access = currentAccessRequest?.data ?? null;
  const loadingAccess = userId !== null && currentAccessRequest === null;

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
    if (userId === null) {
      return;
    }

    const controller = new AbortController();
    new PersonnelPermissionsService()
      .getUserPermissions(userId, { signal: controller.signal })
      .then((response) => {
        if (controller.signal.aborted) return;
        if (!response.data) {
          throw new Error(response.message ?? "اطلاعات دسترسی دریافت نشد.");
        }
        setAccessRequest({
          userId,
          version: accessVersion,
          data: response.data,
          error: null,
        });
      })
      .catch((requestError: unknown) => {
        if (!controller.signal.aborted) {
          setAccessRequest({
            userId,
            version: accessVersion,
            data: null,
            error: handleApiError(requestError),
          });
        }
      });

    return () => controller.abort();
  }, [accessVersion, userId]);

  const displayedError = currentAccessRequest?.error ?? error;

  const setOverride = useCallback(
    async (permission: string, effect: PermissionEffect) => {
      if (userId === null || pendingPermission !== null) return;

      setPendingPermission(permission);
      setError(null);
      try {
        const service = new PersonnelPermissionsService();
        if (effect === "Grant") {
          await service.grant(userId, permission);
        } else {
          await service.deny(userId, permission);
        }
        setAccessVersion((version) => version + 1);
      } catch (requestError: unknown) {
        setError(handleApiError(requestError));
      } finally {
        setPendingPermission(null);
      }
    },
    [pendingPermission, userId],
  );

  const removeOverride = useCallback(
    async (permission: string) => {
      if (userId === null || pendingPermission !== null) return;

      setPendingPermission(permission);
      setError(null);
      try {
        const service = new PersonnelPermissionsService();
        await service.removeOverride(userId, permission);
        setAccessVersion((version) => version + 1);
      } catch (requestError: unknown) {
        setError(handleApiError(requestError));
      } finally {
        setPendingPermission(null);
      }
    },
    [pendingPermission, userId],
  );

  const resetOverrides = useCallback(async () => {
    if (userId === null || resetting) return;

    setResetting(true);
    setError(null);
    try {
      const service = new PersonnelPermissionsService();
      await service.resetOverrides(userId);
      setAccessVersion((version) => version + 1);
    } catch (requestError: unknown) {
      setError(handleApiError(requestError));
    } finally {
      setResetting(false);
    }
  }, [resetting, userId]);

  return {
    permissions,
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
