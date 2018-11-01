
import uploadService from '../services/uploadService'

const state = {
    fileData: uploadService.createFileData({}),
    legalCheck: false,
    uploadProgress: 0,
    showDialog: false
};
const mutations = {
    setUploadProgress(state,val){
        state.uploadProgress = val;
    },
    setLegal(state,  val) {
        state.legalCheck = val;
    },
    setFile(state, data){
        let assignData = Object.assign(state.fileData, data);
        let newFileData = uploadService.createFileData(assignData);
        state.fileData = newFileData
    },
    setDialogUploadState(state, val){
        state.showDialog = val
    },
};
const getters = {
    getFileData: (state) => state.fileData,
    getLegal: (state) => state.legalCheck,
    getUploadProgress: (state) => state.uploadProgress,
    getDialogState: (state) => state.showDialog

};
const actions = {
    updateLegalAgreement({commit},  val) {
        commit('setLegal', val);
    },
    updateFile({commit}, data){
        commit('setFile',  data);
    },
    updateUploadProgress({commit}, val){
        commit('setUploadProgress',val)
    },
    askQuestion({commit, dispatch}, val){
        //close upload
        commit('setDialogUploadState', val);
        //and open new question
        setTimeout(()=>{
            dispatch('updateNewQuestionDialogState', true)
        });
    },
    updateDialogState({commit}, val){
        commit('setDialogUploadState', val);
    }
};

export default {
    actions, state, mutations, getters
};