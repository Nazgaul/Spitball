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
        context.commit(types.UPDATE_SEARCH_PARAMS, { type: page.name })

        if (context.getters.userText) {
            //if page have usertext and has been changed call luis again
            if (page.meta.userText && context.getters.userText !== page.meta.userText) {
                context.dispatch('updateSearchText', page.query.userText)
            } else {
                context.dispatch('fetchingData', page.name);
            }
        }
    },
    fetchingData: (context,pageName) => {
        context.commit(types.UPDATE_LOADING, true);
        activateFunction[pageName](context.getters.searchParams).then(response => {
            context.commit(types.PAGE_LOADED, response);
            })       
    },
    scrollingItems( context , model) {
        return activateFunction[model.name]({ ...context.getters.searchParams, page: model.page })
    }
}

export default {
    state,
    getters,
    actions,
    mutations
}
