const state = {
    fileData: [],
    courseSelected : '',
    uploadProgressArr: [],
    showDialog: false,
    customFileName: '',
    uploadFullMobile: true,
    returnToUpload: false,
    uploadStep: 1,
    uploadState: false
};
const mutations = {
    setUploadProgress(state, val) {
      let  objIndex =  state.fileData.findIndex((obj => obj.id === val.id));
        state.fileData[objIndex].progress = 100;
    },
    updatePriceToAll(state, price){
        state.fileData.forEach(file=>{
            file.price = price;
        });
    },
    updateDocTypeToAll(state, docType){
        state.fileData.forEach(file=>{
            file.type = docType;
        });
    },
    updateDocProfessorAll(state, prof){
        state.fileData.forEach(file=>{
            file.professor = prof;
        });
    },
    updateFileByIndex(state, fileObj){
        state.fileData[fileObj.index] = fileObj.data;
    },
    addFile(state, val) {
        val.course = state.courseSelected;
        state.fileData.push(val);
    },
    deleteFileByIndex(state, index){
        state.fileData.splice(index, 1);
    },
    updateCourse(state, course){
        state.courseSelected = course;
    },
    setDialogUploadState(state, val) {
        state.showDialog = val;
    },
    setFileName(state, data) {
        state.customFileName = data;
    },
    clearUploadData(state) {
        state.fileData.length = 0;
    },
    updateReturnToUpload(state, val) {
        state.returnToUpload = val;
    },
    setUploadStep(state, val) {
        state.uploadStep = val;
    },
    updateBlobName(state, fileIdName){
        let  objIndex =  state.fileData.findIndex((obj => obj.id === fileIdName.id));
        state.fileData[objIndex].blobName = fileIdName.blobName;
    },
    updateErrorByID(state, fileInfo){
        let  objIndex =  state.fileData.findIndex((obj => obj.id === fileInfo.id));
        state.fileData[objIndex].error = fileInfo.error;
        state.fileData[objIndex].errorText = fileInfo.errorText;
        // Update progress to enable Upload button
        state.fileData[objIndex].progress = 100;

    }

};
const getters = {
    getFileData: (state) => state.fileData,
    getDialogState: (state) => state.showDialog,
    getCustomFileName: (state) => state.customFileName,
    getReturnToUpload: (state) => state.returnToUpload
};
const actions = {
    deleteFileByIndex({commit}, index){
        commit('deleteFileByIndex', index);
    },
    updateFileErrorById({commit}, fileInfo){
        commit('updateErrorByID', fileInfo);
    },
    changeFileByIndex({commit}, fileObj){
        commit('updateFileByIndex', fileObj);
    },
    resetUploadData({commit}) {
        commit('clearUploadData');
    },
    updateFileName({commit}, data) {
        commit('setFileName', data);
    },
    updateFile({commit}, data) {
        commit('addFile', data);
    },
    updateStep({commit}, val) {
        commit('setUploadStep', val);
    },
    stopUploadProgress({commit}, val) {
        commit('setUploadProgress', val);
    },
    askQuestion({commit, dispatch}, val) {
        //close upload
        commit('setDialogUploadState', val);
        //and open new question
        setTimeout(() => {
            dispatch('updateNewQuestionDialogState', true);
        }, 300);
    },
    updateDialogState({commit}, val) {
        commit('setDialogUploadState', val);
    },
    setReturnToUpload({commit}, val) {
        commit('updateReturnToUpload', val);
    },
    setAllPrice({commit}, price){
        commit('updatePriceToAll', price);
    },
    setAllDocType({commit}, docType){
        commit('updateDocTypeToAll', docType);
    },
    setAllProfessor({commit}, prof){
        commit('updateDocProfessorAll', prof);
    },
    setCourse({commit}, course){
        commit('updateCourse', course);
    },
    setFileBlobNameById({commit}, fileIdName){
        commit('updateBlobName', fileIdName);
    }


};

export default {
    actions, state, mutations, getters
};