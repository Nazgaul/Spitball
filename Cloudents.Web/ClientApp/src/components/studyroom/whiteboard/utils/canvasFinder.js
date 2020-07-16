//let uint = number =>  Math.sqrt(Math.pow(number, 2));

const inBox = function(x0, y0, rect) {
    let x1 = Math.min(rect.startX, rect.startX + rect.w);
    let x2 = Math.max(rect.startX, rect.startX + rect.w);
    let y1 = Math.min(rect.startY, rect.startY + rect.h);
    let y2 = Math.max(rect.startY, rect.startY + rect.h);
    return (x1 <= x0 && x0 <= x2 && y1 <= y0 && y0 <= y2);
};

const findLiveDraw = function(pointX, pointY, shapeObj){
    let result = false;
    let acceptedOffset = 15;
    let pathPoints = shapeObj.points;
    let boundaries = getBoundriesPoints(shapeObj.points);
    let rect = {
        startX: boundaries.startX,
        startY: boundaries.startY,
        w: boundaries.endX - boundaries.startX,
        h: boundaries.endY - boundaries.startY
    };
    if(inBox(pointX, pointY, rect)){
        console.log(`x ${pointX}, y: ${pointY}`);
        pathPoints.forEach(pointObj=>{
            let leftOffset = (pointX > pointObj.mouseX - acceptedOffset) && (pointX < pointObj.mouseX + acceptedOffset);
            let topOffset = (pointY > pointObj.mouseY - acceptedOffset) && (pointY < pointObj.mouseY + acceptedOffset);
            if(leftOffset && topOffset){
                result = true;
            }
        });
    }
    
    return result;
};


const findLineDraw = function(pointX, pointY, shapeObj){
    let result = false;
    let acceptedDistance = 4;
    
    let rect = {
        startX: shapeObj.points[0].mouseX,
        startY: shapeObj.points[0].mouseY,
        w: shapeObj.points[1].mouseX - shapeObj.points[0].mouseX,
        h: shapeObj.points[1].mouseY - shapeObj.points[0].mouseY
    };
    if(inBox(pointX, pointY, rect)){
        let x1 = shapeObj.points[0].mouseX;
        let x2 = shapeObj.points[1].mouseX;
        let y1 = shapeObj.points[0].mouseY;
        let y2 = shapeObj.points[1].mouseY;
        let dx = x2 - x1;
        let dy = y2 - y1;
        let d = Math.abs(dy*pointX - dx*pointY - x1*y2+x2*y1)/Math.sqrt(Math.pow(dx, 2) + Math.pow(dy, 2));
        if(d < acceptedDistance){
            result = true;
        }
    }
    return result;
};

const findRectangleDraw = function(pointX, pointY, shapeObj){
    let boundaries= {
        startX: shapeObj.points[0].mouseX,
        startY: shapeObj.points[0].mouseY
    };

    let rect = {
        startX: boundaries.startX,
        startY: boundaries.startY,
        w: shapeObj.points[1].mouseX - shapeObj.points[0].mouseX,
        h: shapeObj.points[1].mouseY - shapeObj.points[0].mouseY
    };
    return inBox(pointX, pointY, rect);
    // let path = new Path2D(shapeObj.path.stroke);
    // return ctx.isPointInStroke(path, pointX, pointY);
};

const findEllipseDraw = function(pointX, pointY, shapeObj){
    let boundaries= {
        startX: shapeObj.points[0].mouseX,
        startY: shapeObj.points[0].mouseY
    };

    let rect = {
        startX: boundaries.startX,
        startY: boundaries.startY,
        w: shapeObj.points[1].mouseX - shapeObj.points[0].mouseX,
        h: shapeObj.points[1].mouseY - shapeObj.points[0].mouseY
    };
    return inBox(pointX, pointY, rect);
    //let path = new Path2D(shapeObj.path.stroke);
    // return ctx.isPointInStroke(path, pointX, pointY);
};

const findImageDraw = function(pointX, pointY, shapeObj){
    let boundaries= {
        startX: shapeObj.points[0].mouseX,
        startY: shapeObj.points[0].mouseY,
        endX: shapeObj.points[0].width,
        endY: shapeObj.points[0].height
    };
    let rect = {
        startX: boundaries.startX,
        startY: boundaries.startY,
        w: boundaries.endX,
        h: boundaries.endY
    };
    return inBox(pointX, pointY, rect);
};

const findTextDraw = function(pointX, pointY, shapeObj){
    let boundaries= {
        startX: shapeObj.points[0].mouseX,
        startY: shapeObj.points[0].mouseY - shapeObj.points[0].yOffset,
        endX: shapeObj.points[0].width + shapeObj.points[0].mouseX,
        endY: (shapeObj.points[0].height + shapeObj.points[0].mouseY) - shapeObj.points[0].yOffset
    };
    let rect = {
        startX: boundaries.startX,
        startY: boundaries.startY,
        w: shapeObj.points[0].width,
        h: shapeObj.points[0].height
    };
    rect.startX = rect.startX < 0 ? 0 : rect.startX;
    rect.startY = rect.startY < 0 ? 0 : rect.startY;
    return inBox(pointX, pointY, rect);
};

const clickSearchShapeByType = {
    liveDraw: findLiveDraw,
    lineDraw: findLineDraw,
    drawEllipse: findEllipseDraw,
    drawRectangle: findRectangleDraw,
    imageDraw: findImageDraw,
    eraser: function(){},
    textDraw: findTextDraw,
    equationDraw: findImageDraw,
    iink: findImageDraw
};


const getShapeByPoint = function(x, y, globalObj, dragData){
    let shapes = dragData;
    let ctx = globalObj.context;
    let selectedShape = {};
    // console.log(`x:${x}, y:${y}`)
    if(!!shapes && shapes.length > 0){
        shapes.forEach((shape)=>{
            if(shape.isGhost || !shape.visible) return;
            console.log(shape.type);
                let isShapeFound = clickSearchShapeByType[shape.type](x, y, shape, ctx);
                if(isShapeFound){
                    selectedShape[shape.id] = shape;
                    return;
                }
        });
    }
    return selectedShape;
};

const getShapesByRectangle = function(globalObj, helperBoundries, dragData){
    let shapes = dragData;
    let selectedShape = {};
    if(!!shapes && shapes.length > 0){
        shapes.forEach((shape)=>{
            if(shape.isGhost || !shape.visible) return;
            shape.points.forEach(dragObj=> {
                let isShapeFound = inBox(dragObj.mouseX, dragObj.mouseY, helperBoundries);
                if(isShapeFound){
                    selectedShape[shape.id] = shape;
                }
            });
        });
    }
    return selectedShape;
};


const getBoundriesPoints = function(points){
    let startX = null;
    let startY = null;
    let endX = null;
    let endY = null;
    points.forEach((dragObj, index) => {
        if(dragObj.isRect){
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
        
    });

    return {
        startX,
        startY,
        endX,
        endY
    };
};

// Adds ctx.getTransform() - returns an SVGMatrix
	// Adds ctx.transformedPoint(x,y) - returns an SVGPoint
	function trackTransforms(ctx){
        let svg = document.createElementNS("http://www.w3.org/2000/svg",'svg');
        let xform = svg.createSVGMatrix();
        ctx.getTransform = function(){ return xform; };
  
        let savedTransforms = [];
        let save = ctx.save;
        ctx.save = function(){
            savedTransforms.push(xform.translate(0,0));
            return save.call(ctx);
        };
      
        let restore = ctx.restore;
        ctx.restore = function(){
          xform = savedTransforms.pop();
          return restore.call(ctx);
                };
  
        let scale = ctx.scale;
        ctx.scale = function(sx,sy){
          xform = xform.scaleNonUniform(sx,sy);
          return scale.call(ctx,sx,sy);
          };
      
        let rotate = ctx.rotate;
        ctx.rotate = function(radians){
            xform = xform.rotate(radians*180/Math.PI);
            return rotate.call(ctx,radians);
        };
      
        let translate = ctx.translate;
        ctx.translate = function(dx,dy){
            xform = xform.translate(dx,dy);
            return translate.call(ctx,dx,dy);
        };
      
        let transform = ctx.transform;
        ctx.transform = function(a,b,c,d,e,f){
            var m2 = svg.createSVGMatrix();
            m2.a=a; m2.b=b; m2.c=c; m2.d=d; m2.e=e; m2.f=f;
            xform = xform.multiply(m2);
            return transform.call(ctx,a,b,c,d,e,f);
        };
      
        let setTransform = ctx.setTransform;
        ctx.setTransform = function(a,b,c,d,e,f){
            xform.a = a;
            xform.b = b;
            xform.c = c;
            xform.d = d;
            xform.e = e;
            xform.f = f;
            return setTransform.call(ctx,a,b,c,d,e,f);
        };
      
        let pt  = svg.createSVGPoint();
        ctx.transformedPoint = function(x,y){
            pt.x=x; pt.y=y;
            return pt.matrixTransform(xform.inverse());
        };
    }

      const getRelativeMousePoints = function(ctx, mouseX, mouseY){
        let pt = ctx.transformedPoint(mouseX, mouseY);
        return {mouseX: pt.x, mouseY: pt.y};
      };

export default{
    getShapeByPoint,
    getBoundriesPoints,
    getShapesByRectangle,
    inBox,
    trackTransforms,
    getRelativeMousePoints
}