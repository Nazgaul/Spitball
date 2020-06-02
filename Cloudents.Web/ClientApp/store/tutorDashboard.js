import axios from 'axios'
import constants from './constants/dashboardConstants';
import registerService from '../services/registrationService2';

const state = {
    tutorLinkActions: {},
    tutorNotificationsActions: {},
    tutorNotificationsState: {
        [constants.FOLLOWERS]: {
            value: false,
            new: 0
        },
        [constants.QUESTIONS]: {
            value: false,
            new: 0
        },
        [constants.PAYMENTS]: {
            value: false,
            new: 0
        },
    }
}

const getters = {
    getTutorListActions: state => state.tutorLinkActions,
    getUserNotifications: state => state.tutorNotificationsActions
}

const mutations = {
    setTutorListActions(state, data) {
        let tutorActions = new TutorAction(data)
        
        function TutorAction(objInit) {
            this.PHONE = { value: objInit.phoneVerified }
            this.EMAIL = { value: objInit.emailVerified }
            this.EDIT = { value: objInit.editProfile }
            this.BOOK = { 
                value: objInit.bookedSession.exists,
                tutorId: objInit.bookedSession.tutorId
            }
            this.COURSES = { value: objInit.courses }
            this.STRIPE = { value: objInit.stripeAccount }
            this.CALENDAR = { value: objInit.calendarShared }
            this.TEACH = { value: objInit.haveHours }
            this.SESSIONS = { value: objInit.liveSession }
            this.UPLOAD = { value: objInit.uploadContent }
        }
        
        state.tutorLinkActions = tutorActions
    },
    setTutorNotifications(state, data) {
        let notifyObj = {}
        let notifications = new Notifications(data)

        for (const key in state.tutorNotificationsState) {
            notifyObj[key] = {
                value: notifications[key]
            }
        }

        function Notifications(objInit) {
            this.FOLLOWERS = objInit.calendarShared;
            this.QUESTIONS = objInit.bookedSession;
            this.PAYMENTS = objInit.haveHours;
        }

        state.tutorNotificationsActions = notifyObj
    },
    setEmailTaskComplete(state, email) {
        state.tutorLinkActions[email].value = true
    },
    setPhoneTaskComplete(state) {
        state.tutorLinkActions[constants.PHONE].value = true
    }
}

const actions = {
    updateTutorLinks({commit}) {
        return axios.get('/Account/tutorActions')
            .then(({data}) => {
                commit('setTutorListActions', data)
            })
            .catch(ex => {
                console.log(ex);
        });
    },
    updatePhoneCode({commit}) {
        return registerService.sendSmsCode()
            .then(() => {
                commit('setComponent', 'verifyPhone')
                return
            }).catch(ex => ex)
    },
    verifyTutorEmail() {
        return axios.post('/register/verifyEmail')
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}