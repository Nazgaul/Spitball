let uint = number =>  Math.sqrt(Math.pow(number, 2));

const inBox = function(x0, y0, rect) {
    let x1 = Math.min(rect.startX, rect.startX + rect.w);
    let x2 = Math.max(rect.startX, rect.startX + rect.w);
    let y1 = Math.min(rect.startY, rect.startY + rect.h);
    let y2 = Math.max(rect.startY, rect.startY + rect.h);
    return (x1 <= x0 && x0 <= x2 && y1 <= y0 && y0 <= y2);
}

const findLiveDraw = function(pointX, pointY, shapeObj, ctx){
    let result = false;
    let acceptedOffset = 7;
    let pathPoints = shapeObj.points;
    let boundries = getBoundriesPoints(shapeObj.points)
    let rect = {
        startX: boundries.startX,
        startY: boundries.startY,
        w: boundries.endX - boundries.startX,
        h: boundries.endY - boundries.startY,
    }
    if(inBox(pointX, pointY, rect)){
        console.log(`x ${pointX}, y: ${pointY}`);
        pathPoints.forEach(pointObj=>{
            let leftOffset = (pointX > pointObj.mouseX - acceptedOffset) && (pointX < pointObj.mouseX + acceptedOffset)
            let topOffset = (pointY > pointObj.mouseY - acceptedOffset) && (pointY < pointObj.mouseY + acceptedOffset)
            if(leftOffset && topOffset){
                result = true;
            }
        })
    }
    
    return result;
}


const findLineDraw = function(pointX, pointY, shapeObj, ctx){
    let result = false;
    let acceptedDistance = 2;
    
    let rect = {
        startX: shapeObj.points[0].mouseX,
        startY: shapeObj.points[0].mouseY,
        w: shapeObj.points[1].mouseX - shapeObj.points[0].mouseX,
        h: shapeObj.points[1].mouseY - shapeObj.points[0].mouseY,
    }
    if(inBox(pointX, pointY, rect)){
        let x1 = shapeObj.points[0].mouseX;
        let x2 = shapeObj.points[1].mouseX;
        let y1 = shapeObj.points[0].mouseY;
        let y2 = shapeObj.points[1].mouseY;
        let Dx = x2 - x1;
        let Dy = y2 - y1;
        let d = Math.abs(Dy*pointX - Dx*pointY - x1*y2+x2*y1)/Math.sqrt(Math.pow(Dx, 2) + Math.pow(Dy, 2));
        if(d < acceptedDistance){
            result = true;
        }
    }
    return result;
}

const findRectangleDraw = function(pointX, pointY, shapeObj, ctx){
    let path = shapeObj.path.stroke;
    return ctx.isPointInStroke(path, pointX, pointY);
}

const findEllipseDraw = function(pointX, pointY, shapeObj, ctx){
    let path = shapeObj.path.stroke;
    return ctx.isPointInStroke(path, pointX, pointY);
}

const findImageDraw = function(pointX, pointY, shapeObj){
    let boundries= {
        startX: shapeObj.points[0].mouseX,
        startY: shapeObj.points[0].mouseY,
        endX: shapeObj.points[0].width + shapeObj.points[0].mouseX,
        endY: shapeObj.points[0].height + shapeObj.points[0].mouseY
    }
    let rect = {
        startX: boundries.startX,
        startY: boundries.startY,
        w: boundries.endX,
        h: boundries.endY,
    }
    rect.startX = rect.startX < 0 ? 0 : rect.startX;
    rect.startY = rect.startY < 0 ? 0 : rect.startY;
    return inBox(pointX, pointY, rect);
}

const findTextDraw = function(pointX, pointY, shapeObj){
    let boundries= {
        startX: shapeObj.points[0].mouseX,
        startY: shapeObj.points[0].mouseY - shapeObj.points[0].yOffset,
        endX: shapeObj.points[0].width + shapeObj.points[0].mouseX,
        endY: (shapeObj.points[0].height + shapeObj.points[0].mouseY) - shapeObj.points[0].yOffset
    }
    let rect = {
        startX: boundries.startX,
        startY: boundries.startY,
        w: shapeObj.points[0].width,
        h: shapeObj.points[0].height,
    }
    rect.startX = rect.startX < 0 ? 0 : rect.startX;
    rect.startY = rect.startY < 0 ? 0 : rect.startY;
    return inBox(pointX, pointY, rect);
}

const clickSearchShapeByType = {
    liveDraw: findLiveDraw,
    lineDraw: findLineDraw,
    drawEllipse: findEllipseDraw,
    drawRectangle: findRectangleDraw,
    imageDraw: findImageDraw,
    eraser: function(){},
    textDraw: findTextDraw
}


const getShapeByPoint = function(x, y, globalObj){
    let shapes = globalObj.dragData;
    let ctx = globalObj.context;
    let selectedShape = {};
    // console.log(`x:${x}, y:${y}`)
    if(!!shapes && shapes.length > 0){
        shapes.forEach((shape, index)=>{
            if(shape.isGhost) return;
            console.log(shape.type);
                let isShapeFound = clickSearchShapeByType[shape.type](x, y, shape, ctx)
                if(isShapeFound){
                    selectedShape[shape.id] = shape;
                    return;
                }
        })
    }
    return selectedShape;
}

const getShapesByRectangle = function(globalObj, helperBoundries){
    let shapes = globalObj.dragData;
    let selectedShape = {};
    if(!!shapes && shapes.length > 0){
        shapes.forEach((shape)=>{
            if(shape.isGhost) return;
            shape.points.forEach(dragObj=> {
                let isShapeFound = inBox(dragObj.mouseX, dragObj.mouseY, helperBoundries)
                if(isShapeFound){
                    selectedShape[shape.id] = shape;
                }
            })
        })
    }
    return selectedShape;
}


const getBoundriesPoints = function(points){
    let startX = null;
    let startY = null;
    let endX = null;
    let endY = null;
    points.forEach((dragObj, index) => {
        if(dragObj.option === "imageDraw" || dragObj.option === "textDraw"){
            let yOffset = dragObj.yOffset ? dragObj.yOffset : 0;
            if(index === 0){
                startX = dragObj.mouseX;
                startY = dragObj.mouseY - yOffset;
                endX = dragObj.width + dragObj.mouseX;
                endY = dragObj.height + dragObj.mouseY - yOffset;
            }else{
                if(startX > dragObj.mouseX){
                    startX = dragObj.mouseX;
                }if(startY > dragObj.mouseY - yOffset){
                    startY = dragObj.mouseY - yOffset;
                }if(endX < dragObj.width + dragObj.mouseX){
                    endX = dragObj.width + dragObj.mouseX;
                }if(endY < dragObj.height + dragObj.mouseY - yOffset){
                    endY = dragObj.height + dragObj.mouseY - yOffset;
                }
            }
        }else{
            if(index === 0){
                startX = dragObj.mouseX;
                startY = dragObj.mouseY;
                endX = dragObj.mouseX;
                endY = dragObj.mouseY;
            }else{
                if(startX > dragObj.mouseX){
                    startX = dragObj.mouseX;
                }if(startY > dragObj.mouseY){
                    startY = dragObj.mouseY;
                }if(endX < dragObj.mouseX){
                    endX = dragObj.mouseX;
                }if(endY < dragObj.mouseY){
                    endY = dragObj.mouseY;
                }
            }
        }
        
    })
    return {
        startX,
        startY,
        endX,
        endY
    }
}


export default{
    getShapeByPoint,
    getBoundriesPoints,
    getShapesByRectangle,
    inBox
}