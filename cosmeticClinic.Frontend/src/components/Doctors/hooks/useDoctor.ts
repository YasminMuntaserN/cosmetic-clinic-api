import {useMutation} from "@tanstack/react-query";
import {paginatedList, Search} from "../../../services/BaseApi.ts";
import {Pagination, SearchCriteria} from "../../../types/Pagination.ts";
import {Doctor} from "../../../types/doctor.ts";

export function usePaginatedDoctorsList(){
    const { mutate :getPaginatedDoctorsList, data, status, error } = useMutation({
        mutationFn:(PaginationData :Pagination)=>paginatedList("Doctors" , PaginationData),
        mutationKey:["doctors"]
    });

    return { getPaginatedDoctorsList , 
             doctors :data?.data ,
             totalPages :data?.totalPages ,
             totalCount :data?.totalCount , 
             isLoading: status === "pending",
            error };
}

export function useSearchedDoctors() {
    const {
        mutate: getSearchedDoctors,
        data,
        status,
        error,
    } = useMutation({
        mutationFn: (searchCriteria: SearchCriteria): Promise<Doctor[]> =>
            Search("Doctors", searchCriteria),
        mutationKey: ["SearchedDoctors"],
    });

    return {
        getSearchedDoctors,
        SearchedDoctors: data ?? [],
        isLoading: status === "pending",
        error,
    };
}