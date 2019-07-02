
import whiteBoardService from './whiteBoardService';
import helperUtil from './utils/helper';
import { mapGetters, mapActions } from "vuex";
import canvasFinder from "./utils/canvasFinder";
import equationMapper from "./innerComponents/equationMapper.vue"
import tutorService from "../tutorService";

const HeaderHeight = 108;

export default {
    components: {
        equationMapper
    },
    data() {
        return {
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
            }
        }
    },
    computed: {
        ...mapGetters(['isRoomCreated', 'getDragData','getZoom', 'selectedOptionString','getCanvasTabs', 'getCurrentSelectedTab', 'currentOptionSelected', 'canvasDataStore','undoClicked', 'addImage']),
        helperStyle() {
            return helperUtil.HelperObj.style
        },
        helperClass() {
            return helperUtil.HelperObj.cssClass
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
            return this.getCanvasTabs
        },
        canvasDataColor(){
            //activated from watch
            this.canvasData.color = this.canvasDataStore.color;
            return this.canvasDataStore.color;
        },
        isTutor() {
            return this.getStudyRoomData ? this.getStudyRoomData.isTutor : false;
        },
    },
    watch: {
        //TODO: we remove this to add more juice to network
        //activeRoom:  function(val) {
        //    if (this.getCurrentRoomState !== "active") {
        //        return
        //    }
        //    try {
        //        //Adding canvas stream to twilio
        //        let stream = canvas.captureStream(60);
        //        this.activeRoom.localParticipant.publishTrack(stream.getTracks()[0], {
        //            name: `canvas_${this.isTutor ? "tutor" : "student"}_${
        //                this.accountUserID
        //                }`
        //        });
        //    }
        //    catch (e) {
        //        //TODO: not all browsers support this probably....
        //        //https://developer.mozilla.org/en-US/docs/Web/API/HTMLCanvasElement/captureStream
        //    }
        //},
        canvasDataColor(newVal){
            //watch is activating the canvasDataColor computed
            this.canvasData.color = newVal;
        },
        undoClicked(){
            this.undo();
        },
        addImage(newVal){
                if(!!newVal){
                    this.addShape(newVal.dragObj, newVal.callback)
                }
            }
        },
    methods: {
        ...mapActions(['resetDragData', 'updateDragData', 'updateZoom', 'updatePan', 'setSelectedOptionString', 'changeSelectedTab', 'removeCanvasTab', 'setCurrentOptionSelected', 'setShowPickColorInterface']),
        renameTab() {
            console.log("Rename Tab");
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
        
        
        clearCanvas() {
            this.resetDragData();
            whiteBoardService.redraw(this.canvasData)
            helperUtil.HelperObj.isActive = false;
        },
        addShape(dragObj, callback) {
            let dragUpdate = {
                tab: this.getCurrentSelectedTab,
                data: dragObj
            }
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
            tutorService.dataTrack.send(normalizedData);
            if (!dragObj.isGhost && this.selectedOptionString !== this.enumOptions.draw) {
                // this.selectDefaultTool();
                //case SPITBALL-647
                this.setCurrentOptionSelected(whiteBoardService.init.bind(this.canvasData, this.enumOptions.select)());
                this.setSelectedOptionString(this.enumOptions.select);
            }
        },
        undo() {
            let transferDataObj = {
                type: "undoData",
                data: this.canvasData,
                tab: this.getCurrentSelectedTab,
            };
            let normalizedData = JSON.stringify(transferDataObj);
            tutorService.dataTrack.send(normalizedData);
            whiteBoardService.undo(this.canvasData);

        },
        keyPressed(e) {
            if ((e.which == 121 || e.keyCode == 121)) {
                //F10
                let link = document.createElement('a');
                link.download = `${this.getCurrentSelectedTab.name}.png`;
                link.href = document.getElementById('canvas').toDataURL("image/png")
                link.click();
            }
            //signalR should be fired Here
            if ((e.which == 90 || e.keyCode == 90) && e.ctrlKey) {
                this.undo();
            }
            if (((e.which == 46 || e.keyCode == 46) || (e.which == 8 || e.keyCode == 8)) && this.selectedOptionString === this.enumOptions.select) {
                this.currentOptionSelected.deleteSelectedShape.bind(this.canvasData)();
            }
            if (((e.which == 13 || e.keyCode == 13) || (e.which == 27 || e.keyCode == 27))) {
                //enter or escape in text mode
                if (this.selectedOptionString === this.enumOptions.text) {
                    this.currentOptionSelected.enterPressed.bind(this.canvasData)();
                }

            }
        },
        changeTab(tab) {
            if (tab.id !== this.getCurrentSelectedTab.id) {
                // this.clearTabOption();
                this.changeSelectedTab(tab);
                whiteBoardService.hideHelper();
                whiteBoardService.redraw(this.canvasData)
            }
        },
        resizeCanvas() {
            let canvas = document.getElementById('canvas');
            let ctx = canvas.getContext("2d");
            ctx.setTransform(1, 0, 0, 1, 0, 0);
            this.windowWidth = global.innerWidth,
                this.windowHeight = global.innerHeight - HeaderHeight,
                canvas.width = this.canvasWidth;
            canvas.height = this.canvasHeight;
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
            canvas.addEventListener('mousedown', (e) => {
                // self.clearTabOption();
                if (e.button == 0) {
                    if (!!self.currentOptionSelected && self.currentOptionSelected.mousedown) {
                        self.currentOptionSelected.mousedown.bind(self.canvasData, e)()
                    }
                }

            });
            canvas.addEventListener('mouseup', (e) => {
                if (e.button == 0) {
                    if (!!self.currentOptionSelected && self.currentOptionSelected.mouseup) {
                        self.currentOptionSelected.mouseup.bind(self.canvasData, e)()
                    }
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
            canvasWrapper.addEventListener('scroll', (e) => {
                if (this.selectedOptionString === this.enumOptions.select) {
                    self.currentOptionSelected.reMarkSelectedShapes.bind(self.canvasData, e)()
                }
                let transform = {
                    x: e.target.scrollLeft * -1,
                    y: e.target.scrollTop * -1
                }
                self.updatePan(transform)
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
        let canvasWrapper = document.querySelector('.canvas-wrapper');
        canvas.width = this.canvasWidth;
        canvas.height = this.canvasHeight;
        this.canvasData.context = canvas.getContext("2d");
        this.canvasData.context.font = '16px Open Sans';
        this.canvasData.context.lineJoin = this.canvasData.lineJoin;
        this.canvasData.context.lineWidth = this.canvasData.lineWidth;
        canvasFinder.trackTransforms(this.canvasData.context);
        this.registerCanvasEvents(canvas, canvasWrapper);
        global.document.addEventListener("keydown", this.keyPressed);




    }
}