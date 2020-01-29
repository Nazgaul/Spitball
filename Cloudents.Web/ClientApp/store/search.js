import { SEARCH } from "./mutation-types";

const state = {
    isResetSearch: false,
    //< -----keep this area ----
    loading: false,
    serachLoading: false,
    // -----keep this area ---->
};

const mutations = {
    [SEARCH.UPDATE_LOADING](state, payload) {
        state.loading = payload;
    },
    [SEARCH.UPDATE_SEARCH_LOADING](state, payload) {
        state.serachLoading = payload;
    },
    setResetSearch(state){
        state.isResetSearch = !state.isResetSearch;
    }
};

const getters = {
    getIsLoading: state => state.loading,
    getSearchLoading: state => state.serachLoading,
    getSearchStatus: state => state.isResetSearch
};

const actions = {
    resetSearch({commit}){
        commit('setResetSearch');
    },
};

export default {
    state,
    getters,
    actions,
    mutations
}
