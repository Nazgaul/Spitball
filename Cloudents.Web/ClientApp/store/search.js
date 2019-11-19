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
};

const actions = {
    getAutocmplete(context, term) {
        return searchService.autoComplete(term);
    },
    setDataByVerticalType({commit}, verticalObj) {
        commit(SEARCH.SET_ITEMS_BY_VERTICAL, verticalObj);
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
    //             dispatch('', filtersData);
    //         }
    //     };

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

    //     let documentObj = searchService.createDocumentItem(notificationQuestionObject);
    //     commit(SEARCH.REMOVE_DOCUMENT, documentObj);
    // },
    // reportDocument({commit, dispatch}, data) {
    //     return reportService.reportDocument(data).then(() => {
    //         let objToRemove = {
    //             id: data.id
    //         };
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
