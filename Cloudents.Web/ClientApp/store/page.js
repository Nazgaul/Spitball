import * as types from './mutation-types'
import search from './../api/search'

const state = {
    pageContent: null,
    loading: false,
    isEmpty: false,
    scrollingLoader: false,
    search: {
        userText: '',
        page:1
    }
};

const mutations = {
    [types.UPDATE_FILTER](state, text) {
        state.search.userText = text;
    },
    [types.UPDATE_PAGE_CONTENT](state, payload) {
        state.pageContent = null;
        state.search.page = 1;
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
    pageTitle: state => state.pageContent ? state.pageContent.title : null
}
const actions = {
    updateSearchText: ({ commit }, text) => commit(types.UPDATE_FILTER, text),
    fetchingData: ({ commit }, page) => {
        commit(types.UPDATE_LOADING, true);
        commit(types.UPDATE_SEARCH_PARAMS, page.query);        
            activateFunction[page.name]({}).then(response => {
                commit(types.UPDATE_PAGE_CONTENT, response);
            })       
    },
    scrollingItems({ commit }, model) {
        console.log("scrollllon");
        commit(types.UPDATE_SCROLLING_LOADING, true);
        activateFunction[model.name]({ page:state.search.page }).then(response =>{
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
                commit(types.UPDATE_ITEM_LIST, items); 
                model.scrollState.loaded();
                commit(types.UPDATE_SCROLLING_LOADING, false);
             }
        })

        //loadMore[name]().then()
    }
}
const activateFunction = {
    ask: function (more) {
        if (more) {
            search.getQna(more).then(({ body }) => {resolve(body)})
        }
        return new Promise((resolve, reject) => {
            var items = search.getQna({});
            var answer = search.getShortAnswer(state.userText);
            var video = search.getVideo(state.userText);
            Promise.all([answer, items,video]).then(([short, items,video]) => {
                resolve({ title: short.body, items: items.body,video: video.body.url})
            })
        } )
    },
    note:  (more) => {
        return new Promise((resolve, reject) => {
            search.getDocument(more).then(({ body }) => resolve({ isEmpty: Boolean(body.item1.length),data:{ items: body.item1, sources: body.item2 }}))
        })
    },
    flashcard: function (more) {
        return new Promise((resolve, reject) => {
            search.getFlashcard(more).then(({ body }) => resolve({ isEmpty: Boolean(body.item1.length), data: { items: body.item1, sources: body.item2 }}))
        })
    },
    tutor: function () {
        return new Promise((resolve, reject) => {
            search.getTutor(state.userText).then(({ body }) => resolve({ items: body }));
        })
    },
    job: function () {
        return new Promise((resolve, reject) => {
            search.getJob(state.userText).then(({ body }) => resolve({isEmpty:Boolean(body.length), data:{ items: body }}));
        })
    },
    book: function () {
        return new Promise((resolve, reject) => {
            search.getTutor(state.userText).then(response => resolve({ items: response.item1, sources: response.item2 }));
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
