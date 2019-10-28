import { SEARCH } from "./mutation-types";
import { skeletonData } from '../components/results/consts';
import searchService from "./../services/searchService";

const LOCATION_VERTICALS = new Map([["tutor", true]]);
const state = {
    //< -----keep this area ----
    loading: false,
    serachLoading: false,
    // -----keep this area ---->


    search: {},
    queItemsPerVertical: {
        //ask: [],
        //note: [],
        tutor: []
    },
    itemsPerVertical: {
        //ask: [],
        //note: [],
        tutor: []
    },
    itemsSkeletonPerVertical: {
        //ask: skeletonData.ask,
        //note: skeletonData.note,
        tutor: skeletonData.tutor
    }
};

const mutations = {
    [SEARCH.UPDATE_LOADING](state, payload) {
        state.loading = payload;
    },
    [SEARCH.UPDATE_SEARCH_LOADING](state, payload) {
        state.serachLoading = payload;
    },
    [SEARCH.SET_ITEMS_BY_VERTICAL](state, verticalObj) {
        state.itemsPerVertical[verticalObj.verticalName] = verticalObj.verticalData;
    },
    [SEARCH.UPDATE_ITEMS_BY_VERTICAL](state, verticalObj) {
        state.itemsPerVertical[verticalObj.verticalName].data = state.itemsPerVertical[verticalObj.verticalName].data.concat(verticalObj.verticalData.data);
        state.itemsPerVertical[verticalObj.verticalName].nextPage = verticalObj.verticalData.nextPage;
    },
    [SEARCH.RESETQUE](state) {
        //check if ask Tab was loaded at least once
        for (let verticalName in state.queItemsPerVertical) {
            state.queItemsPerVertical[verticalName] = [];
        }
    }
};

const getters = {
    getIsLoading: state => state.loading,
    getSearchLoading: state => state.serachLoading,
    // getSearchItems: function (state, {getCurrentVertical}) {
    //     //deprecated
    //     return [];
    //     if (getCurrentVertical === "") {
    //         return [];
    //     }
    //     ;
    //     if (state.loading || state.serachLoading) {
    //         //return skeleton
    //         return state.itemsSkeletonPerVertical[getCurrentVertical];
    //     } else {
    //         //return data
    //         if(!!state.itemsPerVertical[getCurrentVertical]){
    //         return state.itemsPerVertical[getCurrentVertical].data;
    //         }
    //     }
    // },
    // getNextPageUrl: function (state, {getCurrentVertical}) {
    //     //deprecated
    //     if(getCurrentVertical !== ""){
    //         if(!!state.itemsPerVertical[getCurrentVertical]){
    //             return state.itemsPerVertical[getCurrentVertical].nextPage;
    //         }
    //     }
    // },
    // getShowQuestionToaster: function (state, {getCurrentVertical}) {
    //     //deprecated
    //     return !!state.queItemsPerVertical[getCurrentVertical] ? state.queItemsPerVertical[getCurrentVertical].length > 0 : false;
    // }
};

const actions = {
    //Always update the current route according the flow
    getAutocmplete(context, term) {
        return searchService.autoComplete(term);
    },
    nextPage(context, {url, vertical}) {
        return searchService.nextPage({url, vertical}).then((data) => {
            let verticalObj = {
                verticalName: vertical,
                verticalData: data
            };
            context.dispatch('updateDataByVerticalType', verticalObj);
            return data;
        });
    },

    // fetchingData(context, {name, params, page, skipLoad}) {
        //deprecated
    //     //let university = context.rootGetters.getUniversity ? context.rootGetters.getUniversity : null;
    //     //let paramsList = {...context.state.search, ...params, university, page};

    //     let paramsList = {...context.state.search, ...params, page};
    //     //update box terms
    //     // context.dispatch('updateAITerm', {vertical: name, data: {text: paramsList.term}});
    //     //get location if needed
    //     let VerticalName = context.getters.getCurrentVertical;
    //     let verticalItems = context.state.itemsPerVertical[VerticalName];
    //     let skip = determineSkip(VerticalName, verticalItems);
    //     let haveQueItems = context.state.queItemsPerVertical[VerticalName].length;
    //     //when entering a question and going back stay on the same position.
    //     //can be removed only when question page willo be part of ask question page
    //     if ((!!verticalItems && !!verticalItems.data && (verticalItems.data.length > 0 && verticalItems.data.length < 150) && !context.state.serachLoading) || skip) {
    //         if (haveQueItems) {
    //             context.commit(SEARCH.INJECT_QUESTION);
    //         }

    //         let filtersData = !!verticalItems.filters ? searchService.createFilters(verticalItems.filters) : null;
    //         let sortData = !!verticalItems.sort ? verticalItems.sort : null;
    //         context.dispatch('updateSort', sortData);
    //         context.dispatch('updateFilters', filtersData);

    //         return verticalItems;
    //     } else {
    //         context.commit(SEARCH.RESETQUE);
    //         return getData();
    //     }


    //     function determineSkip(verticalName, verticalItems) {
    //         if (verticalName === 'ask') {
    //             /*
    //                 if comming from question page we need to make sure before we auto skip the loading
    //                 that we have some vertical items in the system if not then we dont want to skip the load
    //             */
    //             if (!!verticalItems && !!verticalItems.data && verticalItems.data.length > 0) {
    //                 return skipLoad;
    //             } else {
    //                 return false;
    //             }
    //         } else {
    //             return false;
    //         }
    //     }

    //     function getData() {
    //         return new Promise((resolve) => {
    //             if (LOCATION_VERTICALS.has(name) && !paramsList.location) {
    //                 context.dispatch("updateLocation").then((location) => {
    //                     paramsList.location = location;
    //                     resolve();
    //                 });
    //             } else {
    //                 resolve();
    //             }
    //         }).then(() => {
    //             return searchService.activateFunction[name](paramsList).then((data) => {
    //                 let verticalObj = {
    //                     verticalName: name,
    //                     verticalData: data
    //                 };
    //                 context.dispatch('setDataByVerticalType', verticalObj);
    //                 let sortData = !!data.sort ? data.sort : null;
    //                 context.dispatch('updateSort', sortData);
    //                 let filtersData = !!data.filters ? searchService.createFilters(data.filters) : null;
    //                 context.dispatch('updateFilters', filtersData);
    //                 return data;
    //             }, (err) => {
    //                 return Promise.reject(err);
    //             });
    //         });
    //     }
    // },
    setDataByVerticalType({commit}, verticalObj) {
        commit(SEARCH.SET_ITEMS_BY_VERTICAL, verticalObj);
    },
    updateDataByVerticalType({commit}, verticalObj) {
        commit(SEARCH.UPDATE_ITEMS_BY_VERTICAL, verticalObj);
    },
    // addQuestionItemAction({commit, getters}, notificationQuestionObject) {
    //     let user = getters.accountUser;
    //     let questionObj = searchService.createQuestionItem(notificationQuestionObject);
    //     let questionToSend = {
    //         user,
    //         question: questionObj
    //     };
    //     commit(SEARCH.ADD_QUESTION, questionToSend);
    // },
    // updateCoursesFilters({commit, getters, dispatch}, arrCourses) {
    //     let VerticalName = getters.getCurrentVertical;
    //     if (VerticalName.toLowerCase() !== "note") return;

    //     let courses = arrCourses.map(item => {
    //         let currVal = "";
    //         if (typeof item === "string") {
    //             currVal = item
    //         } else {
    //             currVal = item.text
    //         }
    //         return {
    //             key: currVal,
    //             value: currVal
    //         }
    //     })

    //     let MutationObj = {
    //         courses,
    //         fnUpdateCourses: (filtersData) => {
    //             dispatch('updateFilters', filtersData);
    //         }
    //     };
    //     commit('StudyDocuments_updateCoursesFilters', MutationObj);

    // },
    // documentVote({commit, dispatch}, data) {
    //     reputationService.voteDocument(data.id, data.type).then(() => {
    //         commit(SEARCH.UPDATE_DOCUMENT_VOTE, data);
    //         dispatch('profileVote', data);
    //     }, (err) => {
    //         let errorObj = {
    //             toasterText: err.response.data.Id[0],
    //             showToaster: true,
    //         }
    //         dispatch('updateToasterParams', errorObj);
    //     })
    // },

    // removeDocumentItemAction({commit}, notificationQuestionObject) {
    //     let documentObj = searchService.createDocumentItem(notificationQuestionObject);
    //     commit(SEARCH.REMOVE_DOCUMENT, documentObj);
    // },
    // reportDocument({commit, dispatch}, data) {
    //     return reportService.reportDocument(data).then(() => {
    //         let objToRemove = {
    //             id: data.id
    //         };
    //         dispatch('removeDocumentItemAction', objToRemove);
    //         dispatch('removeItemFromProfile', objToRemove);
    //     })
    // },
};

export default {
    state,
    getters,
    actions,
    mutations
}
