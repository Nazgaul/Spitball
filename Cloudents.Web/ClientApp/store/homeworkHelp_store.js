import { skeletonData } from '../components/results/consts';
import searchService from "./../services/searchService";
import reputationService from './../services/reputationService';
import reportService from "./../services/cardActionService"

const state = {
    queItems: [],
    items: {},
    itemsSkeleton: skeletonData.ask,
    dataLoaded: false,
    newBallerDialogState: false
};

const mutations = {

    changeNewBallerDialogState(state, val){
        state.newBallerDialogState = val;
    },

    HomeworkHelp_SetItems(state, data) {
        state.items = data;
    },
    HomeworkHelp_setDataLoaded(state, data){
        state.dataLoaded = data;
    },
    HomeworkHelp_UpdateItems(state, data) {
        state.items.data = state.items.data.concat(data.data);
        state.items.nextPage = data.nextPage;
    },
    HomeworkHelp_ResetQue(state) {
        //check if ask Tab was loaded at least once
        state.queItems = [];
    },

    HomeworkHelp_AddQuestion(state, questionObj) {
        //check if ask Tab was loaded at least once
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            //put the question in que (pop up should show)
            if (!!questionObj.user) {
                //check if cuurent active user is the same as the user that posted the message
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
    HomeworkHelp_injectQuestion(state) {
        //check if ask Tab was loaded at least once
        if (state.queItems.length > 0) {
            state.queItems.forEach((itemToAdd) => {
                if (!!state.items.data && state.items.data.length > 0) {
                    state.items.data.unshift(itemToAdd);
                }
            });
            state.queItems = [];
        }
    },
    HomeworkHelp_removeQuestion(state, questionToRemove) {
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let questionIndex = 0; questionIndex < state.items.data.length; questionIndex++) {
                let currentQuestion = state.items.data[questionIndex];
                if (currentQuestion.id === questionToRemove.id) {
                    //remove the question from the list
                    state.items.data.splice(questionIndex, 1);
                    return;
                }
            }
        }
    },
    HomeworkHelp_markQuestionAsCorrect(state, questionToCorrect) {
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let questionIndex = 0; questionIndex < state.items.data.length; questionIndex++) {
                let currentQuestion = state.items.data[questionIndex];
                if (currentQuestion.id === questionToCorrect.questionId) {
                    //replace the question from the list
                    state.items.data[questionIndex].hasCorrectAnswer = true;
                    state.items.data[questionIndex].correctAnswerId = questionToCorrect.answerId;
                    return;
                }
            }
        }
    },
    HomeworkHelp_updateAnswersCounter(state, actionObj) {
        let questionId = actionObj.questionId;
        let addAnswersCounter = actionObj.addCounter;
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let questionIndex = 0; questionIndex < state.items.data.length; questionIndex++) {
                let currentQuestion = state.items.data[questionIndex];
                if (currentQuestion.id === questionId) {
                    if (addAnswersCounter) {
                        state.items.data[questionIndex].answers += 1;
                    } else {
                        state.items.data[questionIndex].answers -= 1;
                    }
                    return;
                }
            }
        }
    },
    HomeworkHelp_addViewer(state, question) {
        if (!!state.items && state.items.data && state.items.data.length) {
            state.items.data.forEach((ask, index) => {
                if (ask.id === question.id) {
                    state.items.data[index].watchingNow++;
                }
            });
        }
    },
    HomeworkHelp_removeViewer(state, question) {
        if (!question) return;
        if (!!state.items && state.items.data && state.items.data.length) {
            state.items.data.forEach((ask, index) => {
                if (ask.id === question.id) {
                    if (state.items.data[index].watchingNow > 0) {
                        state.items.data[index].watchingNow--;
                    }
                }
            });
        }
    },
    HomeworkHelp_questionVote(state, {id, type}) {
        if (!!state.items && state.items.data && state.items.data.length) {
            state.items.data.forEach((question) => {
                if (question.id === id) {
                    reputationService.updateVoteCounter(question, type);
                }
            });
        }
    },
};

const getters = {
    newBallerDialog : (state) =>{
       return state.newBallerDialogState;
    },

    HomeworkHelp_getItems: function (state, {getIsLoading, getSearchLoading}) {
        if (getIsLoading || getSearchLoading) {
            //return skeleton
            return state.itemsSkeleton;
        } else {
            //return data
            return state.items.data;
        }
    },
    HomeworkHelp_getNextPageUrl: function (state) {
        return state.items.nextPage;
    },
    HomeworkHelp_getShowQuestionToaster: function (state) {
        return !!state.queItems ? state.queItems.length > 0 : false;
    },
    HomeworkHelp_isDataLoaded: function(state){
        return state.dataLoaded;
    }
};

const actions = {
    updateNewBallerDialogState({commit}, val){
        commit('changeNewBallerDialogState', val);
    },
    HomeworkHelp_nextPage(context, {url, vertical}) {
        return searchService.nextPage({url, vertical}).then((data) => {
            context.dispatch('HomeworkHelp_updateData', data);
            return data;
        });
    },

    HomeworkHelp_updateDataLoaded({commit}, data){
        commit('HomeworkHelp_setDataLoaded', data);
    },
    HomeworkHelp_fetchingData(context, {name, params, page, skipLoad}) {
        let paramsList = {...context.state.search, ...params, page};
        //update box terms
        // context.dispatch('updateAITerm', {vertical: name, data: {text: paramsList.term}});
        context.dispatch('HomeworkHelp_updateDataLoaded', false);
        //get location if needed
        let VerticalName = 'ask';
        let verticalItems = context.state.items;
        let skip = determineSkip(VerticalName, verticalItems);
        let haveQueItems = context.state.queItems.length;
        //when entering a question and going back stay on the same position.
        //can be removed only when question page willo be part of ask question page
        if ((!!verticalItems && !!verticalItems.data && (verticalItems.data.length > 0 && verticalItems.data.length < 150) && !context.getters.getSearchLoading) || skip) {
            if (haveQueItems) {
                context.commit('HomeworkHelp_injectQuestion');
            }

            let filtersData = !!verticalItems.filters ? searchService.createFilters(verticalItems.filters) : null;
            let sortData = !!verticalItems.sort ? verticalItems.sort : null;
            context.dispatch('updateSort', sortData);
            context.dispatch('updateFilters', filtersData);
            context.dispatch('HomeworkHelp_updateDataLoaded', true);
            return verticalItems;
        } else {
            context.commit('HomeworkHelp_ResetQue');
            return getData();
        }


        function determineSkip(verticalName, verticalItems) {
            /*
                if comming from question page we need to make sure before we auto skip the loading
                that we have some vertical items in the system if not then we dont want to skip the load
            */
            if (!!verticalItems && !!verticalItems.data && verticalItems.data.length > 0) {
                return skipLoad;
            } else {
                return false;
            }
        }

        function getData() {
            return new Promise((resolve) => {
                resolve();
            }).then(() => {
                return searchService.activateFunction[name](paramsList).then((data) => {
                    context.dispatch('HomeworkHelp_setDataItems', data);
                    let sortData = !!data.sort ? data.sort : null;
                    context.dispatch('updateSort', sortData);
                    let filtersData = !!data.filters ? searchService.createFilters(data.filters) : null;
                    context.dispatch('updateFilters', filtersData);
                    context.dispatch('HomeworkHelp_updateDataLoaded', true);
                    return data;
                }, (err) => {
                    return Promise.reject(err);
                });
            });
        }
    },
    HomeworkHelp_setDataItems({commit}, data) {
        commit('HomeworkHelp_SetItems', data);
    },
    HomeworkHelp_updateData({commit}, data) {
        commit('HomeworkHelp_UpdateItems', data);
    },
    addQuestionItemAction({commit, getters}, notificationQuestionObject) {
        let user = getters.accountUser;
        let questionObj = searchService.createQuestionItem(notificationQuestionObject);
        let questionToSend = {
            user,
            question: questionObj
        };
        commit('HomeworkHelp_AddQuestion', questionToSend);
    },
    removeQuestionItemAction({commit}, notificationQuestionObject) {
        let questionObj = searchService.createQuestionItem(notificationQuestionObject);
        commit('HomeworkHelp_removeQuestion', questionObj);
    },
    updateQuestionCorrect({commit}, notificationQuestionObject) {
        let questionObj = {
            questionId: notificationQuestionObject.questionId,
            answerId: notificationQuestionObject.answerId
        };
        commit('HomeworkHelp_markQuestionAsCorrect', questionObj);
    },
    HomeworkHelp_updateCounter({commit}, actionObj) {
        commit('HomeworkHelp_updateAnswersCounter', actionObj);
    },
    HomeworkHelp_questionVote({commit, dispatch}, data) {
        reputationService.voteQuestion(data.id, data.type).then(() => {
            commit('HomeworkHelp_questionVote', data);
            dispatch('innerQuestionVote', data);
            dispatch('profileVote', data);
        }, (err) => {
            let errorObj = {
                toasterText: err.response.data.Id[0],
                showToaster: true,
            };
            dispatch('updateToasterParams', errorObj);
        });
    },
    HomeworkHelp_reportQuestion({commit, dispatch}, data) {
        return reportService.reportQuestion(data).then(() => {
            let objToRemove = {
                id: data.id
            };
            dispatch('removeQuestionItemAction', objToRemove);
            dispatch('removeItemFromProfile', objToRemove);
        });
    },
    
    HomeworkHelp_reportAnswer({commit, dispatch}, data) {
        let objToRemove = {
            id: data.id
        };
        return reportService.reportAnswer(data).then((success) => {
                console.log(success, 'reported answer');
                dispatch('removeItemFromProfile', objToRemove);
            },
            (error) => {
                console.log(error, 'error report answer');
            }
        );
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
