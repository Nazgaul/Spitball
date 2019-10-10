const createGuid = function(val){
    return `xxxx-${val}-xxxx-xxxx-xxxx-xxxx-xxxx-xxxx`.replace(/[x]/g, ()=>{
        return (Math.random() * 9 | 0).toString();
    });
};

const pointsByOption = {
    liveDraw: DragObj,
    lineDraw: DragObj,
    drawEllipse: EllipseObj,
    drawRectangle: RectangleObj,
    imageDraw: ImageObj,
    eraser: DragObj,
    textDraw: TextObj,
    equationDraw: EquationObj
};

function DragObj(objInit){
    this.mouseX= objInit.mouseX;
    this.mouseY= objInit.mouseY;
    this.isDragging= objInit.isDragging;
    this.strokeStyle= objInit.strokeStyle;
    this.option=objInit.option;
    this.eventName=objInit.eventName;
}
function EllipseObj(objInit){
    this.mouseX= objInit.mouseX;
    this.mouseY= objInit.mouseY;
    this.isDragging= objInit.isDragging;
    this.strokeStyle= objInit.strokeStyle;
    this.option=objInit.option;
    this.eventName=objInit.eventName;
}
function RectangleObj(objInit){
    this.mouseX= objInit.mouseX;
    this.mouseY= objInit.mouseY;
    this.isDragging= objInit.isDragging;
    this.strokeStyle= objInit.strokeStyle;
    this.option=objInit.option;
    this.eventName=objInit.eventName;
}
function ImageObj(objInit){
    this.id = objInit.id || createGuid('image');
    this.mouseX= objInit.mouseX;
    this.mouseY= objInit.mouseY;
    this.width = objInit.width;
    this.height = objInit.height;
    this.option=objInit.option;
    this.eventName=objInit.eventName;
    this.src = objInit.src;
    this.isRect = true;
}

function TextObj(objInit){
    this.mouseX= objInit.mouseX;
    this.mouseY= objInit.mouseY;
    this.yOffset = objInit.yOffset;
    this.text = objInit.text;
    this.eventName=objInit.eventName;
    this.option=objInit.option;
    this.color = objInit.color;
    this.width = objInit.width;
    this.height = objInit.height;
    this.fontFamily = objInit.fontFamily;
    this.id = objInit.id;
    this.isRect = true;
}

function EquationObj(objInit){
    this.mouseX= objInit.mouseX;
    this.mouseY= objInit.mouseY;
    this.width= objInit.width;
    this.height= objInit.height;
    this.color= objInit.color;
    this.option= objInit.option;
    this.eventName= objInit.eventName;
    this.id= objInit.id;
    this.text= objInit.text;
    this.isRect = true;
}

function Path(objInit){
    this.stroke = objInit.stroke || null;
    this.fill = objInit.fill || null;
}



function Shape(objInit){
    this.id = objInit.id || createGuid('Shape');
    this.type = objInit.type;
    this.points = objInit.points.map((dragObj)=>{
        return new pointsByOption[dragObj.option](dragObj);
    });
    this.path = objInit.path ? new Path(objInit.path) : new Path({});
    this.offset = {
        top: 0,
        left: 0
    };
    this.isGhost = false;
    this.visible = typeof objInit.visible !== 'undefined' ?  objInit.visible : true;
}

function GhostShape(objInit){
    //ghost shape will be called when undo occures
    this.id = createGuid('Ghost');
    this.type = objInit.type;
    this.actionType = objInit.actionType;
    this.actionObj = objInit.actionObj;
    this.isGhost = true;
}

function createPointsByOption(dragObj){
   return new pointsByOption[dragObj.option](dragObj);
}

function createShape(objInit){
    return new Shape(objInit);
}

function createGhostShape(objInit){
    return new GhostShape(objInit);
}

export{
    createPointsByOption,
    createShape,
    createGhostShape,
    createGuid
}