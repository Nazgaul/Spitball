import whiteBoardService from "../whiteBoardService";
import store from "../../../../store/index";

let lastX=null;
let lastY=null;
let scaleFactor = null;
let dragStart = false;

let mouseStartX = null;
let mouseStartY = null;
let startScrollPositionTop = null;
let startScrollPositionLeft = null;

const init = function(){
    // lastX = this.context.canvas.width/2;
    // lastY = this.context.canvas.height/2;
    dragStart = false;
    scaleFactor = 1.1;    
};
const draw = function(){
    let p1 = this.context.transformedPoint(0,0);
    let p2 = this.context.transformedPoint(this.context.canvas.width, this.context.canvas.height);
    this.context.clearRect(p1.x,p1.y,p2.x-p1.x,p2.y-p1.y);

    this.context.save();
    this.context.setTransform(1,0,0,1,0,0);
    this.context.clearRect(0,0,this.context.canvas.width, this.context.canvas.height);
    this.context.restore();
    whiteBoardService.redraw(this);
};


const liveDraw = function(){};

const mousedown = function(e){
    //Set Click Position
    this.methods.hideColorPicker();
    dragStart = true;
    mouseStartX = e.clientX;
    mouseStartY = e.clientY;
    startScrollPositionTop = e.target.parentElement.scrollTop;
    startScrollPositionLeft = e.target.parentElement.scrollLeft;
};


const mousemove = function(e){
    // let scrollReachedBottom = (e.target.parentElement.scrollTop + e.target.parentElement.clientHeight) === e.target.height;
    // let scrollReachdRight = (e.target.parentElement.scrollLeft + e.target.parentElement.clientWidth) === e.target.width;
    let scrollableElm = e.target.parentElement;
    let deltaX = mouseStartX - e.clientX;
    let deltaY = mouseStartY - e.clientY;
    if (dragStart){
      scrollableElm.scrollLeft = startScrollPositionLeft + deltaX;
      scrollableElm.scrollTop = startScrollPositionTop + deltaY;
      let transform = {
          x: scrollableElm.scrollLeft*-1,
          y: scrollableElm.scrollTop*-1
      };
      store.dispatch('updatePan', transform);
      draw.bind(this)();
    }
};


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

    //deprecated added scroll
    // let pt = this.context.transformedPoint(lastX, lastY);
    // this.context.translate(pt.x, pt.y);
    // let factor = Math.pow(scaleFactor, clicks);
    // this.context.scale(factor, factor);
    // this.context.translate(-pt.x, -pt.y);
    // whiteBoardService.redraw(this);
    // let transform = this.context.getTransform();
    // store.dispatch('updateZoom', transform.a*100);
    // store.dispatch('updatePan', transform);
};

// const mouseScroll = function(evt){
//     let delta = evt.wheelDelta ? evt.wheelDelta/40 : evt.detail ? -evt.detail : 0;
//     if (delta) {
//         zoom.bind(this, delta)()
//     };
//     return evt.preventDefault() && false;
// }

// const manualScroll = function(zoomIn){
//     let scrollValue = 0.75;
//     let delta = zoomIn ? scrollValue : scrollValue*-1;
//     zoom.bind(this, delta)()
// }

const defineEndPosition = function(e){
    dragStart = false;
};
const mouseup = function(e){
    defineEndPosition.bind(this, e)();
};
const mouseleave = function(e){
    defineEndPosition.bind(this, e)();
};

export default{
    mousedown,
    mouseup,
    mousemove,
    mouseleave,
    // mouseScroll,
    draw: liveDraw,
    init,
    // manualScroll
}