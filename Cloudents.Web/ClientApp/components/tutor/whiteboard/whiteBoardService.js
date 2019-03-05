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
    if(Object.keys(canvasData.context).length === 0){
        canvasData.context = getContext();
    }
    cleanCanvas(canvasData.context);
    let previouseDrawingObj = null;
    canvasData.dragData.forEach((shape)=>{
        if(shape.isGhost) return;
        if(!shape.visible) return;
        shape.points.forEach(dragObj=>{
            previouseDrawingObj = dragObj.eventName === 'start' ? dragObj : previouseDrawingObj;
            optionsEnum[shape.type].draw.bind(canvasData, dragObj, true, previouseDrawingObj)();
        })
        
    })
}

const undo = function(canvasData){
    if(canvasData.dragData.length > 0){
        let lastAction = canvasData.dragData.pop();
        if(lastAction.isGhost){
            console.log("ghost action")
            Object.keys(lastAction.shapesObj).forEach((shapeId)=>{
                canvasData.dragData.forEach((currentShape, index)=>{
                    //replace shape
                    if(currentShape.id === shapeId){
                        canvasData.dragData[index] = createShape(lastAction.shapesObj[shapeId])
                        helper.hideHelper();
                        helper.resetHelperObj();
                    }
                })
            })
        }
        redraw(canvasData);
    }
}

const cleanCanvas = function(ctx){
    ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height); // Clears the canvas
}

export default {
    init,
    undo,
    redraw
}
