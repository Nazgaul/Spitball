﻿import { SESSION, FLOW } from "./mutation-types"
import { interpetPromise } from "./../services/resources"
import searchService from "./../services/searchService"
import { verticals } from "../data";

const state = {
    loading: false,
    location: null,
    docType: null,
    search: {
        term: [],
        foodTerm: [],
        jobTerm: []
    }
};

const mutations = {
    [SESSION.UPDATE_LOADING](state, payload) {
        //state.search.page = payload ? 0 : 1;
        state.loading = payload;
    },
    [SESSION.UPDATE_SEARCH_PARAMS](state, { vertical, term, location, docType }) {
        state.location = location;
        state.docType = docType;
        if (vertical == "job") {
            state.search.jobTerm = term;
        }
        else if (vertical == "food") {
            state.search.foodTerm = term;
        }
        else {
            state.search.term = term;
        }
        //state.search = { ...state.search, term, location };
    }
};

const getters = {
    //items: state => state.pageContent ? state.pageContent.items : null,
    loading: state => state.loading,

    searchParams: (state) => (vertical) => {
        if (vertical == verticals.food.id) {
            return {
                term: state.search.foodTerm
            }
        }
        if (vertical == verticals.job.id) {
            return {
                term: state.search.jobTerm
            }
        }
        if (vertical == verticals.note.id) {
            return {
                term:  state.search.term,
                docType: state.search.docType
            }
        }
        return {
            term: state.search.term
        }
    }

    //searchParams: state => state.search,
    //term: state => state.search.term ? state.search.term[0] : "",
    //luisTerm: state => state.search.term
};
const actions = {
    //Always update the current route according the flow
    updateSearchText(context, text) {
        if (!text) {
            return Promise.resolve("");
        }
        return interpetPromise(text).then(({ data: body }) => {
            let params = { ...body };
            return new Promise((resolve) => {
                //if AI return cords use it
                if (params.hasOwnProperty('cords')) {
                    params.location = `${params.cords.latitude},${params.cords.longitude}`;
                    resolve();
                }
                resolve();
                //else if (["tutor", "job", "food", "purchase"].includes(body.vertical)) {
                //    //if it's vertical that need location get it from my location
                //    context.dispatch("updateLocation").then((location) => {
                //        params.location = location;
                //        resolve();
                //    });
                //} else { resolve(); }
            }).then(() => {
                context.commit(SESSION.UPDATE_SEARCH_PARAMS, { ...params });
                return { result: body.vertical };
            });

        });
    },

    bookDetails: (context, { pageName, isbn13, type }) => {
        return searchService.activateFunction[pageName]({ isbn13, type });
    },
    foodDetails: (context, { id }) => {
        return searchService.activateFunction["foodDetails"]({ id });
    },

    fetchingData: (context, { name, page }) => {
        let university = context.rootGetters.getUniversity || null;
        let paramsList = { ...context.getters.searchParams(name), university, page }
        //let paramsList = { ...context.getters.searchParams, ...params, university, page, docType };
        //if (term !== undefined) {
        //    paramsList = { ...paramsList, term };
        //}
        //get location if needed
        return new Promise((resolve) => {
            if (verticals[name].needLocation) {
                context.dispatch("updateLocation").then((location) => {
                    paramsList.location = location;
                    resolve();
                });
            } else { resolve(); }
        }).then(() => {
            return searchService.activateFunction[name](paramsList);
        });
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
