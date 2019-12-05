import tutorService from '../services/tutorService.js'
import {LanguageService} from '../services/language/languageService.js'

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
    moreTutors:true,
};

const getters = {
    getRequestTutorDialog:  state => state.requestDialog,
    getCurrTutor: state => state.currTutor,
    getTutorRequestAnalyticsOpenedFrom: state => state.tutorRequestAnalyticsOpenedFrom,
    getCurrentTutorReqStep: state => state.currentTutorReqStep,
    getCourseDescription: state => state.courseDescription,
    getSelectedCourse: state => state.selectedCourse,
    getMoreTutors: state => state.moreTutors,
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
        state.moreTutors = false;
    },
    setMoreTutors(state, val) {
        state.moreTutors = val;
    },
};


const actions = {
    updateCurrTutor({commit},tutorObj){ 
        commit('setCurrTutor', tutorObj);
    },
    updateRequestDialog({commit, dispatch}, val){
        commit('setRequestDialog', val);
        if(!val){
            dispatch('updateCurrTutor', null);
        }
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
    sendTutorRequest({dispatch},serverObj){
        dispatch('sendAnalyticEvent',false);
        return tutorService.requestTutor(serverObj)
                .then((response) =>{
                    dispatch('updateTutorReqStep','tutorRequestSuccess');
                }).catch((err)=>{
                    dispatch('updateToasterParams',{
                        toasterText: !!err.response.data.error ? err.response.data.error[0] : LanguageService.getValueByKey("tutorRequest_request_error"),
                        showToaster: true,
                        toasterType: 'error-toaster'
                    });
                    return Promise.reject();
                });
    },
    sendAnalyticEvent({state,getters,dispatch},beforeSubmit){
        let analyticsObject = {
            userId: getters.isAuthUser ? getters.accountUser.id : 'GUEST',
            course: state.tutorCourse,
            fromDialogPath: getters.getTutorRequestAnalyticsOpenedFrom.path,
            fromDialogComponent: getters.getTutorRequestAnalyticsOpenedFrom.component
        };
        if(beforeSubmit){
            dispatch('updateAnalytics_unitedEvent',['Request Tutor Dialog Opened', `${analyticsObject.fromDialogPath}-${analyticsObject.fromDialogComponent}`, `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`])
        }else{
            dispatch('updateAnalytics_unitedEvent',['Request Tutor Submit', `${analyticsObject.fromDialogPath}-${analyticsObject.fromDialogComponent}`, `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`])
        }
    },
    updateMoreTutors({commit},val){
        commit('setMoreTutors',val);
    }

};
export default {
    state,
    mutations,
    getters,
    actions
}