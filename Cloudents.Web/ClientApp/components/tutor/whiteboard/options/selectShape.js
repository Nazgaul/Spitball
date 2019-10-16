import store from '../../../../store/index.js'
import {createShape, createGhostShape} from '../utils/factories'
import whiteBoardService from '../whiteBoardService'
import helper from '../utils/helper'
import canvasFinder from '../utils/canvasFinder'

const optionType = 'selectShape';

const startingMousePosition = {
    x:null,
    y:null
};
let wrapperElm = null;
let startShapes = {};
let topOffset = null;

let dragoffx = 0;
let dragoffy = 0;
let currentX = null;
let currentY = null;

let currentHelperObj = null;
let mouseInsideSelectedRectangle = false;
let multiSelectActive = false;

const init = function(){
    wrapperElm = document.getElementById('canvas-wrapper');
    startShapes = {};
    dragoffx = 0;
    dragoffy = 0;   
    clearMark();
};

const selectedShapes = function(id){
    if(id){
        return store.getters['getShapesSelected'][id];
    }else{
        return store.getters['getShapesSelected'];
    }
    
};

const setSelectedShapes = function(shape){
    if(Object.keys(shape).length > 0){
        Object.keys(shape).forEach(shapeId=>{
            store.dispatch('setShapesSelected', shape[shapeId]);
        });
    }else{
        store.dispatch('clearShapesSelected');
    }
};

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
            strokeStyle: '#000000'
        };
};

const showHelper = function(helperObj){
        helper.showHelper();
        currentHelperObj = helperObj;
        helper.setRectangleShape(helperObj);

};
const liveDraw = function(helperObj){
    showHelper.bind(this, helperObj)();
};

const markShapes = function(){
    //console.log(this.shapesSelected);
    let points = [];
    Object.keys(selectedShapes()).forEach(shapeId=>{
        if(selectedShapes(shapeId).visible){
            points = points.concat(selectedShapes(shapeId).points);
        }
    });
    let rectangleBoundries = canvasFinder.getBoundriesPoints(points, this);
    //a = scale of x / d = scale of y
    let {a, b, c, d, e, f} = this.context.getTransform();
    let {mouseX:startX, mouseY:startY} = canvasFinder.getRelativeMousePoints(this.context, rectangleBoundries.startX*-a, rectangleBoundries.startY*-d );
    let {mouseX:endX, mouseY:endY} = canvasFinder.getRelativeMousePoints(this.context, rectangleBoundries.endX*-a, rectangleBoundries.endY*-d);
    let helperObj = getHelperObj.bind(this, startX*-a, startY*-d, endX*-a, endY*-d)();
    liveDraw.bind(this, helperObj)();
};

const clearMark = function(){
    currentHelperObj = null;
    whiteBoardService.hideHelper();
};

const mousedown = function(e){
    topOffset = e.target.getBoundingClientRect().top;
    startingMousePosition.x = e.clientX;
    startingMousePosition.y = e.clientY;
    this.methods.hideColorPicker();
    this.shouldPaint = true;
    if(!currentHelperObj){
        let scrollLeft = wrapperElm.scrollLeft;
        let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, (currentX - e.target.offsetLeft) + scrollLeft, (currentY - e.target.getBoundingClientRect().top));
        setSelectedShapes(canvasFinder.getShapeByPoint(mouseX, mouseY, this, whiteBoardService.getDragData()));
        if(Object.keys(selectedShapes()).length > 0){
            Object.keys(selectedShapes()).forEach(shapeId=>{
                startShapes[shapeId] = createShape(selectedShapes(shapeId));
            });
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
                };
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
        };
        mouseInsideSelectedRectangle = canvasFinder.inBox(currentX, currentY, rect);
        if(!mouseInsideSelectedRectangle){
            clearMark();
        }else{
            if(Object.keys(selectedShapes()).length > 0){
                Object.keys(selectedShapes()).forEach(shapeId=>{
                    startShapes[shapeId] = createShape(selectedShapes(shapeId));
                });
                markShapes.bind(this)();
            }
        }
    }
};

const moveShapes = function(){
    let {a, b, c, d, e, f} = this.context.getTransform();
    Object.keys(selectedShapes()).forEach(shapeId=>{
        let shape = selectedShapes(shapeId);
        shape.points.forEach((point, index)=>{
            dragoffx =  (currentX - startingMousePosition.x)/a;
            dragoffy =  (currentY - startingMousePosition.y)/d;
            point.mouseX = startShapes[shapeId].points[index].mouseX + dragoffx;
            point.mouseY = startShapes[shapeId].points[index].mouseY + dragoffy;
        });
    });
    whiteBoardService.redraw(this);
    markShapes.bind(this)();
    
};

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
    
};

const MoveObjAction = function(ids, distanceX, distanceY){
    this.distanceX = distanceX;
    this.distanceY = distanceY;
    this.ids = ids;
};

const DeleteObjAction = function(ids){
    this.ids = ids;
};

const addShape = function(actionType, actionObj){
    let localShape = createGhostShape({
        type: optionType,
        actionType: actionType, // move, delete
        actionObj: actionObj
    });
    this.methods.addShape(localShape);
    startShapes = {};
};

const defineEndPosition = function(e){
    if(this.shouldPaint){
        this.shouldPaint = false;
        if(!mouseInsideSelectedRectangle){
            //mark shapes
            if(multiSelectActive){
                multiSelectActive = false;
                //get rectangle with the offsets
                let {mouseX:startX, mouseY:startY} = canvasFinder.getRelativeMousePoints(this.context, currentHelperObj.startPositionLeft - e.target.offsetLeft - e.target.getBoundingClientRect().left, currentHelperObj.startPositionTop - e.target.getBoundingClientRect().top);
                let {mouseX:w, mouseY:h} = canvasFinder.getRelativeMousePoints(this.context, (currentHelperObj.currentX - e.target.offsetLeft - e.target.getBoundingClientRect().left) - startX, (currentHelperObj.currentY - e.target.getBoundingClientRect().top) - startY);
                let rect = {
                    startX,
                    startY,
                    w,
                    h,
                };
                setSelectedShapes(canvasFinder.getShapesByRectangle(this, rect, whiteBoardService.getDragData()));
                if(Object.keys(selectedShapes()).length > 0){
                    Object.keys(selectedShapes()).forEach(shapeId=>{
                        startShapes[shapeId] = createShape(selectedShapes(shapeId));
                    });
                    markShapes.bind(this)();
                }else{
                    startShapes = {};
                    clearMark();
                }
                console.log(startShapes);
            }
        }else{
            // object was just moved
            let moveAction = new MoveObjAction(Object.keys(selectedShapes()), dragoffx, dragoffy);
            addShape.bind(this, "move", moveAction)();
            dragoffx = 0;
            dragoffy = 0;
        }
    }
};

const mouseup = function(e){
    defineEndPosition.bind(this, e)();
};
const mouseleave = function(e){
    defineEndPosition.bind(this, e)();
};


const deleteSelectedShape = function(e){
    console.log("entered Delete Shape");
    if(Object.keys(selectedShapes()).length> 0){
        Object.keys(selectedShapes()).forEach(shapeId=>{
            startShapes[shapeId] = createShape(selectedShapes(shapeId));
            selectedShapes(shapeId).visible = false;
        });
        let deleteAction = new DeleteObjAction(Object.keys(selectedShapes()));
        addShape.bind(this, "delete" ,deleteAction)();
        whiteBoardService.redraw(this);
        clearMark();
        store.dispatch('clearShapesSelected');
    }
    console.log("finish Delete Shape");
    return true;
};
export default{
    mousedown,
    mouseup,
    mousemove,
    mouseleave,
    draw: liveDraw,
    init,
    deleteSelectedShape,
    reMarkSelectedShapes: markShapes,
}