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

const startingMousePosition = {
    x:null,
    y:null
}

const yOffset = 12;

let isWriting = false;
let isEditing = false;
let currentId = null;
let startShapes = {};
let currentShapeEditing = null;

const init = function(){
    currentId = null;
    isWriting = false;
    isEditing = false;
    startShapes = {};
    currentShapeEditing = null;
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
    whiteBoardService.hideHelper();
}

const setHelperObj = function(e, selectedHelper){
    let currentX = selectedHelper ? selectedHelper.mouseX + e.target.offsetLeft : e.clientX;
    let currentY = selectedHelper ? selectedHelper.mouseY : e.clientY - e.target.getBoundingClientRect().top;
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

const changeTextActionObj = function(id, oldShapePoint, newShapePoint){
    this.id = id;
    this.oldText = oldShapePoint.text;
    this.oldWidth = oldShapePoint.width;
    this.newText = newShapePoint.text;
    this.newWidth = newShapePoint.width;
}

const addGhostLocalShape = function(actionType, actionObj){
    let ghostLocalShape = createGhostShape({
        type: OPTION_TYPE,
        actionType: actionType, // changeText
        actionObj: actionObj
    });
    this.methods.addShape(ghostLocalShape);
    startShapes = {};
}

const enterPressed = function(e){
    if(isWriting){
        mousedown.bind(this, e)();
    }
}

const moveToSelectTool = function(){
    this.methods.selectDefaultTool();
}

const mousedown = function(e){
    this.methods.hideColorPicker();
    if(isWriting){
        isWriting = false;
        //here the user finished to write text
        let text = document.getElementsByClassName(currentId)[0];
        if(!!text.value){
            if(!isEditing){
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
                whiteBoardService.redraw(this);
            }else{
                isEditing = false;
                let meassureText = this.context.measureText(text.value);
                currentShapeEditing.points[0].text = text.value;
                currentShapeEditing.points[0].width = meassureText.width;
                let textGhostObj = new changeTextActionObj(currentShapeEditing.id, startShapes.points[0], currentShapeEditing.points[0]);
                addGhostLocalShape.bind(this, "changeText", textGhostObj)();
                whiteBoardService.redraw(this);
                moveToSelectTool.bind(this)();
            }
        }
        hideHelperObj();
    }else{
        //here the user statring to write text
        //Set Click Position
        isWriting = true;
        let mouseX = e.pageX - e.target.offsetLeft;
        let mouseY = e.pageY - e.target.getBoundingClientRect().top;
        let hasShape = canvasFinder.getShapeByPoint(mouseX, mouseY, this, whiteBoardService.getDragData());
        if(Object.keys(hasShape).length > 0){
            let prop = Object.keys(hasShape)[0];
            if(hasShape[prop].type === "textDraw"){
                startingMousePosition.x = hasShape[prop].points[0].mouseX;
                startingMousePosition.y = hasShape[prop].points[0].mouseY - yOffset;
                currentShapeEditing = hasShape[prop];
                isEditing = true;
                currentId = hasShape[prop].id;
                setHelperObj.bind(this, e, hasShape[prop].points[0])();
                startShapes = createShape(hasShape[prop]);
                
            }else{
                let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, e.pageX - e.target.offsetLeft, e.pageY - e.target.getBoundingClientRect().top);
                startingMousePosition.x = mouseX;
                startingMousePosition.y = mouseY;
                currentId = createGuid('text');
                setHelperObj.bind(this, e)();
            }
        }else{
            let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, e.pageX - e.target.offsetLeft, e.pageY - e.target.getBoundingClientRect().top);
            startingMousePosition.x = mouseX;
            startingMousePosition.y = mouseY;
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
    init,
    enterPressed
}