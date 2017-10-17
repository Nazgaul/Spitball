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
    searchPrefix: state => prefixes[state.search.type],
    term: state => state.search.term ? state.search.term[0]:''
}
const actions = {
    updateSearchText(context, text) {
       
        let params = {};
        let isHome = true;
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
            isHome = false;
        }
        return new Promise((resolve, reject) => {
            ai.interpetPromise(params.prefix, params.str).then(({ body }) => {
                context.commit(types.UPDATE_SEARCH_PARAMS, { ...body.data, userText: params.str });
                if (isHome) {
                    context.commit(types.ADD, { ...body });
                    resolve(context.rootGetters.currenFlow);
                    //context.dispatch(types.PAGE_RELOAD, context.rootGetters.currenFlow);
                } else {
                    context.dispatch('fetchingData', { pageName: context.state.search.type, queryParams: {} })
                }
            })
        })
    },

    //[types.PAGE_RELOAD](state, payload) {
    //    console.log("page reload " + payload);
    //},
    newResultPage: (context, page) => {
        context.commit(types.UPDATE_SEARCH_PARAMS, { type: page.name })
        if (context.rootGetters.currenFlow != page.name) {
            context.commit(types.ADD, { result: page.name })
        }
        context.dispatch('fetchingData', {pageName:page.name,queryParams:page.query});
    },
    bookDetails: (context, page) => {
        context.dispatch('fetchingData', { pageName: page.name, queryParams: page.params });
    },
    fetchingData: (context,data) => {
        context.commit(types.UPDATE_LOADING, true);
        activateFunction[data.pageName]({ ...context.getters.searchParams, ...data.queryParams}).then(response => {
            context.commit(types.PAGE_LOADED, response);
            })       
    },
    scrollingItems( context , model) {
        return activateFunction[model.name]({ ...context.getters.searchParams, ...model.params,page: model.page })
    }
}

export default {
    state,
    getters,
    actions,
    mutations
}
