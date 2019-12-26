const state = {
    referral:false,
    
};
const mutations = {
    setReferralDialog(state,val){
        state.referral = val;
    }
    
};
const getters = {
    getReferralDialog:state => state.referral,
    
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