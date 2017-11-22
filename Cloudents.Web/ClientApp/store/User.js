import settingsService from './../services/settingsService'
import { USER } from './mutation-types'
const state = {
    user: {
        universityId: null,
        myCourses: [],
        isFirst: true,
        pinnedCards: {}
    }
};

const mutations = {
    [USER.UPDATE_USER](state, payload) {
        state.user = { ...state.user, ...payload};
    } 
};
const getters = {
    isFirst: state => state.user.isFirst,
    pinnedCards: state => state.user.pinnedCards,
    getUniversity: state => {
        var obj = state.user.universityId || {}; 
        return obj.id;
    },
    getUniversityName: state => {
        var obj = state.user.universityId || {};
        return obj.name;
    },
    myCourses: state => state.user.myCourses,
    myCoursesId: state => (state.user.myCourses.length ? state.user.myCourses.map(i=>i.id):[])
};
const actions = {
    getUniversities(context, data) {
        return settingsService.getUniversity(data.term);
    },
    getCorses(context, {term}) {
        return settingsService.getCourse({term,universityId:context.getters.getUniversity});
    },

    createCourse(context, data) {
        return new Promise((resolve) => {
            data.university = context.getters.getUniversity;
            settingsService.createCourse(data).then(({body}) => {
                context.commit(USER.UPDATE_USER, { myCourses: [...context.getters.myCourses, { id: body.id, name: data.name }] });
                resolve({id:body.id,name:data.name,code:data.code});
            });
        });
    },
    updateFirstTime({ commit }) {
        commit(USER.UPDATE_USER, {isFirst:false});
    },
    updateUniversity({ commit }, {id,name}) {
        commit(USER.UPDATE_USER, { universityId: {id,name} });
    },
    updatePinnedCards(context, data) {
        context.commit(USER.UPDATE_USER, { pinnedCards: { ...context.getters.pinnedCards,...data} });
    }

};
export default {
    state,
    mutations,
    getters,
    actions
}