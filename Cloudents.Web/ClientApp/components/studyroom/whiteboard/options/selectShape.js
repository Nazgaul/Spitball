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
// let topOffset = null;

let dragoffx = 0;
let dragoffy = 0;
let currentX = null;
let currentY = null;
let selectedAnchor = null;

let currentHelperObj = null;
let mouseInsideSelectedRectangle = false;
let multiSelectActive = false;



const resetItems = function(){
    dragoffx = 0;
    dragoffy = 0;
    selectedAnchor = null;
}

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
    let {a, d} = this.context.getTransform();
    let {mouseX:startX, mouseY:startY} = canvasFinder.getRelativeMousePoints(this.context, rectangleBoundries.startX*-a, rectangleBoundries.startY*-d );
    let {mouseX:endX, mouseY:endY} = canvasFinder.getRelativeMousePoints(this.context, rectangleBoundries.endX*-a, rectangleBoundries.endY*-d);
    let helperObj = getHelperObj.bind(this, startX*-a, startY*-d, endX*-a, endY*-d)();
    liveDraw.bind(this, helperObj)();
};

const clearMark = function(){
    currentHelperObj = null;
    whiteBoardService.hideHelper();
};

const anchorHasClicked = function(currentHelperObj){
    let anchorScale = 12;
    let anchor_top_left = {
        startX: currentHelperObj.startPositionLeft - (anchorScale/2),
        startY: currentHelperObj.startPositionTop - (anchorScale/2),
        w: anchorScale,
        h: anchorScale,
        name: 'anchor_top_left',
        type: 0,
    };
    let anchor_top_right = {
        startX: (currentHelperObj.startPositionLeft + currentHelperObj.width) - (anchorScale/2),
        startY: currentHelperObj.startPositionTop - (anchorScale/2),
        w: anchorScale,
        h: anchorScale,
        name: 'anchor_top_right',
        type: 1,
    };
    let anchor_bottom_left = {
        startX: currentHelperObj.startPositionLeft - (anchorScale/2),
        startY: (currentHelperObj.startPositionTop + currentHelperObj.height) - (anchorScale/2),
        w: anchorScale,
        h: anchorScale,
        name: 'anchor_bottom_left',
        type: 2,
    };
    let anchor_bottom_right = {
        startX: (currentHelperObj.startPositionLeft + currentHelperObj.width) - (anchorScale/2),
        startY: (currentHelperObj.startPositionTop + currentHelperObj.height) - (anchorScale/2),
        w: anchorScale,
        h: anchorScale,
        name: 'anchor_bottom_right',
        type: 3,
    };
    if(canvasFinder.inBox(currentX, currentY, anchor_top_left)){
        selectedAnchor = {
            active: anchor_top_left,
            locked: anchor_bottom_right
        }
    }else if(canvasFinder.inBox(currentX, currentY, anchor_top_right)){
        selectedAnchor = {
            active: anchor_top_right,
            locked: anchor_bottom_left
        }
    }else if(canvasFinder.inBox(currentX, currentY, anchor_bottom_left)){
        selectedAnchor = {
            active: anchor_bottom_left,
            locked: anchor_top_right
        }
    }else if(canvasFinder.inBox(currentX, currentY, anchor_bottom_right)){
        selectedAnchor = {
            active: anchor_bottom_right,
            locked: anchor_top_left
        }
    }else{
        selectedAnchor = null;
    }
    return selectedAnchor;
}

const mousedown = function(e){
    // topOffset = e.target.getBoundingClientRect().top;
    console.log(e.target.classList)
    // return;
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
        //check if mouse clicked inside an anchor box
        let anchorClicked = anchorHasClicked(currentHelperObj);
            let rect = {
                startX: currentHelperObj.startPositionLeft,
                startY: currentHelperObj.startPositionTop,
                w: currentHelperObj.currentX - currentHelperObj.startPositionLeft,
                h: currentHelperObj.currentY - currentHelperObj.startPositionTop,
            };
            mouseInsideSelectedRectangle = canvasFinder.inBox(currentX, currentY, rect);
            if(!mouseInsideSelectedRectangle && !anchorClicked){
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
    let {a, d} = this.context.getTransform();
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
const getShapeY = function(mouseY, lockedY, dragoffy){
    if(mouseY > lockedY - 8 && mouseY < lockedY + 8){
        return mouseY;
    }else if(mouseY < lockedY){
        return mouseY + dragoffy
    }else if(mouseY > lockedY){
        return mouseY + dragoffy
    }    
}

const getShapeX = function(mouseX, lockedX, dragoffx){
    if(mouseX > lockedX - 8 && mouseX < lockedX + 8){
        return mouseX;
    }else if(mouseX < lockedX){
        return mouseX + dragoffx
    }else if(mouseX > lockedX){
        return mouseX + dragoffx
    }
}

const getImageY = function(mouseY, lockedY, dragoffy, type){
    if(type === 0 || type === 1){
        if(mouseY > lockedY - 6 && mouseY < lockedY + 6){
            return mouseY;
        }else if(mouseY < lockedY){
            return mouseY + dragoffy
        }else if(mouseY > lockedY){
            return mouseY + dragoffy
        }    
    }else{
        return mouseY;
    }
    
}
const getImageX = function(mouseX, lockedX, dragoffx, type){
    /*0 1
      2 3*/
    if(type === 0 || type === 2){
        if(mouseX > lockedX - 6 && mouseX < lockedX + 6){
            return mouseX;
        }else if(mouseX < lockedX){
            return mouseX + dragoffx
        }else if(mouseX > lockedX){
            return mouseX + dragoffx
        }
    }else{
        return mouseX;
    } 
}

const getImageWidth = function(width, dragoffx, type){
    /*0 1
      2 3*/
    if(type === 0 || type === 2){
        return (width - dragoffx); 
    }else{
        return (width + dragoffx);
    } 
}

const getImageHeight = function(height, dragoffy, type){
    /*0 1
      2 3*/
    if(type === 0 || type === 1){
        return (height - dragoffy);
    }else{
        return (height + dragoffy);
    } 
}

const setRatioOnImage = function(newImagePoints, ratio){
    return{
        x: newImagePoints.x,
        y: newImagePoints.y,
        width: newImagePoints.height / ratio,
        height: newImagePoints.height
    }
}

const getShapeDimensions = function(option, originalPosition, selectedAnchor, dragoffx, dragoffy){
    if(option === 'imageDraw'){
        let keepAspectRation = true; // could be move to store
        let newImagePoints = {
            x: getImageX(originalPosition.mouseX, selectedAnchor.locked.startX + 6 + originalPosition.width, dragoffx, selectedAnchor.active.type),
            y: getImageY(originalPosition.mouseY, selectedAnchor.locked.startY - 98 + originalPosition.height, dragoffy, selectedAnchor.active.type),
            width: getImageWidth(originalPosition.width, dragoffx, selectedAnchor.active.type), 
            height: getImageHeight(originalPosition.height, dragoffy, selectedAnchor.active.type)
        }
        if(keepAspectRation){
            let pointsWithRatioSaved = setRatioOnImage(newImagePoints, originalPosition.aspectRatio)
            return pointsWithRatioSaved;
        }else{
            return newImagePoints
        }
    }else{
        return{
            x: getShapeX(originalPosition.mouseX, selectedAnchor.locked.startX + 6, dragoffx, selectedAnchor.active.type),
            y: getShapeY(originalPosition.mouseY, selectedAnchor.locked.startY - 98, dragoffy, selectedAnchor.active.type),
        }
    }
}

const setNewPoints = function(originalPosition, dragoffx, dragoffy, selectedAnchor, fromUndo){
    let newDragoffx = fromUndo ? dragoffx * -1 : dragoffx;
    let newDragoffy = fromUndo ? dragoffy * -1 : dragoffy;
    if(originalPosition.option === "imageDraw"){
        
        let imageDimensions = getShapeDimensions(originalPosition.option, originalPosition, selectedAnchor, newDragoffx, newDragoffy);
        
        return{
            x: imageDimensions.x,
            y: imageDimensions.y,
            width: imageDimensions.width,
            height: imageDimensions.height
        }
    }else{
        let shapeDimensions = getShapeDimensions(originalPosition.option, originalPosition, selectedAnchor, newDragoffx, newDragoffy);
        
        return{
            x: shapeDimensions.x,
            y: shapeDimensions.y
        }
    }
    
}

const resizeShapes = function(){
    let {a, d} = this.context.getTransform();
    Object.keys(selectedShapes()).forEach(shapeId=>{
        let shape = selectedShapes(shapeId);
        shape.points.forEach((point, index)=>{
            dragoffx =  (currentX - startingMousePosition.x)/a;
            dragoffy =  (currentY - startingMousePosition.y)/d;
            // console.log(`before`, point);
            let newPoints = setNewPoints(startShapes[shapeId].points[index], dragoffx, dragoffy, selectedAnchor);
            if(startShapes[shapeId].points[index].option === "imageDraw"){
                point.width = newPoints.width;
                point.height =  newPoints.height;
            }
            point.mouseX = newPoints.x;
            point.mouseY = newPoints.y;
            // console.log(`after`, point);
        });
    });
    whiteBoardService.redraw(this);
    markShapes.bind(this)();
};

const mousemove = function(e){
    currentX = e.clientX;
    currentY = e.clientY;
    if(this.shouldPaint){
        if(selectedAnchor && (Object.keys(selectedShapes()).length === 1)){
            resizeShapes.bind(this)();
        }else if(mouseInsideSelectedRectangle){
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

const ResizeObjAction = function(ids, dragoffx, dragoffy, selectedAnchor){
    this.ids = ids;
    this.dragoffx = dragoffx;
    this.dragoffy = dragoffy;
    this.selectedAnchor = selectedAnchor;
    
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
        if(!mouseInsideSelectedRectangle  && !selectedAnchor){
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
            if(selectedAnchor){
                let resizeObjAction = new ResizeObjAction(Object.keys(selectedShapes()), dragoffx, dragoffy, selectedAnchor);
                addShape.bind(this, "resize", resizeObjAction)();
                resetItems();
            }else{
                // object was just moved
                let moveAction = new MoveObjAction(Object.keys(selectedShapes()), dragoffx, dragoffy);
                addShape.bind(this, "move", moveAction)();
                resetItems();
            }
            
        }
    }
};



const mouseup = function(e){
    defineEndPosition.bind(this, e)();
};
const mouseleave = function(e){
    defineEndPosition.bind(this, e)();
};


const deleteSelectedShape = function(){
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
    setNewPoints,
}