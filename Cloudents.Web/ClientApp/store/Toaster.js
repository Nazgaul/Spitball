import { TOASTER } from './mutation-types'

const state = {
    params: {
        toasterText: '',
        showToaster:false,
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
};
const actions = {
    updateToasterParams({commit}, val){
        console.log('from toaster',commit, val);
        commit(TOASTER.UPDATE_PARAMS, val);
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}