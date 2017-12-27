import settingsService from './../services/settingsService'
import { USER } from './mutation-types'
const state = {
    user: {
        universityId: null,
        myCourses: [],
        isFirst: true,
        location:null,
        pinnedCards: {}
    }
};

const mutations = {
    [USER.UPDATE_USER](state, payload) {
        state.user = { ...state.user, ...payload};
    }
};
const getters = {
    isFirst: state => false,
    location:state=>state.user.location,
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
    updateLocation(context, data){
        return new Promise((resolve)=>{
            if(!context.getters.location&&navigator.geolocation){
                navigator.geolocation.getCurrentPosition(({ coords }) => {
                    coords = coords || {};
                    context.commit(USER.UPDATE_USER, {location: `${coords.latitude},${coords.longitude}`});
                    resolve(context.getters.location);
                },()=>{resolve(context.getters.location)})
            }
            else{
                resolve(context.getters.location)
            }
        })
    },
    getUniversities(context, data) {
       return context.dispatch("updateLocation").then((location)=>{
           return settingsService.getUniversity({term:data.term,location});
       });
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