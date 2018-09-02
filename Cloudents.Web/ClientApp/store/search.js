import {SEARCH} from "./mutation-types"
import {skeletonData} from '../components/results/consts'
import searchService from "./../services/searchService"
const LOCATION_VERTICALS= new Map([["tutor",true],["job",true]]);
const state = {
    loading: false,
    serachLoading: false,
    search:{},
    queItemsPerVertical: {
        ask:[],
        note:[],
        flashcard:[],
        tutor:[],
        book:[],
        job:[]
    },
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
    [SEARCH.ADD_QUESTION](state, questionObj){
        //check if ask Tab was loaded at least once
        if(!!state.itemsPerVertical.ask && !!state.itemsPerVertical.ask.data && state.itemsPerVertical.ask.data.length > 0){
            //put the question in que (pop up should show)
            if(!!questionObj.user){
                //check if cuurent active user is the same as the user that posted the message
                let author = questionObj.question.user;
                let currentUser = questionObj.user;
                let questionToAdd = questionObj.question;
                if(currentUser.id === author.id){
                    state.itemsPerVertical.ask.data.unshift(questionToAdd);
                }else{
                    state.queItemsPerVertical.ask.unshift(questionToAdd);
                }
            }else{
                state.queItemsPerVertical.ask.unshift(questionToAdd);
            }
        }
    },
    [SEARCH.INJECT_QUESTION](state){
        //check if ask Tab was loaded at least once
        for(let verticalName in state.queItemsPerVertical){
            if(state.queItemsPerVertical[verticalName].length > 0){
                state.queItemsPerVertical[verticalName].forEach((itemToAdd)=>{
                    if(!!state.itemsPerVertical[verticalName].data && state.itemsPerVertical[verticalName].data.length > 0){
                        state.itemsPerVertical[verticalName].data.unshift(itemToAdd);
                    }                    
                })
                state.queItemsPerVertical[verticalName] = [];
            }
        }
    },
    [SEARCH.REMOVE_QUESTION](state, questionToRemove){
        if(!!state.itemsPerVertical.ask && !!state.itemsPerVertical.ask.data && state.itemsPerVertical.ask.data.length > 0){
            for(let questionIndex = 0; questionIndex < state.itemsPerVertical.ask.data.length; i++ ){
                let currentQuestion = state.itemsPerVertical.ask.data[questionIndex];
                if(currentQuestion.id === questionToRemove.id){
                    //remove the question from the list
                    state.itemsPerVertical.ask.data.splice(questionIndex, 1);
                    return;
                }
            }
        }
    },
    [SEARCH.UPDATE_QUESTION](state, questionToUpdate){
        if(!!state.itemsPerVertical.ask && !!state.itemsPerVertical.ask.data && state.itemsPerVertical.ask.data.length > 0){
            for(let questionIndex = 0; questionIndex < state.itemsPerVertical.ask.data.length; questionIndex++ ){
                let currentQuestion = state.itemsPerVertical.ask.data[questionIndex];
                if(currentQuestion.id === questionToUpdate.id){
                    //replace the question from the list
                    state.itemsPerVertical.ask.data[questionIndex].answers = questionToUpdate.answers;
                    return;
                }
            }
        }
    },
    [SEARCH.ADD_QUESTION_VIEWER](state, question){
        if(!!state.itemsPerVertical.ask && state.itemsPerVertical.ask.data && state.itemsPerVertical.ask.data.length){
            state.itemsPerVertical.ask.data.forEach((ask, index) => {
                if(ask.id === question.id){
                    state.itemsPerVertical.ask.data[index].watchingNow++; 
                }
            });
        }
    },
    [SEARCH.REMOVE_QUESTION_VIEWER](state, question){
        if(!question) return;
        if(!!state.itemsPerVertical.ask && state.itemsPerVertical.ask.data && state.itemsPerVertical.ask.data.length){
            state.itemsPerVertical.ask.data.forEach((ask, index) => {
                if(ask.id === question.id){
                    if(state.itemsPerVertical.ask.data[index].watchingNow > 0){
                        state.itemsPerVertical.ask.data[index].watchingNow--; 
                    }
                    
                }
            });
        }
    },
    [SEARCH.RESET_DATA](state){
        for(let prop in state.itemsPerVertical){
            state.itemsPerVertical[prop].data = [];
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
    },
    getShowQuestionToaster: function(state, {getCurrentVertical}){
        return !!state.queItemsPerVertical[getCurrentVertical] ? state.queItemsPerVertical[getCurrentVertical].length > 0 : false;
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
            let skip = determineSkip(VerticalName, verticalItems);
            let haveQueItems = context.state.queItemsPerVertical[VerticalName].length;
                if((!!verticalItems && !!verticalItems.data && (verticalItems.data.length > 0 && verticalItems.data.length < 150) && !context.state.serachLoading) || skip){
                    if(haveQueItems){
                        context.commit(SEARCH.INJECT_QUESTION)
                    }                
                    
                    let filtersData = !!verticalItems.filters ? verticalItems.filters : null;
                    let sortData = !!verticalItems.sort  ? verticalItems.sort : null;
                    context.dispatch('updateSort', sortData);
                    context.dispatch('updateFilters', filtersData);
    
                    return verticalItems
                }else{
                   return getData();
                }
            

            function determineSkip(verticalName, verticalItems){
                if(verticalName === 'ask'){
                    /* 
                        if comming from question page we need to make sure before we auto skip the loading 
                        that we have some vertical items in the system if not then we dont want to skip the load
                    */
                    if(!!verticalItems && !!verticalItems.data && verticalItems.data.length > 0){
                        return skipLoad
                    }else{
                        return false;
                    }
                }else{
                    return false;
                }
            }

            function getData(){
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
                        let sortData = !!data.sort ? data.sort : null;
                        context.dispatch('updateSort', sortData);   
                        let filtersData = !!data.filters ? data.filters : null;
                        context.dispatch('updateFilters', filtersData);
                        return data;
                    },(err) => {
                        return Promise.reject(err);
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
    addQuestionItemAction({ commit, getters }, notificationQuestionObject){
       let user = getters.accountUser;
       let questionObj = searchService.createQuestionItem(notificationQuestionObject);
       let questionToSend = {
           user,
           question: questionObj
       }
       commit(SEARCH.ADD_QUESTION, questionToSend);
    },
    removeQuestionItemAction({ commit }, notificationQuestionObject){
       let questionObj = searchService.createQuestionItem(notificationQuestionObject);
       commit(SEARCH.REMOVE_QUESTION, questionObj);
    },
    updateQuestionItem({ commit }, notificationQuestionObject){
        let questionObj = searchService.createQuestionItem(notificationQuestionObject);
        commit(SEARCH.UPDATE_QUESTION, questionObj);
    },
    addQuestionViewer({ commit }, notificationQuestionObject){
        let questionObj = notificationQuestionObject;
        commit(SEARCH.ADD_QUESTION_VIEWER, questionObj);
    },
    removeQuestionViewer({ commit }, notificationQuestionObject){
        let questionObj = notificationQuestionObject;
        commit(SEARCH.REMOVE_QUESTION_VIEWER, questionObj);
    },
    resetData({commit}){
        commit(SEARCH.RESET_DATA)
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
