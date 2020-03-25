const state = {
    referral:false,
    registerDialog: false
};
const mutations = {
    setReferralDialog(state,val){
        state.referral = val;
    },
    setRegisterDialog(state, val) {
        state.registerDialog = val
    }
    
};
const getters = {
    getReferralDialog:state => state.referral,
    getRegisterDialog: state => state.registerDialog
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