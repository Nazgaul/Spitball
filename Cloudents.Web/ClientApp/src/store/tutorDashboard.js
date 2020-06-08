import axios from 'axios'
import constants from './constants/dashboardConstants';
import registerService from '../services/registrationService2';

const state = {
    tutorLinkActions: {},
    tutorNotificationsActions: {}
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
        let notifications = new Notifications(data)

        function Notifications(objInit) {
            this.CHAT = { 
                value: objInit?.unReplyChat,
                amount: objInit?.amount
            };
            this.BROADCAST = { 
                value: objInit?.newRegisterStudent,
                amount: objInit?.amount
            };
            this.FOLLOWERS = { 
                value: objInit?.newFollower,
                amount: objInit?.amount
            };
            this.QUESTIONS = { 
                value: objInit?.unAnswerQuestions,
                amount: objInit?.amount
            };
            this.PAYMENTS = { 
                value: objInit?.pendingPayments,
                amount: objInit?.amount
            };
        }

        state.tutorNotificationsActions = notifications
    },
    setEmailTaskComplete(state) {
        state.tutorLinkActions[constants.EMAIL].value = true
    },
    setPhoneTaskComplete(state) {
        state.tutorLinkActions[constants.PHONE].value = true
    }
}

const actions = {
    updateTutorLinks({commit}) {
        return axios.get('/dashboard/actions')
            .then(({data}) => {
                commit('setTutorListActions', data)
            })
    },
    updateTutorNotifications({commit}) {
        // return axios.get('/Account/notifications')
            // .then(({data}) => {
                // commit('setTutorNotifications', data)
                commit('setTutorNotifications')
            // })
    },
    updatePhoneCode() {
        return registerService.sendSmsCode()
    },
    verifyTutorEmail() {
        return axios.post('/register/verifyEmail')
    },
}

export default {
    state,
    getters,
    mutations,
    actions
}