import homePageService from '../services/homePageService.js'

const BANNER_STORAGE_NAME = "sb_banner";

const state = {
    bannerParams: null,
};
const mutations = {
    setBannerParams: (state,val) => state.bannerParams = val,
};
const getters = {
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
            commit('setBannerParams',null);
        }
    },
    updateBannerParams({commit}){
        homePageService.getBannerParams().then(params => {
            params = params || {};
            if(params.id){
                let localStorageList = JSON.parse(global.localStorage.getItem(BANNER_STORAGE_NAME));
                if(localStorageList && localStorageList.includes(params.id)){
                    commit('setBannerParams',null);
                }else{
                    commit('setBannerParams',params);
                }
            }else{
                commit('setBannerParams',null);
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
