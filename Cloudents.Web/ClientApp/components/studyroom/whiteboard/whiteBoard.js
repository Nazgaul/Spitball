import {VueMathjax} from 'vue-mathjax'

import whiteBoardService from './whiteBoardService';
import helperUtil from './utils/helper';
import { mapGetters, mapActions, mapMutations } from "vuex";
import canvasFinder from "./utils/canvasFinder";
import equationMapper from "./innerComponents/equationMapper.vue";
// import iinkDrawer from "./innerComponents/iinkDrawer.vue";
const iinkDrawer = () => import("./innerComponents/iinkDrawer.vue");
import { LanguageService } from '../../../services/language/languageService';
import imageDraw from './options/imageDraw';
import pencilSVG from '../images/noun-edit-684936.svg';
import uploadSVG from '../images/outline-open-in-browser-24-px.svg';
// import whiteBoardLayers from './innerComponents/whiteBoardLayers.vue'

const HeaderHeight = 108;

export default {
    components: {
        equationMapper,
        pencilSVG,
        uploadSVG,
        iinkDrawer,
        VueMathjax
        // whiteBoardLayers
    },
    data() {
        return {
            canvas: null,
            isEdit: false,
            currentTabId: null,
            canvasWidth: 2800,
            canvasHeight: 850,
            windowWidth: global.innerWidth, // 10 stands for the scroll offset
            windowHeight: global.innerHeight - HeaderHeight, 
            showPickColorInterface: false,
            showHelper: false,
            tabEditId: null,
            formula: 'x = {-b \\pm \\sqrt{b^2-4ac} \\over 2a}.',
            
            enumOptions: {
                draw: 'liveDraw',
                line: 'lineDraw',
                circle: 'drawEllipse',
                rectangle: 'drawRectangle',
                image: 'imageDraw',
                eraser: 'eraser',
                text: 'textDraw',
                equation: 'equationDraw',
                iink: 'iink',
                select: 'selectShape',
                pan: 'panTool',
            },
            canvasData: {
                shouldPaint: false,
                context: null,
                helper: helperUtil.HelperObj,
                metaData: {
                    previouseDrawingPosition: null,
                },
                lineJoin: "round",
                lineWidth: 2,
                color: this.canvasDataColor,
                methods: {
                    addShape: this.addShape,
                    hideColorPicker: this.hideColorPicker,
                    selectDefaultTool: this.selectDefaultTool
                },
                objDetected: false
            },
            textScales:[
                {
                    text: LanguageService.getValueByKey('tutor_fontSize_small'),
                    value: '20'
                },
                {
                    text: LanguageService.getValueByKey('tutor_fontSize_normal'),
                    value: '40'
                },
                {
                    text: LanguageService.getValueByKey('tutor_fontSize_large'),
                    value: '60'
                }
            ],
            sizeText: LanguageService.getValueByKey('tutor_size_label'),
            isRtl: global.isRtl
        };
    },
    computed: {
        ...mapGetters([
            'getDragData',
            'getZoom', 
            'selectedOptionString',
            'getCanvasTabs', 
            'getCurrentSelectedTab', 
            'currentOptionSelected', 
            'canvasDataStore',
            'undoClicked', 
            'addImage',
            'clearAllClicked',
            'getTabIndicator',
            'getImgLoader',
            'getShowBoxHelper',
            'getShapesSelected',
            'getFontSize']),
        equationSizeX(){
            return (window.innerWidth / 2) - 300;
        },
        equationSizeY(){
            return window.innerHeight / 3.5;
        },
        helperStyle() {
            return helperUtil.HelperObj.style;
        },
        helperClass() {
            return helperUtil.HelperObj.cssClass;
        },
        helperShow() {
            return helperUtil.HelperObj.isActive;
        },
        dragData() {
            return this.getDragData;
        },
        zoom() {
            return this.getZoom.toFixed();
        },
        computedFormula: {
            get() {
                return `$$${this.formula}$$`;
            },
            set(val) {
                this.formula = val;
            }
        },
        canvasTabs() {
            return this.getCanvasTabs;
        },
        canvasDataColor(){
            //activated from watch
            this.canvasData.color = this.canvasDataStore.color;
            return this.canvasDataStore.color;
        },
        isTutor() {
            return this.$store.getters.getRoomIsTutor;
        },
        showAnchors(){
            let unsupportedResizeShapes = ["liveDraw", "textDraw", "equationDraw", "iink"];
            if(Object.keys(this.getShapesSelected).length === 1){
                let shapeId = Object.keys(this.getShapesSelected)[0];
                if(unsupportedResizeShapes.indexOf(this.getShapesSelected[shapeId].type) > -1){
                    return false;
                }else{
                    return true;
                }
            }else{
                return false;
            }
        },
        fontSize:{
            get(){
                return this.getFontSize;
            },
            set(val){
                this.setFontSize(val);
            }
        }
    },
    watch: {
        canvasDataColor(newVal){
            //watch is activating the canvasDataColor computed
            this.canvasData.color = newVal;
        },
        undoClicked(){
            this.undo();
        },
        addImage(newVal){
                if(!!newVal){
                    this.addShape(newVal.dragObj, newVal.callback);
                }
            },
        clearAllClicked(){
            let shouldClear = window.confirm(LanguageService.getValueByKey('tutor_clearAll_warning_text'));
            if(shouldClear){
                this.clearCanvas();
            }
        }
    },
    methods: {
        ...mapActions(['updateShowBoxHelper','updateImgLoader','resetDragData', 'updateDragData', 'updateZoom', 'updatePan', 'setSelectedOptionString', 'changeSelectedTab', 'removeCanvasTab', 'setCurrentOptionSelected', 'setShowPickColorInterface', 'setFontSize']),
        ...mapMutations(['setTabName']),
        renameTab() {
            console.log("Rename Tab");
        },
        editTabName(tabId){
            this.isEdit = true;
            this.currentTabId = tabId;
            let tab = document.getElementById(tabId);
            tab.contentEditable = "true";
            let range = document.createRange();
            range.selectNodeContents(tab);
            let selection = global.getSelection();
            selection.removeAllRanges();
            selection.addRange(range);
        },
        saveNewTabName(){
            if(this.isEdit){
                let newTabName = document.getElementById(this.currentTabId).innerText;
                let tabData = {
                    tabId: this.currentTabId,
                    tabName: newTabName
                };
                let transferDataObj = {
                    type: "updateTab",
                    data: tabData
                };
                let normalizedData = JSON.stringify(transferDataObj);
                this.$store.dispatch('sendDataTrack',normalizedData)
    
                let tab = document.getElementById(this.currentTabId);
                let selection = global.getSelection();
                selection.empty();
                tab.contentEditable = "false";
                this.isEdit = false;
            }
        },
        uploadImage(){
            this.setCurrentOptionSelected(whiteBoardService.init.bind(this.canvasData, 'imageDraw')());
            this.setSelectedOptionString('imageDraw');
            let inputImgElm = document.getElementById('imageUpload');
            inputImgElm.click();
            this.setCurrentOptionSelected(whiteBoardService.init.bind(this.canvasData, this.enumOptions.select)());
            this.setSelectedOptionString(this.enumOptions.select);
            this.updateShowBoxHelper(false);
        },
        finishEquation(){
            let mouseEvent = new MouseEvent("mousedown", {});
            this.canvas.dispatchEvent(mouseEvent);
        },
        deleteTab(tab) {
            this.removeCanvasTab(tab);
            this.changeTab(this.getCanvasTabs[0]);
            console.log("Delete Tab");
        },
        showColorPicker() {
            this.setShowPickColorInterface(true);
        },
        hideColorPicker() {
            this.setShowPickColorInterface(false);
        },
        returnToDefaultState(dragObj){
            let stateToDefault = ['textDraw', 'selectShape'];
            let returnToDefault = !!this.selectedOptionString ? stateToDefault.indexOf(this.selectedOptionString) > -1 : true;
            return !dragObj.isGhost && returnToDefault;
        },
        addShape(dragObj, callback) {
            if(!dragObj){
                this.setCurrentOptionSelected(whiteBoardService.init.bind(this.canvasData, this.enumOptions.select)());
                this.setSelectedOptionString(this.enumOptions.select);
            } else{
                if(dragObj.type === 'imageDraw'){
                    this.updateImgLoader(false);
                }
                let dragUpdate = {
                    tab: this.getCurrentSelectedTab,
                    data: dragObj
                };
                this.updateDragData(dragUpdate);
                if (callback) {
                    callback();
                }
                let canvasData = {
                    context: this.canvasData.context,
                    metaData: this.canvasData.metaData,
                    tab: this.getCurrentSelectedTab
                };
                let data = {
                    canvasContext: canvasData,
                    dataContext: dragObj,
                };
                let transferDataObj = {
                    type: "passData",
                    data: data
                };
                let normalizedData = JSON.stringify(transferDataObj);
                this.$store.dispatch('sendDataTrack',normalizedData)
                if (this.returnToDefaultState(dragObj)) {
                    this.setCurrentOptionSelected(whiteBoardService.init.bind(this.canvasData, this.enumOptions.select)());
                    this.setSelectedOptionString(this.enumOptions.select);
                }
                
                this.updateShowBoxHelper(false);
            }
        },
        undo() {
            let transferDataObj = {
                type: "undoData",
                data: this.canvasData,
                tab: this.getCurrentSelectedTab,
            };
            let normalizedData = JSON.stringify(transferDataObj);
            this.$store.dispatch('sendDataTrack',normalizedData)
            whiteBoardService.undo(this.canvasData);

        },
        clearCanvas(){
            this.resetDragData(this.getCurrentSelectedTab);
            whiteBoardService.redraw(this.canvasData);
            let transferDataObj = {
                type: "clearCanvas",
                data: this.canvasData,
                tab: this.getCurrentSelectedTab,
            };
            let normalizedData = JSON.stringify(transferDataObj);
            this.$store.dispatch('sendDataTrack',normalizedData)
            whiteBoardService.clearData(this.canvasData, this.getCurrentSelectedTab);
            helperUtil.HelperObj.isActive = false;
        },
        keyPressed(e) {
            let isPressedF10 = this.keyCodeChecker(e,121);
            let isPressedZ = this.keyCodeChecker(e,90);
            let isPressedDelete = this.keyCodeChecker(e,46);
            let isPressedBackspace = this.keyCodeChecker(e,8);
            let isPressedEnter = this.keyCodeChecker(e,13);
            let isPressedEscape = this.keyCodeChecker(e,27);

            if (isPressedF10) {
                let link = document.createElement('a');
                link.download = `${this.getCurrentSelectedTab.name}.png`;
                link.href = document.getElementById('canvas').toDataURL("image/png");
                link.click();
            }
            //signalR should be fired Here
            if (isPressedZ && e.ctrlKey) {
                this.undo();
            }
            if ((isPressedDelete || isPressedBackspace) && this.selectedOptionString === this.enumOptions.select) {
                this.currentOptionSelected.deleteSelectedShape.bind(this.canvasData)();
            }
            if ((isPressedEnter || isPressedEscape) && this.selectedOptionString === this.enumOptions.text) {
                this.currentOptionSelected.enterPressed.bind(this.canvasData)();
            }
        },
        keyCodeChecker(e,keyCode){
            return (e.which == keyCode || e.keyCode == keyCode);
        },
        changeTab(tab) {
            this.$ga.event("tutoringRoom", `changeTab:${tab}`);

            this.currentTabId = tab.id;

            if (tab.id !== this.getCurrentSelectedTab.id) {

                let tabData = {
                    tabId: this.currentTabId,
                };
                let transferDataObj = {
                    type: "updateTabById",
                    data: tabData
                };
                let normalizedData = JSON.stringify(transferDataObj);
                this.$store.dispatch('sendDataTrack',normalizedData)
                
                this.changeSelectedTab(tab);
                whiteBoardService.hideHelper();
                whiteBoardService.redraw(this.canvasData);
            }
        },
        resizeCanvas() {
            // let canvas = document.getElementById('canvas');
            let ctx = this.canvas.getContext("2d");
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            this.windowWidth = global.innerWidth,
                this.windowHeight = global.innerHeight - HeaderHeight,
                this.canvas.width = this.canvasWidth;
                this.canvas.height = this.canvasHeight;
            whiteBoardService.redraw(this.canvasData);
        },
        injectToTextArea(textToInject) {
            let textAreaElm = document.getElementById('textArea-tutoring');
            let newValue = textAreaElm.insertAtCaret(textToInject);
            this.helperStyle.text = newValue;
        },
        // doZoom(zoomType){
        //     whiteBoardService.hideHelper();
        //     let panTool = whiteBoardService.init.bind(this.canvasData, this.enumOptions.pan)();
        //     panTool.manualScroll.bind(this.canvasData, zoomType)();
        // },
        registerCanvasEvents(canvas, canvasWrapper) {
            let self = this;
            global.addEventListener('resize', this.resizeCanvas, false);
            let dropArea = canvas;
            dropArea.addEventListener('dragenter', () =>{
            }, false);
            dropArea.addEventListener('dragleave', () =>{

            }, false);
            dropArea.addEventListener('dragover', (e) =>{
                e.preventDefault();
                
            }, false);
            global.addEventListener('drop', (e) =>{
                e.preventDefault();
                imageDraw.handleImage(e,true);
                self.updateShowBoxHelper(false);
                self.setSelectedOptionString(self.enumOptions.select);
            }, false);
            canvas.addEventListener('mousedown', (e) => {
                // self.clearTabOption();
                if (e.button == 0) {
                    if (!!self.currentOptionSelected && self.currentOptionSelected.mousedown) {
                        self.currentOptionSelected.mousedown.bind(self.canvasData, e)();
                    }
                }
                self.updateShowBoxHelper(false);
            });
            canvas.addEventListener('mouseup', (e) => {
                if (e.button == 0) {
                    if (!!self.currentOptionSelected && self.currentOptionSelected.mouseup) {
                        self.currentOptionSelected.mouseup.bind(self.canvasData, e)();
                    }
                }
            });
            canvas.addEventListener('mouseleave', (e) => {
                if (!!self.currentOptionSelected && self.currentOptionSelected.mouseleave) {
                    self.currentOptionSelected.mouseleave.bind(self.canvasData, e)();
                }
            });
            canvas.addEventListener('mousemove', (e) => {
                if (!!self.currentOptionSelected && self.currentOptionSelected.mousemove) {
                    self.currentOptionSelected.mousemove.bind(self.canvasData, e)();
                }
            });
            canvas.addEventListener('DOMMouseScroll', (e) => {
                if (!!self.currentOptionSelected && self.currentOptionSelected.mouseScroll) {
                    self.currentOptionSelected.mouseScroll.bind(self.canvasData, e)();
                }
            });
            canvasWrapper.addEventListener('scroll', (e) => {
                if (this.selectedOptionString === this.enumOptions.select) {
                    self.currentOptionSelected.reMarkSelectedShapes.bind(self.canvasData, e)();
                }
                let transform = {
                    x: e.target.scrollLeft * -1,
                    y: e.target.scrollTop * -1
                };
                self.updatePan(transform);
            });
            canvas.addEventListener('mousewheel', (e) => {
                if (!!self.currentOptionSelected && self.currentOptionSelected.mouseScroll) {
                    self.currentOptionSelected.mouseScroll.bind(self.canvasData, e)();
                }
            });
            canvas.addEventListener("touchstart", function (e) {
                if (e.target == canvas) {
                    e.preventDefault();
                }
                let touch = e.touches[0];
                let mouseEvent = new MouseEvent("mousedown", {
                    clientX: touch.clientX,
                    clientY: touch.clientY
                });
                canvas.dispatchEvent(mouseEvent);
            }, false);
            canvas.addEventListener("touchend", function (e) {
                if (e.target == canvas) {
                    e.preventDefault();
                }
                let touch = e.changedTouches[0];
                let mouseEvent = new MouseEvent("mouseup", {
                    clientX: touch.clientX,
                    clientY: touch.clientY
                });
                canvas.dispatchEvent(mouseEvent);
            }, false);
            canvas.addEventListener("touchmove", function (e) {
                if (e.target == canvas) {
                    e.preventDefault();
                }
                let touch = e.touches[0];
                let mouseEvent = new MouseEvent("mousemove", {
                    clientX: touch.clientX,
                    clientY: touch.clientY
                });
                canvas.dispatchEvent(mouseEvent);
            }, false);
        }
    },
    mounted() {
        this.canvas = document.querySelector('canvas');
        let canvasWrapper = document.querySelector('.canvas-wrapper');
        this.canvas.width = this.canvasWidth;
        this.canvas.height = this.canvasHeight;
        this.canvasData.context = this.canvas.getContext("2d");
        this.canvasData.context.font = `16px Open Sans`;
        this.canvasData.context.lineJoin = this.canvasData.lineJoin;
        this.canvasData.context.lineWidth = this.canvasData.lineWidth;
        canvasFinder.trackTransforms(this.canvasData.context);
        this.registerCanvasEvents(this.canvas, canvasWrapper);
        global.document.addEventListener("keydown", this.keyPressed);
    }
}