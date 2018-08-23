﻿import {SEARCH} from "./mutation-types"
import {skeletonData} from '../components/results/consts'
import searchService from "./../services/searchService"
const LOCATION_VERTICALS= new Map([["tutor",true],["job",true]]);
const state = {
    loading: false,
    serachLoading: false,
    search:{},
    itemsPerVertical: {
        ask:[],
        note:[],
        flashcard:[],
        tutor:[],
        book:[],
        job:[]
    },
    itemsSkeletonPerVertical: {
        ask: skeletonData.ask,
        note: skeletonData.note,
        flashcard: skeletonData.flashcard,
        tutor: skeletonData.tutor,
        book: skeletonData.book,
        job: skeletonData.job
    }
};

const mutations = {
    [SEARCH.UPDATE_LOADING](state, payload) {
        state.loading = payload;
    },
    [SEARCH.UPDATE_SEARCH_LOADING](state, payload) {
        state.serachLoading = payload;
    },
    [SEARCH.UPDATE_SEARCH_PARAMS](state, updatedDate) {
        state.search = {...updatedDate};
    },
    [SEARCH.SET_ITEMS_BY_VERTICAL](state, verticalObj){
        state.itemsPerVertical[verticalObj.verticalName] = verticalObj.verticalData;
    },
    [SEARCH.UPDATE_ITEMS_BY_VERTICAL](state, verticalObj){
        state.itemsPerVertical[verticalObj.verticalName].data = state.itemsPerVertical[verticalObj.verticalName].data.concat(verticalObj.verticalData.data)
        state.itemsPerVertical[verticalObj.verticalName].nextPage = verticalObj.verticalData.nextPage
    },
    [SEARCH.ADD_QUESTION](state, questionToAdd){
        state.itemsPerVertical.ask.data.unshift(questionToAdd);
    },
    [SEARCH.REMOVE_QUESTION](state, questionToRemove){
        for(let questionIndex = 0; questionIndex < state.itemsPerVertical.ask.data.length; i++ ){
            let currentQuestion = state.itemsPerVertical.ask.data[questionIndex];
            if(currentQuestion.id === questionToRemove.id){
                //remove the question from the list
                state.itemsPerVertical.ask.data.splice(questionIndex, 1);
                return;
            }
        }
    },
    [SEARCH.ADD_QUESTION_VIEWR](state, question){
        let questionIndex = state.itemsPerVertical.ask.data.indexOf(question);
        state.itemsPerVertical.ask.data[questionIndex].watchingNow++; 
    },
    [SEARCH.REMOVE_QUESTION_VIEWR](state, question){
        let questionIndex = state.itemsPerVertical.ask.data.indexOf(question);
        if(state.itemsPerVertical.ask.data[questionIndex].watchingNow > 0){
            state.itemsPerVertical.ask.data[questionIndex].watchingNow--; 
        }
    }
};

const getters = {
    getIsLoading: state => state.loading,
    getSearchItems: function(state, {getCurrentVertical}) { 
        if(getCurrentVertical === ""){ 
            return [];
        };
        if(state.loading || state.serachLoading){
            //return skeleton
            return state.itemsSkeletonPerVertical[getCurrentVertical];
        }else{
            //return data
            return state.itemsPerVertical[getCurrentVertical].data;   
        }
    }
};

const actions = {
    //Always update the current route according the flow
    bookDetails: (context, { pageName, isbn13, type }) => {
        return searchService.activateFunction[pageName]({ isbn13, type });
    },
    getAutocmplete(context, term) {
        return searchService.autoComplete(term);
    },
    nextPage(context, {url,vertical}){
        return searchService.nextPage({url,vertical}).then((data)=>{
            let verticalObj = {
                verticalName: vertical,
                verticalData: data
            }
            context.dispatch('updateDataByVerticalType', verticalObj);
            return data;
        });
    },
    fetchingData(context, { name, params, page, skipLoad}){
        let university = context.rootGetters.getUniversity ? context.rootGetters.getUniversity : null;
         let paramsList = {...context.state.search,...params, university, page};
            //update box terms
            context.dispatch('updateAITerm',{vertical:name,data:{text:paramsList.term}});
            //get location if needed
            let VerticalName = context.getters.getCurrentVertical;
            let verticalItems = context.state.itemsPerVertical[VerticalName];
            if((!!verticalItems && !!verticalItems.data && (verticalItems.data.length > 0 && verticalItems.data.length < 150) && !context.state.serachLoading) || skipLoad){
                return verticalItems
            }else{
                return new Promise((resolve) => {
                    if (LOCATION_VERTICALS.has(name) && !paramsList.location) {
                        context.dispatch("updateLocation").then((location) => {
                            paramsList.location = location;
                            resolve();
                        });
                    } else { resolve(); }
                }).then(() => {
                    return searchService.activateFunction[name](paramsList).then((data) => {
                        let verticalObj = {
                            verticalName: name,
                            verticalData: data
                        }
                        context.dispatch('setDataByVerticalType', verticalObj);
                        return data;
                    },(err) => {
                        return err;
                    })
                });
            }
    },
    setDataByVerticalType({ commit }, verticalObj){
        commit(SEARCH.SET_ITEMS_BY_VERTICAL, verticalObj);
    },
    updateDataByVerticalType({ commit }, verticalObj){
        commit(SEARCH.UPDATE_ITEMS_BY_VERTICAL, verticalObj);
    },
    addQuestionItemAction({ commit }, notificationQuestionObject){
       let questionObj = searchService.createQuestionItem(notificationQuestionObject);
       commit(SEARCH.ADD_QUESTION, questionObj);
    },
    removeQuestionItemAction({ commit }, notificationQuestionObject){
        let questionObj = searchService.createQuestionItem(notificationQuestionObject);
       commit(SEARCH.REMOVE_QUESTION, questionObj);
    },
    addQuestionViewr({ commit }, notificationQuestionObject){
        let questionObj = notificationQuestionObject;
        commit(SEARCH.ADD_QUESTION_VIEWR, questionObj);
    },
    removeQuestionViewr({ commit }, notificationQuestionObject){
        let questionObj = notificationQuestionObject;
        commit(SEARCH.REMOVE_QUESTION_VIEWR, questionObj);
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
