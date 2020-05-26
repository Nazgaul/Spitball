import axios from 'axios'
import constants from './constants/dashboardConstants';
// const dashboardInstance = axios.create({
//     baseURL: '/'
// })

const state = {
    tutorState: {
        [constants.UPLOAD]: false,
        [constants.CALENDAR]: false,
        [constants.TEACH]: false,
        [constants.SESSIONS]: false,
        [constants.MARKETING]: false,
        [constants.BOOK]: false,
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
                state.tutorListActions[key] = state.tutorState[key]
            }
        }
    }
}

const actions = {
    updateTutorLinks({commit}) {
        commit('setTutorListActions')
    }
}

export default {
    state,
    getters,
    mutations,
    actions
}