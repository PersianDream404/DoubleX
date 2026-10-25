"use client";

import { useCallback, useState } from "react";
import { handleApiError } from "@/lib/api/handle-api-error";

import { InventoryConfirmDialog } from "../shared/inventory-confirm-dialog";
import { InventoryDetailsDialog } from "../shared/inventory-details-dialog";
import {
  InventoryListPage,
  type InventoryColumn,
} from "../shared/inventory-list-page";
import { UnitFormDialog } from "./unit-form-dialog";
import { UnitService } from "./unit.service";
import { useUnits } from "./use-units";
import type { UnitDto, UnitType } from "./unit.types";

type ConfirmTarget = {
  kind: "delete" | "status";
  id: number;
  name: string;
  isActive: boolean;
};

const UNIT_TYPE_LABELS: Record<UnitType, string> = {
  1: "وزن",
  2: "حجم",
  3: "تعداد",
};

const COLUMNS: InventoryColumn<UnitDto>[] = [
  { key: "name", title: "نام واحد", render: (unit) => unit.name },
  { key: "symbol", title: "نماد", render: (unit) => <span dir="ltr">{unit.symbol}</span> },
  { key: "type", title: "نوع", render: (unit) => UNIT_TYPE_LABELS[unit.unitType] },
];

export function UnitsPage() {
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const [refreshKey, setRefreshKey] = useState(0);
  const [formOpen, setFormOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [detailsId, setDetailsId] = useState<number | null>(null);
  const [confirmTarget, setConfirmTarget] = useState<ConfirmTarget | null>(null);
  const [confirmError, setConfirmError] = useState<string | null>(null);
  const [pending, setPending] = useState(false);

  const { data, error, loading } = useUnits(query, page, refreshKey);
  const units = data?.list ?? [];
  const totalPages = Math.max(1, data?.pagination.totalPages ?? 1);

  const loadUnit = useCallback(async (id: number, signal: AbortSignal) => {
    const response = await new UnitService().getById(id, { signal });
    if (!response.data) {
      throw new Error(response.message ?? "اطلاعات واحد دریافت نشد.");
    }
    return response.data;
  }, []);

  function requestConfirmation(target: ConfirmTarget) {
    setConfirmError(null);
    setConfirmTarget(target);
  }

  async function confirmAction() {
    if (!confirmTarget || pending) {
      return;
    }

    setPending(true);
    setConfirmError(null);
    try {
      const service = new UnitService();
      if (confirmTarget.kind === "delete") {
        await service.delete(confirmTarget.id);
      } else {
        await service.setActive(confirmTarget.id, !confirmTarget.isActive);
      }
      setConfirmTarget(null);
      setRefreshKey((current) => current + 1);
    } catch (cause: unknown) {
      setConfirmError(handleApiError(cause));
    } finally {
      setPending(false);
    }
  }

  return (
    <>
      <InventoryListPage
        title="فهرست واحدها"
        breadcrumbHref="/inventory/units"
        searchPlaceholder="جستجو بر اساس نام یا نماد واحد..."
        query={query}
        onQueryChange={(value) => {
          setQuery(value);
          setPage(1);
        }}
        page={page}
        onPageChange={setPage}
        totalPages={totalPages}
        items={units}
        columns={COLUMNS}
        loading={loading}
        error={error}
        onCreate={() => {
          setEditingId(null);
          setFormOpen(true);
        }}
        onView={(unit) => setDetailsId(unit.id)}
        onEdit={(unit) => {
          setEditingId(unit.id);
          setFormOpen(true);
        }}
        onSetActive={(unit) =>
          requestConfirmation({
            kind: "status",
            id: unit.id,
            name: unit.name,
            isActive: unit.isActive,
          })
        }
        onDelete={(unit) =>
          requestConfirmation({
            kind: "delete",
            id: unit.id,
            name: unit.name,
            isActive: unit.isActive,
          })
        }
      />

      <UnitFormDialog
        open={formOpen}
        unitId={editingId}
        onOpenChange={(open) => {
          setFormOpen(open);
          if (!open) {
            setEditingId(null);
          }
        }}
        onSaved={() => setRefreshKey((current) => current + 1)}
      />

      <InventoryDetailsDialog
        open={detailsId !== null}
        id={detailsId}
        title="جزئیات واحد"
        onOpenChange={(open) => {
          if (!open) {
            setDetailsId(null);
          }
        }}
        load={loadUnit}
        render={(unit) => (
          <dl className="grid gap-4 rounded-lg border p-4 sm:grid-cols-2">
            <div><dt className="text-xs text-muted-foreground">نام واحد</dt><dd className="mt-1 font-medium">{unit.name}</dd></div>
            <div><dt className="text-xs text-muted-foreground">نماد</dt><dd className="mt-1 font-medium" dir="ltr">{unit.symbol}</dd></div>
            <div><dt className="text-xs text-muted-foreground">نوع واحد</dt><dd className="mt-1 font-medium">{UNIT_TYPE_LABELS[unit.unitType]}</dd></div>
            <div><dt className="text-xs text-muted-foreground">وضعیت</dt><dd className="mt-1 font-medium">{unit.isActive ? "فعال" : "غیرفعال"}</dd></div>
          </dl>
        )}
      />

      <InventoryConfirmDialog
        open={confirmTarget !== null}
        title={confirmTarget?.kind === "delete" ? "حذف واحد" : "تغییر وضعیت واحد"}
        description={
          confirmTarget?.kind === "delete"
            ? `آیا از حذف نرم واحد «${confirmTarget.name}» مطمئن هستید؟ اگر این واحد در کالایی استفاده شده باشد، حذف توسط سرور رد می‌شود.`
            : `وضعیت واحد «${confirmTarget?.name}» به ${confirmTarget?.isActive ? "غیرفعال" : "فعال"} تغییر کند؟`
        }
        confirmLabel={confirmTarget?.kind === "delete" ? "حذف نرم" : "تأیید تغییر"}
        pendingLabel={confirmTarget?.kind === "delete" ? "در حال حذف..." : "در حال تغییر..."}
        error={confirmError}
        pending={pending}
        destructive={confirmTarget?.kind === "delete"}
        onOpenChange={(open) => {
          if (!open) {
            setConfirmTarget(null);
            setConfirmError(null);
          }
        }}
        onConfirm={() => void confirmAction()}
      />
    </>
  );
}
