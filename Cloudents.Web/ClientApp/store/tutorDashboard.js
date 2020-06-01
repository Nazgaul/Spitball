import axios from 'axios'
import constants from './constants/dashboardConstants';
// const dashboardInstance = axios.create({
//     baseURL: '/'
// })

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
            this.PHONE = { 
                value: objInit.phoneVerified,
                priority: 1
            }
            this.EMAIL = { 
                value: objInit.emailVerified,
                priority: 1
            }
            this.EDIT = { 
                value: objInit.editProfile,
                priority: 1
            }
            this.BOOK = { 
                value: objInit.bookedSession.exists,
                tutorId: objInit.bookedSession.tutorId,
                priority: 1
            }
            this.COURSES = { 
                value: objInit.courses,
                priority: 1
            }
            this.STRIPE = { 
                value: objInit.stripeAccount,
                priority: 1
            }
            this.CALENDAR = { 
                value: objInit.calendarShared,
                priority: 2
            }
            this.TEACH = { 
                value: objInit.haveHours,
                priority: 2
            }
            this.SESSIONS = { 
                value: objInit.liveSession,
                priority: 3
            }
            this.UPLOAD = { 
                value: objInit.uploadContent,
                priority: 3
            }
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
    updateTutorNotifications({commit}) {
        return axios.get('/Account/tutorActions')
            .then(({data}) => {
                commit('setTutorNotifications', data)
            })
            .catch(ex => {
                console.log(ex);
        });
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}