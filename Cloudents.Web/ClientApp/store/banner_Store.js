import bannerService from '../services/bannerService.js'

const state = {
    bannerStatus: false, 
    bannerParams: null,
};
const mutations = {
    setBannerStatus: (state,val) => state.bannerStatus = val,
    setBannerParams: (state,val) => state.bannerParams = val,
};
const getters = {
    getBannerStatus: state => state.bannerStatus,
    getBannerParams: state => state.bannerParams,
};
const actions = {
    updateBannerStatus({commit,state,dispatch},val){
        if(val){
            dispatch('updateBannerParams');
        }else{
            bannerService.bannerStorage(state.bannerParams.id);
            commit('setBannerStatus',false);
        }
    },
    updateBannerParams({commit}){
        bannerService.getBannerParams().then(params => {
            params = params || {};
            if(params.id){
                let localStorageList = JSON.parse(global.localStorage.getItem("sb_banner"));
                if(localStorageList && localStorageList.includes(params.id)){
                    commit('setBannerStatus',false);
                }else{
                    commit('setBannerStatus',true);
                    commit('setBannerParams',params);
                }
            }else{
                commit('setBannerStatus',false);
            }
        });
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}
