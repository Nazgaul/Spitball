const state = {
    referral:false,
    loginDialog: false,
    registerDialog:  false,
    globalLoading: false,
};

const getters = {
    getReferralDialog:state => state.referral,
    getLoginDialog: state => state.loginDialog,
    getRegisterDialog: state => state.registerDialog,
    getGlobalLoading: state => state.globalLoading,
};

const mutations = {
    setReferralDialog(state,val){
        state.referral = val;
    },
    setLoginDialog(state, val) {
        state.loginDialog = val
    },
    setRegisterDialog(state, val) {
        state.registerDialog = val
    },
    setGlobalLoading(state, val) {
        state.globalLoading = val
    }
};

const actions = {
    updateReferralDialog({commit},val){
        commit('setReferralDialog',val);
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}