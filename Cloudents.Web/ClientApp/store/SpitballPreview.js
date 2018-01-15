import previewService from "../services/spitballPreviewService";
import {PREVIEW} from "./mutation-types";
const store={
    item:{details:{}}
};
const mutations = {
    [PREVIEW.UPDATE_ITEM_PREVIEW](state, payload) {
        state.item = { ...state.item, ...payload };
    }
};
const getters={
    itemDetails:state=>state.item.details
};
const actions={
    getPreview(context, model) {
        return previewService.getPreview(model);
    },
    updateItemDetails({ commit }, {details}) {
       commit(PREVIEW.UPDATE_ITEM_PREVIEW,{details})
    }
};
export default {
    actions,
    store,
    getters,
    mutations
}