import documentService from "../services/documentService";
import {PREVIEW} from "./mutation-types";
const state={
    item:{details:{}}
};
const mutations = {
    [PREVIEW.UPDATE_ITEM_PREVIEW](state, payload) {
        state.item = {...state.item,...payload};
    }
};
const getters={
    getDocumentDetails : state => state.item.details
};
const actions={
    getDocumentPreview(context, model) {
        let id = Number(model.id);
        return documentService.getDocument(id)
            .then((resp)=>{
                console.log('resp store', resp)
              // self.item = {...body.details, preview: body.preview};
              let docDetails =  documentService.createDocumentItem(body.details);
                commit(PREVIEW.UPDATE_ITEM_PREVIEW,{docDetails})
            })
    },
    updateDocumentDetails({ commit }, {details}) {
        commit(PREVIEW.UPDATE_ITEM_PREVIEW,{details})
    }
};
export default {
    actions,
    state,
    getters,
    mutations
}