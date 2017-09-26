import * as types from './mutation-types'
import search from './../api/search'

const state = {
    pageContent: null,
    loading: false,
    userText:''
};

const mutations = {
    [types.UPDATE_FILTER](state, text) {
        state.userText = text;
    },
    [types.UPDATE_PAGE_CONTENT](state, payload) {
        console.log("update page content")
        state.pageContent = payload
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
    loading : state => state.loading
}
const actions = {
    updateSearchText: ({ commit }, text) => commit(types.UPDATE_FILTER, text),
    rootChange: ({ commit}, page) => {
        //if (page.meta) {
        //    commit(types.MERGE_META);
        //} else {

        //}
    },
    fetchingData: ({ commit }, page) => {
        return new Promise((resolve, reject) => {
            commit(types.UPDATE_LOADING, true);
            activateFunction[page.name]().then(response => {
                commit(types.UPDATE_PAGE_CONTENT, response);
                console.log('resolve')
                resolve('yifat')
            })
            //commit(types.UPDATE_PAGE_CONTENT, pageContent);
            //console.log('resolve')
            //resolve('yifat')
        })
       
    }
}
const activateFunction = {
    ask: function () {
        return new Promise((resolve, reject) => {
            var promise1 = search.getQna({}, null, null);
            var promise2 = search.getShortAnswer({},null,null);
            Promise.all([promise1, promise2]).then(([short, items]) => {
                resolve({ short: short.body, items: items.body })
            })
        } )
        //Promise.all(short,moreData)
    },
    note: function () {
        return new Promise((resolve, reject) => {
            search.getDocument({}, null, null).then(response=>resolve({ items: response.item1, sources: response.item2 }))
        })
    }
}
export default {
    state,
    getters,
    actions,
    mutations
}
