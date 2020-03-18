import universityService from '../services/universityService';
import courseService from '../services/courseService';

const state = {
    universities: [],
    classes: [],
    schoolName: '',
    selectedClasses: [],
    selectedClassesCache: [],
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
        let index = state.selectedClasses.filter(v => v.text !== val);
        state.selectedClasses = index
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
    updateTeachCourse(context,courseName){
        return courseService.teachCourse(courseName)
    },
    updateTeachingClasses({commit}){
        commit('setAllClassesTeaching');
    },
    clearClassesCahce({commit}){
        commit('clearClassesCahce');
    },
    syncUniData({commit, dispatch},{courses ,university}) {
        dispatch('setLock_selectedClass', true);
        commit('setSchoolName', university.text);
        dispatch('setSelectedClasses', courses);
        dispatch('assignSelectedClassesCache', courses);

        setTimeout(() => {
            dispatch('releaseResultLock', "uni");
            dispatch('releaseResultLock', "class");
        }, 2000);
    },
    createCourse({dispatch}, courseToCreate) {
        courseService.createCourse(courseToCreate).then((course) => {
            dispatch('pushClassToSelectedClasses', course);
        });
    },

    createUniversity({commit}, uniTocreate) {
        return universityService.createUni(uniTocreate).then((uni) => {
            commit('setSchoolName', uni);
        });
    },
    changeReflectChangeToPage({commit}) {
        commit('setReflectChangeToPage');
    },
    updateSchoolName({commit}, val) {
        if (!val) return;
            
        let uniId = val.id;
        let uniName = val.name;
        return universityService.assaignUniversity(uniId).then(() => {
            commit('setSchoolName', uniName);
            return true;
        });
    },
    updateUniversities({commit}, val) {
        if(val || val === ''){
           return universityService.getUni(val).then(data => {
                commit('setUniversities', data);
                return data;
           }, () => {
                commit('setUniversities', []);
            });
        }
    },
    addUniversities({commit}, val){
        if(val || val === ''){
            return universityService.getUni(val).then(data => {
                commit('addUniversities', data);
                return data;
            }, () => {
                commit('setUniversities', []);
            });
        }
    },

    clearUniversityList({commit}) {
        commit('setUniversities', []);
    },
    updateClasses({commit}, val) {
     return  courseService.getCourse(val).then(data => {
            commit('setClasses', data);
            return data;
     });
    },
    addClasses({commit}, val){
        if(val || val === ''){
            return courseService.getCourse(val).then(data => {
                commit('addClasses', data);
                return data;
            }, () => {
                commit('setClasses', []);
            });
        }
    },
    clearClasses({commit}) {
        commit('setClasses', []);
    },
    deleteClass({commit}, val) {
        return courseService.deleteCourse(val).then((resp) => {
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
    assignClasses({dispatch}, course) {
        let courseName = [{name: course}]
        return courseService.assaignCourse(courseName).then(() => {
            //Update Filters in note page
            dispatch('changeReflectChangeToPage');
            Promise.resolve(true);
        });
    },
    assignSelectedClassesCache({commit, state}) {
        commit("setSelectedClassesCahce", state.selectedClasses);
    },
    addToCachedClasses({commit}, val) {
        commit("updateCachedList", val);
    },
    changeClassesToCachedClasses({state, dispatch}) {
        if(state.selectedClassesCache.length > 0) {
            dispatch('setSelectedClasses', [].concat(state.selectedClassesCache));
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
    changeCreateDialogState({commit}, val) {
        commit('updateCreateDialogState', val);
    },
    updateVerification({commit}, val) {
        commit('verifyCreation', val);
    },
    changeUniCreateDialogState({commit}, val) {
        commit('updateUniCreateDialogState', val);
    },
    updateUniVerification({commit}, val) {
        commit('verifyUniCreation', val);
    },
    setSelectedClasses({commit, dispatch}, val){
        commit('setSelectedClasses', val);
        dispatch('setLock_selectedClass', false);
    },
    setLock_selectedClass({commit}, val){
        commit('setLock_selectedClass', val);
    },
    getManageCourses({commit, dispatch}) {
        return courseService.getEditManageCourse().then(({data}) => {
            // dispatch('setLock_selectedClass', true);
            commit('setSelectedClasses', data)
            return data
        })
    }
};

export default {
    state,
    mutations,
    getters,
    actions
};