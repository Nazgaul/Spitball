const state = {
    universities: [],
    classes: [],
    schoolName: '',
    selectedClasses: [],
    showSelectUniInterface: false
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
    },
    setSelectedClasses(state, val){
        state.selectedClasses = val;
    },
    setSelectUniState(state, val){
        state.showSelectUniInterface = val;
    }
};
const getters = {
    getUniversities:  state => state.universities,
    getSchoolName:  state => state.schoolName,
    getClasses: state => state.classes,
    getSelectedClasses: state => state.selectedClasses,
    getShowSelectUniInterface: state => state.showSelectUniInterface
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
    updateSelectedClasses({commit}, val){
        commit('setSelectedClasses', val);
    },
    changeSelectUniState({commit}, val){
        commit('setSelectUniState', val);
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}