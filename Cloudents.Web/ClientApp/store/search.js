import * as types from './mutation-types'

const state = {
    userText: null
};

const mutations = {
    [types.UPDATE_FILTER](state, text) {
        state.userText = text;
    }
};
const getters = {
    userText: state => state.userText
}
const actions = {
    updateSearchText: ({ commit }, text) => commit(types.UPDATE_FILTER, text)
}

export default {
    state,
    getters,
    actions,
    mutations
}