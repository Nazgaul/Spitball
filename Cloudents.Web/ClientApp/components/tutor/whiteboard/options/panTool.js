import whiteBoardService from "../whiteBoardService";
import store from "../../../../store/index";

let lastX=null;
let lastY=null;
let scaleFactor = null;
let dragStart = null;
let dragged = false;

const init = function(){
    lastX = this.context.canvas.width/2;
    lastY = this.context.canvas.height/2;
    dragged = false;
    scaleFactor = 1.1;    
}
const draw = function(){
    let p1 = this.context.transformedPoint(0,0);
    let p2 = this.context.transformedPoint(this.context.canvas.width, this.context.canvas.height);
    this.context.clearRect(p1.x,p1.y,p2.x-p1.x,p2.y-p1.y);

    this.context.save();
    this.context.setTransform(1,0,0,1,0,0);
    this.context.clearRect(0,0,this.context.canvas.width, this.context.canvas.height);
    this.context.restore();
    whiteBoardService.redraw(this);
}


const liveDraw = function(){};

const mousedown = function(evt){
    //Set Click Position
    this.methods.hideColorPicker();
    lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
    lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);
    dragStart = this.context.transformedPoint(lastX,lastY);
    dragged = false;
    
}
const mousemove = function(evt){
    lastX = evt.offsetX || (evt.pageX - canvas.offsetLeft);
    lastY = evt.offsetY || (evt.pageY - canvas.offsetTop);
    dragged = true;
    if (dragStart){
      var pt = this.context.transformedPoint(lastX,lastY);
      this.context.translate(pt.x-dragStart.x, pt.y-dragStart.y);
      draw.bind(this)();
    }
}


const zoom = function(clicks){
    //if we want to limit zoom
    // let maxLimit = 2;
    // let minLimit = 0.5;
    
    // if(a > maxLimit){
    //     this.context.setTransform(maxLimit,b,c,maxLimit,e,f);
    //     return;
    // }else if(a < minLimit){
    //     this.context.setTransform(minLimit,b,c,minLimit,e,f);
    //     return;
    // }
    // if(a === maxLimit && clicks > 0){
    //     return;
    // }else if(a === minLimit && clicks < 0){
    //     return;
    // }
    let pt = this.context.transformedPoint(lastX, lastY);
    this.context.translate(pt.x, pt.y);
    let factor = Math.pow(scaleFactor, clicks);
    this.context.scale(factor, factor);
    this.context.translate(-pt.x, -pt.y);
    whiteBoardService.redraw(this);
    let {a} = this.context.getTransform();
    store.dispatch('updateZoom', a*100);
}

const mouseScroll = function(evt){
    let delta = evt.wheelDelta ? evt.wheelDelta/40 : evt.detail ? -evt.detail : 0;
    if (delta) {
        zoom.bind(this, delta)()
    };
    return evt.preventDefault() && false;
}

const manualScroll = function(zoomIn){
    let scrollValue = 0.75;
    let delta = zoomIn ? scrollValue : scrollValue*-1;
    zoom.bind(this, delta)()
}

const defineEndPosition = function(e){
    dragStart = null;
}
const mouseup = function(e){
    defineEndPosition.bind(this, e)()
}
const mouseleave = function(e){
    defineEndPosition.bind(this, e)()
}

export default{
    mousedown,
    mouseup,
    mousemove,
    mouseleave,
    mouseScroll,
    draw: liveDraw,
    init,
    manualScroll
}