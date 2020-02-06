
const state = {
    isResetSearch: false,
};

const mutations = {
    setResetSearch(state){
        state.isResetSearch = !state.isResetSearch;
    }
};

const getters = {
    getSearchStatus: state => state.isResetSearch
};

const actions = {
    resetSearch({commit}){
        commit('setResetSearch');
    },
};

export default {
    state,
    getters,
    actions,
    mutations
}
