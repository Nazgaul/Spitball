import bannerService from '../services/bannerService.js'

const state = {
    bannerSatus: false, 
    bannerParams: null,
}
const mutations = {
    setBannerSatus: (state,val) => state.bannerSatus = val,
    setBannerParams: (state,val) => state.bannerParams = val,
}
const getters = {
    getBannerSatus: state => state.bannerSatus,
    getBannerParams: state => state.bannerParams,
}
const actions = {
    updateBannerSatus({commit,state,dispatch},val){
        if(val){
            dispatch('updateBannerParams')
        }else{
            bannerService.bannerStorage(state.bannerParams.id)
            commit('setBannerSatus',false)
        }
    },
    updateBannerParams({commit}){
        bannerService.getBannerParams().then(params=>{ 
            if(!!params && params.id){
                let localStorageList = JSON.parse(global.localStorage.getItem("sb_banner"));
                if(localStorageList !== null && localStorageList.includes(params.id)){
                    commit('setBannerSatus',false)
                }else{
                    commit('setBannerSatus',true)
                    commit('setBannerParams',params)
                }
            }else{
                commit('setBannerSatus',false)
            }
        })
    }
}
export default {
    state,
    mutations,
    getters,
    actions
}
