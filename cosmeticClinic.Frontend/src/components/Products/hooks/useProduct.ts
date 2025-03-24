import {useMutation} from "@tanstack/react-query";
import {Pagination, SearchCriteria} from "../../../types/Pagination.ts";
import {getAllBy, paginatedList, Search} from "../../../services/BaseApi.ts";
import {Product} from "../../../types/product.ts";

export function usePaginatedProductsList(){
    const { mutate :getPaginatedProductsList, data, status, error } = useMutation({
        mutationFn:(PaginationData :Pagination)=>paginatedList("Products" , PaginationData),
        mutationKey:["Products"]
    });

    return { getPaginatedProductsList ,
        products :data?.data ,
        totalPages :data?.totalPages ,
        totalCount :data?.totalCount ,
        isLoading: status === "pending",
        error };
}

export function useProductsListByCategory() {
    const {
        mutate: getProductsByCategory,
        data,
        status,
        error,
    } = useMutation({
        mutationFn: (category: string):Promise<Product[]> =>
            getAllBy("Products", `getByCategory?category=${category}`), 
        mutationKey: ["ProductsByCategory"],
    });

    return {
        getProductsByCategory,
        productsByCategory: data ?? [], 
        isLoading: status === "pending",
        error,
    };
}

export function useSearchedProducts() {
    const {
        mutate: getSearchedProducts,
        data,
        status,
        error,
    } = useMutation({
        mutationFn: (searchCriteria:SearchCriteria):Promise<Product[]> =>
            Search("Products" ,searchCriteria),
        mutationKey: ["SearchedProducts"],
    });

    return {
        getSearchedProducts,
        SearchedProducts: data ?? [],
        isLoading: status === "pending",
        error,
    };
}
