import universityService from '../services/universityService';

const state = {
    universities: [],
    classes: [],
    schoolName: '',
    selectedClasses: [],
    selectedClassesCache: [],
    showSelectUniPopUpInterface: false,
    resultLockForSchoolNameChange: false,
    resultLockForClassesChange: false,
    selectForTheFirstTime: false,
    reflectChangeToPage: 0,
    createDialog: false,
    creationVerified: false,
    createUniDialog: false,
    uniCreationVerified: false,
    lock_selectedClass:true,
    searchedCourse: '',
};

const getters = {
    getUniversities: state => state.universities,
    getSchoolName: state => state.schoolName,
    getClasses: state => state.classes,
    // the sorting is moved to the cmp
    getSelectedClasses: state => state.selectedClasses,
    getSelectedClassesCache: state => state.selectedClassesCache,
    getShowSelectUniPopUpInterface: state => state.showSelectUniPopUpInterface,
    getAllSteps: state => state.stepsEnum,
    getCurrentStep: state => state.currentStep,
    getUniversityPopStorage: state => state.universityPopStorage,
    createDialogVisibility: state => state.createDialog,
    creationVerified: state => state.creationVerified,
    getCreateDialogVisibility: state => state.createUniDialog,
    uniCreationVerified: state => state.uniCreationVerified,
    getIsSelectedClassLocked: state => state.lock_selectedClass,
    getSearchedCourse: state => state.searchedCourse,
};

const mutations = {
    //uni create dialog mutations
    verifyUniCreation(state, val) {
        state.uniCreationVerified = val;
    },
    updateUniCreateDialogState(state, val) {
        state.createUniDialog = val;
    },
    //course create dialog mutations
    verifyCreation(state, val) {
        state.creationVerified = val;
    },
    updateCreateDialogState(state, val) {
        state.createDialog = val;
    },
    //end dialogs mutations
    deleteCourse(state, val) {
        let index = state.selectedClasses.indexOf(val);
        state.selectedClasses.splice(index, 1);
    },
    
    setUniversities(state, val) {
        state.universities = val;
    },
    addUniversities(state, val) {
        state.universities = state.universities.concat(val);
    },
    addClasses(state, val) {
        state.classes = state.classes.concat(val);
    },
    setSchoolName(state, val) {
        state.schoolName = val;
    },
    setClasses(state, val) {
        state.classes = val;
    },
    
    pushClass(state, val) {
        state.selectedClasses.unshift(val);
    },
    setSelectedClasses(state, val) {
        state.selectedClasses = val;
    },
    clearClassesCahce(state) {
        state.selectedClassesCache.length = 0;
    },
    setSelectedClassesCahce(state, val) {
        state.selectedClassesCache = [].concat(val);
    },
    updateCachedList(state, val) {
        state.selectedClassesCache.push(val);
    },
    deleteFromCachedList(state, val){
        let index = state.selectedClassesCache.indexOf(val);
        state.selectedClassesCache.splice(index, 1);
    },
    setSelectUniState() {
        // state.showSelectUniInterface = val;
    },
    setCurrentStep(state, val) {
        state.currentStep = val;
    },
    setSelectPopUpUniState(state, val) {
        state.showSelectUniPopUpInterface = val;
    },
    setUniversityPopStorage(state, val) {
        window.sessionStorage.setItem('sb_uniSelectPoped_s', true);
        window.localStorage.setItem('sb_uniSelectPoped_l', val);
        state.universityPopStorage.session = true;
        state.universityPopStorage.local = val;

    },
    openResultLockForSchoolNameChange(state) {
        state.resultLockForSchoolNameChange = true;
    },
    openResultLockForClassesChange(state) {
        state.resultLockForClassesChange = true;
    },
    setSelectForTheFirstTime(state, val) {
        state.selectForTheFirstTime = val;
    },
    setReflectChangeToPage(state) {
        state.reflectChangeToPage++;
    },
    setAllClassesTeaching(state){
        if(state.selectedClasses && state.selectedClasses.length){
            state.selectedClasses.forEach((item, index)=>{
                return state.selectedClasses[index].isTeaching = true;
            });
        }
    },
    setLock_selectedClass(state, val){
        state.lock_selectedClass = val;
    },
    setSearchedCourse(state,val){
        state.searchedCourse = val;
    }
};

const actions = {
    updateTeachingClasses({commit}){
        commit('setAllClassesTeaching');
    },
    clearClassesCahce({commit}){
        commit('clearClassesCahce');
    },
    syncUniData({commit, dispatch}) {
        dispatch('setLock_selectedClass', true);
        universityService.getProfileUniversity().then((university) => {
            commit('setSchoolName', university.text);
            setTimeout(() => {
            dispatch('releaseResultLock', "uni");
            }, 2000); 
        });
        universityService.getProfileCourses().then((courses) => {
                dispatch('setSelectedClasses', courses);
                dispatch('assignSelectedClassesCache', courses);
                setTimeout(() => {
                    dispatch('releaseResultLock', "class");
                }, 2000);
        });
    },
    //to sync courses only
    syncCoursesData({commit, dispatch}) {
        universityService.getProfileCourses().then((courses) => {
            if(courses.length > 0) {
                dispatch('setSelectedClasses', courses);
                dispatch('assignSelectedClassesCache', courses);
                setTimeout(() => {
                    dispatch('releaseResultLock', "class");
                }, 2000);
            }
        });
    },
    createCourse({commit, dispatch}, courseToCreate) {
        universityService.createCourse(courseToCreate).then((course) => {
            dispatch('pushClassToSelectedClasses', course);
        });
    },

    createUniversity({commit, dispatch}, uniTocreate) {
        return universityService.createUni(uniTocreate).then((uni) => {
            commit('setSchoolName', uni);
        });
    },


    changeReflectChangeToPage({commit}) {
        commit('setReflectChangeToPage');
    },

    changeSelectPopUpUniState({commit}, val) {
        commit('setSelectPopUpUniState', val);
    },
    updateSchoolName({commit, dispatch}, val) {
        if (!val) return;
            
        let uniId = val.id;
        let uniName = val.name;
        return universityService.assaignUniversity(uniId).then(() => {
            commit('setSchoolName', uniName);
            //update profile data with new university
            //let currentProfID = this.getters.accountUser.id;
            dispatch('updateUniExists', true);
            //dispatch("syncProfile", currentProfID);
            return true;
        });
    },
    updateUniversities({commit}, val) {
        if(val || val === ''){
           return universityService.getUni(val).then(data => {
                commit('setUniversities', data);
                return data;
           }, err => {
                commit('setUniversities', []);
            });
        }
    },
    addUniversities({commit}, val){
        if(val || val === ''){
            return universityService.getUni(val).then(data => {
                commit('addUniversities', data);
                return data;
            }, err => {
                commit('setUniversities', []);
            });
        }
    },

    clearUniversityList({commit}) {
        commit('setUniversities', []);
    },
    updateClasses({commit}, val) {
     return  universityService.getCourse(val).then(data => {
            commit('setClasses', data);
            return data;
     });
    },
    addClasses({commit}, val){
        if(val || val === ''){
            return universityService.getCourse(val).then(data => {
                commit('addClasses', data);
                return data;
            }, err => {
                commit('setClasses', []);
            });
        }
    },
    clearClasses({commit}) {
        commit('setClasses', []);
    },
    deleteClass({commit}, val) {
        let name = val.text;
        return universityService.deleteCourse(name).then((resp) => {
            commit('deleteCourse', val);
            //clean cached list
            commit('deleteFromCachedList', val);
            return resp;
        });

    },
    removeFromCached({commit}, val){
        commit('deleteFromCachedList', val);
    },
    updateSelectedClasses({dispatch}, val) {
        dispatch('setSelectedClasses', val);
    },
    pushClassToSelectedClasses({commit}, val) {
        commit('pushClass', val);
    },
    assignClasses({state, dispatch}, courses) {
        let coursesToSend = courses ? courses : state.selectedClasses;
        return universityService.assaignCourse(coursesToSend).then(() => {
            //Update Filters in note page
            dispatch('changeReflectChangeToPage');
            Promise.resolve(true);
        });
    },
    assignSelectedClassesCache({commit, state}) {
        commit("setSelectedClassesCahce", state.selectedClasses);
    },
    addToCachedClasses({commit, state}, val) {
        commit("updateCachedList", val);
    },
    changeClassesToCachedClasses({commit, state, dispatch}) {
        if(state.selectedClassesCache.length > 0) {
            dispatch('setSelectedClasses', [].concat(state.selectedClassesCache));
        }
    },
    updateCurrentStep({commit, state}, val) {
        if(state.stepArr.indexOf(val) > -1) {
            commit("setCurrentStep", val);
        }
    },
    setUniversityPopStorage_session({commit, state}, val) {
        let localPopedItem = state.universityPopStorage.local;
        if(localPopedItem < 3) {
            localPopedItem++;
            commit('setUniversityPopStorage', localPopedItem);
        }
    },
    releaseResultLock({commit}, val) {
        if(val === "uni") {
            commit('openResultLockForSchoolNameChange');
        } else if(val === "class") {
            commit('openResultLockForClassesChange');
        }

    },
    updateSelectForTheFirstTime({commit}, val) {
        commit('setSelectForTheFirstTime', val);
    },
    closeSelectUniFromNav({commit}) {
        commit('setSelectUniState', false);
    },
    changeCreateDialogState({commit, state}, val) {
        commit('updateCreateDialogState', val);
    },
    updateVerification({commit, state}, val) {
        commit('verifyCreation', val);
    },
    changeUniCreateDialogState({commit, state}, val) {
        commit('updateUniCreateDialogState', val);
    },
    updateUniVerification({commit, state}, val) {
        commit('verifyUniCreation', val);
    },
    setSelectedClasses({commit, dispatch}, val){
        commit('setSelectedClasses', val);
        dispatch('setLock_selectedClass', false);
    },
    setLock_selectedClass({commit}, val){
        commit('setLock_selectedClass', val);
    }

};

export default {
    state,
    mutations,
    getters,
    actions
};