import { TOASTER } from './mutation-types'

const state = {
    params: {
        undoAction: false,
        closeToaster: false,
        toasterText: '',
        showToaster:false,
        toasterTimeOut: 5000
    }
};
const mutations = {
    [TOASTER.UPDATE_PARAMS](state,val) {
        state.params={...state.params,...val};
    },
};
const getters = {
    getShowToaster:  state => state.params.showToaster,
    isUndoAction: state => state.params.undoAction,
    isCloseToaster: state => state.params.closeToaster,
    getToasterText: state => state.params.toasterText,
    getToasterTimeOut: state => state.params.toasterTimeOut,
};
const actions = {
    updateParams({commit}, val){
        commit(TOASTER.UPDATE_PARAMS, val);
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}