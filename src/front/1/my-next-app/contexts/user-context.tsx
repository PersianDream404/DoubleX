"use client";

import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";

type UserContextValue = {
  salonId: number | null;
  loading: boolean;
  error: string | null;
  refresh: () => void;
};

type UserState = Pick<UserContextValue, "salonId" | "loading" | "error">;

const UserContext = createContext<UserContextValue | null>(null);

function readSalonIdFromToken(): number | null {
  const token = window.localStorage.getItem("access_token");
  if (!token) return null;

  const payload = token.split(".")[1];
  if (!payload) throw new Error("توکن ورود ساختار معتبری ندارد.");

  const normalizedPayload = payload.replace(/-/g, "+").replace(/_/g, "/");
  const binaryPayload = window.atob(
    normalizedPayload.padEnd(Math.ceil(normalizedPayload.length / 4) * 4, "="),
  );
  const bytes = Uint8Array.from(binaryPayload, (character) =>
    character.charCodeAt(0),
  );
  const decodedPayload = new TextDecoder().decode(bytes);
  const claims: unknown = JSON.parse(decodedPayload);
  if (!claims || typeof claims !== "object") {
    throw new Error("اطلاعات توکن ورود معتبر نیست.");
  }

  const record = claims as Record<string, unknown>;
  const claimValue = record.SalonId ?? record.salonId;
  const salonId =
    typeof claimValue === "string" || typeof claimValue === "number"
      ? Number(claimValue)
      : Number.NaN;
  if (!Number.isSafeInteger(salonId) || salonId <= 0) {
    throw new Error("شناسه سالن در توکن ورود موجود نیست.");
  }

  const expiresAt = record.exp;
  if (
    typeof expiresAt === "number" &&
    expiresAt <= Math.floor(Date.now() / 1000)
  ) {
    throw new Error("توکن ورود منقضی شده است.");
  }

  return salonId;
}

export function UserProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<UserState>({
    salonId: null,
    loading: true,
    error: null,
  });

  const refresh = useCallback(() => {
    try {
      setState({ salonId: readSalonIdFromToken(), loading: false, error: null });
    } catch (error: unknown) {
      setState({
        salonId: null,
        loading: false,
        error:
          error instanceof Error
            ? error.message
            : "خواندن اطلاعات کاربر از توکن انجام نشد.",
      });
    }
  }, []);

  useEffect(() => {
    let active = true;
    queueMicrotask(() => {
      if (active) refresh();
    });
    window.addEventListener("storage", refresh);
    window.addEventListener("auth-token-changed", refresh);
    return () => {
      active = false;
      window.removeEventListener("storage", refresh);
      window.removeEventListener("auth-token-changed", refresh);
    };
  }, [refresh]);

  const value = useMemo(() => ({ ...state, refresh }), [state, refresh]);
  return <UserContext.Provider value={value}>{children}</UserContext.Provider>;
}

export function useUserContext() {
  const context = useContext(UserContext);
  if (!context) {
    throw new Error("useUserContext باید داخل UserProvider استفاده شود.");
  }
  return context;
}
