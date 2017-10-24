import settingsService from './../services/settingsService'
import { USER } from './mutation-types'
const state = {
    user: {
        universityId: null,
        myCourses: [],
        isFirst: true
    }
};

const mutations = {
    [USER.UPDATE_USER](state, payload) {
        state.user = { ...state.user, ...payload};
    } 
}
const getters = {
    isFirst: state => state.user.isFirst,
    getUniversity: state => state.user.universityId
}
const actions = {
    getUniversities(context, data) {
        return settingsService.getUniversity(data.term)
    },
    getCorses(context, data) {
        return settingsService.getCourse(data)
    },
    createCourse(context, data) {
        return settingsService.createCourse(data)
    },
    updateFirstTime({ commit }) {
        commit(USER.UPDATE_USER, {isFirst:false});
    },
    updateUniversity({ commit }, university) {
        commit(USER.UPDATE_USER, { universityId: university });
    }

}
export default {
    state,
    mutations,
    getters,
    actions
}