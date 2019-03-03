import {createPointsByOption, createShape} from '../utils/factories'

const OPTION_TYPE = 'imageDraw';

let localShape = createShape({
    type: OPTION_TYPE,
    points: []
});

const clearLocalShape = function(){
    localShape = createShape({
        type: OPTION_TYPE,
        points: []
    });
}

const imageXDefaultPosition = 0;
const imageYDefaultPosition = 0;

const init = function(){
    
}

const draw = function(imgObj){
    let img = imgObj.img;
    this.context.drawImage(img, imgObj.mouseX, imgObj.mouseY, imgObj.width, imgObj.height);
}
const liveDraw = function(imgObj){
    // this.context.beginPath();
    draw.bind(this, imgObj)();
    // this.context.closePath();
    // this.context.stroke();
    
}

const mousedown = function(e){
    //Set Click Position
    let mouseX = imageXDefaultPosition;
    let mouseY = imageYDefaultPosition;
    this.methods.hideColorPicker();

    let img = new Image();
    let self = this;
    img.onload = function(){
        let imgObj = createPointsByOption({
            mouseX,
            mouseY,
            width:img.width,
            height: img.height,
            option: OPTION_TYPE,
            eventName: 'start',
            img: img
        })
        localShape.points.push(imgObj);
        liveDraw.bind(self, imgObj)();
        self.methods.addShape(localShape, clearLocalShape);
        
        
    }
    img.src = 'http://s3.media.squarespace.com/production/454594/5165598/wordpress/wp-content/uploads/2007/11/fantasygn-ideas-07-bugglefug.thumbnail.jpg';
}
const mousemove = function(e){
}

const defineEndPosition = function(e){
    return;
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