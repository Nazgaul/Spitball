const state = {
    requestDialog: false,
    currTutor: null,
    tutorRequestAnalyticsOpenedFrom: {
        component: null,
        path: null
    }
};

const getters = {
    getRequestTutorDialog:  state => state.requestDialog,
    getCurrTutor: state => state.currTutor,
    getTutorRequestAnalyticsOpenedFrom: state => state.tutorRequestAnalyticsOpenedFrom
};

const mutations = {
    setRequestDialog(state, val) {
        state.requestDialog = val;
    },
    setCurrTutor: (state, tutorObj) => {
        state.currTutor = tutorObj;
    },
    setTutorRequestAnalyticsOpenedFrom: (state, val) => {
        state.tutorRequestAnalyticsOpenedFrom.component = val.component;
        state.tutorRequestAnalyticsOpenedFrom.path = val.path;
    }
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
    setTutorRequestAnalyticsOpenedFrom: ({commit}, val) => {
        commit('setTutorRequestAnalyticsOpenedFrom', val);
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}