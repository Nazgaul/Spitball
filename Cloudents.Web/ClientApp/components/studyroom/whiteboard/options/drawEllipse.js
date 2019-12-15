import {createPointsByOption, createShape} from '../utils/factories'
import helper from '../utils/helper'
import canvasFinder from '../utils/canvasFinder'
import whiteBoardService from '../whiteBoardService';

const optionType = 'drawEllipse';

let localShape = createShape({
    type: optionType,
    points: [],
    path:{
        stroke: null,
        fill: null
    }
});

const clearLocalShape = function(){
    localShape = createShape({
        type: optionType,
        points: [],
        path:{
            stroke: null,
            fill: null
        }
    });
};

const startingMousePosition = {
    x:null,
    y:null
};

let linePath2D = null; //Made for the hit detection

const init = function(){
    
};

const draw = function(dragObj, fromArray, previouseDrawingObj){
    //determin the stroke color
    this.context.strokeStyle = dragObj.strokeStyle;
    let previousCustomDragObject ;
    if(fromArray){
        previousCustomDragObject = previouseDrawingObj ? previouseDrawingObj : dragObj;
    }else{
        previousCustomDragObject = this.metaData.previouseDrawingPosition;
    }
    let startX = previousCustomDragObject.mouseX;
    let startY = previousCustomDragObject.mouseY;
    let x = dragObj.mouseX;
    let y = dragObj.mouseY;
    let radiusX = (x - startX) * 0.5;
    let radiusY = (y - startY) * 0.5;
    let centerX = startX + radiusX;
    let centerY = startY + radiusY;
    radiusX = radiusX < 0 ? radiusX*-1 : radiusX;
    radiusY = radiusY < 0 ? radiusY*-1 : radiusY;
    linePath2D.ellipse(centerX, centerY, radiusX, radiusY, 0, 0, 2 * Math.PI, false);
};
const liveDraw = function(dragObj, fromArray, previouseDrawingObj){
    this.context.beginPath();
    linePath2D = new Path2D();
    draw.bind(this, dragObj, fromArray, previouseDrawingObj)();
    this.context.closePath();
    this.context.stroke(linePath2D);
    
};

const mousedown = function(e){
    //Set Click Position
    this.methods.hideColorPicker();
    
    startingMousePosition.x = e.clientX;
    startingMousePosition.y = e.clientY;
    
    let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, e.pageX - e.target.offsetLeft - e.target.getBoundingClientRect().left, e.pageY - e.target.getBoundingClientRect().top);
    let dragObj = createPointsByOption({
        mouseX: mouseX,
        mouseY: mouseY,
        isDragging: false,
        strokeStyle: this.color.hex,
        option: optionType,
        eventName: 'start'
    });
    this.metaData.previouseDrawingPosition = dragObj;
    this.shouldPaint = true;
    localShape.points.push(dragObj);
    liveDraw.bind(this, dragObj)();
    helper.showHelper();
};
const mousemove = function(e){
    if(this.shouldPaint){
    let x = e.pageX;
    let y = e.pageY;
    let startX = startingMousePosition.x;
    let startY = startingMousePosition.y;
    let radiusX = (x - startX) * 0.5;
    let radiusY = (y - startY) * 0.5;
    let centerX = startX + radiusX;
    let centerY = startY + radiusY;
        let helperObj = {
            cx: centerX,
            cy: centerY,
            rx: radiusX,
            ry: radiusY,
            strokeStyle: this.color.hex
        };
        helper.setEllipseShape(helperObj);
    }
};
const defineEndPosition = function(e){
    if(this.shouldPaint){
        let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, e.pageX - e.target.offsetLeft - e.target.getBoundingClientRect().left, e.pageY - e.target.getBoundingClientRect().top);
        let dragObj = createPointsByOption({
            mouseX: mouseX,
            mouseY: mouseY,
            isDragging: false,
            strokeStyle: this.color.hex,
            option: optionType,
            eventName: 'end'
        });
        localShape.points.push(dragObj);
        liveDraw.bind(this, dragObj)();
        this.shouldPaint = false;
        localShape.path.stroke = linePath2D;
        this.methods.addShape(localShape, clearLocalShape);
        whiteBoardService.hideHelper();
    }
};
const mouseup = function(e){
    defineEndPosition.bind(this, e)();
};
const mouseleave = function(e){
    defineEndPosition.bind(this, e)();
};
export default{
    mousedown,
    mouseup,
    mousemove,
    mouseleave,
    draw: liveDraw,
    init
}