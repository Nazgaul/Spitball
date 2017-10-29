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
    getUniversity: state => state.user.universityId,
    myCourses: state => state.user.myCourses
}
const actions = {
    getUniversities(context, data) {
        return settingsService.getUniversity(data.term)
    },
    getCorses(context, data) {
        return settingsService.getCourse(data)
    },
    updateMyCourses({ commit }, data) {
        return Promise.resolve(commit(USER.UPDATE_USER, { myCourses:data}))
    },
    createCourse(context, data) {
        return new Promise((resolve, reject) => {
            settingsService.createCourse(data).then(({body}) => {
                console.log(body.id)
                context.commit(USER.UPDATE_USER, { myCourses: [...context.getters.myCourses, body.id] });
                resolve({id:body.id,name:data.name,code:data.code})
            })
        })
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