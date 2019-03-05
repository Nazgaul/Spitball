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

const imageXDefaultPosition = 100;
const imageYDefaultPosition = 75;

const init = function(){
    let imageElm = document.getElementById('imageUpload');
    imageElm.removeEventListener('change', handleImage.bind(this), false);
    imageElm.addEventListener('change', handleImage.bind(this), false);
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

const handleImage = function(e){
    //Set Click Position
    let mouseX = imageXDefaultPosition;
    let mouseY = imageYDefaultPosition;
    this.methods.hideColorPicker();

    let reader = new FileReader();
    let self = this;
    reader.onload = (event)=>{
        let img = new Image();
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
        img.src = event.target.result;
    }
    reader.readAsDataURL(e.target.files[0]);
}

const mousedown = function(e){
    return;
}
const mousemove = function(e){
    return;
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