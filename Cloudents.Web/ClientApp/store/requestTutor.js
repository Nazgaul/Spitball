const state = {
    requestDialog: false,
    currTutor: {
        name: null
    }
};

const getters = {
    getRequestTutorDialog:  state => state.requestDialog,
    getCurrTutor: state => state.currTutor
};

const mutations = {
    setRequestDialog(state, val) {
        state.requestDialog = val
    },
    setCurrTutor: (state, name) => {
        state.currTutor.name = name;
    },
};

const actions = {
    updateCurrTutor({commit},name){ 
        commit('setCurrTutor', name)},
    updateRequestDialog({commit, dispatch}, val){
        commit('setRequestDialog', val);
        if(!val){
            dispatch(updateCurrTutor, null);
        }
    },
};
export default {
    state,
    mutations,
    getters,
    actions
}