"use client";

import { useCallback, useState } from "react";
import { handleApiError } from "@/lib/api/handle-api-error";

import { InventoryConfirmDialog } from "../shared/inventory-confirm-dialog";
import { InventoryDetailsDialog } from "../shared/inventory-details-dialog";
import {
  InventoryListPage,
  type InventoryColumn,
} from "../shared/inventory-list-page";
import { MaterialFormDialog } from "./material-form-dialog";
import { MaterialService } from "./material.service";
import { useMaterials } from "./use-materials";
import type { MaterialDto } from "./material.types";
import {
  StockMovementDialog,
  type StockMovementTarget,
} from "../stock-movements/stock-movement-dialog";

type ConfirmTarget = {
  kind: "delete" | "status";
  id: number;
  name: string;
  isActive: boolean;
};

const COLUMNS: InventoryColumn<MaterialDto>[] = [
  { key: "name", title: "نام کالا", render: (material) => material.name },
  { key: "code", title: "کد کالا", render: (material) => <span dir="ltr">{material.code}</span> },
  { key: "brand", title: "برند", render: (material) => material.brand || "—" },
  {
    key: "base-unit",
    title: "واحد پایه",
    render: (material) => `${material.baseUnitName} (${material.baseUnitSymbol})`,
  },
  {
    key: "purchase-unit",
    title: "واحد خرید",
    render: (material) => `${material.purchaseUnitName} (${material.purchaseUnitSymbol})`,
  },
  {
    key: "stock",
    title: "موجودی / حداقل",
    render: (material) =>
      `${material.currentStock.toLocaleString("fa-IR")} / ${material.minimumStock.toLocaleString("fa-IR")}`,
  },
];

export function MaterialsPage() {
  const [query, setQuery] = useState("");
  const [page, setPage] = useState(1);
  const [refreshKey, setRefreshKey] = useState(0);
  const [formOpen, setFormOpen] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [detailsId, setDetailsId] = useState<number | null>(null);
  const [stockMovementTarget, setStockMovementTarget] =
    useState<StockMovementTarget | null>(null);
  const [stockMovementOperation, setStockMovementOperation] =
    useState<"receipt" | "consumption">("receipt");
  const [confirmTarget, setConfirmTarget] = useState<ConfirmTarget | null>(null);
  const [confirmError, setConfirmError] = useState<string | null>(null);
  const [pending, setPending] = useState(false);

  const { data, error, loading } = useMaterials(query, page, refreshKey);
  const materials = data?.list ?? [];
  const totalPages = Math.max(1, data?.pagination.totalPages ?? 1);

  const loadMaterial = useCallback(async (id: number, signal: AbortSignal) => {
    const response = await new MaterialService().getById(id, { signal });
    if (!response.data) {
      throw new Error(response.message ?? "اطلاعات کالا دریافت نشد.");
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
      const service = new MaterialService();
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
        title="فهرست کالاها"
        breadcrumbHref="/inventory/materials"
        searchPlaceholder="جستجو بر اساس نام، کد یا برند کالا..."
        query={query}
        onQueryChange={(value) => {
          setQuery(value);
          setPage(1);
        }}
        page={page}
        onPageChange={setPage}
        totalPages={totalPages}
        items={materials}
        columns={COLUMNS}
        loading={loading}
        error={error}
        onCreate={() => {
          setEditingId(null);
          setFormOpen(true);
        }}
        onView={(material) => setDetailsId(material.id)}
        onEdit={(material) => {
          setEditingId(material.id);
          setFormOpen(true);
        }}
        onSetActive={(material) =>
          requestConfirmation({
            kind: "status",
            id: material.id,
            name: material.name,
            isActive: material.isActive,
          })
        }
        onDelete={(material) =>
          requestConfirmation({
            kind: "delete",
            id: material.id,
            name: material.name,
            isActive: material.isActive,
          })
        }
        onIncreaseStock={(material) => {
          setStockMovementOperation("receipt");
          setStockMovementTarget({
            material,
            idempotencyKey: crypto.randomUUID(),
          });
        }}
        onWithdrawStock={(material) => {
          setStockMovementOperation("consumption");
          setStockMovementTarget({
            material,
            idempotencyKey: crypto.randomUUID(),
          });
        }}
      />

      <MaterialFormDialog
        open={formOpen}
        materialId={editingId}
        onOpenChange={(open) => {
          setFormOpen(open);
          if (!open) {
            setEditingId(null);
          }
        }}
        onSaved={() => setRefreshKey((current) => current + 1)}
      />

      <StockMovementDialog
        key={stockMovementTarget?.idempotencyKey}
        target={stockMovementTarget}
        operation={stockMovementOperation}
        onOpenChange={(open) => {
          if (!open) {
            setStockMovementTarget(null);
          }
        }}
        onSaved={() => setRefreshKey((current) => current + 1)}
      />

      <InventoryDetailsDialog
        open={detailsId !== null}
        id={detailsId}
        title="جزئیات کالا"
        onOpenChange={(open) => {
          if (!open) {
            setDetailsId(null);
          }
        }}
        load={loadMaterial}
        render={(material) => (
          <dl className="grid gap-3 rounded-lg border p-4 sm:grid-cols-2">
            <div><dt className="text-xs text-muted-foreground">نام کالا</dt><dd className="mt-1 font-medium">{material.name}</dd></div>
            <div><dt className="text-xs text-muted-foreground">کد کالا</dt><dd className="mt-1 font-medium" dir="ltr">{material.code}</dd></div>
            <div><dt className="text-xs text-muted-foreground">برند</dt><dd className="mt-1 font-medium">{material.brand || "—"}</dd></div>
            <div><dt className="text-xs text-muted-foreground">وضعیت</dt><dd className="mt-1 font-medium">{material.isActive ? "فعال" : "غیرفعال"}</dd></div>
            <div><dt className="text-xs text-muted-foreground">واحد پایه</dt><dd className="mt-1 font-medium">{material.baseUnitName} ({material.baseUnitSymbol})</dd></div>
            <div><dt className="text-xs text-muted-foreground">واحد خرید</dt><dd className="mt-1 font-medium">{material.purchaseUnitName} ({material.purchaseUnitSymbol})</dd></div>
            <div><dt className="text-xs text-muted-foreground">ضریب تبدیل</dt><dd className="mt-1 font-medium">{material.purchaseConversionFactor.toLocaleString("fa-IR")}</dd></div>
            <div><dt className="text-xs text-muted-foreground">حداقل موجودی (واحد پایه)</dt><dd className="mt-1 font-medium">{material.minimumStock.toLocaleString("fa-IR")}</dd></div>
            <div><dt className="text-xs text-muted-foreground">موجودی فعلی (واحد پایه)</dt><dd className="mt-1 font-medium">{material.currentStock.toLocaleString("fa-IR")}</dd></div>
          </dl>
        )}
      />

      <InventoryConfirmDialog
        open={confirmTarget !== null}
        title={confirmTarget?.kind === "delete" ? "حذف کالا" : "تغییر وضعیت کالا"}
        description={
          confirmTarget?.kind === "delete"
            ? `آیا از حذف نرم کالای «${confirmTarget.name}» مطمئن هستید؟ سابقه گردش انبار حذف نمی‌شود.`
            : `وضعیت کالای «${confirmTarget?.name}» به ${confirmTarget?.isActive ? "غیرفعال" : "فعال"} تغییر کند؟`
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
