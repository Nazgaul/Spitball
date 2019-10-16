import store from '../../../store/index.js'
import liveDraw from './options/liveDraw'
import lineDraw from './options/lineDraw'
import drawEllipse from './options/drawEllipse'
import drawRectangle from './options/drawRectangle'
import imageDraw from './options/imageDraw'
import eraser from './options/eraser'
import textDraw from './options/textDraw'
import equationDraw from './options/equationDraw'
import iink from './options/iink'
import panTool from './options/panTool'
import selectShape from './options/selectShape'
import {createShape} from './utils/factories'
import helper from './utils/helper'

let dragData = [];

const getDragData = function(){
    return store.getters['getDragData'];
};

const optionsEnum = {
    liveDraw,
    lineDraw,
    drawEllipse,
    drawRectangle,
    imageDraw,
    eraser,
    selectShape,
    textDraw,
    panTool,
    equationDraw,
    iink
};

const ghostMoveData = function(actionObj, fromUndo){
    dragData = store.getters['getDragData'];
    dragData.forEach(shape=>{
        if(actionObj.ids.indexOf(shape.id) > -1){
            shape.points.forEach(point=>{
                point.mouseX = fromUndo ? point.mouseX - actionObj.distanceX : point.mouseX + actionObj.distanceX;
                point.mouseY = fromUndo ? point.mouseY - actionObj.distanceY : point.mouseY + actionObj.distanceY;
            });
        }
    });
};

const ghostDeleteData = function(actionObj, fromUndo){
    dragData = store.getters['getDragData'];
    dragData.forEach(shape=>{
        if(actionObj.ids.indexOf(shape.id) > -1){
            shape.visible = fromUndo ? true : false;
        }
    });
};

const ghostChangeText = function(actionObj, fromUndo){
    dragData = store.getters['getDragData'];
    dragData.forEach(shape=>{
        if(actionObj.id.indexOf(shape.id) > -1){
            shape.points[0].text = fromUndo ? actionObj.oldText : actionObj.newText;
            if(!!actionObj.oldWidth){
                shape.points[0].width = fromUndo ? actionObj.oldWidth : actionObj.newWidth;
            }
        }
    });
};

const ghostByAction = {
    move: ghostMoveData,
    delete: ghostDeleteData,
    changeText: ghostChangeText
};

const init = function(optionName){
    if(!optionsEnum[optionName]){
        console.log(`no such options ${optionName}`);
        return null;
    }
    if(optionsEnum[optionName].init){
        optionsEnum[optionName].init.bind(this)();
    }
    return optionsEnum[optionName];
};

const getContext = function(){
    let canvas = document.getElementById('canvas');
    return canvas.getContext("2d");
};

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
        });

    });
    if( store.getters['selectedOptionString'] === 'selectShape'){
        selectShape.reMarkSelectedShapes.bind(canvasData)();
    }
};

const undo = function(canvasData, tab){
    let undoTab = !!tab ? tab.id : store.getters['getCurrentSelectedTab'].id;
    dragData = store.getters['getAllDragData'][undoTab];
    if(dragData.length > 0){
        store.dispatch('popDragData', undoTab).then((lastAction)=>{
            if(lastAction.isGhost){
                ghostByAction[lastAction.actionType](lastAction.actionObj, true);
                hideHelper();
            }
            redraw(canvasData);
        });
    }
};

const uploadImage = function(data){
    return store.dispatch('uploadImage', data);
};

const hideHelper = function(){
    helper.hideHelper();
    helper.resetHelperObj(); 
};

const cleanCanvas = function(ctx){
    let p1 = ctx.transformedPoint(0,0);
    let p2 = ctx.transformedPoint(ctx.canvas.width, ctx.canvas.height);
    ctx.clearRect(p1.x,p1.y,p2.x-p1.x,p2.y-p1.y);
    ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height); // Clears the canvas
};

const passData = function(canvasData, changedDragData){
    if(changedDragData.isGhost){
        ghostByAction[changedDragData.actionType](changedDragData.actionObj);
    }
    let tabToDraw = canvasData.tab;
    let data = {
        tab: tabToDraw,
        data: changedDragData
    };
    store.dispatch('updateDragData', data);
    redraw(canvasData);
};

const clearData = function(canvasData, tab){
    store.dispatch('resetDragData', tab);
    redraw(canvasData);
};

export default {
    init,
    undo,
    redraw,
    getDragData,
    uploadImage,
    passData,
    hideHelper,
    getContext,
    clearData
}
