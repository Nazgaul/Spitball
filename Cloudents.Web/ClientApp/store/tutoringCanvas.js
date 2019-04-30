import tutorService from '../components/tutor/tutorService'

const state = {
    dragData: [],
    zoom: 100,
    pan:{
        x:0,
        y:0
    },
    selectedOptionString: 'liveDraw',
    shapesSelected: {},
};
const getters = {
    getDragData: state => state.dragData,
    getZoom: state => state.zoom,
    getPanX: state => state.pan.x,
    getPanY: state => state.pan.y,
    selectedOptionString: state => state.selectedOptionString,
    getShapesSelected: state => state.shapesSelected,
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
        state.pan.x = !!transform.x ? transform.x : state.pan.x;
        state.pan.y = !!transform.y ? transform.y : state.pan.y;
    },
    setSelectedOptionString(state, val){
        state.selectedOptionString = val
    },
    setShapesSelected(state, shape){
        state.shapesSelected = {...state.shapesSelected, [shape.id]:shape}
    },
    clearShapesSelected(state){
        state.shapesSelected = {}
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
        return tutorService.uploadCanvasImage(data).then((response)=>{
            return response.data.link;
        })
    },
    updateZoom({commit}, val){
        commit('setZoom', val);
    },
    updatePan({commit}, transform){
        commit('setPan', transform);
    }, 
    setSelectedOptionString({commit}, val){
        commit('setSelectedOptionString', val)
    },
    setShapesSelected({commit}, shape){
        commit('setShapesSelected', shape);
    },
    clearShapesSelected({commit}){
        commit('clearShapesSelected');
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}