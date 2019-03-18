import {
    Compact
} from 'vue-color'
import whiteBoardService from './whiteBoardService';
import helperUtil from './utils/helper';
import { dataTrack } from '../tutorService';
import shareRoomBtn from '../tutorHelpers/shareRoomBtn.vue'
import { mapGetters, mapActions } from "vuex";
import canvasFinder from "./utils/canvasFinder";
import equationMapper from "./innerComponents/equationMapper.vue"

export default {
    components: {
        'sliderPicker': Compact,
         shareRoomBtn,
         equationMapper
    },
    data() {
        return {
            canvasWidth: global.innerWidth,
            canvasHeight: global.innerHeight -64,
            showPickColorInterface: false,
            showHelper: false,
            formula: 'x = {-b \\pm \\sqrt{b^2-4ac} \\over 2a}.',
            predefinedColors:[
                '#000000',
                '#FF0000',
                '#00ff00',
                '#40e0d0',
                '#800000',
                '#0000ff',
                '#008000',
                '#ffd700',
                '#8a2be2',
                '#ff00ff',
                '#c0c0c0',
                '#ffff00',
                '#088da5',
                '#003366'
            ],
            enumOptions: {
                draw: 'liveDraw',
                line: 'lineDraw',
                circle: 'drawEllipse',
                rectangle: 'drawRectangle',
                image: 'imageDraw',
                eraser: 'eraser',
                text: 'textDraw',
                equation: 'equationDraw',
                select: 'selectShape',
                pan: 'panTool',
            },
            currentOptionSelected: whiteBoardService.init('liveDraw'),
            selectedOptionString: 'liveDraw',
            canvasData: {
                shapesSelected: {},
                shouldPaint: false,
                context: null,
                helper: helperUtil.HelperObj,
                metaData: {
                    previouseDrawingPosition: null,
                },
                lineJoin: "round",
                lineWidth: 2,
                color: {
                    hex: '#000000'
                },
                methods: {
                    addShape: this.addShape,
                    hideColorPicker: this.hideColorPicker,
                    selectDefaultTool: this.selectDefaultTool
                },
                objDetected: false
            }
        }
    },
    computed:{
        ...mapGetters(['isRoomCreated', 'getDragData','getZoom']),
        helperStyle(){
            return helperUtil.HelperObj.style
        },
        helperClass(){
            return helperUtil.HelperObj.cssClass
        },
        helperShow(){
            return helperUtil.HelperObj.isActive;
        },
        dragData(){
            return this.getDragData;
        },
        zoom(){
            return this.getZoom.toFixed();
        },
        computedFormula:{
            get(){
                return `$$${this.formula}$$`;
            },
            set(val){
                this.formula = val;
            }
        }
    },
    methods: {
        ...mapActions(['resetDragData', 'updateDragData', 'updateZoom', 'updatePan']),
        selectDefaultTool(){
            this.setOptionType(this.enumOptions.select);
        },
        setOptionType(selectedOption) {
            this.currentOptionSelected = whiteBoardService.init.bind(this.canvasData, selectedOption)();
            this.selectedOptionString = selectedOption;
            helperUtil.HelperObj.isActive = false;
            if(selectedOption === this.enumOptions.image){
                let inputImgElm = document.getElementById('imageUpload');
                inputImgElm.click();
                this.selectDefaultTool();
            }
        },
        showColorPicker() {
            this.showPickColorInterface = true;
        },
        hideColorPicker() {
            this.showPickColorInterface = false;
        },
        clearCanvas() {
            this.resetDragData();
            whiteBoardService.redraw(this.canvasData)
            helperUtil.HelperObj.isActive = false;
        },
        addShape(dragObj, callback) {
            this.updateDragData(dragObj);
            if(callback){
                callback();
            }
            let canvasData = {
                context: this.canvasData.context,
                metaData: this.canvasData.metaData
            };
            let data = {
                canvasContext: canvasData,
                dataContext: dragObj
            };
            let transferDataObj = {
                type: "passData",
                data: data
            };
            let normalizedData = JSON.stringify(transferDataObj);
            dataTrack.send(normalizedData);
            if(!dragObj.isGhost && this.selectedOptionString !== this.enumOptions.draw){
                this.selectDefaultTool(); //case SPITBALL-647
            }
        },
        undo(){
            let transferDataObj = {
                type: "undoData",
                data: this.canvasData
            };
            let normalizedData = JSON.stringify(transferDataObj);
            dataTrack.send(normalizedData);
            whiteBoardService.undo(this.canvasData);

        },
        keyPressed(e) {
            //signalR should be fired Here
            if ((e.which == 90 || e.keyCode == 90) && e.ctrlKey) {
                this.undo();
            }
            if(((e.which == 46 || e.keyCode == 46)||(e.which == 8 || e.keyCode == 8)) && this.selectedOptionString === this.enumOptions.select){
                this.currentOptionSelected.deleteSelectedShape.bind(this.canvasData)();
            }
            if(((e.which == 13 || e.keyCode == 13) || (e.which == 27 || e.keyCode == 27)) && this.selectedOptionString === this.enumOptions.text){
               //enter or escape in text mode
                this.currentOptionSelected.enterPressed.bind(this.canvasData)();
            }
        },
        resetZoom(){
            whiteBoardService.hideHelper();
            this.updateZoom(100);
            this.updatePan({e:0, f:0})
        },
        resizeCanvas(){
            let canvas = document.getElementById('canvas');
            let ctx = canvas.getContext("2d");
            ctx.setTransform(1,0,0,1,0,0);
            this.resetZoom();
            this.canvasWidth = (global.innerWidth -50);
            this.canvasHeight = (global.innerHeight -50);
            canvas.width = this.canvasWidth;
            canvas.height = this.canvasHeight;
            whiteBoardService.redraw(this.canvasData);
        },
        injectToTextArea(textToInject){
            let textAreaElm = document.getElementById('textArea-tutoring');
            let newValue = textAreaElm.insertAtCaret(textToInject);            
            this.helperStyle.text = newValue;
        },
        doZoom(zoomType){
            whiteBoardService.hideHelper();
            let panTool = whiteBoardService.init.bind(this.canvasData, this.enumOptions.pan)();
            panTool.manualScroll.bind(this.canvasData, zoomType)();
        },
        registerCanvasEvents(canvas){
            let self = this;

            global.addEventListener('resize', this.resizeCanvas, false);
            canvas.addEventListener('mousedown', (e) => {
                if (!!self.currentOptionSelected && self.currentOptionSelected.mousedown) {
                    self.currentOptionSelected.mousedown.bind(self.canvasData, e)()
                }
            });
            canvas.addEventListener('mouseup', (e) => {
                if (!!self.currentOptionSelected && self.currentOptionSelected.mouseup) {
                    self.currentOptionSelected.mouseup.bind(self.canvasData, e)()
                }
            });
            canvas.addEventListener('mouseleave', (e) => {
                if (!!self.currentOptionSelected && self.currentOptionSelected.mouseleave) {
                    self.currentOptionSelected.mouseleave.bind(self.canvasData, e)()
                }
            });
            canvas.addEventListener('mousemove', (e) => {
                    if (!!self.currentOptionSelected && self.currentOptionSelected.mousemove) {
                        self.currentOptionSelected.mousemove.bind(self.canvasData, e)()
                    }
            });
            canvas.addEventListener('DOMMouseScroll', (e) => {
                if (!!self.currentOptionSelected && self.currentOptionSelected.mouseScroll) {
                    self.currentOptionSelected.mouseScroll.bind(self.canvasData, e)()
                }
            });
            canvas.addEventListener('mousewheel', (e) => {
                if (!!self.currentOptionSelected && self.currentOptionSelected.mouseScroll) {
                    self.currentOptionSelected.mouseScroll.bind(self.canvasData, e)()
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
        let canvas = document.querySelector('canvas');
        canvas.width = this.canvasWidth;
        canvas.height = this.canvasHeight;
        this.canvasData.context = canvas.getContext("2d");
        this.canvasData.context.font = '16px Open Sans';
        this.canvasData.context.lineJoin = this.canvasData.lineJoin;
        this.canvasData.context.lineWidth = this.canvasData.lineWidth;
        canvasFinder.trackTransforms(this.canvasData.context);
        this.registerCanvasEvents(canvas);
        global.document.addEventListener("keydown", this.keyPressed);
        
    }
}