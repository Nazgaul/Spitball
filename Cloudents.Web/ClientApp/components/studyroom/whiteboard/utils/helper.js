import store from "../../../../store";

const HelperObj = {
    isActive: false,
    style:{},
    cssClass: "",
    type: ""
};

const resetHelperObj = function(){
    HelperObj.isActive = false;
    HelperObj.style = {};
    HelperObj.cssClass = "";
    HelperObj.type = "";
};

const showHelper = function(){
    HelperObj.isActive = true;
};

const hideHelper = function(){
    HelperObj.isActive = false;
    store.dispatch('clearShapesSelected');
};

const setRectangleShape = function(helperObj){
    let correctedWidth = helperObj.width < 0 ? helperObj.width * -1 : helperObj.width;
    let correctedHeight = helperObj.height < 0 ? helperObj.height * -1 : helperObj.height;
    let shouldFlipHorizontal = helperObj.width < 0;
    let shouldFlipVertical = helperObj.height < 0;
    helperObj.type = "rect";
    HelperObj.style = {
        y: `${shouldFlipVertical ? helperObj.currentY : helperObj.startPositionTop}px`,
        x: `${shouldFlipHorizontal ? helperObj.currentX : helperObj.startPositionLeft}px`,
        width: `${correctedWidth}px`,
        height: `${correctedHeight}px`,
        stroke: `${helperObj.strokeStyle}`
    };
    HelperObj.cssClass = "rectangular-helper";
};


const setLineShape = function(helperObj){
    HelperObj.style = {
        y1: `${helperObj.startPositionTop}px`,
        x1: `${helperObj.startPositionLeft}px`,
        y2: `${helperObj.currentY}px`,
        x2: `${helperObj.currentX}px`,
        stroke: `${helperObj.strokeStyle}`
    };
    HelperObj.cssClass = "line-helper";
};

const setEllipseShape = function(helperObj){
    let correctedRx = helperObj.rx < 0 ? helperObj.rx * -1 : helperObj.rx;
    let correctedRy = helperObj.ry < 0 ? helperObj.ry * -1 : helperObj.ry;
    HelperObj.style = {
        cx: `${helperObj.cx}`,
        cy: `${helperObj.cy}`,
        rx: `${correctedRx}`,
        ry: `${correctedRy}`,
        stroke: `${helperObj.strokeStyle}`
    };
    HelperObj.cssClass = "ellipse-helper";
};


const setTextShape = function(helperObj){
    HelperObj.style = {
        top: `${helperObj.currentY}px`,
        left: `${helperObj.currentX}px`,
        text: `${helperObj.text}`,
        color: `${helperObj.strokeStyle}`,
    };
    HelperObj.cssClass = `text-helper ${helperObj.id}`;
};

const setEquationShape = function(helperObj){
    HelperObj.style = {
        top: `${helperObj.currentY}px`,
        left: `${helperObj.currentX}px`,
        text: `${helperObj.text}`,
        color: `${helperObj.strokeStyle}`,
    };
    HelperObj.cssClass = `equation-helper ${helperObj.id}`;
};

// const setIinkShape = function(helperObj){
//     HelperObj.style = {
//         top: `${helperObj.currentY}px`,
//         left: `${helperObj.currentX}px`,
//         text: `${helperObj.text}`,
//         color: `${helperObj.strokeStyle}`,
//     };
//     HelperObj.cssClass = `iink-helper ${helperObj.id}`;
// }

export default {
    showHelper,
    hideHelper,
    setRectangleShape,
    setLineShape,
    setEllipseShape,
    HelperObj,
    setTextShape,
    resetHelperObj,
    setEquationShape
}