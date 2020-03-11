import analyticsService from '../services/analytics.service.js';
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
    moreTutors:false,
    currentTutorPhoneNumber: null,
    guestName: '',
    guestMail: '',
    guestPhone: ''
};

const getters = {
    getRequestTutorDialog:  state => state.requestDialog,
    getCurrTutor: state => state.currTutor,
    getTutorRequestAnalyticsOpenedFrom: state => state.tutorRequestAnalyticsOpenedFrom,
    getCurrentTutorReqStep: state => state.currentTutorReqStep,
    getCourseDescription: state => state.courseDescription,
    getSelectedCourse: state => state.selectedCourse,
    getMoreTutors: state => state.moreTutors,
    getCurrentTutorPhoneNumber: state => state.currentTutorPhoneNumber,
    getGuestName: state => state.guestName,
    getGuestMail: state => state.guestMail,
    getGuestPhone: state => state.guestPhone,
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
        state.currentTutorPhoneNumber = null;
        state.guestName = '';
        state.guestMail = '';
        state.guestPhone = '';
    },
    setMoreTutors(state, val) {
        state.moreTutors = val;
    },
    setCurrentTutorPhoneNumber(state, number) {
        state.currentTutorPhoneNumber = number;
    },
    setGuestName(state, name) {
        state.guestName = name;
    },
    setGuestMail(state, mail) {
        state.guestMail = mail;
    },
    setGuestPhone(state, phone) {
        state.guestPhone = phone;
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
    sendTutorRequest({commit, dispatch},serverObj){
        dispatch('sendAnalyticEvent',false);
        return tutorService.requestTutor(serverObj)
                .then((response) =>{
                    if(response) {
                        commit('setCurrentTutorPhoneNumber', response.phoneNumber);
                    }
                    dispatch('updateTutorReqStep','tutorRequestSuccess');
                }).catch((err)=>{
                   
                    //Same converntaion as the server
                    // let serverResponse = err.response.data || { error : [LanguageService.getValueByKey("tutorRequest_request_error")]};
                    // let errorMsg = serverResponse[Object.keys(serverResponse)[0]][0];
                    // dispatch('updateToasterParams',{
                    //     toasterText: errorMsg,
                    //     showToaster: true,
                    //     toasterType: 'error-toaster'
                    // });
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
    updateMoreTutors({commit},val){
        commit('setMoreTutors',val);
    },
    setGuestName({commit}, name) {
        commit('setGuestName', name)
    },
    setGuestMail({commit}, mail) {
        commit('setGuestMail', mail)
    },
    setGuestPhone({commit}, phone) {
        commit('setGuestPhone', phone)
    },

};
export default {
    state,
    mutations,
    getters,
    actions
}