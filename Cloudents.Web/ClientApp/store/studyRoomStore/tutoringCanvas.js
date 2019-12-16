import tutorService from '../../components/studyroom/tutorService'
import {LanguageService} from '../../services/language/languageService'
import whiteBoardService from '../../components/studyroom/whiteboard/whiteBoardService'

const state = {
    canvasDataStore:{
        color: {
            hex: '#000000'
        },
    },
    dragData: {
        'tab-0':[]
    },
    undoClicked: false,
    clearAllClicked: true,
    addImage: null,
    currentOptionSelected: whiteBoardService.init('liveDraw'),
    showPickColorInterface: false,
    zoom: 100,
    pan:{
        x:0,
        y:0
    },
    selectedOptionString: 'liveDraw',
    shapesSelected: {},
    canvasTabs: [
        {
            name: LanguageService.getValueByKey('tutor_tab') + ' 1',
            id: 'tab-0'
        },
        {
            name: LanguageService.getValueByKey('tutor_tab') + ' 2',
            id: 'tab-1'
        },
        {
            name: LanguageService.getValueByKey('tutor_tab') + ' 3',
            id: 'tab-2'
        },
        {
            name: LanguageService.getValueByKey('tutor_tab') + ' 4',
            id: 'tab-3'
        },
        {
            name: LanguageService.getValueByKey('tutor_tab') + ' 5',
            id: 'tab-4'
        },
        {
            name: LanguageService.getValueByKey('tutor_tab') + ' 6',
            id: 'tab-5'
        },
        {
            name: LanguageService.getValueByKey('tutor_tab') + ' 7',
            id: 'tab-6'
        },
        {
            name: LanguageService.getValueByKey('tutor_tab') + ' 8',
            id: 'tab-7'
        },
    ],
    currentSelectedTab: {
        name: LanguageService.getValueByKey('tutor_tab') + ' 1',
        id: 'tab-0'
    },
    tabIndicator: 'tab-0',
    imgLoader: false,
    showBoxHelper: true,
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
    getAllDragData: state => state.dragData,
    currentOptionSelected: state => state.currentOptionSelected,
    showPickColorInterface: state => state.showPickColorInterface,
    canvasDataStore: state => state.canvasDataStore,
    undoClicked: state => state.undoClicked,
    clearAllClicked: state => state.clearAllClicked,
    addImage: (state) => {
        if(!!state.addImage){
            return state.addImage;
        }
    },
    getTabIndicator:state => state.tabIndicator,
    getImgLoader:state => state.imgLoader,
    getShowBoxHelper:state => state.showBoxHelper,
};

const mutations = {
    setTabName(state, {tabName,tabId}){
        state.canvasTabs.forEach(tab =>{
            if(tab.id === tabId){
                tab.name = tabName;
            }
        });
    },
    setDragData(state, val) {
        let tab = val.tab.id;
        if(!state.dragData[tab]){
            state.dragData = {...state.dragData, [tab]:[]};
        }
        state.dragData[tab].push(val.data);
    },
    resetDragDataMutation(state, tab){
        let tabId = tab.id;
        state.dragData = {...state.dragData, [tabId]:[]};
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
        state.selectedOptionString = val;
    },
    setShapesSelected(state, shape){
        state.shapesSelected = {...state.shapesSelected, [shape.id]:shape};
    },
    clearShapesSelected(state){
        state.shapesSelected = {};
    },
    changeSelectedTab(state, tab){
        //make sure drag data will return something with the new tab
        if(!state.dragData[tab.id]){
            state.dragData = {...state.dragData, [tab.id]:[]};
        }
        state.currentSelectedTab.name = tab.name;
        state.currentSelectedTab.id = tab.id;
    },
    removeCanvasTab(state, deletedTab){
        let tabIndex = null;
        if(state.canvasTabs.length > 1){
            state.canvasTabs.forEach((canvasTab, index)=>{
                if(canvasTab.id === deletedTab.id){
                    tabIndex = index;
                }
            });
            state.canvasTabs.splice(tabIndex, 1);
        }
        return Promise.resolve(tabIndex);
    },
    setCurrentOptionSelected(state, val){
        state.currentOptionSelected = val;
    },
    setShowPickColorInterface(state, val){
        state.showPickColorInterface = val;
    },
    setCanvasDataStore(state, val){
        state.canvasDataStore = {...state.canvasDataStore, val};
    },
    setUndoClicked(state){
        state.undoClicked = !state.undoClicked;
    },
    setAddImage(state, val){
        state.addImage = val;
    },
    setClearAllClicked(state){
        state.clearAllClicked = !state.clearAllClicked;
    },
    setTab(state,{tabId}){
        state.tabIndicator = tabId;
    },
    setImgLoader(state,val){
        state.imgLoader = val;
    },
    setShowBoxHelper(state,val){
        state.showBoxHelper = val;
    }
};

const actions = {
    updateDragData({commit}, val) {
        let dragData = {
            tab: val.tab,
            data: val.data
        };
        commit('setDragData', dragData);
    },
    resetDragData({commit}, tab){
        commit('resetDragDataMutation', tab);
    },
    replaceDragData({commit}, val){
        commit('replaceDragDataMutation', val);
    },
    popDragData({state}, tab){
        let tabToPop = !!tab ? tab : state.currentSelectedTab.id;
        return Promise.resolve(state.dragData[tabToPop].pop());
    },

    uploadImage(context, data){
        return tutorService.uploadCanvasImage(data).then((response)=>{
            return response.data.link;
        });
    },
    updateZoom({commit}, val){
        commit('setZoom', val);
    },
    updatePan({commit}, transform){
        commit('setPan', transform);
    }, 
    setSelectedOptionString({commit}, val){
        commit('setSelectedOptionString', val);
    },
    setShapesSelected({commit}, shape){
        commit('setShapesSelected', shape);
    },
    clearShapesSelected({commit}){
        commit('clearShapesSelected');
    },
    changeSelectedTab({commit, state}, tab){
        let isExists = state.canvasTabs.filter((currentTab)=>{
            return currentTab.id === tab.id;
        });
        if(isExists.length > 0){
            commit('changeSelectedTab', tab);
        }
    },
    removeCanvasTab({commit}, tab){
        return commit('removeCanvasTab', tab);
    },
    setCurrentOptionSelected({commit}, val){
        commit('setCurrentOptionSelected', val);
    },
    setShowPickColorInterface({commit}, val){
        commit('setShowPickColorInterface', val);
    }, 
    setCanvasDataStore({commit}, val){
        commit('setCanvasDataStore', val);
    },
    setUndoClicked({commit}){
        commit('setUndoClicked');
    },
    setAddImage({commit}, val){
        commit('setAddImage', val);
    },
    setClearAllClicked({commit}){
        commit('setClearAllClicked');
    },
    updateTab({commit},updateTabObj){
        commit('setTabName',updateTabObj);
    },
    updateImgLoader({commit},val){
        commit('setImgLoader',val);
    },
    updateShowBoxHelper({commit},val){
        commit('setShowBoxHelper',val);
    }
};
export default {
    state,
    mutations,
    getters,
    actions
}