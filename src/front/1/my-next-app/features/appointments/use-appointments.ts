"use client";

import { useEffect, useState } from "react";

import { handleApiError } from "@/lib/api/handle-api-error";
import { AppointmentService } from "./appointment.service";
import type { AppointmentListItemDto } from "./appointment.types";

type AppointmentState = {
  requestKey: string;
  appointments: AppointmentListItemDto[];
  error: string | null;
};

export function useAppointments(
  refreshKey: number,
  from: string,
  to: string,
  serviceId: number | null,
) {
  const requestKey = JSON.stringify([refreshKey, from, to, serviceId]);
  const [state, setState] = useState<AppointmentState>({
    requestKey: "",
    appointments: [],
    error: null,
  });

  useEffect(() => {
    const controller = new AbortController();
    void new AppointmentService()
      .getAppointmentsForRange(from, to, serviceId, controller.signal)
      .then((appointments) => {
        if (!controller.signal.aborted) {
          setState({ requestKey, appointments, error: null });
        }
      })
      .catch((cause: unknown) => {
        if (!controller.signal.aborted) {
          setState({ requestKey, appointments: [], error: handleApiError(cause) });
        }
      });
    return () => controller.abort();
  }, [from, requestKey, serviceId, to]);

  const isCurrent = state.requestKey === requestKey;
  return {
    appointments: isCurrent ? state.appointments : [],
    loading: !isCurrent,
    error: isCurrent ? state.error : null,
  };
}
