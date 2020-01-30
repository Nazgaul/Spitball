
import searchService from "../services/searchService";
import reportService from "../services/cardActionService";

const state = {
    queItems: [],
    items: {},
    dataLoaded: false,
};

const getters = {
    Feeds_getItems: (state, {getIsLoading, getSearchLoading}) => {
        return (getIsLoading || getSearchLoading) ? state.dataLoaded : state.items.data;
    },
    Feeds_getNextPageUrl: (state) =>  state.items.nextPage,
};

const mutations = {
    Feeds_SetItems(state, data) {
        state.items = data;
    },
    Feeds_setDataLoaded(state, data){
        state.dataLoaded = data;
    },
    Feeds_UpdateItems(state, data) {
        state.items.data = state.items.data.concat(data.data);
        state.items.nextPage = data.nextPage;
    },
    Feeds_ResetQue(state) {
        state.queItems = [];
    },
    Feeds_AddQuestion(state, questionObj) {
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            if (!!questionObj.user) {
                let authorId = questionObj.question.userId;
                let currentUser = questionObj.user;
                let questionToAdd = questionObj.question;
                if (currentUser.id === authorId) {
                    state.items.data.unshift(questionToAdd);
                } else {
                    state.queItems.unshift(questionToAdd);
                }
            } else {
                state.queItems.unshift(questionObj.question);
            }
        }
    },
    Feeds_removeQuestion(state, questionIndex) {
        state.items.data.splice(questionIndex, 1);
    },
    Feeds_removeDocItem(state, itemIndex) {
        state.items.data.splice(itemIndex, 1);
    },
    Feeds_markQuestionAsCorrect(state, questionObj) {
        state.items.data[questionObj.questionIndex].hasCorrectAnswer = true;
        state.items.data[questionObj.questionIndex].correctAnswerId = questionObj.answerId;
    }
};

const actions = {
    Feeds_nextPage({dispatch}, {url, vertical}) {
        return searchService.nextPage({url, vertical}).then((data) => {
            dispatch('Feeds_updateData', data);
            return data;
        });
    },
    Feeds_updateDataLoaded({commit}, data){
        commit('Feeds_setDataLoaded', data);
    },
    Feeds_setDataItems({commit}, data) {
        commit('Feeds_SetItems', data);
    },
    Feeds_updateData({commit}, data) {
        commit('Feeds_UpdateItems', data);
    },
    Feeds_fetchingData({state, commit, dispatch}, {name, params, page}) {
        dispatch('Feeds_updateDataLoaded', false);
        commit('Feeds_ResetQue');

        let paramsList = {...state.search, ...params, page};
        let route = name.toLowerCase();
        
        return searchService.activateFunction[route](paramsList).then((data) => {
            update(data);
            dispatch('Feeds_setDataItems', data);
            return data;
        }, (err) => {
            return Promise.reject(err);
        });
        function update(data) {
            let sortData = !!data.sort ? data.sort : null;
            let filtersData = !!data.filters ? searchService.createFilters(data.filters) : null;
            dispatch('updateSort', sortData);
            dispatch('updateFilters', filtersData);
            dispatch('Feeds_updateDataLoaded', true);
        }
    },
    addQuestionItemAction({commit, getters}, notificationQuestionObject) {
        let questionToSend = {
            user: getters.accountUser,
            question: searchService.createQuestionItem(notificationQuestionObject)
        };
        commit('Feeds_AddQuestion', questionToSend);
    },
    removeQuestionItemAction({commit, state}, notificationQuestionObject) {
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let questionIndex = 0; questionIndex < state.items.data.length; questionIndex++) {
                let currentQuestion = state.items.data[questionIndex];
                if (currentQuestion.id === notificationQuestionObject.id) {
                    commit('Feeds_removeQuestion', questionIndex);
                }
            }
        }
    },
    removeDocItemAction({commit, state}, notificationDocItemObject) {
        let documentToRemove = searchService.createDocumentItem(notificationDocItemObject);
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let documentIndex = 0; documentIndex < state.items.data.length; documentIndex++) {
                let currentDocument = state.items.data[documentIndex];
                if (currentDocument.id === documentToRemove.id) {
                    commit('Feeds_removeDocItem',documentIndex);
                }
            }
        }
    },
    updateQuestionCorrect({commit, state}, notificationQuestionObject) {
        let questionObj = {
            questionId: notificationQuestionObject.questionId,
            answerId: notificationQuestionObject.answerId
        };
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let questionIndex = 0; questionIndex < state.items.data.length; questionIndex++) {
                let currentQuestion = state.items.data[questionIndex];
                if (currentQuestion.id === questionObj.questionId) {
                    commit('Feeds_markQuestionAsCorrect', {answerId: questionObj.answerId, questionIndex});
                }
            }
        }
    },
    Feeds_updateCounter({commit, state}, actionObj) {
        let questionId = actionObj.questionId;
        let addAnswersCounter = actionObj.addCounter;
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let questionIndex = 0; questionIndex < state.items.data.length; questionIndex++) {
                let currentQuestion = state.items.data[questionIndex];
                if (currentQuestion.id === questionId) {
                    let val = (addAnswersCounter) ? 1 : -1;
                    commit('Feeds_updateAnswersCounter', val);
                }
            }
        }
    },
    Feeds_reportQuestion({dispatch}, data) {
        return reportService.reportQuestion(data).then(() => {
            let objToRemove = { id: data.id };
            dispatch('removeQuestionItemAction', objToRemove);
            dispatch('removeItemFromProfile', objToRemove);
        }, (error) => {
            console.log(error, 'error report question');
        });
    },
    Feeds_reportAnswer({dispatch}, data) {
        let objToRemove = { id: data.id };
        return reportService.reportAnswer(data).then(() => {
            dispatch('removeItemFromProfile', objToRemove);
        }, (error) => {
            console.log(error, 'error report answer');
        });
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}