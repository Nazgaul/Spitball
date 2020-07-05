import analyticsService from '../services/analytics.service.js';
import tutorService from '../services/tutorService.js'

const state = {
    requestDialog: false,
    currTutor: null,
    tutorRequestAnalyticsOpenedFrom: {
        component: null,
        path: null
    },
    currentTutorReqStep:'tutorRequestCourseInfo',
    courseDescription:'',
    selectedCourse:'',
    currentTutorPhoneNumber: null,
    registerStepFromTutorRequest: false,
};

const getters = {
    getRequestTutorDialog:  state => state.requestDialog,
    getCurrTutor: state => state.currTutor,
    getTutorRequestAnalyticsOpenedFrom: state => state.tutorRequestAnalyticsOpenedFrom,
    getCurrentTutorReqStep: state => state.currentTutorReqStep,
    getCourseDescription: state => state.courseDescription,
    getSelectedCourse: state => state.selectedCourse,
getCurrentTutorPhoneNumber: state => state.currentTutorPhoneNumber,
    getIsFromTutorStep: state => state.registerStepFromTutorRequest,
};

const mutations = {
    setRequestDialog(state, val) {
        state.requestDialog = val;
    },
    setCurrTutor: (state, tutorObj) => {
        state.currTutor = tutorObj;
    },
    setTutorRequestAnalyticsOpenedFrom: (state, val) => {
        state.tutorRequestAnalyticsOpenedFrom.component = val.component;
        state.tutorRequestAnalyticsOpenedFrom.path = val.path;
    },
    setCourseDescription(state, val) {
        state.courseDescription = val;
    },
    setSelectedCourse(state, val) {
        state.selectedCourse = val;
    },
    setTutorReqStep(state, stepName) {
        state.currentTutorReqStep = stepName;
    },
    setRequestTutor(state) {
        state.currentTutorReqStep = 'tutorRequestCourseInfo';
        state.courseDescription = '';
        state.selectedCourse = '';
        state.currentTutorPhoneNumber = null;
        state.registerStepFromTutorRequest = false
        state.currTutor = null
    },
    setCurrentTutorPhoneNumber(state, number) {
        state.currentTutorPhoneNumber = number;
    },
    setIsFromTutorStep(state, val) {
        state.registerStepFromTutorRequest = val
    }
};


const actions = {
    updateCurrTutor({commit},tutorObj){ 
        commit('setCurrTutor', tutorObj);
    },
    updateRequestDialog({commit}, val){
        commit('setRequestDialog', val);
    },
    setTutorRequestAnalyticsOpenedFrom: ({commit}, val) => {
        commit('setTutorRequestAnalyticsOpenedFrom', val);
    },
    updateTutorReqStep({commit},stepName){
        commit('setTutorReqStep',stepName);
    },
    updateCourseDescription({commit},str){
        commit('setCourseDescription',str);
    },
    updateSelectedCourse({commit},str){
        commit('setSelectedCourse',str);
    },
    resetRequestTutor({commit}){
        commit('setRequestTutor');
    },
    sendTutorRequest({commit, dispatch},serverObj){
        dispatch('sendAnalyticEvent',false);
        return tutorService.requestTutor(serverObj)
                .then((response) =>{
                    if(response) {
                        commit('setCurrentTutorPhoneNumber', response.phoneNumber);
                    }
                    dispatch('updateTutorReqStep','tutorRequestSuccess');
                }).catch((err)=>{
                   throw err;
                });
    },
    sendAnalyticEvent({state,getters},beforeSubmit){
        let analyticsObject = {
            userId: getters.isAuthUser ? getters.accountUser.id : 'GUEST',
            course: state.tutorCourse,
            fromDialogPath: getters.getTutorRequestAnalyticsOpenedFrom.path,
            fromDialogComponent: getters.getTutorRequestAnalyticsOpenedFrom.component
        };
        if(beforeSubmit){
            analyticsService.sb_unitedEvent('Request Tutor Dialog Opened', `${analyticsObject.fromDialogPath}-${analyticsObject.fromDialogComponent}`, `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
        }else{
            analyticsService.sb_unitedEvent('Request Tutor Submit', `${analyticsObject.fromDialogPath}-${analyticsObject.fromDialogComponent}`, `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
        }
    },
    
};
export default {
    state,
    mutations,
    getters,
    actions
}