﻿import { SEARCH, FLOW } from "./mutation-types"
import {interpetPromise} from "./../services/resources"
import searchService from "./../services/searchService"

const state = {
    loading: false,
    scrollingLoader: false,
    search: {
        page: 0,
        term: ""
    }
};

const mutations = {
    [SEARCH.UPDATE_LOADING](state, payload) {
        state.search.page = payload ? 0 : 1;
        state.loading = payload;
    },
    [SEARCH.UPDATE_SEARCH_PARAMS](state, {term,location}) {
        if(!location&&!state.search.location&&navigator.geolocation){
            navigator.geolocation.getCurrentPosition(({ coords }) => {
                coords = coords || {};
                state.search.location = coords.latitude + ',' + coords.longitude;
            })
        }
        state.search = { ...state.search, term,location} ;
    }
};

const getters = {
    items: state => state.pageContent ? state.pageContent.items : null,
    loading: state => state.loading,
    searchParams: state => state.search,
    term: state => state.search.term ? state.search.term[0] : "",
    luisTerm: state => state.search.term
};
const actions = {
    //Always update the current route according the flow
    updateSearchText(context, text) {
        console.log(context);
            if (!text) {
                return ""
            }
           return interpetPromise(text).then(({data:body}) => {
                let params={...body};
                if(params.hasOwnProperty('cords')){
                    params.location=`${params.cords.latitude},${params.cords.longitude}`
                }
                context.commit(SEARCH.UPDATE_SEARCH_PARAMS, { ...params });
                return{ result: body.vertical, term: body.term };
            });
    },

    bookDetails: (context, {pageName,isbn13,type}) => {
        return searchService.activateFunction[pageName]({isbn13,type});
    },

    fetchingData: (context, { name, params, page, luisTerm: term }) => {
        let university = context.rootGetters.getUniversity ? context.rootGetters.getUniversity : null;
        return searchService.activateFunction[name]({ ...context.getters.searchParams, ...params, university, page, term });
    },

    getPreview(context, model) {
        return searchService.getPreview(model);
    }

};

export default {
    state,
    getters,
    actions,
    mutations
}
