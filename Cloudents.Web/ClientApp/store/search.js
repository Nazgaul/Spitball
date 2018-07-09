import {SEARCH} from "./mutation-types"
import searchService from "./../services/searchService"
const LOCATION_VERTICALS= new Map([["tutor",true],["job",true]]);
const state = {
    loading: false,
    search:{}
};

const mutations = {
    [SEARCH.UPDATE_LOADING](state, payload) {
        // debugger;
        state.loading = payload;
    },
    [SEARCH.UPDATE_SEARCH_PARAMS](state, updatedDate) {
        state.search = {...updatedDate};
    }
};

const getters = {
    getIsLoading: state => state.loading
};
const actions = {
    //Always update the current route according the flow
    bookDetails: (context, { pageName, isbn13, type }) => {
        return searchService.activateFunction[pageName]({ isbn13, type });
    },
    getAutocmplete(context, term) {
        return searchService.autoComplete(term);
    },
    nextPage(context, {url,vertical}){
        return searchService.nextPage({url,vertical});
    },
    fetchingData(context, { name, params, page}){
        let university = context.rootGetters.getUniversity ? context.rootGetters.getUniversity : null;
         let paramsList = {...context.state.search,...params, university, page};
            //update box terms
            context.dispatch('updateAITerm',{vertical:name,data:{text:paramsList.term}});
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
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
