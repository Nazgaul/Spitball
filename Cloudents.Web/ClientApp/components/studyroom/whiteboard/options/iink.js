import {createPointsByOption, createShape, createGhostShape, createGuid} from '../utils/factories'
import canvasFinder from '../utils/canvasFinder'
import helper from '../utils/helper'
import whiteBoardService from '../whiteBoardService'

const OPTION_TYPE = 'iink';

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

let imageCache = {};


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

const sizeProportion = 2;

const getImageDimensions = function(text, id){
   return new Promise(function(resolve){
    MathJax.AuthorInit(`$$${text}$$`, (output)=>{
        var DOMURL = window.URL || window.webkitURL || window;     
        let img = new Image();
        var svg = new Blob([output.svg], {type: 'image/svg+xml'});
        var url = DOMURL.createObjectURL(svg);
        img.onload = function() {
            let imgObj = {
                img,
                text
            }
           imageCache[id] = imgObj;
           resolve({width: img.width, height:img.height}); 
        }
        img.src = url;
    });
   }) 
}


const drawContext = function(svgText, textObj){
    var DOMURL = window.URL || window.webkitURL || window;     
    let img = new Image();
    let self = this; 
    var svg = new Blob([svgText.svg], {type: 'image/svg+xml'});
     var url = DOMURL.createObjectURL(svg);
     img.onload = function() {
        let imgObj = {
            img,
            text: textObj.text
        }
        imageCache[textObj.id] = imgObj;
        self.context.drawImage(img, textObj.mouseX, textObj.mouseY, img.width*sizeProportion, img.height*sizeProportion);
        DOMURL.revokeObjectURL(url);
     }
     img.src = url;
}

const draw = function(textObj){
    //determin the stroke color
    this.context.fillStyle = textObj.color;
    this.context.font = `${textObj.height}px ${textObj.fontFamily}`;
    let img = imageCache[textObj.id];
    //create svg with the MathJax object out from the text value
    if(!!img && img.text === textObj.text){
        this.context.drawImage(img.img, textObj.mouseX, textObj.mouseY, img.img.width*sizeProportion, img.img.height*sizeProportion);
    }else{
        MathJax.AuthorInit(`$$${textObj.text}$$`, (output)=>{  
            drawContext.bind(this, output, textObj)();
        });
    }
    
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
    let currentY = selectedHelper ? selectedHelper.mouseY : e.clientY;
    let helperObj = {
        currentX,
        currentY,
        color: this.color.hex,
        text: selectedHelper ? selectedHelper.text : '',
        id: currentId
    }
    helper.setEquationShape(helperObj);
    helper.showHelper();
}

const changeTextActionObj = function(id, oldShapePoint, newShapePoint){
    this.id = id;
    this.oldText = oldShapePoint.text;
    this.newText = newShapePoint.text;
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
                const instancedId = currentId + "";
                let meassureText = this.context.measureText(text.value);
                getImageDimensions(text.value, instancedId).then(dimensions=>{
                    let textObj = createPointsByOption({
                        mouseX: (window.innerWidth / 2) - (meassureText.width / 2),
                        mouseY: window.innerHeight / 3.5,
                        width: dimensions.width*sizeProportion,
                        height: dimensions.height*sizeProportion,
                        color: this.color.hex,
                        option: OPTION_TYPE,
                        eventName: 'start',
                        id: instancedId,
                        text: text.value
                    })
                    localShape.id = textObj.id;
                    localShape.points.push(textObj);
                    //draw
                    liveDraw.bind(this, textObj)();
                    this.methods.addShape(localShape, clearLocalShape);
                    whiteBoardService.redraw(this);
                })
                
            }else{
                isEditing = false;
                currentShapeEditing.points[0].text = text.value;
                getImageDimensions(text.value, currentShapeEditing.id).then(dimensions=>{
                    currentShapeEditing.points[0].width = dimensions.width;
                    currentShapeEditing.points[0].height = dimensions.height;
                    let textGhostObj = new changeTextActionObj(currentShapeEditing.id, startShapes.points[0], currentShapeEditing.points[0]);
                    addGhostLocalShape.bind(this, "changeText", textGhostObj)();
                    whiteBoardService.redraw(this);
                    moveToSelectTool.bind(this)();
                })
            }
        }
        this.methods.addShape(null, clearLocalShape);
        hideHelperObj();
    }else{
        //here the user statring to write text
        //Set Click Position
        isWriting = true;
        // let mouseX = e.pageX - e.target.offsetLeft;
        // let mouseY = e.pageY - e.target.getBoundingClientRect().top;
        // let hasShape = canvasFinder.getShapeByPoint(mouseX, mouseY, this, whiteBoardService.getDragData());
        let hasShape = {};
        if(Object.keys(hasShape).length > 0){
            let prop = Object.keys(hasShape)[0];
            if(hasShape[prop].type === "iink"){
                startingMousePosition.x = hasShape[prop].points[0].mouseX;
                startingMousePosition.y = hasShape[prop].points[0].mouseY;
                currentShapeEditing = hasShape[prop];
                isEditing = true;
                currentId = hasShape[prop].id;
                setHelperObj.bind(this, e, hasShape[prop].points[0])();
                startShapes = createShape(hasShape[prop]);
                
            }else{
                let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, e.pageX - e.target.offsetLeft - e.target.getBoundingClientRect().left, e.pageY - e.target.getBoundingClientRect().top);
                startingMousePosition.x = mouseX;
                startingMousePosition.y = mouseY;
                currentId = createGuid('iink');
                setHelperObj.bind(this, e)();
            }
        }else{
            let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(this.context, e.pageX - e.target.offsetLeft - e.target.getBoundingClientRect().left, e.pageY - e.target.getBoundingClientRect().top);
            startingMousePosition.x = mouseX;
            startingMousePosition.y = mouseY;
            currentId = createGuid('iink');
            setHelperObj.bind(this, e)();
        }
        setTimeout(()=>{
            let textElm = document.getElementsByClassName(currentId)[0];
            textElm.focus();
        })
    }
}
const mousemove = function(){
}

const defineEndPosition = function(){
}



const mouseup = function(e){
    console.log('mouseUp')
    console.log('mouseupmouseup')
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