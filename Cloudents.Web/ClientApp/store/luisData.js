﻿import {LUIS} from "./mutation-types"
// import { interpetPromise } from "./../services/resources"
// import searchService from "./../services/searchService"
let general={term:"",text:""};
const state = {
    luis:{
        job:{term:"",text:""},
        food:{term:"",text:""},
        ask:general,
        flashcard:general
    }
    // loading: false,
    // search: {
    //     page: 0,
    //     term: ""
    // }
};

const mutations = {
    [LUIS.UPDATE_TERM](state, {vertical,data}) {
        state[vertical]=data;
    }
    // [SEARCH.UPDATE_SEARCH_PARAMS](state, { term, location }) {
    //     state.search = { ...state.search, term, location };
    // }
};

const getters = {
    // items: state => state.pageContent ? state.pageContent.items : null,
    // loading: state => state.loading,
    // searchParams: state => state.search,
    // term: state => state.search.term ? state.search.term[0] : "",
    // luisTerm: state => state.search.term
};
const actions = {
    updateTerm({commit},{vertical,data}){
        commit(LUIS.UPDATE_TERM,{vertical,data})
    }
    //Always update the current route according the flow
    // updateSearchText(context, text) {
    //     if (!text) {
    //         return Promise.resolve("");
    //     }
    //     return interpetPromise(text).then(({ data: body }) => {
    //         let params = { ...body };
    //         context.commit(UPDATE_GLOBAL_TERM,text);
    //         return new Promise((resolve) => {
    //             //if AI return cords use it
    //             if (params.hasOwnProperty('cords')) {
    //                 params.location = `${params.cords.latitude},${params.cords.longitude}`;
    //                 resolve();
    //             } else if (["tutor", "job", "food", "purchase"].includes(body.vertical)) {
    //                 //if it's vertical that need location get it from my location
    //                 context.dispatch("updateLocation").then((location) => {
    //                     params.location = location;
    //                     resolve();
    //                 });
    //             } else { resolve(); }
    //         }).then(() => {
    //             context.commit(SEARCH.UPDATE_SEARCH_PARAMS, { ...params });
    //             return { result: body.vertical, term: body.term, docType: body.docType };
    //         });
    //
    //     });
    // },
    //
    // bookDetails: (context, { pageName, isbn13, type }) => {
    //     return searchService.activateFunction[pageName]({ isbn13, type });
    // },
    // foodDetails: (context, { id }) => {
    //     return searchService.activateFunction["foodDetails"]({ id });
    // },
    //
    // fetchingData: (context, { name, params, page, luisTerm: term, docType }) => {
    //     console.log(term);
    //     let university = context.rootGetters.getUniversity ? context.rootGetters.getUniversity : null;
    //     let paramsList = { ...context.getters.searchParams, ...params, university, page, docType };
    //     if (term !== undefined) {
    //         if(!term){context.commit(SEARCH.UPDATE_SEARCH_PARAMS, { term });}
    //         paramsList = { ...paramsList, term };
    //     }
    //     //get location if needed
    //     return new Promise((resolve) => {
    //         if (!paramsList.term && !paramsList.location && ["tutor", "job", "food", "purchase"].includes(name)) {
    //             context.dispatch("updateLocation").then((location) => {
    //                 paramsList.location = location;
    //                 resolve();
    //             });
    //         } else { resolve(); }
    //     }).then(() => {
    //         return searchService.activateFunction[name](paramsList);
    //     });
    // }
};

export default {
    state,
    getters,
    actions,
    mutations
}
