import * as types from './mutation-types'
import search from './../api/search'
import ai from './../api/ai'

const state = {
    pageContent: null,
    loading: false,
    isEmpty: false,
    scrollingLoader: false,
    search: {
        userText: '',
        page: 0,
        prefix: '',
        term:''
    }
};

const mutations = {
    [types.UPDATE_FILTER](state, text) {
        state.search.userText = text;
    },
    [types.UPDATE_PAGE_CONTENT](state, payload) {
        state.pageContent = null;
        if (!payload.hasOwnProperty('isEmpty')) {
            state.isEmpty = false;
            state.pageContent = payload;
        }else{
            state.pageContent = payload.data;
            state.isEmpty = payload.isEmpty;
        }
        this.commit(types.UPDATE_LOADING, false)
    },
    [types.MERGE_META](state, payload) {
        console.log("merge meta")
        //state.pageContent = payload
    },
    [types.UPDATE_LOADING](state, payload) {
        console.log("update loading")
        state.search.page = payload?0:1;
        state.loading = payload
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
    }
};
const getters = {
    userText: state => state.search.userText,
    pageContent : state => state.pageContent,
    items: state => state.pageContent?state.pageContent.items:null,
    loading : state => state.loading,
    isEmpty: state => state.isEmpty,
    scrollingLoader: state => state.scrollingLoader,
    pageTitle: state => state.pageContent ? state.pageContent.title : null,
    searchParams: state => state.search
}
const actions = {
    updateSearchText: ({ commit }, text) => {
        ai.interpetPromise(text).then(( response ) => {
            console.log(response);
            commit(types.UPDATE_FILTER, text)
            commit(types.ADD, response)
        })
    },
    fetchingData: ( context , page) => {
        context.commit(types.UPDATE_LOADING, true);
        context.commit(types.UPDATE_SEARCH_PARAMS, page.query);
        activateFunction[page.name](context.getters.searchParams).then(response => {
                context.commit(types.UPDATE_PAGE_CONTENT, response);
            })       
    },
    scrollingItems( context , model) {
        console.log("scrollllon");
        context.commit(types.UPDATE_SCROLLING_LOADING, true);
        activateFunction[model.name](context.getters.searchParams).then(response => {
            var items = response;
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
const activateFunction = {
    ask: function (params) {
            return new Promise((resolve, reject) => {
                var items = search.getQna(params);
                if (params.page) items.then(({ body }) => { resolve(body) })
                else {
                    var answer = search.getShortAnswer(params.userText);
                    var video = search.getVideo(params.userText);
                    Promise.all([answer, items, video]).then(([short, items, video]) => {
                        resolve({ title: short.body, items: items.body, video: video.body.url })
                    })
                }
        } )
    },
    note:  (params) => {
        return new Promise((resolve, reject) => {
            search.getDocument(params).then(({ body }) => resolve({ isEmpty: !Boolean(body.item1.length),data:{ items: body.item1, sources: body.item2 }}))
        })
    },
    flashcard: function (params) {
        return new Promise((resolve, reject) => {
            search.getFlashcard(params).then(({ body }) => resolve({ isEmpty: !Boolean(body.item1.length), data: { items: body.item1, sources: body.item2 }}))
        })
    },
    tutor: function (params) {
        return new Promise((resolve, reject) => {
            search.getTutor(params.userText).then(({ body }) => resolve({ items: body }));
        })
    },
    job: function (params) {
        return new Promise((resolve, reject) => {
            search.getJob(params.userText).then(({ body }) => resolve({isEmpty:!Boolean(body.length), data:{ items: body }}));
        })
    },
    book: function (params) {
        return new Promise((resolve, reject) => {
            search.getTutor(params.userText).then(response => resolve({ items: response.item1, sources: response.item2 }));
        })
    },
    purchase: function () {
        return new Promise((resolve, reject) => {
            search.getTutor(state.userText).then(response => resolve({ items: response.item1, sources: response.item2 }));
        })
    }
}
export default {
    state,
    getters,
    actions,
    mutations
}
