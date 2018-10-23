const state = {
    universities: []
};
const mutations = {
    setUniversities(state,val) {
        state.universities=val;
    },
};
const getters = {
    getUniversities:  state => state.universities,
};
const actions = {
    updateUniversities({commit}, val){
        commit('setUniversities', val);
    },
    clearUniversityList({commit}){
        commit('setUniversities', []);
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}