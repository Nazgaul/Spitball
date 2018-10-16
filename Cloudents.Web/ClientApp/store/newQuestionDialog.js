import analyticsService from '../services/analytics.service'

const state = {
    newQuestionDialog: false,
};
const mutations = {
    setNewQuestionState(state, data) {
        state.newQuestionDialog = data;
    },
};
const getters = {
    newQuestionDialogSate: (state) => state.newQuestionDialog,

};
const actions = {
    updateNewQuestionDialogState({commit}, data, from = 0) {
        //ab testing start
        if(!!data){
            let FromTypes = {
                0: "ORIGINAL",
                1: "HEADER_BUTTON",
                2: "BLOCK_CARD",
                3: "FAB_ICON",
                4: "STRIP_BUTTON",
                5: "PROFILE_ORIGINAL"
            }
            //fire event only when dialog opens
            analyticsService.sb_unitedEvent('AB_TESTING', 'ASK_QUESTION_CLICKED', FromTypes[from])
        }
        //ab testing end
        commit('setNewQuestionState', data);
    },
};
export default {
    actions, state, mutations, getters
};