import axios from "axios";
import {PaginatedResponse, Pagination, SearchCriteria} from "../types/Pagination.ts";

//const AUTH_TOKEN = localStorage.getItem('accessToken');
const AUTH_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI2N2RiNDQ1ZDhhNjhmYzBkOWMxNDMyZTEiLCJlbWFpbCI6InlhcmFAZ21haWwuY29tIiwicm9sZSI6IkFkbWluIiwiUGVybWlzc2lvbnMiOiI0MTk0MzAzIiwibmJmIjoxNzQyODQyNjY4LCJleHAiOjE3NDI4NDk4NjgsImlhdCI6MTc0Mjg0MjY2OCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3QiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUxNzMvIn0.JcVLva7QLxYU-9QKgemHOD32zIkmeF891yoflAqIrFE";
const apiClient = axios.create({
    baseURL: "http://localhost:5030/api",
    headers: {
        "Content-Type": "application/json",
        "Accept": "application/json",
        "Authorization": `Bearer ${AUTH_TOKEN}`,
    },
});

export async function addEntity(entityName:string,data:any) {
    try {
        const response = await apiClient.post( `/${entityName}`, data, {
            headers: {
                Authorization: `Bearer ${AUTH_TOKEN}`, 
                Accept: "text/plain", 
            },
        });

        return response.data;
    } catch (error) {
        console.error("Error adding doctor:", error);
        throw error;
    }
}
//http://localhost:5030/api/Doctors/paginated?PageNumber=1&PageSize=5&OrderBy=firstName' \
//http://localhost:5030/api/Doctors/paginated?PageNumber=1&PageSize=5&OrderBy=firstName
export async function paginatedList(
    entityName: string,
    pagination: Pagination): Promise<PaginatedResponse> {
    try {
        const response = await apiClient.get<PaginatedResponse>(
            `/${entityName}/paginated?PageNumber=${pagination.PageNumber}&PageSize=${pagination.PageSize}&OrderBy=${pagination.OrderBy}`,{
            headers: {
                Authorization: `Bearer ${AUTH_TOKEN}`,
                Accept: "text/plain",
            }
        },
        );

        return response.data; 
    } catch (error) {
        console.error(`Error fetching paginated ${entityName}:`, error);
        throw error; 
    }
}

//http://localhost:5030/api/Products/getByCategory?category=Skincare
export async function getAllBy<T>(entityName: string, value: string): Promise<T[]> {
    try {
        const response = await apiClient.get<T[]>(
            `/${entityName}/${value}`,
            {
                headers: {
                    Authorization: `Bearer ${AUTH_TOKEN}`,
                    Accept: "text/plain",
                },
            }
        );
        return response.data;
    } catch (error) {
        console.error(`Error fetching paginated ${entityName}:`, error);
        throw error;
    }
}

//http://localhost:5030/api/Products/search
export async function Search<T>( entityName:string ,searchCriteria :SearchCriteria): Promise<T[]> {
    try {
        const response = await apiClient.post<T[]>(`/${entityName}/search`, searchCriteria,
            {
                headers: {
                    Authorization: `Bearer ${AUTH_TOKEN}`,
                    Accept: "text/plain",
                    "Content-Type": "application/json"
                }
            }
        );
        return response.data;
    } catch (error) {
        console.error(`Error searching ${entityName}:`, error);
        throw error;
    }
}