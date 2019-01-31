
import uploadService from '../services/uploadService';

const state = {
    fileData: [],
    uploadProgress: 0,
    showDialog: false,
    customFileName : '',
    uploadFullMobile: true,
    returnToUpload: false,
    uploadStep: 1,
    uploadState: false
};
const mutations = {
    setUploadProgress(state,val){
        state.uploadProgress = val;
    },
    setFile(state, val){
        // let assignData = Object.assign(state.fileData, data);
        // let newFileData = uploadService.createFileData(assignData);
        // state.fileData = newFileData
        // state.fileData = val;
    },
    addFile(state, val){
        // let assignData = Object.assign(state.fileData, data);
        // let newFileData = uploadService.createFileData(assignData);
        // state.fileData = newFileData
        state.fileData.push(val);
    },
    setDialogUploadState(state, val){
        state.showDialog = val
    },
    setFileName(state, data){
        state.customFileName = data
    },
    // setUploadFullMobile(state, val){
    //     state.uploadFullMobile = val
    // },
    clearUploadData(state, val){
        state.fileData = val;
    },
    updateReturnToUpload(state, val){
        state.returnToUpload = val
    },
    setUploadStep(state, val){
        state.uploadStep = val;
    },
    updateUploadProcessState(state, val){
        state.uploadState = val
    }
};
const getters = {
    getFileData: (state) => state.fileData,
    getUploadProgress: (state) => state.uploadProgress,
    getDialogState: (state) => state.showDialog,
    getCustomFileName: (state) => state.customFileName,
    getUploadFullMobile: (state) => state.uploadFullMobile,
    getReturnToUpload: (state) => state.returnToUpload,
    isUploadActiveProcess: (state) => state.uploadState
};
const actions = {
    resetUploadData({commit}, val){
      commit('clearUploadData', val);
    },
    changeUploadState({commit}, val){
        commit("updateUploadProcessState", val)
    },
    updateFileName({commit}, data){
        commit('setFileName',  data);
    },
    updateFile({commit}, data){
        //service add stuff
        commit('addFile',  data);

    },
    updateStep({commit}, val){
        commit('setUploadStep',val)
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
        }, 300);
    },
    updateDialogState({commit}, val){
        commit('setDialogUploadState', val);
    },
    updateUploadFullMobile({commit}, val){
      commit('setUploadFullMobile', val)
    },
    setReturnToUpload({commit}, val){
        commit('updateReturnToUpload', val)
    }
};

export default {
    actions, state, mutations, getters
};