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

const ghostMoveData = function(actionObj, fromUndo){
    dragData = store.getters['getDragData'];
    dragData.forEach(shape=>{
        if(actionObj.ids.indexOf(shape.id) > -1){
            shape.points.forEach(point=>{
                point.mouseX = fromUndo ? point.mouseX - actionObj.distanceX : point.mouseX + actionObj.distanceX;
                point.mouseY = fromUndo ? point.mouseY - actionObj.distanceY : point.mouseY + actionObj.distanceY;
            })
        }
    })
}

const ghostDeleteData = function(actionObj, fromUndo){
    dragData = store.getters['getDragData'];
    dragData.forEach(shape=>{
        if(actionObj.ids.indexOf(shape.id) > -1){
            shape.visible = fromUndo ? true : false;
        }
    })
}

const ghostChangeText = function(actionObj, fromUndo){
    dragData = store.getters['getDragData'];
    dragData.forEach(shape=>{
        if(actionObj.id.indexOf(shape.id) > -1){
            shape.points[0].text = fromUndo ? actionObj.oldText : actionObj.newText;
            shape.points[0].width = fromUndo ? actionObj.oldWidth : actionObj.newWidth;
        }
    })
}

const ghostByAction = {
    move: ghostMoveData,
    delete: ghostDeleteData,
    changeText: ghostChangeText
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
                ghostByAction[lastAction.actionType](lastAction.actionObj, true)  
                helper.hideHelper();
                helper.resetHelperObj(); 
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
        ghostByAction[changedDragData.actionType](changedDragData.actionObj)        
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
