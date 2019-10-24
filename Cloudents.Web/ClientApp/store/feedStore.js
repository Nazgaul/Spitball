import { skeletonData } from '../components/results/consts';
import searchService from "../services/searchService";
import reportService from "../services/cardActionService";

const state = {
    queItems: [],
    items: {},
    itemsSkeleton: skeletonData.ask,
    dataLoaded: false,
}

const getters = {
    Feeds_getItems: (state, {getIsLoading, getSearchLoading}) => {
        return (getIsLoading || getSearchLoading) ? state.itemsSkeleton : state.items.data
    },
    Feeds_getNextPageUrl: (state) =>  state.items.nextPage,
    Feeds_getShowQuestionToaster: (state) => !!state.queItems ? state.queItems.length > 0 : false,
    Feeds_isDataLoaded: (state) => state.dataLoaded,
}

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
                let author = questionObj.question.user;
                let currentUser = questionObj.user;
                let questionToAdd = questionObj.question;
                if (currentUser.id === author.id) {
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
    Feeds_markQuestionAsCorrect(state, questionObj) {
        state.items.data[questionObj.questionIndex].hasCorrectAnswer = true;
        state.items.data[questionObj.questionIndex].correctAnswerId = questionObj.answerId;
    },
    Feeds_updateAnswersCounter(state, counter) {
        state.items.data[questionIndex].answers += counter;
    },
    Feeds_injectQuestion(state) {
        //check if ask Tab was loaded at least once
        if (state.queItems.length > 0) {
            state.queItems.forEach((itemToAdd) => {
                if (!!state.items.data && state.items.data.length > 0) {
                    state.items.data.unshift(itemToAdd);
                }
            })
            state.queItems = [];
        }
    },
}

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
    Feeds_fetchingData({state, commit, getters, dispatch}, {name, params, page, skipLoad}) {
        dispatch('Feeds_updateDataLoaded', false);
        let verticalItems = state.items;
        let skip = (!!verticalItems && !!verticalItems.data && verticalItems.data.length > 0) ? skipLoad : false;
        if ((!!verticalItems && !!verticalItems.data && (verticalItems.data.length > 0 && verticalItems.data.length < 150) && !getters.getSearchLoading) || skip) {
            if (state.queItems.length) {
                commit('Feeds_injectQuestion')
            }
            update(verticalItems);
            return verticalItems;
        } else {
            commit('Feeds_ResetQue');
            let paramsList = {...state.search, ...params, page};
            return searchService.activateFunction[name](paramsList).then((data) => {
                update(data);
                dispatch('Feeds_setDataItems', data);
                return data;
            }, (err) => {
                return Promise.reject(err);
            })
        }
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
        }
        commit('Feeds_AddQuestion', questionToSend);
    },
    removeQuestionItemAction({commit, state}, notificationQuestionObject) {
        let questionObj = searchService.createQuestionItem(notificationQuestionObject);
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let questionIndex = 0; questionIndex < state.items.data.length; questionIndex++) {
                let currentQuestion = state.items.data[questionIndex];
                if (currentQuestion.id === questionObj.id) {
                    commit('Feeds_removeQuestion', questionIndex);
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
        let questionId = actionObj.questionId
        let addAnswersCounter = actionObj.addCounter
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let questionIndex = 0; questionIndex < state.items.data.length; questionIndex++) {
                let currentQuestion = state.items.data[questionIndex];
                if (currentQuestion.id === questionId) {
                    let val = (addAnswersCounter) ? 1 : -1;
                    commit('Feeds_updateAnswersCounter', val)
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
        })
    },
    Feeds_reportAnswer({dispatch}, data) {
        let objToRemove = { id: data.id };
        return reportService.reportAnswer(data).then((success) => {
            dispatch('removeItemFromProfile', objToRemove);
        }, (error) => {
            console.log(error, 'error report answer');
        })
    }
}

export default {
    state,
    mutations,
    getters,
    actions
}