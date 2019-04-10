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
    showSchoolBlock: global.innerWidth > 599 ? true : false,
    createDialog: false,
    creationVerified: false,
    createUniDialog: false,
    uniCreationVerified: false,

};

const getters = {
    getUniversities: state => state.universities,
    getSchoolName: state => state.schoolName,
    getClasses: state => state.classes,
    getSelectedClasses: state => state.selectedClasses,
    getSelectedClassesCache: state => state.selectedClassesCache,
    getShowSelectUniPopUpInterface: state => state.showSelectUniPopUpInterface,
    getAllSteps: state => state.stepsEnum,
    getCurrentStep: state => state.currentStep,
    getUniversityPopStorage: state => state.universityPopStorage,
    getResultLockForSchoolNameChange: state => state.resultLockForSchoolNameChange,
    getResultLockForClassesChange: state => state.resultLockForClassesChange,
    getReflectChangeToPage: state => state.reflectChangeToPage,
    getShowSchoolBlock: state => state.showSchoolBlock,
    createDialogVisibility: state => state.createDialog,
    creationVerified: state => state.creationVerified,
    getCreateDialogVisibility: state => state.createUniDialog,
    uniCreationVerified: state => state.uniCreationVerified
};

const actions = {
    syncUniData({commit, dispatch}) {
        universityService.getProfileUniversity().then((university) => {
            commit('setSchoolName', university.text);
            setTimeout(() => {
                dispatch('releaseResultLock', "uni");
            }, 2000);


        });
        universityService.getProfileCourses().then((courses) => {
            if(courses.length > 0) {
                commit('setSelectedClasses', courses);
                dispatch('assignSelectedClassesCache', courses);
                setTimeout(() => {
                    dispatch('releaseResultLock', "class");
                }, 2000);
            }
        });
    },
    //to sync courses only
    syncCoursesData({commit, dispatch}) {
        universityService.getProfileCourses().then((courses) => {
            if(courses.length > 0) {
                commit('setSelectedClasses', courses);
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
        universityService.createUni(uniTocreate).then((uni) => {
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
        if(!val) return;
        let uniId = val.id;
        let uniName = val.name;
        return universityService.assaignUniversity(uniId).then(() => {
            commit('setSchoolName', uniName);
            //update profile data with new university
            let currentProfID = this.getters.accountUser.id;

            //dispatch("syncProfile", currentProfID);
            Promise.resolve(true);
        });
    },
    updateUniversities({commit}, val) {
        if(val || val === ''){
            universityService.getUni(val).then(data => {
                commit('setUniversities', data);
            }, err => {
                commit('setUniversities', []);
            });
        }

    },
    clearUniversityList({commit}) {
        commit('setUniversities', []);
    },
    updateClasses({commit}, val) {
        universityService.getCourse(val).then(data => {
            commit('setClasses', data);
        });
    },
    clearClasses({commit}) {
        commit('setClasses', []);
    },
    deleteClass({commit}, val) {
        let name = val.text;
       return universityService.deleteCourse(name).then((resp) => {
            commit('deleteCourse', val);
            return resp
        });

    },

    updateSelectedClasses({commit}, val) {
        commit('setSelectedClasses', val);
    },
    pushClassToSelectedClasses({commit}, val) {
        commit('pushClass', val);
    },
    assignClasses({state, dispatch}) {
        universityService.assaignCourse(state.selectedClasses).then(() => {
            //Update Filters in note page
            //dispatch("updateCoursesFilters", state.selectedClasses);
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
    changeClassesToCachedClasses({commit, state}) {
        if(state.selectedClassesCache.length > 0) {
            commit('setSelectedClasses', [].concat(state.selectedClassesCache));
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
    toggleShowSchoolBlock({commit, state}, val) {
        if(typeof val !== "undefined") {
            commit('updtaeShowSchoolBlock', val);
        } else {
            commit('updtaeShowSchoolBlock', !state.showSchoolBlock);
        }

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
    setSchoolName(state, val) {
        state.schoolName = val;
    },
    setClasses(state, val) {
        state.classes = val;
    },
    updateCachedList(state, val) {
        state.selectedClassesCache.push(val);
    },
    pushClass(state, val) {
        state.selectedClasses.unshift(val);
    },
    setSelectedClasses(state, val) {
        state.selectedClasses = val;
    },
    setSelectedClassesCahce(state, val) {
        state.selectedClassesCache = [].concat(val);
    },
    setSelectUniState(state, val) {
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
    updtaeShowSchoolBlock(state, val) {
        state.showSchoolBlock = val;
    }
};

export default {
    state,
    mutations,
    getters,
    actions
};