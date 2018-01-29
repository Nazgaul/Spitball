import settingsService from './../services/settingsService'
import { USER } from './mutation-types'
const state = {
    user: {
        universityId: null,
        myCourses: [],
        isFirst: true,
        location: null,
        pinnedCards: {}
    },
    facet:""
};

const mutations = {
    [USER.UPDATE_USER](state, payload) {
        state.user = { ...state.user, ...payload };
    },
    [USER.UPDATE_FACET](state,payload){
        state.facet=payload;
    }
};
const getters = {
    isFirst: state => state.user.isFirst,
    location: state => {
        let location=state.user.location;
        if(location&&location.constructor===String){
            let[latitude,longitude]=location.split(',');
            return {latitude,longitude}
        }else{return location}},
    pinnedCards:
        state => state.user.pinnedCards,
    getUniversity: state => {
        let obj = state.user.universityId || {};
        return obj.id;
    },
    getUniversityName: state => {
        let obj = state.user.universityId || {};
        return obj.name;
    },
    getUniversityImage: state => {
        let obj = state.user.universityId || {};
        return obj.image;
    },
    myCourses: state => state.user.myCourses,
    myCoursesId: state => (state.user.myCourses.length ? state.user.myCourses.map(i => i.id) : []),
    getFacet:state=>state.facet
};
const actions = {
    updateLocation(context) {
        return new Promise((resolve) => {
            if (!context.getters.location && navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(({ coords }) => {
                    coords = coords || {};
                    context.commit(USER.UPDATE_USER, { location: {latitude:coords.latitude,longitude:coords.longitude}});
                    resolve(context.getters.location);
                }, () => { resolve(context.getters.location) });
            }
            else {
                resolve(context.getters.location);
            }
        });
    },
    getUniversities(context, data) {
        return context.dispatch("updateLocation").then((location) => {
            return settingsService.getUniversity({ term: data.term, location });
        });
    },
    getCorses(context, { term }) {
        return settingsService.getCourse({ term, universityId: context.getters.getUniversity });
    },

    createCourse(context, { name, code }) {
        const university = context.getters.getUniversity;
        return settingsService.createCourse({ name, code, university }).then(({ data: body }) => {
            context.commit(USER.UPDATE_USER, { myCourses: [...context.getters.myCourses, { id: body.id, name: name }] });
        });
    },
    updateFirstTime({ commit }, val) {
        let ob = { [val]: false };
        commit(USER.UPDATE_USER, ob);
    },
    updateUniversity({ commit }, { id, name, image }) {
        commit(USER.UPDATE_USER, { universityId: { id, name, image } });
    },
    updatePinnedCards(context, data) {
        context.commit(USER.UPDATE_USER, { pinnedCards: { ...context.getters.pinnedCards, ...data } });
    },
    updateFacet({commit},data){
        commit(USER.UPDATE_FACET,data)
    }

};
export default {
    state,
    mutations,
    getters,
    actions
}