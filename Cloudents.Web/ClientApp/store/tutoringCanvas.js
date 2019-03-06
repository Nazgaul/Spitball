import {uploadCanvasImage} from '../components/tutor/tutorService'

const state = {
    dragData: [],
};
const getters = {
    getDragData: state => state.dragData,
};

const mutations = {
    setDragData(state, val) {
        state.dragData.push(val);
    },
    resetDragDataMutation(state){
        state.dragData.length = 0;
    },
    replaceDragDataMutation(state, val){
        state.dragData = val;
    }
};

const actions = {
    updateDragData({commit}, val) {
        commit('setDragData', val)
    },
    resetDragData({commit}){
        commit('resetDragDataMutation');
    },
    replaceDragData({commit}, val){
        commit('replaceDragDataMutation', val);
    },
    popDragData({state}){
        return Promise.resolve(state.dragData.pop());
    },
    uploadImage(context, data){
        return uploadCanvasImage(data).then((response)=>{
            return response.data.link;
        })
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}