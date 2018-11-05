import documentService from "../services/documentService";
import { PREVIEW } from "./mutation-types";

const state = {
    item: {details: {}}
};
const mutations = {
    [PREVIEW.UPDATE_ITEM_PREVIEW](state, payload) {
        state.item = payload;
    }
};
const getters = {
    getDocumentDetails: state => state.item.details,
    getDocumentItem : state => state.item
};
const actions = {
    setDocumentPreview(context, model) {
       let id = model.id ? model.id : '';
       return documentService.getDocument(id)
            .then(({data}) => {
                let item = {details: data.details, preview: data.preview};
                // let item = {};
                // item.details = data.details;
                // item.preview = data.preview;
                let postfix = item.preview[0].split('?')[0].split('.');
                item.contentType = postfix[postfix.length - 1];
                item.details =  documentService.createDocumentItem(item.details);
                context.commit(PREVIEW.UPDATE_ITEM_PREVIEW, item);

            })
    },
};
export default {
    actions,
    state,
    getters,
    mutations
}