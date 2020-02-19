
import searchService from "../services/searchService";
import reportService from "../services/cardActionService";
const emptyStateSelection = {key:'Empty',value:''};
const state = {
    queItems: [],
    items: {},
    dataLoaded: false,
    isFeedLoading: false,
};

const getters = {
    Feeds_getIsLoading: state => state.isFeedLoading,
    Feeds_getItems: (state) => {
        return (state.isFeedLoading) ? state.dataLoaded : state.items.data;
    },
    Feeds_getFilters: (state) => {
        let x = state.items.filters || [];
        let filters = x || [];
        if(filters){
            filters = filters.map(filter=>{
                return {key: filter,value: filter}
            })
            filters.unshift(emptyStateSelection)
        }
        return filters;
    },
    Feeds_getCurrentQuery (state, getters, rootState)   {
        let route = rootState.route;
        function getFilter(){
            if(route.query.filter === 'Question' && route.query.term){
                return emptyStateSelection
            }else{
                return route.query.filter || emptyStateSelection
            }
        }
        return {
            filter : getFilter(),
            course : route.query.Course,
            term: route.query.term
        };
    }
};

const mutations = {
    Feeds_SetLoading(state, data){
        state.isFeedLoading = data;
    },
    Feeds_SetItems(state, data) {
        state.items = data;
    },
    Feeds_setDataLoaded(state, data){
        state.dataLoaded = data;
    },
    Feeds_UpdateItems(state, data) {
        state.items.data = state.items.data.concat(data.data);
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
    },
    Feeds_updateAnswersCounter(state, {counter,questionIndex}) {
        state.items.data[questionIndex].answers += counter;
    },
};

const actions = {
    Feeds_UpdateLoading({commit},loadingStatus){
        commit('Feeds_SetLoading',loadingStatus)
    },
    Feeds_nextPage({dispatch}, {url}) {
        return searchService.nextPage({url}).then((data) => {
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
    Feeds_fetchingData({commit, dispatch}, {params}) {
        dispatch('Feeds_updateDataLoaded', false);
        dispatch('Feeds_UpdateLoading',true)
        commit('Feeds_ResetQue');
        
        let paramsList = {...params};
        if(paramsList.filter?.key || paramsList.filter === 'Question' && paramsList.term){
            delete paramsList.filter;
        }
        return searchService.activateFunction.feed(paramsList).then((data) => {
            dispatch('Feeds_updateDataLoaded', true)
            dispatch('Feeds_setDataItems', data);
            return data;
        }, (err) => {
            return Promise.reject(err);
        }).finally(()=>{
            dispatch('Feeds_UpdateLoading',false)
            return
        });
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
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let documentIndex = 0; documentIndex < state.items.data.length; documentIndex++) {
                let currentDocument = state.items.data[documentIndex];
                if (currentDocument.id === notificationDocItemObject.id) {
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
                    let counter = (addAnswersCounter) ? 1 : -1;
                    commit('Feeds_updateAnswersCounter', {counter,questionIndex});
                }
            }
        }
    },
    Feeds_reportQuestion({dispatch}, data) {
        return reportService.reportQuestion(data).then(() => {
            let objToRemove = { id: data.id };
            dispatch('removeQuestionItemAction', objToRemove);
        }, (error) => {
            console.log(error, 'error report question');
        });
    },
    Feeds_reportAnswer(context, data) {
        return reportService.reportAnswer(data).then(() => {
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