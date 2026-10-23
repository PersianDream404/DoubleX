export interface PageParams {
  pageSize?: number | null;
  pageNumber?: number | null;
}

export interface PagedResult<TItem> {
  list: TItem[];
  pagination: {
    totalCount: number;
    totalPages: number;
    pageNumber: number;
    pageSize: number;
  };
  extraData: unknown | null;
}
