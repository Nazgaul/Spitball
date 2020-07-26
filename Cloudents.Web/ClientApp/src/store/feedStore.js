

//const emptyStateSelection = {key:'Empty',value:''};
const state = {
    //queItems: [],
    items: {},
    dataLoaded: false,
    isFeedLoading: false,
};

const getters = {

}

const mutations = {

    Feeds_removeDocItem(state, itemIndex) {
        state.items.data.splice(itemIndex, 1);
    },
    // Feeds_markQuestionAsCorrect(state, questionObj) {
    //     state.items.data[questionObj.questionIndex].hasCorrectAnswer = true;
    //     state.items.data[questionObj.questionIndex].correctAnswerId = questionObj.answerId;
    // },
    // Feeds_updateAnswersCounter(state, {counter,questionIndex}) {
    //     state.items.data[questionIndex].answers += counter;
    // },
};

const actions = {

    removeDocItemAction({commit, state}, notificationDocItemObject) {
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let documentIndex = 0; documentIndex < state.items.data.length; documentIndex++) {
                let currentDocument = state.items.data[documentIndex];
                if (currentDocument.id === notificationDocItemObject.id) {
                    commit('Feeds_removeDocItem',documentIndex);
                }
            }
        }
    },

};

export default {
    state,
    mutations,
    getters,
    actions
}