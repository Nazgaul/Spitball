import {uploadCanvasImage} from '../components/tutor/tutorService'

const state = {
    dragData: [],
    zoom: 100,
    pan:{
        x:0,
        y:0
    }
};
const getters = {
    getDragData: state => state.dragData,
    getZoom: state => state.zoom,
    getPanX: state => state.pan.x,
    getPanY: state => state.pan.y
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
    },
    setZoom(state, val){
        state.zoom = val;
    },
    setPan(state, transform){
        state.pan.x = transform.e;
        state.pan.y = transform.f;
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
    },
    updateZoom({commit}, val){
        commit('setZoom', val);
    },
    updatePan({commit}, transform){
        commit('setPan', transform);
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}