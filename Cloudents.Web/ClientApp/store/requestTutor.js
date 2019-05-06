const state = {
    requestDialog: false
};

const getters = {
    getRequestTutorDialog:  state => state.requestDialog,
};

const mutations = {
    setRequestDialog(state, val) {
        state.requestDialog = val
    },
};


const actions = {
    updateRequestDialog({commit}, val){
        commit('setRequestDialog', val);
    },
};
export default {
    state,
    mutations,
    getters,
    actions
}