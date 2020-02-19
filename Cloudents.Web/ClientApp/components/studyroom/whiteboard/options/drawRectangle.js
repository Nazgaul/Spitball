import {createPointsByOption, createShape} from '../utils/factories'
import helper from '../utils/helper'
import canvasFinder from '../utils/canvasFinder'

const optionType = 'drawRectangle';

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

let rectPath2D = null; //Made for the hit detection


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
    let width = dragObj.mouseX - startX;
    let height = dragObj.mouseY - startY;
    //draw
    rectPath2D.rect(startX, startY, width, height);
};
const liveDraw = function(dragObj, fromArray, previouseDrawingObj){
    this.context.beginPath();
    rectPath2D = new Path2D();
    draw.bind(this, dragObj, fromArray, previouseDrawingObj)();
    this.context.closePath();
    this.context.stroke(rectPath2D);
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
        
        let currentX = e.clientX;
        let currentY = e.clientY;
        let helperObj = {
            startPositionLeft: startingMousePosition.x,
            startPositionTop: startingMousePosition.y,
            currentX,
            currentY,
            width: currentX - startingMousePosition.x ,
            height: currentY - startingMousePosition.y,
            strokeStyle: this.color.hex
        };
        helper.setRectangleShape(helperObj);
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
        localShape.path.stroke = rectPath2D;
        this.methods.addShape(localShape, clearLocalShape);
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