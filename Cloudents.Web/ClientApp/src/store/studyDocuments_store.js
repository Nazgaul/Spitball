import searchService from "./../services/searchService";

const state = {
    items: {},
};

const mutations = {
    // StudyDocuments_filterDocument(state, data) {
    //     state.items.data = data;
    // },
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
    removeDocumentItemAction({commit}, notificationQuestionObject) {
       let documentObj = searchService.createDocumentItem(notificationQuestionObject);
        commit('StudyDocuments_removeDocument', documentObj);
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
