import {SEARCH,FLOW} from './mutation-types'
import ai from './../services/ai'
import searchService from './../services/searchService'

const state = {
    loading: false,
    scrollingLoader: false,
    search: {
        page: 0,
        term: ''
    }
};

const mutations = {
    [SEARCH.UPDATE_LOADING](state, payload) {
        state.search.page = payload ? 0 : 1;
        state.loading = payload
    },
    [SEARCH.UPDATE_SEARCH_PARAMS](state, payload) {
        state.search = { ...state.search, ...payload }
    }
};

const getters = {
    items: state => state.pageContent?state.pageContent.items:null,
    loading : state => state.loading,
    searchParams: state => state.search,
    term: state => state.search.term ? state.search.term[0]:'',
    luisTerm: state => state.search.term
}
const actions = {
    //Always update the current route according the flow
    updateSearchText(context, text) {
       
        if (!text) {
            //TODO: need to figure out what do to in here.
            //context.commit(types.UPDATE_SEARCH_PARAMS, { userText: null });
            return;
        }

        return new Promise((resolve, reject) => {
            ai.interpetPromise(text).then(({ body }) => {
                context.commit(SEARCH.UPDATE_SEARCH_PARAMS, { ...body.data });
                    context.commit(FLOW.ADD, { ...body });
                    resolve(context.rootGetters.currenFlow);
            })
        })
    },

    updateLuisAndFetch(context, page) {
        return new Promise((resolve,reject) => {
            ai.interpetPromise(page.query.q).then(({ body }) => {
                context.commit(SEARCH.UPDATE_SEARCH_PARAMS, body.data);
                // context.commit(FLOW.ADD, { result:page.path.slice(1) });
                var params = { ...page.query, ...page.params, ...body.data };
                let university = context.rootGetters.getUniversity ? context.rootGetters.getUniversity:null
                if(!body.data.term.length){reject(body.result);}
                else{resolve(searchService.activateFunction[page.path.slice(1)]({ ...params, university }));}
            })
        })
    },
    updateResult(context, page) {
        context.commit(SEARCH.UPDATE_LOADING, true);
        let university = context.rootGetters.getUniversity ? context.rootGetters.getUniversity : null;
        searchService.activateFunction[page.path.slice(1)]({ ...context.getters.searchParams, ...page.query, ...page.params, university })
            .then((response) => {
                context.commit(SEARCH.UPDATE_LOADING, false)
            })
    },
    bookDetails: (context, data) => {
        return searchService.activateFunction[data.pageName](data.params) 
    },
    fetchingData: (context, data) => {
        let university = context.rootGetters.getUniversity ? context.rootGetters.getUniversity : null;
        return searchService.activateFunction[data.pageName]({ ...context.getters.searchParams,...data.queryParams, university })
    },
    scrollingItems( context , model) {
        return searchService.activateFunction[model.name]({ ...context.getters.searchParams, ...model.params,page: model.page })
    },
    getPreview(context, model) {
        return searchService.getPreview(model)
    }

}

export default {
    state,
    getters,
    actions,
    mutations
}
