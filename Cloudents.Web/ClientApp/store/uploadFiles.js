
import uploadService from '../services/uploadService'

const state = {
    fileData: uploadService.createFileData({}),
    legalCheck: false,
    uploadProgress: 0,
    showDialog: false,
    customFileName : '',
    uploadFullMobile: true,
    // confirmatioDialog: false
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
    setFileName(state, data){
        state.customFileName = data
    },
    setUploadFullMobile(state, val){
        state.uploadFullMobile = val
    },
    // setConfirmationDialogState(state, val){
    //     state.confirmatioDialog = val
    //
    // }
};
const getters = {
    getFileData: (state) => state.fileData,
    getLegal: (state) => state.legalCheck,
    getUploadProgress: (state) => state.uploadProgress,
    getDialogState: (state) => state.showDialog,
    getCustomFileName: (state) => state.customFileName,
    getUploadFullMobile: (state) => state.uploadFullMobile,
    // confirmationDialogState: (state) => state.confirmatioDialog
};
const actions = {
    // updateConfirmatioDialogState({commit},val){
    //   commit('setConfirmationDialogState', val);
    // },
    updateLegalAgreement({commit},  val) {
        commit('setLegal', val);
    },
    updateFileName({commit}, data){
        commit('setFileName',  data);
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
    },
    updateUploadFullMobile({commit}, val){
      commit('setUploadFullMobile', val)
    }
};

export default {
    actions, state, mutations, getters
};