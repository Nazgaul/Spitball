import {
    Compact
} from 'vue-color'
import whiteBoardService from './whiteBoardService';
import helperUtil from './utils/helper';
import { dataTrack } from '../tutorService';
import shareRoomBtn from '../tutorHelpers/shareRoomBtn.vue'
import AppLogo from "../../../../wwwroot/Images/logo-spitball.svg";
import { mapGetters, mapActions } from "vuex";

export default {
    components: {
        'sliderPicker': Compact,
         shareRoomBtn,
         AppLogo
    },
    data() {
        return {
            canvasWidth: global.innerWidth -50,
            canvasHeight: global.innerHeight -50,
            showPickColorInterface: false,
            showHelper: false,
            predefinedColors:[
                '#f00', '#00ff00', '#00ff0055', 'rgb(201, 76, 76)', 'rgba(0,0,255,1)', 'hsl(89, 43%, 51%)', 'hsla(89, 43%, 51%, 0.6)'
            ],
            enumOptions: {
                draw: 'liveDraw',
                line: 'lineDraw',
                circle: 'drawEllipse',
                rectangle: 'drawRectangle',
                image: 'imageDraw',
                eraser: 'eraser',
                text: 'textDraw',
                select: 'selectShape'
            },
            currentOptionSelected: whiteBoardService.init('liveDraw'),
            selectedOptionString: '',
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
        ...mapGetters(['getDragData']),
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
        }
        
    },
    methods: {
        ...mapActions(['resetDragData', 'updateDragData']),
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
            callback();
            let data = {
                canvasContext: this.canvasData,
                dataContext: this.dragData
            }
            let transferDataObj = {
                type: "redrawData",
                data: data
            };
            let normalizedData = JSON.stringify(transferDataObj);
            dataTrack.send(normalizedData);
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
            if((e.which == 46 || e.keyCode == 46) && this.selectedOptionString === this.enumOptions.select){
                this.currentOptionSelected.deleteSelectedShape.bind(this.canvasData)();
            }
            if(((e.which == 13 || e.keyCode == 13) || (e.which == 27 || e.keyCode == 27)) && this.selectedOptionString === this.enumOptions.text){
               //enter or escape in text mode
                this.currentOptionSelected.enterPressed.bind(this.canvasData)();
            }
        },
        resizeCanvas(){
            let canvas = document.getElementById('canvas');
            this.canvasWidth = global.innerWidth -50;
            this.canvasHeight = global.innerHeight -50;
            canvas.width = this.canvasWidth;
            canvas.height = this.canvasHeight;
            whiteBoardService.redraw(this.canvasData);
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
        }
    },
    mounted() {
        let canvas = document.querySelector('canvas');
        canvas.width = this.canvasWidth;
        canvas.height = this.canvasHeight;
        this.canvasData.context = canvas.getContext("2d");
        this.canvasData.context.lineJoin = this.canvasData.lineJoin;
        this.canvasData.context.lineWidth = this.canvasData.lineWidth;
        this.registerCanvasEvents(canvas);
        global.document.addEventListener("keydown", this.keyPressed);
    }
}