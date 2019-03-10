const state = {
    sharedDocLink: ''
};
const getters = {
    sharedDocUrl: state => state.sharedDocLink,
};

const mutations = {
    setSharedDocumentLink(state, val) {
        state.sharedDocLink = val
    },
};

const actions = {
    updateSharedDocLink({commit, state}, val) {
        console.log('store', val)
        commit('setSharedDocumentLink', val)
    },
};
export default {
    state,
    mutations,
    getters,
    actions
}