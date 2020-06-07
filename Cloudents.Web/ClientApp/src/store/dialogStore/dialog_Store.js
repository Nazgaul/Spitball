const state = {
    referral:false,
    globalLoading: false,
};

const getters = {
    getReferralDialog:state => state.referral,
    getGlobalLoading: state => state.globalLoading,
};

const mutations = {
    setReferralDialog(state,val){
        state.referral = val;
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