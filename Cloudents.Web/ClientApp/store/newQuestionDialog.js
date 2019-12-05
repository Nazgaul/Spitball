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
    updateNewQuestionDialogState({commit,dispatch}, data) {
        let status = data.status ? data.status : data;
        let fromButton = data.from ? data.from : 0;
        if(!!status){
            let FromTypes = {
                0: "ORIGINAL",
                1: "HEADER_BUTTON",
                2: "BLOCK_CARD",
                3: "FAB_ICON",
                4: "STRIP_BUTTON",
                5: "PROFILE_ORIGINAL"};
            //fire event only when dialog opens
            dispatch('updateAnalytics_unitedEvent',['AB_TESTING', 'ASK_QUESTION_CLICKED', FromTypes[fromButton]])
        }
        //ab testing end
        commit('setNewQuestionState', status);
    },
};
export default {
    actions, state, mutations, getters
};