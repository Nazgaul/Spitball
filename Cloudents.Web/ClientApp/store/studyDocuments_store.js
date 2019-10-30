import { skeletonData } from '../components/results/consts';
import searchService from "./../services/searchService";
import reputationService from './../services/reputationService';
import reportService from "./../services/cardActionService"

const state = {
    items: {},
    itemsSkeleton: skeletonData.note,
    dataLoaded: false
};

const mutations = {
    StudyDocuments_filterDocument(state, data) {
        state.items.data = data;
    },
    StudyDocuments_setItems(state, data) {
        state.items = data;
    },
    StudyDocuments_setDataLoaded(state, data){
        state.dataLoaded = data;
    },
    StudyDocuments_updateItems(state, data) {
        state.items.data = state.items.data.concat(data.data);
        state.items.nextPage = data.nextPage;
    },    
    
    StudyDocuments_updateDocumentVote(state, {docs, id, type}) {
        if (docs && docs.length) {
            docs.forEach((document) => {                
                if (document.id === id) {
                    reputationService.updateVoteCounter(document, type);
                }
            });
        }
    },
    //search filter calls this fuction
    // StudyDocuments_updateCoursesFilters(state, MutationObj) {
    //     if (!!state.items && !!state.items.filters) {
    //         let coursesFiltersIndex = null;
    //         state.items.filters.forEach((item, index) => {
    //             if (item.id === "Course") {
    //                 coursesFiltersIndex = index;
    //             }
    //         });
    //         if (coursesFiltersIndex !== null) {
    //             state.items.filters[coursesFiltersIndex].data = MutationObj.courses;
    //             let filters = searchService.createFilters(state.items.filters);
    //             MutationObj.fnUpdateCourses(filters)
    //         }
    //     }
    // },
    StudyDocuments_removeDocument(state, documentToRemove) {
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let documentIndex = 0; documentIndex < state.items.data.length; documentIndex++) {
                let currentDocument = state.items.data[documentIndex];
                if (currentDocument.id === documentToRemove.id) {
                    //remove the document from the list
                    state.items.data.splice(documentIndex, 1);
                    return;
                }
            }
        }
    },
    
};

const getters = {
    StudyDocuments_getItems: function (state, {getIsLoading, getSearchLoading}) {
        if (getIsLoading || getSearchLoading) {
            //return skeleton
            return state.itemsSkeleton;
        } else {
            //return data
            return state.items.data;
        }
    },
    StudyDocuments_getNextPageUrl: function (state) {
        return state.items.nextPage;
    },
    StudyDocuments_isDataLoaded: function(state){
        return state.dataLoaded;
    }
};

const actions = {
    StudyDocuments_nextPage(context, {url, vertical}) {
        return searchService.nextPage({url, vertical}).then((data) => {
            context.dispatch('StudyDocuments_updateData', data);
            return data;
        });
    },
    StudyDocuments_updateDataLoaded({commit}, data){
        commit('StudyDocuments_setDataLoaded', data);
    },
    StudyDocuments_fetchingData(context, {name, params, page}) {
        let paramsList = {...context.state.search, ...params, page};
        //update box terms
        // context.dispatch('updateAITerm', {vertical: name, data: {text: paramsList.term}});
        context.dispatch('StudyDocuments_updateDataLoaded', false);
        //get location if needed
        let verticalItems = context.state.items;
        //when entering a question and going back stay on the same position.
        //can be removed only when question page willo be part of ask question page
        if ((!!verticalItems && !!verticalItems.data && (verticalItems.data.length > 0 && verticalItems.data.length < 150) && !context.getters.getSearchLoading)) {
            let filtersData = !!verticalItems.filters ? searchService.createFilters(verticalItems.filters) : null;
            let sortData = !!verticalItems.sort ? verticalItems.sort : null;
            context.dispatch('updateSort', sortData);
            context.dispatch('updateFilters', filtersData);
            context.dispatch('StudyDocuments_updateDataLoaded', true);
            return verticalItems;
        } else {
            return getData();
        }

        function getData() {
            return new Promise((resolve) => {
                    resolve();                
            }).then(() => {
                return searchService.activateFunction[name](paramsList).then((data) => {
                    context.dispatch('StudyDocuments_setDataItems', data);
                    let sortData = !!data.sort ? data.sort : null;
                    context.dispatch('updateSort', sortData);
                    let filtersData = !!data.filters ? searchService.createFilters(data.filters) : null;
                    context.dispatch('updateFilters', filtersData);
                    context.dispatch('StudyDocuments_updateDataLoaded', true);
                    return data;
                }, (err) => {
                    return Promise.reject(err);
                });
            });
        }
    },
    StudyDocuments_setDataItems({commit}, data) {
        commit('StudyDocuments_setItems', data);
    },
    StudyDocuments_updateData({commit}, data) {
        commit('StudyDocuments_updateItems', data);
    },
    documentVote({commit, getters, dispatch}, data) {
        reputationService.voteDocument(data.id, data.type).then(() => {
            let docs = getters.Feeds_getItems;
            data.docs = docs
            
            commit('StudyDocuments_updateDocumentVote', data);
            dispatch('profileVote', data);
        }, (err) => {
            let errorObj = {
                toasterText: err.response.data.Id[0],
                showToaster: true,
            };
            dispatch('updateToasterParams', errorObj);
        });
    },

    removeDocumentItemAction({commit}, notificationQuestionObject) {
        // ??? there is no data and no need to create obj in order to delete by ID line below do we need ?
       let documentObj = searchService.createDocumentItem(notificationQuestionObject);
        // let documentObj = notificationQuestionObject;
        commit('StudyDocuments_removeDocument', documentObj);
    },
    reportDocument({commit, dispatch}, data) {
        return reportService.reportDocument(data).then(() => {
            let objToRemove = {
                id: data.id
            };
            
            // dispatch('removeDocumentItemAction', objToRemove);
            dispatch('removeDocItemAction', objToRemove);
            dispatch('removeItemFromProfile', objToRemove);
        });
    },
    removeItemFromList({state, commit, dispatch}, itemId) {
        let docToRemove = { id: itemId };
        dispatch('removeDocumentItemAction', docToRemove);
    }
};

export default {
    state,
    getters,
    actions,
    mutations
}
