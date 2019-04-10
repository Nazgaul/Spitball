import {createPointsByOption, createShape} from '../utils/factories'
import helper from '../utils/helper'
import canvasFinder from '../utils/canvasFinder'
import whiteBoardService from '../whiteBoardService'

const OPTION_TYPE = 'lineDraw';

const mouseState = {
    mouseX: null,
    mouseY: null
}

const startingMousePosition = {
    x:null,
    y:null
}

let localShape = createShape({
    type: OPTION_TYPE,
    points: [],
});

const init = function(){
    
}

const clearLocalShape = function(){
    localShape = createShape({
        type: OPTION_TYPE,
        points: [],
    });
}

const draw = function(dragObj){
    //determin the stroke color
    this.context.strokeStyle = dragObj.strokeStyle;
    let prevouseDragObj = this.metaData.previouseDrawingPosition;
    
    //draw
    if(dragObj.isDragging && !!prevouseDragObj){
        this.context.moveTo(prevouseDragObj.mouseX, prevouseDragObj.mouseY);
    }else{
        this.context.moveTo(dragObj.mouseX-1, dragObj.mouseY);
    }
    this.context.lineTo(dragObj.mouseX, dragObj.mouseY);
    this.metaData.previouseDrawingPosition = dragObj;
}
const liveDraw = function(dragObj){
    this.context.beginPath();
    draw.bind(this, dragObj)();
    this.context.closePath();
    this.context.stroke();
    
}
const defineEndPosition = function(e){
    if(this.shouldPaint){
        let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, e.pageX - e.target.offsetLeft - e.target.getBoundingClientRect().left, e.pageY - e.target.getBoundingClientRect().top);
        let dragObj = createPointsByOption({
            mouseX: mouseX,
            mouseY: mouseY,
            isDragging: true,
            strokeStyle: this.color.hex,
            option: OPTION_TYPE,
            eventName: 'end'
        })
        let previousDragObj = createPointsByOption({
            mouseX: mouseState.mouseX,
            mouseY: mouseState.mouseY,
            isDragging: true,
            strokeStyle: this.color.hex,
            option: OPTION_TYPE,
            eventName: 'end'
        })
            localShape.points.push(dragObj);
            liveDraw.bind(this, dragObj, previousDragObj)()
            this.methods.addShape(localShape, clearLocalShape);
    }
    this.shouldPaint = false;
    whiteBoardService.hideHelper();
    
}
const mousedown = function(e){
    //Set Click Position
    let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, e.pageX - e.target.offsetLeft - e.target.getBoundingClientRect().left, e.pageY - e.target.getBoundingClientRect().top);
    mouseState.mouseX = mouseX;
    mouseState.mouseY = mouseY;
    //helper obj
    startingMousePosition.x = e.pageX;
    startingMousePosition.y = e.pageY;

    this.methods.hideColorPicker();

    let dragObj = createPointsByOption({
        mouseX: mouseState.mouseX,
        mouseY: mouseState.mouseY,
        isDragging: false,
        strokeStyle: this.color.hex,
        option: OPTION_TYPE,
        eventName: 'start'
    })

    this.shouldPaint = true;
    localShape.points.push(dragObj);
    liveDraw.bind(this, dragObj)();
    helper.showHelper();
}
const mousemove = function(e){
    if(this.shouldPaint){
        let currentX = e.pageX;
        let currentY = e.pageY;
        let helperObj = {
            startPositionLeft: startingMousePosition.x,
            startPositionTop: startingMousePosition.y,
            currentX,
            currentY,
            strokeStyle: this.color.hex
        }
        helper.setLineShape(helperObj)
    }
}
const mouseup = function(e){
    defineEndPosition.bind(this, e)()
}
const mouseleave = function(e){
    defineEndPosition.bind(this, e)()
}
export default{
    mousedown,
    mouseup,
    mousemove,
    mouseleave,
    draw: liveDraw,
    init
}