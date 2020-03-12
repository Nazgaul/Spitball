import { TOASTER } from './mutation-types'

const state = {
    toasterTypes:{
        default: '',
        error: 'error-toaster',
    },
    params: {
        toasterText: '',
        showToaster:false,
        toasterType: '', //class name
        toasterTimeout: 5000
    },
    value: '',
};
const mutations = {
    [TOASTER.UPDATE_PARAMS](state,val) {
        if(!val.hasOwnProperty('toasterTimeout')){
            val.toasterTimeout = 5000;
        }else if(val.toasterTimeout === undefined){
            val.toasterTimeout = 5000;
        }
        
        state.params={...state.params,...val};
        if(!val.toasterType){
            state.params.toasterType = '';
        }
    },

    setToaster(state, type) {
        state.value = type
    },
};
const getters = {
    getShowToaster:  state => state.params.showToaster,
    getToasterText: state => state.params.toasterText,
    getShowToasterType: state => state.params.toasterType,
    getToasterTimeout: state => state.params.toasterTimeout,
    getIsShowToaster: state => state.value
};
const actions = {
    updateToasterParams({commit}, val){
        commit(TOASTER.UPDATE_PARAMS, val);
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}