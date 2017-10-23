import * as types from './mutation-types'
import { prefixes} from './consts'
import ai from './../services/ai'
import searchService from './../services/searchService'

const state = {
    loading: false,
    scrollingLoader: false,
    search: {
        page: 0,
        term: '',
        sort:"Relevance"
    }
};

const mutations = {
    [types.UPDATE_LOADING](state, payload) {        
        state.search.page = payload ? 0 : 1;
        console.log("update loading page:" + state.search.page);
        state.loading = payload
    },
    [types.UPDATE_SEARCH_PARAMS](state, payload) {
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
    updateSearchText(context, text) {
       
        console.log(text);
        if (!text) {
            //TODO: need to figure out what do to in here.
            //context.commit(types.UPDATE_SEARCH_PARAMS, { userText: null });
            return;
        }

        return new Promise((resolve, reject) => {
            ai.interpetPromise(text).then(({ body }) => {
                context.commit(types.UPDATE_SEARCH_PARAMS, { ...body.data });
                    context.commit(types.ADD, { ...body });
                    resolve(context.rootGetters.currenFlow);
            })
        })
    },

    updateLuisAndFetch({commit}, page) {
        return new Promise((resolve) => {
            ai.interpetPromise(page.query.q).then(({ body }) => {
                commit(types.UPDATE_SEARCH_PARAMS, body.data);
                //context.commit(types.ADD, { ...body });
                var params = { ...page.query, ...page.params, ...body.data }
                console.log("params "+params);
                resolve(searchService.activateFunction[page.path.slice(1)](params))
            })
        })
    },
    updateResult(context, page) {
        context.commit(types.UPDATE_LOADING, true)
        searchService.activateFunction[page.path.slice(1)]({ ...context.getters.searchParams, ...page.query, ...page.params })
            .then((response) => {
                context.commit(types.UPDATE_LOADING, false)
            })
    },
    newResultPage: (context, page) => {
        return new Promise((resolve)=>{
            if (context.rootGetters.currenFlow !== page.name) {
                context.commit(types.ADD, { result: page.name })
            }
        })
    },
    bookDetails: (context, data) => {
        return searchService.activateFunction[data.pageName](data.params) 
    },
    fetchingData: (context, data) => {
        return searchService.activateFunction[data.pageName]({ ...data.queryParams, ...context.getters.searchParams })    
    },
    scrollingItems( context , model) {
        return searchService.activateFunction[model.name]({ ...context.getters.searchParams, ...model.params,page: model.page })
    }
}

export default {
    state,
    getters,
    actions,
    mutations
}
