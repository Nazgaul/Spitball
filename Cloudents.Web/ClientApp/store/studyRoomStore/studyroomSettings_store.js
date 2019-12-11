const state = {
    stepHistory:[],
    visitedSettingPage: false,
};

const getters = {
    getStepHistory:state=> state.stepHistory,
    getVisitedSettingPage:state=> state.visitedSettingPage,
};

const mutations = {
    reOrderStepHistory(state, currentPageIndex){
        let newStepHistory = state.stepHistory.slice(0, currentPageIndex+1);
        state.stepHistory = newStepHistory;
    },
    setStepHistory(state, step){
        state.stepHistory.push(step);
    },
    setVisitedSettingPage(state, val){
        state.visitedSettingPage = val;
    },
};

const actions = {
    reOrderStepHistory({commit}, currentPageIndex){
        commit('reOrderStepHistory', currentPageIndex);
    },
    setStepHistory({commit}, step){
        commit('setStepHistory', step);
    },
    pushHistoryState({state}){
        history.pushState({page: state.stepHistory[state.stepHistory.length-1]}, `page ${state.stepHistory.length-1}`);
    },
    replaceHistoryState({state}){
        if(state.stepHistory.length === 1){
            history.replaceState({page: state.stepHistory[state.stepHistory.length-1]}, `page ${state.stepHistory.length-1}`);
        }
    },
    setVisitedSettingPage({commit}, val){
        commit('setVisitedSettingPage', val);
    },
};

export default {
    state,
    mutations,
    getters,
    actions
}