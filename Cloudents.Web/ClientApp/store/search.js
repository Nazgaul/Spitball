﻿import {SEARCH, LUIS} from "./mutation-types"
import { interpetPromise } from "./../services/resources"
import searchService from "./../services/searchService"
const LOCATION_VERTICALS= new Map([["tutor",true],["job",true], ["food",true], ["purchase",true]]);
const state = {
    loading: false,
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
    [SEARCH.UPDATE_SEARCH_PARAMS](state, { term }) {
        state.search = { ...state.search, term };
    }
};

const getters = {
    items: state => state.pageContent ? state.pageContent.items : null,
    loading: state => state.loading,
    searchParams: state => state.search,
    term: state => state.search.term ? state.search.term[0] : ""
};
const actions = {
    //Always update the current route according the flow
    updateSearchText(context, {text,vertical}) {
        if (!text) {
            context.commit(LUIS.UPDATE_TERM,{vertical,data:{text,term:""}});
            return Promise.resolve("");
        }
        return interpetPromise(text).then(({ data: body }) => {
            let params = { ...body };
            let {docType,term}=body;
            let currentVertical=vertical?vertical:body.vertical;
            context.commit(LUIS.UPDATE_TERM,{vertical:currentVertical,data:{text,term,docType}});
            return new Promise((resolve) => {
                //if AI return cords use it
                if (params.hasOwnProperty('cords')) {
                    params.location = {latitude:params.cords.latitude,longitude:params.cords.longitude};
                    resolve();
                } else if (LOCATION_VERTICALS.has(currentVertical)) {
                    //if it's vertical that need location get it from my location
                    context.dispatch("updateLocation").then((location) => {
                        params.location = location;
                        resolve();
                    });
                } else { resolve(); }
            }).then(() => {
                context.commit(SEARCH.UPDATE_SEARCH_PARAMS, { ...params });
                return { result: body.vertical, term: body.term, docType: body.docType };
            });

        });
    },

    bookDetails: (context, { pageName, isbn13, type }) => {
        return searchService.activateFunction[pageName]({ isbn13, type });
    },
    foodDetails: (context, { id }) => {
        return searchService.activateFunction["foodDetails"]({ id });
    },

    fetchingData(context, { name, params, page}) {
        let university = context.rootGetters.getUniversity ? context.rootGetters.getUniversity : null;
        return context.dispatch('getAIDataForVertical',name).then((aiData)=>{
            let paramsList = { ...context.getters.searchParams, ...params, university, page, ...aiData};
            //get location if needed
            return new Promise((resolve) => {
                if (LOCATION_VERTICALS.has(name) && !paramsList.location) {
                    context.dispatch("updateLocation").then((location) => {
                        paramsList.location = location;
                        resolve();
                    });
                } else { resolve(); }
            }).then(() => {
                return searchService.activateFunction[name](paramsList);
            });
        })
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
