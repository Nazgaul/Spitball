const state = {
   mobileMarketingBoxState: false
};

const getters = {
    getMobMarketingState:  state => state.mobileMarketingBoxState
};

const mutations = {
    setMarketingMobileState(state, val) {
        state.mobileMarketingBoxState = val;
    },
};


const actions = {
    changemobileMarketingBoxState({commit}, val){
        commit('setMarketingMobileState', val);
    },
};
export default {
    state,
    mutations,
    getters,
    actions
}