const state = {
    referral:false,
    buyTokens:false,
    
}
const mutations = {
    setReferralDialog(state,val){
        state.referral = val
    },
    setBuyTokensDialog(state,val){
        state.buyTokens = val
    }
}
const getters = {
    getReferralDialog:state => state.referral,
    getBuyTokensDialog:state => state.buyTokens,
    
}
const actions = {
    updateReferralDialog({commit},val){
        commit('setReferralDialog',val)
    },
    updateBuyTokensDialog({commit},val){
        commit('setBuyTokensDialog',val)
    },
}
export default {
    state,
    mutations,
    getters,
    actions
}