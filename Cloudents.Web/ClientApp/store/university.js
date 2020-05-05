import courseService from '../services/courseService';

const state = {
    classes: [],
    selectedClasses: [],
    selectedClassesCache: [],
    resultLockForSchoolNameChange: false,
    resultLockForClassesChange: false,
    reflectChangeToPage: 0,
    createDialog: false,
    creationVerified: false,
    lock_selectedClass:true,
    searchedCourse: '',
    isCourseRequestQuery: false
};

const getters = {
    getClasses: state => state.classes,
    getSelectedClasses: state => state.selectedClasses,
    createDialogVisibility: state => state.createDialog,
    creationVerified: state => state.creationVerified,
    getSearchedCourse: state => state.searchedCourse,
};

const mutations = {
    verifyCreation(state, val) {
        state.creationVerified = val;
    },
    updateCreateDialogState(state, val) {
        state.createDialog = val;
    },
    deleteCourse(state, val) {
        let index = state.selectedClasses.filter(v => v.text !== val);
        state.selectedClasses = index
    },
    addClasses(state, val) {
        state.classes = state.classes.concat(val);
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
    syncUniData({dispatch},{courses}) {
        dispatch('setLock_selectedClass', true);
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
    changeReflectChangeToPage({commit}) {
        commit('setReflectChangeToPage');
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
    changeCreateDialogState({commit}, val) {
        commit('updateCreateDialogState', val);
    },
    updateVerification({commit}, val) {
        commit('verifyCreation', val);
    },
    setSelectedClasses({commit, dispatch}, val){
        commit('setSelectedClasses', val);
        dispatch('setLock_selectedClass', false);
    },
    setLock_selectedClass({commit}, val){
        commit('setLock_selectedClass', val);
    },
    getManageCourses({commit, getters, state}) {
        let isRequest = state.isCourseRequestQuery
        if(!isRequest) {
            return courseService.getEditManageCourse().then((data) => {
                commit('setSelectedClasses', data);
                state.isCourseRequestQuery = true;
                return getters.getSelectedClasses
            })
        }
        return Promise.resolve( getters.getSelectedClasses)
    },
};

export default {
    state,
    mutations,
    getters,
    actions
};