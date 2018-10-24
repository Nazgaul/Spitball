const state = {
    universities: [],
    schoolName: '',
    classes: []
};
const mutations = {
    setUniversities(state,val) {
        state.universities = val;
    },
    setSchoolName(state,val) {
        state.schoolName = val;
    },
    setClasses(state, val){
        state.classes = val;
    }
};
const getters = {
    getUniversities:  state => state.universities,
    getSchoolName:  state => state.schoolName,
    getClasses: state => state.classes
};
const actions = {
    updateSchoolName({commit}, val){
        commit('setSchoolName', val);
    },
    updateUniversities({commit}, val){
        commit('setUniversities', val);
    },
    clearUniversityList({commit}){
        commit('setUniversities', []);
    },
    updateClasses({commit}, val){
        commit('setClasses', val);
    },
    clearClasses({commit}){
        commit('setClasses', []);
    },
};

export default {
    state,
    mutations,
    getters,
    actions
}