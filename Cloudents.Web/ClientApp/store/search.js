import { SEARCH } from "./mutation-types";
import searchService from "./../services/searchService";

const state = {
    isResetSearch: false,
    //< -----keep this area ----
    loading: false,
    serachLoading: false,
    // -----keep this area ---->


    search: {},
    queItemsPerVertical: {
        //ask: [],
        //note: [],
        tutor: []
    },
    itemsPerVertical: {
        //ask: [],
        //note: [],
        tutor: []
    },
};

const mutations = {
    [SEARCH.UPDATE_LOADING](state, payload) {
        state.loading = payload;
    },
    [SEARCH.UPDATE_SEARCH_LOADING](state, payload) {
        state.serachLoading = payload;
    },
    [SEARCH.SET_ITEMS_BY_VERTICAL](state, verticalObj) {
        state.itemsPerVertical[verticalObj.verticalName] = verticalObj.verticalData;
    },
    [SEARCH.UPDATE_ITEMS_BY_VERTICAL](state, verticalObj) {
        state.itemsPerVertical[verticalObj.verticalName].data = state.itemsPerVertical[verticalObj.verticalName].data.concat(verticalObj.verticalData.data);
        state.itemsPerVertical[verticalObj.verticalName].nextPage = verticalObj.verticalData.nextPage;
    },
    [SEARCH.RESETQUE](state) {
        //check if ask Tab was loaded at least once
        for (let verticalName in state.queItemsPerVertical) {
            state.queItemsPerVertical[verticalName] = [];
        }
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
    getAutocmplete(context, term) {
        return searchService.autoComplete(term);
    },
    setDataByVerticalType({commit}, verticalObj) {
        commit(SEARCH.SET_ITEMS_BY_VERTICAL, verticalObj);
    },
};

export default {
    state,
    getters,
    actions,
    mutations
}
