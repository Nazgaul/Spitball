const state = {
    universities: [],
    classes: [],
    schoolName: '',
    selectedClasses: [],
    showSelectUniInterface: false,
    currentStep: 'SetSchoolLanding',
    stepsEnum: {
        set_school_landing: 'SetSchoolLanding',
        set_school: 'SetSchool',
        set_class: 'SetClass',
        done: 'done'
    }
};

const getters = {
    getUniversities:  state => state.universities,
    getSchoolName:  state => state.schoolName,
    getClasses: state => state.classes,
    getSelectedClasses: state => state.selectedClasses,
    getShowSelectUniInterface: state => state.showSelectUniInterface,
    getAllSteps: state => state.stepsEnum,
    getCurrentStep: state => state.currentStep
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
    },
    updateCurrentStep({commit}, val){
        commit("setCurrentStep", val);
    }
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
    },
    setCurrentStep(state, val){
        state.currentStep = val;
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}