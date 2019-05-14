import store from '../../../../store/index.js'
import {createPointsByOption, createShape} from '../utils/factories'
import canvasFinder from '../utils/canvasFinder';
import whiteBoardService from '../whiteBoardService'
import selectShape from './selectShape';

const OPTION_TYPE = 'eraser';

let localShape = createShape({
    type: OPTION_TYPE,
    points: []
});

const clearLocalShape = function(){
    localShape = createShape({
        type: OPTION_TYPE,
        points: []
    });
}

let wrapperElm = null;
let lockProtection = false;
let currentX = null;
let currentY = null;

const init = function(){
    wrapperElm = document.getElementById('canvas-wrapper');
}
const liveDraw = function(dragObj){
}

const setSelectedShapes = function(shape){
    if(Object.keys(shape).length > 0){
        Object.keys(shape).forEach(shapeId=>{
            store.dispatch('setShapesSelected', shape[shapeId]);
        })
    }else{
        store.dispatch('clearShapesSelected');
    }
}

const selectedShapes = function(id){
    if(id){
        return store.getters['getShapesSelected'][id];
    }else{
        return store.getters['getShapesSelected'];
    }
    
}

const mousedown = function(e){
    //Set Click Position
    this.methods.hideColorPicker();
    this.shouldPaint = true;
}

const mousemove = function(e){
    currentX = e.clientX;
    currentY = e.clientY;
    if(this.shouldPaint){
        //mark selected shape
        let scrollLeft = wrapperElm.scrollLeft;
        let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, (currentX - e.target.offsetLeft) + scrollLeft, (currentY - e.target.getBoundingClientRect().top));
        setSelectedShapes(canvasFinder.getShapeByPoint(mouseX, mouseY, this, whiteBoardService.getDragData()));
        if(Object.keys(selectedShapes()).length > 0){
            selectShape.deleteSelectedShape.bind(this)()
        }        
    }
}

const defineEndPosition = function(e){
    this.shouldPaint = false;
}

const mouseup = function(e){
    console.log('mouseUp')
    defineEndPosition.bind(this, e)()
}

const mouseleave = function(e){
    console.log('mouseLeave')
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