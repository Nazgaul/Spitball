import {
    Slider
} from 'vue-color'
import whiteBoardService from './whiteBoardService';
import helperUtil from './utils/helper';

export default {
    components: {
        'sliderPicker': Slider,
    },
    data() {
        return {
            canvasWidth: 1000,
            canvasHeight: 500,
            showPickColorInterface: false,
            showHelper: false,
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
                dragData: [],
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
                    hex: '#194d33'
                },
                methods: {
                    addShape: this.addShape,
                    hideColorPicker: this.hideColorPicker
                },
                objDetected: false
            }
        }
    },
    computed:{
        helperStyle(){
            return helperUtil.HelperObj.style
        },
        helperClass(){
            return helperUtil.HelperObj.cssClass
        },
        helperShow(){
            return helperUtil.HelperObj.isActive;
        }
    },
    methods: {
        setOptionType(selectedOption) {
            this.currentOptionSelected = whiteBoardService.init(selectedOption);
            this.selectedOptionString = selectedOption;
            helperUtil.HelperObj.isActive = false;
        },
        showColorPicker() {
            this.showPickColorInterface = true;
        },
        hideColorPicker() {
            this.showPickColorInterface = false;
        },
        clearCanvas() {
            whiteBoardService.cleanCanvas(this.canvasData.context);
            this.canvasData.dragData = [];
            helperUtil.HelperObj.isActive = false;
        },
        addShape(dragObj, callback) {
            this.canvasData.dragData.push(dragObj);
            callback();
        },
        undo(){
            whiteBoardService.cleanCanvas(this.canvasData.context);
            whiteBoardService.undo(this.canvasData);
        },
        keyPressed(e) {
            //signalR should be fired Here
            if ((e.which == 90 || e.keyCode == 90) && e.ctrlKey) {
                this.undo();
            }if((e.which == 46 || e.keyCode == 46) && this.selectedOptionString === this.enumOptions.select){
                this.currentOptionSelected.deleteSelectedShape.bind(this.canvasData)();
            }
        },
        resizeCanvas(){
            let canvas = document.getElementById('canvas');
            canvas.width = this.canvasWidth;
            canvas.height = this.canvasHeight;
            whiteBoardService.redraw(this.canvasData);
        }
    },
    mounted() {
        let canvas = document.querySelector('canvas');
        canvas.width = this.canvasWidth;
        canvas.height = this.canvasHeight;
        this.canvasData.context = canvas.getContext("2d");
        this.canvasData.context.lineJoin = this.canvasData.lineJoin;
        this.canvasData.context.lineWidth = this.canvasData.lineWidth;
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
        global.document.addEventListener("keydown", self.keyPressed);
    }
}