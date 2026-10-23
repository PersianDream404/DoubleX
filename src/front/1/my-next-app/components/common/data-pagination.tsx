"use client";

import Link from "next/link";

type DataPaginationProps = {
  pathname: string;
  page: number;
  totalPages: number;
  pageSize?: number;
  search?: string;
};

export function DataPagination({
  pathname,
  page,
  totalPages,
  pageSize = 10,
  search,
}: DataPaginationProps) {
  if (totalPages <= 1) {
    return null;
  }

  const createPageUrl = (pageNumber: number) => {
    const params = new URLSearchParams();

    if (search) {
      params.set("search", search);
    }

    params.set("page", String(pageNumber));
    params.set("pageSize", String(pageSize));

    return `${pathname}?${params.toString()}`;
  };

  return (
    <div className="mt-4 flex items-center justify-center gap-1">
      {/* قبلی */}
      {page > 1 && (
        <Link
          href={createPageUrl(page - 1)}
          className="rounded-md border px-3 py-2 text-sm transition hover:bg-muted"
        >
          قبلی
        </Link>
      )}

      {/* شماره صفحات */}
      {Array.from({ length: totalPages }, (_, index) => index + 1).map(
        (pageNumber) => (
          <Link
            key={pageNumber}
            href={createPageUrl(pageNumber)}
            className={`rounded-md border px-3 py-2 text-sm transition ${
              pageNumber === page
                ? "bg-primary text-primary-foreground"
                : "hover:bg-muted"
            }`}
          >
            {pageNumber}
          </Link>
        ),
      )}

      {/* بعدی */}
      {page < totalPages && (
        <Link
          href={createPageUrl(page + 1)}
          className="rounded-md border px-3 py-2 text-sm transition hover:bg-muted"
        >
          بعدی
        </Link>
      )}
    </div>
  );
}
