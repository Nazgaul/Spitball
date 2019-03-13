import documentService from "../services/documentService";
import { PREVIEW } from "./mutation-types";
import { LanguageService } from "../services/language/languageService";
import analyticsService from '../services/analytics.service';

const state = {
    item: {details: {}}
};
const mutations = {
    [PREVIEW.UPDATE_ITEM_PREVIEW](state, payload) {
        state.item = payload;
    },
    clearDocPreviewItem(state) {
        state.item = {};
    },
    updateDownload(state){
        state.item.details.downloads = state.item.details.downloads + 1;
    }

};
const getters = {
    getDocumentDetails: state => state.item.details,
    getDocumentItem: state => state.item
};
const actions = {
    setDocumentPreview(context, model) {
        let id = model.id ? model.id : '';
        return documentService.getDocument(id)
            .then(({data}) => {
                let item = {
                    details: data.details, 
                    preview: data.preview, 
                    content: data.content ? data.content : ''};
                let postfix;
                if (!item.preview || item.preview.length === 0) {
                    let location = `${global.location.origin}/images/doc-preview-empty.png`;
                    item.preview.push(location);
                    item.details.isPlaceholder = true;
                }
                postfix = item.preview[0].split('?')[0].split('.');
                item.contentType = postfix[postfix.length - 1];
                item.details = documentService.createDocumentItem(item.details);
                context.commit(PREVIEW.UPDATE_ITEM_PREVIEW, item);
            })
    },
    clearDocPreview({commit}) {
        commit('clearDocPreviewItem');
    },
    updateDownloadsCount({commit}){
        commit('updateDownload');
    },
    purchaseAction({commit, dispatch, getters}, item) {
        let userBalance = 0;
        let id = item.id ? item.id : '';
        if(!!getters.accountUser && getters.accountUser.balance){
            userBalance = getters.accountUser.balance
        }
        if(userBalance >= item.price){
            return documentService.purchaseDocument(id)
            .then((resp) => {
                    console.log('purchased success', resp);
                    analyticsService.sb_unitedEvent('STUDY_DOCS', 'DOC_PURCHASED', item.price);
                    dispatch('setDocumentPreview', item);
                },
                (error) => {
                    console.log('purchased Error', error);

                }
            )
        }else{
            dispatch('updateToasterParams', {
                toasterText: LanguageService.getValueByKey("resultNote_unsufficient_fund"),
                showToaster: true,
            });
        }
    }

};
export default {
    actions,
    state,
    getters,
    mutations
}