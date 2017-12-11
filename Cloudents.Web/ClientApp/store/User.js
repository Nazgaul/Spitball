import settingsService from './../services/settingsService'
import { USER } from './mutation-types'
const state = {
    user: {
        universityId: null,
        myCourses: [],
        isFirst: true,
        courseFirstTime:true,
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
    courseFirstTime:state => state.user.courseFirstTime,
    pinnedCards: state => state.user.pinnedCards,
    getUniversity: state => {
        let obj = state.user.universityId || {};
        return obj.id;
    },
    getUniversityName: state => {
        let obj = state.user.universityId || {};
        return obj.name;
    },
    getUniversityImage:state => {
        let obj = state.user.universityId || {};
        return obj.image;
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

    createCourse(context, {name,code}) {
            const university = context.getters.getUniversity;
            return settingsService.createCourse({name,code,university}).then(({data:body}) => {
                context.commit(USER.UPDATE_USER, { myCourses: [...context.getters.myCourses, { id: body.id, name: name }] });
            });
    },
    updateFirstTime( {commit},val ) {
        let ob={[val]:false};
        commit(USER.UPDATE_USER, ob);
    },
    updateUniversity({ commit }, {id,name,image}) {
        commit(USER.UPDATE_USER, { universityId: {id,name,image} });
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