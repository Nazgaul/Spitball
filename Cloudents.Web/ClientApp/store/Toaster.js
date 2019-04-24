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
    }
};
const mutations = {
    [TOASTER.UPDATE_PARAMS](state,val) {
        state.params={...state.params,...val};
    },
};
const getters = {
    getShowToaster:  state => state.params.showToaster,
    getToasterText: state => state.params.toasterText,
    getShowToasterType: state=> state.params.toasterType,
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