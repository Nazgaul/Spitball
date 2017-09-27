import * as types from './mutation-types'
import search from './../api/search'

const state = {
    pageContent: null,
    loading: false,
    userText: '',
    isEmpty:false
};

const mutations = {
    [types.UPDATE_FILTER](state, text) {
        state.userText = text;
    },
    [types.UPDATE_PAGE_CONTENT](state, payload) {
        console.log(payload)
        if (!payload.hasOwnProperty('isEmpty')) {
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
    }
};
const getters = {
    userText: state => state.userText,
    pageContent : state => state.pageContent,
    items: state => state.pageContent?state.pageContent.items:null,
    loading : state => state.loading,
    isEmpty: state => state.isEmpty,
    pageTitle: state => state.pageContent ? state.pageContent.title : null
}
const actions = {
    updateSearchText: ({ commit }, text) => commit(types.UPDATE_FILTER, text),
    fetchingData: ({ commit }, page) => {
            commit(types.UPDATE_LOADING, true);
            activateFunction[page.name]().then(response => {
                commit(types.UPDATE_PAGE_CONTENT, response);
            })       
    }
}
const activateFunction = {
    ask: function () {
        return new Promise((resolve, reject) => {
            var promise2 = search.getQna({});
            var promise1 = search.getShortAnswer(state.userText);
            Promise.all([promise1, promise2]).then(([short, items]) => {
                resolve({ title: short.body, items: items.body })
            })
        } )
    },
    note: function () {
        return new Promise((resolve, reject) => {
            search.getDocument({}).then(({ body }) => resolve({ isEmpty: Boolean(body.item1.length),data:{ items: body.item1, sources: body.item2 }}))
        })
    },
    flashcard: function () {
        return new Promise((resolve, reject) => {
            search.getFlashcard({}).then(({ body }) => resolve({ isEmpty: Boolean(body.item1.length), data: { items: body.item1, sources: body.item2 }}))
        })
    },
    tutor: function () {
        return new Promise((resolve, reject) => {
            search.getTutor(state.userText).then(({ body }) => resolve({ items: body }));
        })
    },
    job: function () {
        return new Promise((resolve, reject) => {
            search.getTutor(state.userText).then(response => resolve({ items: response.item1, sources: response.item2 }));
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
