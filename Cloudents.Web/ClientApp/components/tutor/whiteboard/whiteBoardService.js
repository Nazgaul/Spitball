import liveDraw from './options/liveDraw'
import lineDraw from './options/lineDraw'
import drawEllipse from './options/drawEllipse'
import drawRectangle from './options/drawRectangle'
import imageDraw from './options/imageDraw'
import eraser from './options/eraser'
import textDraw from './options/textDraw'
import selectShape from './options/selectShape'
import {createShape} from './utils/factories'
import helper from './utils/helper'
import store from '../../../store/index'

let dragData = store.getters['getDragData'];

const getDragData = function(){
    return store.getters['getDragData'];
}

const optionsEnum = {
    liveDraw,
    lineDraw,
    drawEllipse,
    drawRectangle,
    imageDraw,
    eraser,
    selectShape,
    textDraw
}

const init = function(optionName){
    if(!optionsEnum[optionName]){
        console.log(`no such options ${optionName}`);
        return null;
    }
    if(optionsEnum[optionName].init){
        optionsEnum[optionName].init.bind(this)();
    }
    return optionsEnum[optionName];
}

const getContext = function(){
    let canvas = document.getElementById('canvas');
    return canvas.getContext("2d");
}

const redraw = function(canvasData){
    dragData = store.getters['getDragData'];
    if(Object.keys(canvasData.context).length === 0){
        canvasData.context = getContext();
    }
    cleanCanvas(canvasData.context);
    let previouseDrawingObj = null;
    dragData.forEach((shape)=>{
        if(shape.isGhost) return;
        if(!shape.visible) return;
        shape.points.forEach(dragObj=>{
            previouseDrawingObj = dragObj.eventName === 'start' ? dragObj : previouseDrawingObj;
            optionsEnum[shape.type].draw.bind(canvasData, dragObj, true, previouseDrawingObj)();
        })
        
    })
}

const undo = function(canvasData){
    dragData = store.getters['getDragData'];
    if(dragData.length > 0){
        store.dispatch('popDragData').then((lastAction)=>{
            if(lastAction.isGhost){
                console.log("ghost action")
                Object.keys(lastAction.shapesObj).forEach((shapeId)=>{
                    dragData.forEach((currentShape, index)=>{
                        //replace shape
                        if(currentShape.id === shapeId){
                            dragData[index] = createShape(lastAction.shapesObj[shapeId])
                            helper.hideHelper();
                            helper.resetHelperObj();
                        }
                    })
                })
            }
            redraw(canvasData);
        });
    }
}

const uploadImage = function(data){
    return store.dispatch('uploadImage', data);
}

const cleanCanvas = function(ctx){
    ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height); // Clears the canvas
}

const passData = function(canvasData, changedDragData){
    if(changedDragData.isGhost){
        dragData = store.getters['getDragData'];
        Object.keys(changedDragData.newShapes).forEach((newShapeId)=>{
            dragData.forEach((currentShape, index)=>{
                //replace shape
                if(currentShape.id === newShapeId){
                    dragData[index] = createShape(changedDragData.newShapes[newShapeId])
                }
            })
        })
    }
    store.dispatch('updateDragData', changedDragData)
    redraw(canvasData);
}

export default {
    init,
    undo,
    redraw,
    getDragData,
    uploadImage,
    passData
}
