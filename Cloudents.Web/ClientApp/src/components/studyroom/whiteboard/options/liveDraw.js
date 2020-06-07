import {createPointsByOption, createShape} from '../utils/factories'
import canvasFinder from '../utils/canvasFinder';

const optionType = 'liveDraw';

let localShape = createShape({
    type: optionType,
    points: []
});

const clearLocalShape = function(){
    localShape = createShape({
        type: optionType,
        points: []
    });
};
const init = function(){
    
};
const draw = function(dragObj){
    //determin the stroke color
    this.context.strokeStyle = dragObj.strokeStyle;
    let prevouseDragObj = this.metaData.previouseDrawingPosition;
    //draw
    if(dragObj.isDragging && !!prevouseDragObj){
        this.context.moveTo(prevouseDragObj.mouseX, prevouseDragObj.mouseY);
    }else{
        this.context.moveTo(dragObj.mouseX-1, dragObj.mouseY);
    }
    this.context.lineTo(dragObj.mouseX, dragObj.mouseY);
    this.metaData.previouseDrawingPosition = dragObj;
};
const liveDraw = function(dragObj){
    this.context.beginPath();
    draw.bind(this, dragObj)();
    this.context.closePath();
    this.context.stroke();
    
};

const mousedown = function(e){
    //Set Click Position
    let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, e.pageX - e.target.offsetLeft - e.target.getBoundingClientRect().left, e.pageY - e.target.getBoundingClientRect().top);
    this.methods.hideColorPicker();
    
    let dragObj = createPointsByOption({
        mouseX,
        mouseY,
        isDragging: false,
        strokeStyle: this.color.hex,
        option: optionType,
        eventName: 'start'
    });

    this.shouldPaint = true;
    localShape.points.push(dragObj);
    liveDraw.bind(this, dragObj)();
};
const mousemove = function(e){
    if(this.shouldPaint){
        let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, e.pageX - e.target.offsetLeft - e.target.getBoundingClientRect().left, e.pageY - e.target.getBoundingClientRect().top);
        let dragObj = createPointsByOption({
            mouseX,
            mouseY,
            isDragging: true,
            strokeStyle: this.color.hex,
            option: optionType,
            eventName: 'move'
        });
        localShape.points.push(dragObj);
        liveDraw.bind(this, dragObj)();
    }
};

const defineEndPosition = function(){
    if(this.shouldPaint){
        this.methods.addShape(localShape, clearLocalShape);
    }
    this.shouldPaint = false;
};


const mouseup = function(e){
    defineEndPosition.bind(this, e)();
};

const mouseleave = function(e){
    defineEndPosition.bind(this, e)();
};

export default{
    mousedown,
    mouseup,
    mousemove,
    mouseleave,
    draw: liveDraw,
    init
}