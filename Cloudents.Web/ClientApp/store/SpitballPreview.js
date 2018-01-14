import previewService from "../services/spitballPreviewService";

const actions={
    getPreview(context, model) {
        return previewService.getPreview(model);
    }
};
export default {
    actions
}