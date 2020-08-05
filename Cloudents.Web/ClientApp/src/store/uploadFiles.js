import axios from 'axios'

const state = {
    fileData: [],
    // courseSelected : '',
    // uploadStep: 1,
};

const getters = {
    getFileData: (state) => state.fileData.sort((a, b)=> b.error-a.error),
};

const mutations = {
    setUploadProgress(state, file) {
        let  objIndex =  state.fileData.findIndex((obj => obj.id === file.id));
        state.fileData[objIndex].progress = file.progress;
    },
    // updateCourseToAll(state, course){
    //     state.fileData.forEach(file=>{
    //         file.course = course.text || course;
    //     });
    // },
    addFile(state, file) {
        function FileData(objInit){
            this.id = objInit.id || '';
            this.blobName = objInit.blobName || '';
            this.name= objInit.name || '';
            this.course= objInit.course || '';
            this.progress = objInit.progress || 100;
            this.link  = objInit.link || '';
            this.size  = objInit.bytes || 0;
            this.error = objInit.error || false;
            this.errorText = objInit.errorText || '';
            this.description = objInit.description || '';
            this.visible = objInit.visible === undefined ? true : objInit.visible
        }
        state.fileData.push(new FileData(file));
    },
    deleteFileByIndex(state, index){
        state.fileData.splice(index, 1);
    },
    // updateCourse(state, course){
    //     state.courseSelected = course;
    // },
    // clearUploadData(state) {
    //     state.fileData.length = 0;
    // },
    setUploadStep(state, val) {
        state.uploadStep = val;
    },
    updateBlobName(state, fileIdName){
        let  objIndex =  state.fileData.findIndex((obj => obj.id === fileIdName.id));
        state.fileData[objIndex].blobName = fileIdName.blobName;
    },
    updateErrorByID(state, fileInfo){
        let objIndex = state.fileData.findIndex((obj => obj.id === fileInfo.id));
        state.fileData[objIndex].error = fileInfo.error;
        state.fileData[objIndex].errorText = fileInfo.errorText;
        // Update progress to enable Upload button
        state.fileData[objIndex].progress = 100;
    },
    resetUploadFiles(state) {
        state.fileData = []
    }
};

const actions = {
    uploadDropbox({commit}, file) {
        let serverFormatFileData = {
            blobName: file.blobName,
            name: file.name,
            course: file.course,
            description: file.description
        }
        return axios.post("/Document/dropbox", serverFormatFileData)
            .then(res => {
                file.blobName = res.data.fileName ? res.data.fileName : '';
                commit('addFile', file)
                return
            }).catch(ex => {
                console.error('error drop box api call', ex);
            })
    }
    // deleteFileByIndex({commit}, index){
    //     commit('deleteFileByIndex', index);
    // },
    // updateFileErrorById({commit}, fileInfo){
    //     commit('updateErrorByID', fileInfo);
    // },
    // changeFileByIndex({commit}, fileObj){
    //     commit('updateFileByIndex', fileObj);
    // },
    // resetUploadData({commit}) {
    //     commit('clearUploadData');
    // },
    // updateFile({commit}, data) {
    //     commit('addFile', data);
    // },
    // updateStep({commit}, val) {
    //     commit('setUploadStep', val);
    // },
    // stopUploadProgress({commit}, val) {
    //     commit('setUploadProgress', val);
    // },
    // setAllCourse({commit}, course){
    //     commit('updateCourseToAll', course);
    // },
    // setCourse({commit}, course){
    //     commit('updateCourse', course);
    // },
    // setFileBlobNameById({commit}, fileIdName){
    //     commit('updateBlobName', fileIdName);
    // }
};

export default {
    actions,
    state,
    mutations,
    getters
};