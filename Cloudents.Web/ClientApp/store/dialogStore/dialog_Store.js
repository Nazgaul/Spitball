const state = {
    referral:false,
    buyTokens:false,
    login:false,
    
}
const mutations = {
    setReferralDialog(state,val){
        state.referral = val
    },
    setBuyTokensDialog(state,val){
        state.buyTokens = val
    },
    setLoginDialog(state,val){
        state.login = val
    }
}
const getters = {
    getReferralDialog:state => state.referral,
    getBuyTokensDialog:state => state.buyTokens,
    getLoginDialog:state => state.login,
    
}
const actions = {
    updateReferralDialog({commit},val){
        commit('setReferralDialog',val)
    },
    updateBuyTokensDialog({commit},val){
        commit('setBuyTokensDialog',val)
    },
    updateLoginDialog({commit},val){
        commit('setLoginDialog',val)
    },
}
export default {
    state,
    mutations,
    getters,
    actions
}