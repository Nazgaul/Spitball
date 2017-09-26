import * as types from './mutation-types'

const state = {
    pageContent: null,
    loading: false
};

const mutations = {
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
    pageContent : state => state.pageContent,
    loading : state => state.loading
}
const actions = {
    rootChange: ({ commit}, page) => {
        if (page.meta) {
            commit(types.MERGE_META);
        } else {

        }
    },
    fetchingData: ({ commit}, page) => {
        var pageContent = activateFunction[page.name]();
        commit(types.UPDATE_PAGE_CONTENT, pageContent);
    }
}
let activateFunction = {
    ask: function () {
        console.log('ask')
        var walfram = '';
        var listItems = '';

        return {walfram:'18 shekel'}
    },
    note: function () { console.log('note') }
}
export default {
    state,
    getters,
    actions,
    mutations
}
