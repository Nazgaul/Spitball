import {createShape, createGhostShape} from '../utils/factories'
import whiteBoardService from '../whiteBoardService'
import helper from '../utils/helper'
import canvasFinder from '../utils/canvasFinder'

const OPTION_TYPE = 'selectShape';

const startingMousePosition = {
    x:null,
    y:null
}

let startShapes = {}
let topOffset = null;

let dragoffx = 0;
let dragoffy = 0;
let currentX = null;
let currentY = null;

let currentHelperObj = null;
let mouseInsideSelectedRectangle = false;
let multiSelectActive = false;

const init = function(){
    startShapes = {};
    dragoffx = 0;
    dragoffy = 0;   
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
    let rectangleBoundries = canvasFinder.getBoundriesPoints(points, this);
    //a = scale of x / d = scale of y
    let {a, b, c, d, e, f} = this.context.getTransform();
    let {mouseX:startX, mouseY:startY} = canvasFinder.getRelativeMousePoints(this.context, rectangleBoundries.startX*-a, rectangleBoundries.startY*-d );
    let {mouseX:endX, mouseY:endY} = canvasFinder.getRelativeMousePoints(this.context, rectangleBoundries.endX*-a, rectangleBoundries.endY*-d);
    let helperObj = getHelperObj.bind(this, startX*-a, startY*-d -topOffset, endX*-a, endY*-d -topOffset)();
        
    liveDraw.bind(this, helperObj)();
}

const clearMark = function(){
    currentHelperObj = null;
    whiteBoardService.hideHelper();
}

const mousedown = function(e){
    topOffset = e.target.getBoundingClientRect().top;
    startingMousePosition.x = e.clientX;
    startingMousePosition.y = e.clientY - topOffset;
    this.methods.hideColorPicker();
    this.shouldPaint = true;
    if(!currentHelperObj){
        let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, currentX - e.target.offsetLeft, currentY);
        this.shapesSelected = canvasFinder.getShapeByPoint(mouseX, mouseY, this, whiteBoardService.getDragData());
        if(Object.keys(this.shapesSelected).length > 0){
            Object.keys(this.shapesSelected).forEach(shapeId=>{
                startShapes[shapeId] = createShape(this.shapesSelected[shapeId])
            })
            markShapes.bind(this)();
            mouseInsideSelectedRectangle = true;
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
            }
        }
    }
}

const moveShapes = function(){
    let {a, b, c, d, e, f} = this.context.getTransform();
    Object.keys(this.shapesSelected).forEach(shapeId=>{
        let shape = this.shapesSelected[shapeId];
        shape.points.forEach((point, index)=>{
            dragoffx =  (currentX - startingMousePosition.x)/a;
            dragoffy =  (currentY - startingMousePosition.y)/d;
            point.mouseX = startShapes[shapeId].points[index].mouseX + dragoffx
            point.mouseY = startShapes[shapeId].points[index].mouseY + dragoffy
        })
    })
    whiteBoardService.redraw(this);
    markShapes.bind(this)();
    
}

const mousemove = function(e){
    currentX = e.clientX;
    currentY = e.clientY - topOffset;
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

const moveObjAction = function(ids, distanceX, distanceY){
    this.distanceX = distanceX;
    this.distanceY = distanceY;
    this.ids = ids
}

const deleteObjAction = function(ids){
    this.ids = ids;
}

const addShape = function(actionType, actionObj){
    let localShape = createGhostShape({
        type: OPTION_TYPE,
        actionType: actionType, // move, delete
        actionObj: actionObj
    });
    this.methods.addShape(localShape);
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
                let {mouseX:startX, mouseY:startY} = canvasFinder.getRelativeMousePoints(this.context, currentHelperObj.startPositionLeft - e.target.offsetLeft, currentHelperObj.startPositionTop - e.target.getBoundingClientRect().top);
                let {mouseX:w, mouseY:h} = canvasFinder.getRelativeMousePoints(this.context, (currentHelperObj.currentX - e.target.offsetLeft) - startX, (currentHelperObj.currentY - e.target.getBoundingClientRect().top) - startY);
                // let startX = currentHelperObj.startPositionLeft - e.target.offsetLeft;
                // let startY = currentHelperObj.startPositionTop - e.target.getBoundingClientRect().top;
                // let w = (currentHelperObj.currentX - e.target.offsetLeft) - startX;
                // let h = (currentHelperObj.currentY - e.target.getBoundingClientRect().top) - startY;
                let rect = {
                    startX,
                    startY,
                    w,
                    h,
                }
                this.shapesSelected = canvasFinder.getShapesByRectangle(this, rect, whiteBoardService.getDragData());
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
            let moveAction = new moveObjAction(Object.keys(this.shapesSelected), dragoffx, dragoffy)
            addShape.bind(this, "move", moveAction)();
            dragoffx = 0;
            dragoffy = 0;
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
        let deleteAction = new deleteObjAction(Object.keys(this.shapesSelected));
        addShape.bind(this, "delete" ,deleteAction)();
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