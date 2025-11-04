export type Pagination = {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export type PaginatedResult<T> = {
    // you may use pagination with any type of data (members, messages, photos etc...)
  items: T[];
  metadata: Pagination;
};