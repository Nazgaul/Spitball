
import homePageService from '../services/homePageService.js';


const state = {
    tutorsList:[],
    itemsList:[],
    subjectsList:[],
    stats:{},
    reviews:[]
}

const getters = {
    getHPTutors: state => state.tutorsList,
    getHPItems: state => state.itemsList,
    getHPSubjects: state => state.subjectsList,
    getHPStats: state => state.stats,
    getHPReviews: state => state.reviews,
}

const mutations = {
    setHPTutors(state,tutors){
        state.tutorsList = tutors
    },
    setHPItems(state,items){
        state.itemsList = items
    },
    setHPSubjects(state,subjects){
        state.subjectsList = subjects
    },
    setHPStats(state,stats){
        state.stats = stats
    },
    setHPReviews(state,reviews){
        state.reviews = reviews
    },
}

const actions = {
    updateHPTutors({commit},count){
        homePageService.getHomePageTutors(count).then(res=>{
            commit('setHPTutors',res)
        })
    },
    updateHPItems({commit},count){
        homePageService.getHomePageItems(count).then(res=>{
            commit('setHPItems',res)
        })
    },
    updateHPSubjects({commit},count){
        homePageService.getHomePageSubjects(count).then(res=>{
            commit('setHPSubjects',res)
        })
    },
    updateHPStats({commit}){
        homePageService.getHomePageStats().then(res=>{
            commit('setHPStats',res)
        })
    },
    updateHPReviews({commit}){
        homePageService.getHomePageReviews().then(res=>{
            commit('setHPReviews',res)
        })
    }
}
export default {
    state,
    getters,
    mutations,
    actions,
}