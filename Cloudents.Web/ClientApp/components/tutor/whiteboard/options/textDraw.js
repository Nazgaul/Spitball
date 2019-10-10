import {createPointsByOption, createShape, createGhostShape, createGuid} from '../utils/factories'
import helper from '../utils/helper'
import whiteBoardService from '../whiteBoardService'
import store from '../../../../store';

const optionType = 'textDraw';

let localShape = createShape({
    type: optionType,
    points: [],
    id: null
});

const clearLocalShape = function(){
    localShape = createShape({
        type: optionType,
        points: [],
        id: null
    });
};

const startingMousePosition = {
    x:null,
    y:null
};

const yOffset = 25;

let isWriting = false;
let currentId = null;
let startShapes = {};

const init = function(){
    currentId = null;
    isWriting = false;
    startShapes = {};
};
const draw = function(textObj){
    //determin the stroke color
    this.context.fillStyle = textObj.color;
    this.context.font = `${textObj.height}px ${textObj.fontFamily}`;
    this.context.fillText(textObj.text, textObj.mouseX, textObj.mouseY);
};
const liveDraw = function(textObj){
    draw.bind(this, textObj)();
};

const hideHelperObj = function(){
    currentId = null;
    whiteBoardService.hideHelper();
};

const setHelperObj = function(e, selectedHelper){
    let popupSize = 172;
    let currentX = (window.innerWidth / 2) - (popupSize / 2);
    let currentY = (window.innerHeight / 3);
    let helperObj = {
        currentX,
        currentY,
        color: this.color.hex,
        text: selectedHelper ? selectedHelper.text : '',
        id: currentId
    };
    helper.setTextShape(helperObj);
    helper.showHelper();
};

const changeTextActionObj = function(id, oldShapePoint, newShapePoint){
    this.id = id;
    this.oldText = oldShapePoint.text;
    this.oldWidth = oldShapePoint.width;
    this.newText = newShapePoint.text;
    this.newWidth = newShapePoint.width;
};

const addGhostLocalShape = function(actionType, actionObj){
    let ghostLocalShape = createGhostShape({
        type: optionType,
        actionType: actionType, // changeText
        actionObj: actionObj
    });
    this.methods.addShape(ghostLocalShape);
    startShapes = {};
};

const enterPressed = function(e){
    if(isWriting){
        mousedown.bind(this, e)();
    }
};

const moveToSelectTool = function(){
    this.methods.selectDefaultTool();
};

const mousedown = function(e){
    this.methods.hideColorPicker();
    if(isWriting){
        console.log('if 1');
        isWriting = false;
        //here the user finished to write text
        let text = document.getElementsByClassName(currentId)[0];
        if(!!text.value){
            // if(!isEditing){
                this.context.font = `30px serif`;
                let meassureText = this.context.measureText(text.value);
                let textObj = createPointsByOption({
                    mouseX: (window.innerWidth / 2) - (meassureText.width / 2),
                    mouseY: window.innerHeight / 3.5,
                    yOffset: yOffset,
                    width: meassureText.width,
                    height: 30,
                    fontFamily: 'serif',
                    color: this.color.hex,
                    option: optionType,
                    eventName: 'start',
                    id: currentId,
                    text: text.value
                });
                localShape.id = textObj.id;
                localShape.points.push(textObj);
                //draw
                liveDraw.bind(this, textObj)();
                this.methods.addShape(localShape, clearLocalShape);   
        }else{
            this.methods.addShape(null, clearLocalShape);
        }
        hideHelperObj();
    }else{
        console.log('else 2 ');
        //STARTING POINT - SET INPUT ELEMENT POSITION (local)
        isWriting = true;
        let popupSize = 172;
        startingMousePosition.x = (window.innerWidth / 2) - (popupSize / 2);;
        startingMousePosition.y = (window.innerHeight / 3);
        currentId = createGuid('text');
        setHelperObj.bind(this, e)();
        
        setTimeout(()=>{
            let textElm = document.getElementsByClassName(currentId)[0];
            textElm.focus();
        });
    }
};
const mousemove = function(e){
};

const defineEndPosition = function(e){
};


const mouseup = function(e){
    defineEndPosition.bind(this, e)();
};

const mouseleave = function(e){
    console.log('mouseLeave');
    defineEndPosition.bind(this, e)();
};

export default{
    mousedown,
    mouseup,
    mousemove,
    mouseleave,
    draw: liveDraw,
    init,
    enterPressed
}