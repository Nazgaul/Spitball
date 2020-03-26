const state = {
    referral:false,
    loginDialog: false,
};
const mutations = {
    setReferralDialog(state,val){
        state.referral = val;
    },
    setLoginDialog(state, val) {
        state.loginDialog = val
    },
};
const getters = {
    getReferralDialog:state => state.referral,
    getLoginDialog: state => state.loginDialog
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