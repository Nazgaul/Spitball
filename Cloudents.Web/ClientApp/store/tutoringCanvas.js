import tutorService from '../components/tutor/tutorService'

const state = {
    dragData: {
        'tab-0':[]
    },
    zoom: 100,
    pan:{
        x:0,
        y:0
    },
    selectedOptionString: 'liveDraw',
    shapesSelected: {},
    canvasTabs: [
        {
            name: 'Default',
            id: 'tab-0'
        },
        {
            name: 'Tab-1',
            id: 'tab-1'
        },
        {
            name: 'Tab-2',
            id: 'tab-2'
        },
    ],
    currentSelectedTab: {
        name: 'Default',
        id: 'tab-0'
    }
};
const getters = {
    getDragData: state => state.dragData[state.currentSelectedTab.id],
    getZoom: state => state.zoom,
    getPanX: state => state.pan.x,
    getPanY: state => state.pan.y,
    selectedOptionString: state => state.selectedOptionString,
    getShapesSelected: state => state.shapesSelected,
    getCanvasTabs: state => state.canvasTabs,
    getCurrentSelectedTab: state => state.currentSelectedTab,
    getAllDragData: state => state.dragData
};

const mutations = {
    setDragData(state, val) {
        let tab = val.tab.id;
        if(!state.dragData[tab]){
            state.dragData = {...state.dragData, [tab]:[]};
        }
        state.dragData[tab].push(val.data);
    },
    resetDragDataMutation(state){
        //TODO add tab as param when this feature will be added
        state.dragData[state.currentSelectedTab.id].length = 0;
    },
    replaceDragDataMutation(state, val){
        state.dragData[state.currentSelectedTab.id] = val;
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
    },
    changeSelectedTab(state, tab){
        //make sure drag data will return something with the new tab
        if(!state.dragData[tab.id]){
            state.dragData = {...state.dragData, [tab.id]:[]};
        }
        state.currentSelectedTab.name = tab.name;
        state.currentSelectedTab.id = tab.id;
    }
};

const actions = {
    updateDragData({commit}, val) {
        let dragData = {
            tab: val.tab,
            data: val.data
        }
        commit('setDragData', dragData)
    },
    resetDragData({commit}){
        commit('resetDragDataMutation');
    },
    replaceDragData({commit}, val){
        commit('replaceDragDataMutation', val);
    },
    popDragData({state}, tab){
        let tabToPop = !!tab ? tab : state.currentSelectedTab.id
        return Promise.resolve(state.dragData[tabToPop].pop());
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
    },
    changeSelectedTab({commit}, tab){
        commit('changeSelectedTab', tab);
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}