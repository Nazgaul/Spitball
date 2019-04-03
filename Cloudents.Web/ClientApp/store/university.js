import universityService from '../services/universityService'
const state = {
    universities: [],
    classes: [],
    schoolName: '',
    selectedClasses: [],
    selectedClassesCache: [],
    showSelectUniInterface: false,
    showSelectUniPopUpInterface: false,
    currentStep: 'SetSchoolLanding',
    stepsEnum: {
        set_school_landing: 'SetSchoolLanding',
        set_school: 'SetSchool',
        set_class: 'SetClass',
        done: 'done'
    },
    stepArr:[
        'SetSchoolLanding','SetSchool','SetClass','setSchool','setClass','done'
    ],
    universityPopStorage:{
        session: !!window.sessionStorage.getItem('sb_uniSelectPoped_s'), //boolean
        local: window.localStorage.getItem('sb_uniSelectPoped_l') || 0 //integer
    },
    resultLockForSchoolNameChange: false,
    resultLockForClassesChange: false,
    selectForTheFirstTime: false,
    reflectChangeToPage: 0,
    showSchoolBlock: global.innerWidth > 599 ? true : false
};

const getters = {
    getUniversities:  state => state.universities,
    getSchoolName:  state => state.schoolName,
    getClasses: state => state.classes,
    getSelectedClasses: state => state.selectedClasses,
    getSelectedClassesCache: state => state.selectedClassesCache,
    getShowSelectUniInterface: state => state.showSelectUniInterface,
    getShowSelectUniPopUpInterface: state => state.showSelectUniPopUpInterface,
    getAllSteps: state => state.stepsEnum,
    getCurrentStep: state => state.currentStep,
    getUniversityPopStorage: state => state.universityPopStorage,
    getResultLockForSchoolNameChange: state => state.resultLockForSchoolNameChange,
    getResultLockForClassesChange: state => state.resultLockForClassesChange,
    getReflectChangeToPage: state => state.reflectChangeToPage,
    getShowSchoolBlock: state => state.showSchoolBlock
};

const actions = {
    syncUniData({commit, dispatch}){
        universityService.getProfileUniversity().then((university)=>{
            commit('setSchoolName', university.text);
            setTimeout(()=>{
                dispatch('releaseResultLock', "uni");
            }, 2000);
            

        });
        universityService.getProfileCourses().then((courses)=>{
            if(courses.length > 0){
                commit('setSelectedClasses', courses);
                dispatch('assignSelectedClassesCache', courses);
                setTimeout(()=>{
                    dispatch('releaseResultLock', "class");
                }, 2000);
            }
        })
    },
    changeReflectChangeToPage({commit}){
        commit('setReflectChangeToPage');
    },
    changeSelectUniState({commit, dispatch, getters, rootState}, val){
        return //DEPRECATED using route now!
        if(!val){
            dispatch('changeClassesToCachedClasses')
            //if(state.selectForTheFirstTime){
                //after register should open the tour on the correct page
                //dispatch('updateSelectForTheFirstTime', false);
                let VerticalName = getters.getCurrentVertical;
                if(VerticalName === "note"){
                    //invokes the watch on the app.vue file
                    dispatch('StudyDocuments_updateDataLoaded', false);
                    setTimeout(()=>{
                        dispatch('StudyDocuments_updateDataLoaded', true);
                    })
                }else if(VerticalName === "ask"){
                    //invokes the watch on the app.vue file
                    dispatch('HomeworkHelp_updateDataLoaded', false);
                    setTimeout(()=>{
                        dispatch('HomeworkHelp_updateDataLoaded', true);
                    })
                }
            //}
        }
        commit('setSelectUniState', val);
    },
    changeSelectPopUpUniState({commit}, val){
        commit('setSelectPopUpUniState', val);
    },
    updateSchoolName({commit, dispatch}, val){
        if(!val) return;
        return universityService.assaignUniversity(val).then(()=>{
            commit('setSchoolName', val);
            //update profile data with new university
            let currentProfID = this.getters.accountUser.id;
            
            //dispatch("syncProfile", currentProfID);
            Promise.resolve(true);
        })
    },
    updateUniversities({commit}, val){
        if(!val) return;
        universityService.getUni(val).then(data=>{
            commit('setUniversities', data);
        }, err=>{
            commit('setUniversities', []);
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
    pushClassToSelectedClasses({commit}, val){
        commit('pushClass', val);
    },
    assignClasses({state, dispatch}){
        universityService.assaignCourse(state.selectedClasses).then(()=>{
            //Update Filters in note page
            //dispatch("updateCoursesFilters", state.selectedClasses);
            dispatch('changeReflectChangeToPage')
            Promise.resolve(true);
            
        })
    },
    assignSelectedClassesCache({commit, state}){
        commit("setSelectedClassesCahce", state.selectedClasses);
    },
    changeClassesToCachedClasses({commit, state}){
        if(state.selectedClassesCache.length > 0){
            commit('setSelectedClasses', [].concat(state.selectedClassesCache))
        }
    },
    updateCurrentStep({commit, state}, val){
        if(state.stepArr.indexOf(val) > -1){
            commit("setCurrentStep", val);
        }
    },
    setUniversityPopStorage_session({commit, state}, val){
        let localPopedItem = state.universityPopStorage.local;
        if(localPopedItem < 3){
            localPopedItem++;
            commit('setUniversityPopStorage', localPopedItem);
        }        
    },
    releaseResultLock({commit}, val){
        if(val === "uni"){
            commit('openResultLockForSchoolNameChange');
        }else if(val === "class"){
            commit('openResultLockForClassesChange');
        }
        
    },
    updateSelectForTheFirstTime({commit}, val){
        commit('setSelectForTheFirstTime', val);
    },
    closeSelectUniFromNav({commit}){
        commit('setSelectUniState', false);
    },
    toggleShowSchoolBlock({commit, state}, val){
        if(typeof val !== "undefined"){
            commit('updtaeShowSchoolBlock', val)
        }else{
            commit('updtaeShowSchoolBlock', !state.showSchoolBlock)
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
    pushClass(state, val){
        state.selectedClasses.push(val);
    },
    setSelectedClasses(state, val){
        state.selectedClasses = val;
    },
    setSelectedClassesCahce(state, val){
        state.selectedClassesCache = [].concat(val);
    },
    setSelectUniState(state, val){
        // state.showSelectUniInterface = val;
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

    },
    openResultLockForSchoolNameChange(state){
        state.resultLockForSchoolNameChange = true;
    },
    openResultLockForClassesChange(state){
        state.resultLockForClassesChange = true;
    },
    setSelectForTheFirstTime(state, val){
        state.selectForTheFirstTime = val;
    },
    setReflectChangeToPage(state){
        state.reflectChangeToPage++;
    },
    updtaeShowSchoolBlock(state, val){
        state.showSchoolBlock = val;
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}