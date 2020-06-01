// import axios from 'axios'
import constants from './constants/dashboardConstants';
// const dashboardInstance = axios.create({
//     baseURL: '/'
// })

const state = {
    tutorLinkActions: {},
    tutorLinkState: {
        [constants.UPLOAD]: {
            value: false,
            priority: 1
        },
        [constants.CALENDAR]: {
            value: false,
            priority: 1
        },
        [constants.TEACH]: {
            value: false,
            priority: 1
        },
        [constants.SESSIONS]: {
            value: false,
            priority: 2
        },
        [constants.MARKETING]: {
            value: false,
            priority: 2
        },
        [constants.BOOK]: {
            value: false,
            priority: 3
        },
    },
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
        console.log(data);
        
        for (const key in state.tutorLinkState) {
            if(key) {
                state.tutorLinkActions[key] = {
                    value: state.tutorLinkState[key].value
                }
            }
        }
    },
    setTutorNotifications(state, notifications) {
        console.log(notifications);
        
        for (const key in state.tutorNotificationsState) {
            if(key) {
                state.tutorNotificationsActions[key] = {
                    value: state.tutorNotificationsState[key].value
                }
            }
        }
        // function Notifications(objInit) {
        //     // this.
        // }
        // notifications.map(notify => {
        //     state.tutorNotificationsActions
        // })
    }
}

const actions = {
    updateTutorLinks({commit}) {
        // return dashboardInstance.getTutorDashboardList()
            // .then(({data}) => {
                commit('setTutorListActions')
                // commit('setTutorListActions', data)
                // }).catch(ex => {
                // console.log(ex);
            // })
    },
    updateTutorNotifications({commit}) {
        // return dashboardInstance.getTutorNotifications()
            // .then(({data}) => {
                commit('setTutorNotifications')
                // commit('setTutorListActions', data)
                // }).catch(ex => {
                // console.log(ex);
            // })
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}