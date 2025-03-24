import {useMutation} from "@tanstack/react-query";
import {paginatedList, Search} from "../../../services/BaseApi.ts";
import {Pagination, SearchCriteria} from "../../../types/Pagination.ts";
import {Patient} from "../../../types/patient.ts";

export function usePaginatedPatientsList(){
    const { mutate :getPaginatedPatientsList, data, status, error } = useMutation({
        mutationFn:(PaginationData :Pagination)=>paginatedList("Patients" , PaginationData),
        mutationKey:["patients"]
    });

    return { getPaginatedPatientsList , 
             patients :data?.data ,
             totalPages :data?.totalPages ,
             totalCount :data?.totalCount , 
             isLoading: status === "pending",
            error };
}

export function useSearchedPatients() {
    const {
        mutate: getSearchedPatients,
        data,
        status,
        error,
    } = useMutation({
        mutationFn: (searchCriteria:SearchCriteria):Promise<Patient[]> =>
            Search("Patients" ,searchCriteria),
        mutationKey: ["SearchedPatients"],
    });

    return {
        getSearchedPatients,
        SearchedPatients: data ?? [],
        isLoading: status === "pending",
        error,
    };
}