import { TOASTER } from './mutation-types'

const state = {
    params: {
        undoAction: null,
        toasterText: '',
        showToaster:false,
        undoReturnPath: ''
    }
};
const mutations = {
    [TOASTER.UPDATE_PARAMS](state,val) {
        state.params={...state.params,...val};
    },
};
const getters = {
    getShowToaster:  state => state.params.showToaster,
    getUndoAction: state => state.params.undoAction,
    getToasterText: state => state.params.toasterText,
    getUndoReturnPath: state => state.params.undoReturnPath,
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