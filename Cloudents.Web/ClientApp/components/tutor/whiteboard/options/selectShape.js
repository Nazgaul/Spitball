import {createShape, createGhostShape} from '../utils/factories'
import whiteBoardService from '../whiteBoardService'
import helper from '../utils/helper'
import canvasFinder from '../utils/canvasFinder'

const OPTION_TYPE = 'selectShape';

let localShape = createGhostShape({
    type: OPTION_TYPE,
    shapesObj: {}
});

const clearLocalShape = function(){
    localShape = createGhostShape({
        type: OPTION_TYPE,
        shapesObj: {}
    });
}

const startingMousePosition = {
    x:null,
    y:null
}

let startShapes = {}

let currentX = null;
let currentY = null;

let currentHelperObj = null;
let mouseInsideSelectedRectangle = false;
let multiSelectActive = false;

const init = function(){
    startShapes = {};
    clearMark();
}

const getHelperObj = function(sx, sy, ex, ey, supressOffset){
        let leftOffset = supressOffset ? 0 : this.context.canvas.getBoundingClientRect().left;
        let topOffset = supressOffset ? 0 : this.context.canvas.getBoundingClientRect().top;
        let startX = sx + leftOffset;
        let startY = sy + topOffset;
        let endX = ex + leftOffset;
        let endY = ey + topOffset;
        return {
            startPositionLeft: startX,
            startPositionTop: startY,
            currentX: endX,
            currentY: endY,
            width: endX - startX,
            height: endY - startY,
            strokeStyle: this.color.hex
        }
}

const showHelper = function(helperObj){
        helper.showHelper();
        currentHelperObj = helperObj;
        helper.setRectangleShape(helperObj)
    
}
const liveDraw = function(helperObj){
    showHelper.bind(this, helperObj)();
}

const markShapes = function(){
    //console.log(this.shapesSelected);
    let points = [];
    Object.keys(this.shapesSelected).forEach(shapeId=>{
        if(this.shapesSelected[shapeId].visible){
            points = points.concat(this.shapesSelected[shapeId].points);
        }
    })
    let rectangleBoundries = canvasFinder.getBoundriesPoints(points);
    let helperObj = getHelperObj.bind(this, rectangleBoundries.startX, rectangleBoundries.startY, rectangleBoundries.endX, rectangleBoundries.endY)();
        
    liveDraw.bind(this, helperObj)();
}

const clearMark = function(){
    currentHelperObj = null;
    helper.hideHelper();
    helper.resetHelperObj();
}

const mousedown = function(e){
    startingMousePosition.x = e.clientX;
    startingMousePosition.y = e.clientY;
    let mouseX = currentX - e.target.offsetLeft;
    let mouseY = currentY - e.target.getBoundingClientRect().top;
    this.shouldPaint = true;
    if(!currentHelperObj){
        this.shapesSelected = canvasFinder.getShapeByPoint(mouseX, mouseY, this);
        if(Object.keys(this.shapesSelected).length > 0){
            Object.keys(this.shapesSelected).forEach(shapeId=>{
                startShapes[shapeId] = createShape(this.shapesSelected[shapeId])
            })
            markShapes.bind(this)();
            //mouseInsideSelectedRectangle = true;
        }else{
            startShapes = {};
            //chack if mouse clicked inside a selection box
            if(!!currentHelperObj){
                let rect = {
                    startX: currentHelperObj.startPositionLeft,
                    startY: currentHelperObj.startPositionTop,
                    w: currentHelperObj.currentX - currentHelperObj.startPositionLeft,
                    h: currentHelperObj.currentY - currentHelperObj.startPositionTop,
                }
                mouseInsideSelectedRectangle = canvasFinder.inBox(currentX, currentY, rect);
                if(!mouseInsideSelectedRectangle){
                    clearMark();
                }
            }
            else{
                mouseInsideSelectedRectangle = false;
                clearMark();
            }
        }
    }else{
        let rect = {
            startX: currentHelperObj.startPositionLeft,
            startY: currentHelperObj.startPositionTop,
            w: currentHelperObj.currentX - currentHelperObj.startPositionLeft,
            h: currentHelperObj.currentY - currentHelperObj.startPositionTop,
        }
        mouseInsideSelectedRectangle = canvasFinder.inBox(currentX, currentY, rect);
        if(!mouseInsideSelectedRectangle){
            clearMark();
        }else{
            if(Object.keys(this.shapesSelected).length > 0){
                Object.keys(this.shapesSelected).forEach(shapeId=>{
                    startShapes[shapeId] = createShape(this.shapesSelected[shapeId])
                })
                markShapes.bind(this)();
                //mouseInsideSelectedRectangle = true;
            }
        }
    }
}

const moveShapes = function(){
    Object.keys(this.shapesSelected).forEach(shapeId=>{
        let shape = this.shapesSelected[shapeId];
        shape.points.forEach((point, index)=>{
            let dragoffx =  (currentX - startingMousePosition.x);
            let dragoffy =  (currentY - startingMousePosition.y);
            point.mouseX = startShapes[shapeId].points[index].mouseX + dragoffx
            point.mouseY = startShapes[shapeId].points[index].mouseY + dragoffy
            //console.log(`start: ${startShapes[shapeId].points[index].mouseX} current: ${point.mouseX}`)
        })
    })
    whiteBoardService.cleanCanvas(this.context);
    whiteBoardService.redraw(this);
    markShapes.bind(this)();
    
}

const mousemove = function(e){
    currentX = e.clientX;
    currentY = e.clientY;
    if(this.shouldPaint){
        if(mouseInsideSelectedRectangle){
            //move shape
            moveShapes.bind(this)();
        }else{
            //mark objects
            clearMark();
            let helperObj = getHelperObj.bind(this, startingMousePosition.x, startingMousePosition.y, currentX, currentY, true)();
            liveDraw.bind(this, helperObj)();
            multiSelectActive = true;
        }
    }
    
}

const addShape = function(){
    localShape.shapesObj = startShapes;
    this.methods.addShape(localShape, clearLocalShape);
    startShapes = {};
}

const defineEndPosition = function(e){
    if(this.shouldPaint){
        this.shouldPaint = false;
        if(!mouseInsideSelectedRectangle){
            //mark shapes
            if(multiSelectActive){
                multiSelectActive = false;
                //get rectangle with the offsets
                let startX = currentHelperObj.startPositionLeft - e.target.offsetLeft;
                let startY = currentHelperObj.startPositionTop - e.target.getBoundingClientRect().top;
                let w = (currentHelperObj.currentX - e.target.offsetLeft) - startX;
                let h = (currentHelperObj.currentY - e.target.getBoundingClientRect().top) - startY;
                let rect = {
                    startX,
                    startY,
                    w,
                    h,
                }
                this.shapesSelected = canvasFinder.getShapesByRectangle(this, rect);
                if(Object.keys(this.shapesSelected).length > 0){
                    Object.keys(this.shapesSelected).forEach(shapeId=>{
                        startShapes[shapeId] = createShape(this.shapesSelected[shapeId])
                    })
                    markShapes.bind(this)();
                }else{
                    startShapes = {};
                    clearMark();
                }
                console.log(startShapes);
            }
        }else{
            // object was just moved
            addShape.bind(this)();
        }
    }
}

const mouseup = function(e){
    defineEndPosition.bind(this, e)()
}
const mouseleave = function(e){
    defineEndPosition.bind(this, e)()
}

const deleteSelectedShape = function(e){
    if(Object.keys(this.shapesSelected).length> 0){
        Object.keys(this.shapesSelected).forEach(shapeId=>{
            startShapes[shapeId] = createShape(this.shapesSelected[shapeId])
            this.shapesSelected[shapeId].visible = false;
        })
        addShape.bind(this)();
        whiteBoardService.cleanCanvas(this.context);
        whiteBoardService.redraw(this);
        clearMark();
    }
}
export default{
    mousedown,
    mouseup,
    mousemove,
    mouseleave,
    draw: liveDraw,
    init,
    deleteSelectedShape
}