import {createPointsByOption, createShape, createGhostShape, createGuid} from '../utils/factories'
import canvasFinder from '../utils/canvasFinder'
import helper from '../utils/helper'
import whiteBoardService from '../whiteBoardService'

const OPTION_TYPE = 'textDraw';

let localShape = createShape({
    type: OPTION_TYPE,
    points: [],
    id: null
});

const clearLocalShape = function(){
    localShape = createShape({
        type: OPTION_TYPE,
        points: [],
        id: null
    });
}

let ghostLocalShape = createGhostShape({
    type: OPTION_TYPE,
    shapesObj: {}
});

const ghostClearLocalShape = function(){
    localShape = createGhostShape({
        type: OPTION_TYPE,
        shapesObj: {}
    });
}

const startingMousePosition = {
    x:null,
    y:null
}

const yOffset = 12;

let isWriting = false;
let isEditing = false;
let currentId = null;

const init = function(){
    currentId = null;
    isWriting = false;
    isEditing = false;
}
const draw = function(textObj){
    //determin the stroke color
    this.context.fillStyle = textObj.color;
    this.context.font = `${textObj.height}px ${textObj.fontFamily}`
    this.context.fillText(textObj.text, textObj.mouseX, textObj.mouseY);
}
const liveDraw = function(textObj){
    draw.bind(this, textObj)();
}

const hideHelperObj = function(){
    currentId = null;
    isEditing = false;
    helper.hideHelper();
    helper.resetHelperObj();
}

const setHelperObj = function(e, selectedHelper){
    let currentX = selectedHelper ? selectedHelper.mouseX + e.target.offsetLeft : e.clientX;
    let currentY = selectedHelper ? selectedHelper.mouseY + e.target.offsetTop : e.clientY;
    let helperObj = {
        currentX,
        currentY,
        color: this.color.hex,
        text: selectedHelper ? selectedHelper.text : '',
        id: currentId
    }
    helper.setTextShape(helperObj);
    helper.showHelper();
}

const addGhostLocalShape = function(){
    ghostLocalShape.shapesObj = startShapes;
    this.methods.addShape(ghostLocalShape, ghostClearLocalShape);
    startShapes = {};
}

const mousedown = function(e){
    this.methods.hideColorPicker();
    this.shouldPaint = true;
    if(isWriting){
        isWriting = false;
        //here the user finished to write text
        let text = document.getElementsByClassName(currentId)[0];
        if(!!text.value){
            this.context.font = `17px serif`
            let meassureText = this.context.measureText(text.value);
            let textObj = createPointsByOption({
                mouseX: startingMousePosition.x,
                mouseY: startingMousePosition.y + yOffset,
                yOffset: yOffset,
                width: meassureText.width,
                height: 17,
                fontFamily: 'serif',
                color: this.color.hex,
                option: OPTION_TYPE,
                eventName: 'start',
                id: currentId,
                text: text.value
            })
            localShape.id = textObj.id;
            localShape.points.push(textObj);
            //draw
            liveDraw.bind(this, textObj)();
            this.methods.addShape(localShape, clearLocalShape);
            whiteBoardService.cleanCanvas(this.context);
            whiteBoardService.redraw(this);
        }
        hideHelperObj();
    }else{
        //here the user statring to write text
        //Set Click Position
        isWriting = true;
        let mouseX = e.pageX - e.target.offsetLeft;
        let mouseY = e.pageY - e.target.offsetTop;
        let hasShape = canvasFinder.getShapeByPoint(mouseX, mouseY, this);
        if(Object.keys(hasShape).length > 0){
            let prop = Object.keys(hasShape)[0];
            if(hasShape[prop].type === "textDraw"){
                startingMousePosition.x = hasShape[prop].points[0].mouseX;
                startingMousePosition.y = hasShape[prop].points[0].mouseY - yOffset;
                isEditing = true;
                currentId = hasShape[prop].id;
                setHelperObj.bind(this, e, hasShape[prop].points[0])();
                hasShape[prop].visible = false;
            }else{
                currentId = createGuid('text');
                setHelperObj.bind(this, e)();
            }
        }else{
            startingMousePosition.x = e.pageX - e.target.offsetLeft;
            startingMousePosition.y = e.pageY - e.target.offsetTop;
            currentId = createGuid('text');
            setHelperObj.bind(this, e)();
        }
        setTimeout(()=>{
            let textElm = document.getElementsByClassName(currentId)[0];
            textElm.focus();
        })
    }
}
const mousemove = function(e){
    this.shouldPaint = false;
}

const defineEndPosition = function(e){
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