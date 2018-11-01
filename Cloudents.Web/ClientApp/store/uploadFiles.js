
import uploadService from '../services/uploadService'

const state = {
    fileData: uploadService.createFileData({}),
    legalCheck: false
};
const mutations = {
    setLegal(state,  val) {
        state.legalCheck = val;
    },
    setFile(state, data){
        let assignData = Object.assign(state.fileData, data);
        let newFileData = uploadService.createFileData(assignData);
        state.fileData = newFileData
    }
};
const getters = {
    getFileData: (state) => state.fileData,
    getLegal: (state) => state.legalCheck

};
const actions = {
    updateLegalAgreement({commit},  val) {
        commit('setLegal', val);
    },
    updateFile({commit}, data){
        commit('setFile',  data);
    }
};

export default {
    actions, state, mutations, getters
};