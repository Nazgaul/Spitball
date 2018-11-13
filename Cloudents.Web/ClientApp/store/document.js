import documentService from "../services/documentService";
import { PREVIEW } from "./mutation-types";

const state = {
    item: {details: {}}
};
const mutations = {
    [PREVIEW.UPDATE_ITEM_PREVIEW](state, payload) {
        state.item = payload;
    },
    clearDocPreviewItem(state){
        state.item = {};
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
                let postfix;
                if(!item.preview || item.preview.length === 0) {
                    let location = `${global.location.origin}/images/doc-preview-empty.png`;
                    item.preview.push(location);
                    item.details.isPlaceholder = true;
                }
                    postfix = item.preview[0].split('?')[0].split('.');

                item.contentType = postfix[postfix.length - 1];
                item.details =  documentService.createDocumentItem(item.details);
                context.commit(PREVIEW.UPDATE_ITEM_PREVIEW, item);

            })
    },
    clearDocPreview({commit}){
        commit('clearDocPreviewItem');
    }
};
export default {
    actions,
    state,
    getters,
    mutations
}