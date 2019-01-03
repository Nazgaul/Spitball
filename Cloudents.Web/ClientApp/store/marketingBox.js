const state = {
   mobileMarketingBoxState: false
};

const getters = {
    getMobMarketingState:  state => state.mobileMarketingBoxState,
};

const mutations = {
    setMarketingMobileState(state) {
        state.mobileMarketingBoxState = !state.mobileMarketingBoxState;
    },
};


const actions = {
    changemobileMarketingBoxState({commit}){
        commit('setMarketingMobileState');
    },
};
export default {
    state,
    mutations,
    getters,
    actions
}