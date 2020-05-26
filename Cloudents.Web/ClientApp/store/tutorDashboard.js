import axios from 'axios'
import constants from './constants/dashboardConstants';
const dashboardInstance = axios.create({
    baseURL: '/'
})

const state = {
    tutorState: {
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
    tutorListActions: {}
}

const getters = {
    getTutorListActions: state => state.tutorListActions
}

const mutations = {
    setTutorListActions(state, data) {
        for (const key in state.tutorState) {
            if(key) {
                state.tutorListActions[key] = {
                    value: state.tutorState[key].value
                }
            }
        }
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
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}