import searchService from "./../services/searchService";
import reputationService from './../services/reputationService';
import reportService from "./../services/cardActionService"

const state = {
    items: {},
};

const mutations = {
    StudyDocuments_filterDocument(state, data) {
        state.items.data = data;
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
    StudyDocuments_removeDocument(state, documentToRemove) {
        if (!!state.items && !!state.items.data && state.items.data.length > 0) {
            for (let documentIndex = 0; documentIndex < state.items.data.length; documentIndex++) {
                let currentDocument = state.items.data[documentIndex];
                if (currentDocument.id === documentToRemove.id) {
                    state.items.data.splice(documentIndex, 1);
                    return;
                }
            }
        }
    },
    
};

const actions = {     
    documentVote({commit, getters, dispatch}, data) {
        reputationService.voteDocument(data.id, data.type).then(() => {
            let docs = getters.Feeds_getItems;
            let doc = getters.getDocumentDetails;
            if(doc) {
                data.docs = [doc.details.feedItem];
            } else  {
                data.docs = docs;
            }

            commit('StudyDocuments_updateDocumentVote', data);
        }, (err) => {
            let errorObj = {
                toasterText: err.response.data.Id[0],
                showToaster: true,
            };
            dispatch('updateToasterParams', errorObj);
        });
    },

    removeDocumentItemAction({commit}, notificationQuestionObject) {
       let documentObj = searchService.createDocumentItem(notificationQuestionObject);
        commit('StudyDocuments_removeDocument', documentObj);
    },
    reportDocument({dispatch}, data) {
        return reportService.reportDocument(data).then(() => {
            let objToRemove = {
                id: data.id
            };
            dispatch('removeDocItemAction', objToRemove);
        });
    },
    removeItemFromList({dispatch}, itemId) {
        let docToRemove = { id: itemId };
        dispatch('removeDocumentItemAction', docToRemove);
    }
};

export default {
    state,
    actions,
    mutations
}
