const state = {
    requestDialog: false,
    currTutor: null
};

const getters = {
    getRequestTutorDialog:  state => state.requestDialog,
    getCurrTutor: state => state.currTutor
};

const mutations = {
    setRequestDialog(state, val) {
        state.requestDialog = val
    },
    setCurrTutor: (state, tutorObj) => {
        state.currTutor = tutorObj;
    },
};

const actions = {
    updateCurrTutor({commit},tutorObj){ 
        commit('setCurrTutor', tutorObj)
    },
    updateRequestDialog({commit, dispatch}, val){
        commit('setRequestDialog', val);
        if(!val){
            dispatch('updateCurrTutor', null);
        }
    },
};
export default {
    state,
    mutations,
    getters,
    actions
}