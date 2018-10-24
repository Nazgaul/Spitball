const state = {
    universities: [],
    schoolName: '',
};
const mutations = {
    setUniversities(state,val) {
        state.universities = val;
    },
    setSchoolName(state,val) {
        state.schoolName = val;
    },
};
const getters = {
    getUniversities:  state => state.universities,
    getSchoolName:  state => state.schoolName,
};
const actions = {
    updateUniversities({commit}, val){
        commit('setUniversities', val);
    },
    clearUniversityList({commit}){
        commit('setUniversities', []);
    },
    updateSchoolName({commit}, val){
        commit('setSchoolName', val);
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}