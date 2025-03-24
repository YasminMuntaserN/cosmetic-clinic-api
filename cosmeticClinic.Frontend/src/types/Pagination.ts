import {Doctor} from "./doctor.ts";
import {Patient} from "./patient.ts";
import {Product} from "./product.ts";

export interface Pagination {
    PageNumber: number;
    PageSize: number;
    OrderBy: string;
    Ascending?:boolean;
}

export interface PaginatedResponse {
    data :Doctor[] | Patient[] |Product[] |any;
    totalPages : number;
    totalCount:number;
}

export interface SearchCriteria {
    value :string;
    field : string;
}