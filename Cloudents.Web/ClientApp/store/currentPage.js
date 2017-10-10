import * as types from './mutation-types'
import { prefixes, activateFunction} from './consts'
import ai from './../api/ai'

const state = {
    loading: false,
    scrollingLoader: false,
    search: {
        userText: '',
        page: 0,
        type: '',
        term: '',
        sort:"Relevance"
    }
};

const mutations = {
    [types.UPDATE_FILTER](state, text) {
        state.search.userText = text;
    },
    [types.UPDATE_LOADING](state, payload) {        
        updateLoaded(state, payload);
    },
    [types.UPDATE_ITEM_LIST](state, payload) {
        console.log("update item list")
        state.pageContent.items = [...state.pageContent.items, ...payload];
        state.search.page++;
    },
    [types.UPDATE_SCROLLING_LOADING](state, payload) {
        console.log("update scroll loader")
        state.scrollingLoader = payload
    },
    [types.UPDATE_SEARCH_PARAMS](state, payload) {
        console.log(payload);
        state.search = { ...state.search, ...payload }
    },
    [types.PAGE_LOADED](state, payload) {
        console.log("page loaded " + payload);
        updateLoaded(state, false);
    }
};
function updateLoaded(state,val) {
    state.search.page = val ? 0 : 1;
    console.log("update loading page:" + state.search.page);
    state.loading = val
}
const getters = {
    userText: state => state.search.userText,
    items: state => state.pageContent?state.pageContent.items:null,
    loading : state => state.loading,
    scrollingLoader: state => state.scrollingLoader,
    searchParams: state => state.search,
    searchPrefix: state => prefixes[state.search.type]
}
const actions = {
    updateSearchText: (context, text) => {
        let params = {};
        console.log(text);
        if (!text){
            context.commit(types.UPDATE_SEARCH_PARAMS, { userText: null });
            return;
        }

        if (typeof text === typeof {}) {
            params = text;
        }
        else {
            params.prefix = context.getters.searchPrefix;
            params.str = text;
        }
        ai.interpetPromise(params.prefix, params.str).then(({ body }) => {
            context.commit(types.UPDATE_SEARCH_PARAMS, { ...body.data, userText: params.str });
            context.commit(types.ADD, body)
            if (context.state.search.type)context.dispatch('fetchingData', context.state.search.type);
        })
    },
    newResultPage: (context, page) => {
        console.log(page.name);
        //let query=page.query
        //if (page.name !== context.state.search.type) { query = { ...page.query, source: null, sort: "relevance" } }
        context.commit(types.UPDATE_SEARCH_PARAMS, { type: page.name })
       
        //if page have usertext and has been changed call luis again
        if (page.meta.userText && context.getters.user !== page.meta.userText) {
            context.dispatch('updateSearchText', page.meta.userText)
        } else {
            context.dispatch('fetchingData', page.name);
        }
    },
    fetchingData: (context,pageName) => {
        context.commit(types.UPDATE_LOADING, true);
        activateFunction[pageName](context.getters.searchParams).then(response => {
            //context.commit(types.UPDATE_PAGE_CONTENT, response);
            context.commit(types.PAGE_LOADED, response);
            })       
    },
    scrollingItems( context , model) {
        context.commit(types.UPDATE_SCROLLING_LOADING, true);
        activateFunction[model.name](context.getters.searchParams).then(response => {
            var items = response.items;
            if (response.hasOwnProperty('data'))
            {
                items = response.data.items;
            }
                              
            if (!items.length) {
                model.scrollState.complete();
                return false;
            }
            else {
                context.commit(types.UPDATE_ITEM_LIST, items); 
                model.scrollState.loaded();
                context.commit(types.UPDATE_SCROLLING_LOADING, false);
             }
        })
    }
}

export default {
    state,
    getters,
    actions,
    mutations
}
