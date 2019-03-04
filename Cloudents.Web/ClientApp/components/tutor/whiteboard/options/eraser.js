import {createPointsByOption, createShape} from '../utils/factories'

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

const init = function(){
    
}

const draw = function(dragObj){
    //determin the stroke color
    this.context.globalCompositeOperation = "destination-out";  
    let prevouseDragObj = this.metaData.previouseDrawingPosition;
    //draw
    if(dragObj.isDragging && !!prevouseDragObj){
        this.context.moveTo(prevouseDragObj.mouseX, prevouseDragObj.mouseY);
    }else{
        this.context.moveTo(dragObj.mouseX-1, dragObj.mouseY);
    }
    this.context.lineTo(dragObj.mouseX, dragObj.mouseY);
    this.metaData.previouseDrawingPosition = dragObj;
}
const liveDraw = function(dragObj){
    this.context.beginPath();
    draw.bind(this, dragObj)();
    this.context.closePath();
    this.context.stroke();
    this.context.globalCompositeOperation = "source-over"; 
}

const mousedown = function(e){
    //Set Click Position
    let mouseX = e.pageX - e.target.offsetLeft;
    let mouseY = e.pageY - e.target.getBoundingClientRect().top;
    this.methods.hideColorPicker();

    let dragObj = createPointsByOption({
        mouseX,
        mouseY,
        isDragging: false,
        strokeStyle: "rgb(255, 255, 255)",
        option: OPTION_TYPE,
        eventName: 'start'
    })

    this.shouldPaint = true;
    localShape.points.push(dragObj);
    liveDraw.bind(this, dragObj)();
}
const mousemove = function(e){
    if(this.shouldPaint){
        let mouseX = e.pageX - e.target.offsetLeft;
        let mouseY = e.pageY - e.target.getBoundingClientRect().top;
        let dragObj = createPointsByOption({
            mouseX,
            mouseY,
            isDragging: true,
            strokeStyle: "rgb(255, 255, 255)",
            option: OPTION_TYPE,
            eventName: 'end'
        })
        localShape.points.push(dragObj);
        liveDraw.bind(this, dragObj)();
    }
}

const defineEndPosition = function(e){
    if(this.shouldPaint){
        this.methods.addShape(localShape, clearLocalShape);
    }
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