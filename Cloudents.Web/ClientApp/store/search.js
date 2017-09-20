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

export default {
    state,
    getters,
    mutations
}