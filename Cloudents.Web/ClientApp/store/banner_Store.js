import homePageService from '../services/homePageService.js'

const BANNER_STORAGE_NAME = "sb_banner";

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
            let bannerId = state.bannerParams.id;
            let localStorageList = JSON.parse(global.localStorage.getItem(BANNER_STORAGE_NAME));
            if(localStorageList == null){
                localStorageList = JSON.stringify([bannerId]);
                global.localStorage.setItem(BANNER_STORAGE_NAME,localStorageList);  
            }else{
                localStorageList = JSON.stringify(localStorageList.push(bannerId));
                global.localStorage.setItem(BANNER_STORAGE_NAME,localStorageList); 
            }
            commit('setBannerStatus',false);
        }
    },
    updateBannerParams({commit}){
        homePageService.getBannerParams().then(params => {
            params = params || {};
            if(params.id){
                let localStorageList = JSON.parse(global.localStorage.getItem(BANNER_STORAGE_NAME));
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
