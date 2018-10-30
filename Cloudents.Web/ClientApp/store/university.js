import universityService from '../services/universityService'
const state = {
    universities: [],
    classes: [],
    schoolName: '',
    selectedClasses: [],
    showSelectUniInterface: false,
    showSelectUniPopUpInterface: false,
    currentStep: 'SetSchoolLanding',
    stepsEnum: {
        set_school_landing: 'SetSchoolLanding',
        set_school: 'SetSchool',
        set_class: 'SetClass',
        done: 'done'
    },
    universityPopStorage:{
        session: !!window.sessionStorage.getItem('sb_uniSelectPoped_s'), //boolean
        local: window.localStorage.getItem('sb_uniSelectPoped_l') || 0 //integer
    }
};

const getters = {
    getUniversities:  state => state.universities,
    getSchoolName:  state => state.schoolName,
    getClasses: state => state.classes,
    getSelectedClasses: state => state.selectedClasses,
    getShowSelectUniInterface: state => state.showSelectUniInterface,
    getShowSelectUniPopUpInterface: state => state.showSelectUniPopUpInterface,
    getAllSteps: state => state.stepsEnum,
    getCurrentStep: state => state.currentStep,
    getUniversityPopStorage: state => state.universityPopStorage
};

const actions = {
    syncUniData({commit}){
        universityService.getProfileUniversity().then((university)=>{
            commit('setSchoolName', university.name);
        })
        universityService.getProfileCourses().then((courses)=>{
            commit('updateSelectedClasses', courses);
        })
        
    },
    changeSelectUniState({commit}, val){
        commit('setSelectUniState', val);
    },
    changeSelectPopUpUniState({commit}, val){
        commit('setSelectPopUpUniState', val);
    },
    updateSchoolName({commit}, val){
        if(!val) return;
        return universityService.assaignUniversity(val).then(()=>{
            commit('setSchoolName', val);
            Promise.resolve(true);
        })
    },
    updateUniversities({commit}, val){
        if(!val) return;
        universityService.getUni(val).then(data=>{
            commit('setUniversities', data);
        })
    },
    clearUniversityList({commit}){
        commit('setUniversities', []);
    },
    updateClasses({commit}, val){
        universityService.getCourse(val).then(data=>{
            commit('setClasses', data);
        });
    },
    clearClasses({commit}){
        commit('setClasses', []);
    },
    updateSelectedClasses({commit}, val){
        commit('setSelectedClasses', val);
    },
    assignClasses({state}){
        universityService.assaignCourse(state.selectedClasses).then(()=>{
            Promise.resolve(true);
        })
    },
    updateCurrentStep({commit}, val){
        commit("setCurrentStep", val);
    },
    setUniversityPopStorage_session({commit, state}, val){
        let localPopedItem = state.universityPopStorage.local;
        if(localPopedItem < 3){
            localPopedItem++;
            commit('setUniversityPopStorage', localPopedItem);
        }        
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
    },
    setSelectPopUpUniState(state, val){
        state.showSelectUniPopUpInterface = val;
    },
    setUniversityPopStorage(state, val){
        window.sessionStorage.setItem('sb_uniSelectPoped_s', true);
        window.localStorage.setItem('sb_uniSelectPoped_l', val);
        state.universityPopStorage.session = true;
        state.universityPopStorage.local = val;

    }
};

export default {
    state,
    mutations,
    getters,
    actions
}