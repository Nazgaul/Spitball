import {createPointsByOption, createShape} from '../utils/factories'
import helper from '../utils/helper'

const OPTION_TYPE = 'drawRectangle';

let localShape = createShape({
    type: OPTION_TYPE,
    points: [],
    path:{
        stroke: null,
        fill: null
    }
});

const clearLocalShape = function(){
    localShape = createShape({
        type: OPTION_TYPE,
        points: [],
        path:{
            stroke: null,
            fill: null
        }
    });
}

const startingMousePosition = {
    x:null,
    y:null
}

let rectPath2D = null; //Made for the hit detection


const init = function(){
    
}

const draw = function(dragObj, fromArray, previouseDrawingObj){
    //determin the stroke color
    this.context.strokeStyle = dragObj.strokeStyle;
    let previousCustomDragObject = null;
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
}
const liveDraw = function(dragObj, fromArray, previouseDrawingObj){
    this.context.beginPath();
    rectPath2D = new Path2D();
    draw.bind(this, dragObj, fromArray, previouseDrawingObj)();
    this.context.closePath();
    this.context.stroke(rectPath2D);
}

const mousedown = function(e){
    //Set Click Position
    this.methods.hideColorPicker();
    startingMousePosition.x = e.clientX;
    startingMousePosition.y = e.clientY;
    let dragObj = createPointsByOption({
        mouseX: e.pageX - e.target.offsetLeft,
        mouseY: e.pageY - e.target.offsetTop,
        isDragging: false,
        strokeStyle: this.color.hex,
        option: OPTION_TYPE,
        eventName: 'start'
    })
    this.metaData.previouseDrawingPosition = dragObj;
    this.shouldPaint = true;
    localShape.points.push(dragObj);
    liveDraw.bind(this, dragObj)();
    helper.showHelper();
}
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
        }
        helper.setRectangleShape(helperObj)
    }
}

const defineEndPosition = function(e){
    if(this.shouldPaint){
        let dragObj = createPointsByOption({
            mouseX: e.pageX - e.target.offsetLeft,
            mouseY: e.pageY - e.target.offsetTop,
            isDragging: false,
            strokeStyle: this.color.hex,
            option: OPTION_TYPE,
            eventName: 'end'
        })
        localShape.points.push(dragObj);
        console.log(this.dragData)
        liveDraw.bind(this, dragObj)();
        this.shouldPaint = false;
        localShape.path.stroke = rectPath2D;
        this.methods.addShape(localShape, clearLocalShape);
        helper.hideHelper();
        helper.resetHelperObj();
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