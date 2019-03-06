import {createPointsByOption, createShape} from '../utils/factories'
import whiteBoardService from '../whiteBoardService';

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

let imageDictionary = {};
const submitElm = document.getElementById('imageUploadSubmit');

const init = function(){
    let imageElm = document.getElementById('imageUpload');
    imageElm.removeEventListener('change', handleImage.bind(this), false);
    imageElm.addEventListener('change', handleImage.bind(this), false);
}

const draw = function(imgObj){
    if(!!imageDictionary[imgObj.id]){
        let img = imageDictionary[imgObj.id].img;
        this.context.drawImage(img, imgObj.mouseX, imgObj.mouseY, imgObj.width, imgObj.height);
    }else{
        let img = new Image();
        let self = this;
        img.onload = function(){
            let dictionaryImage = {
                imgObj,
                img
            }
            imageDictionary[imgObj.id] = dictionaryImage;
            self.context.drawImage(img, imgObj.mouseX, imgObj.mouseY, imgObj.width, imgObj.height);
        }
        img.src = imgObj.src;
    }
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

    let formData = new FormData();
    let fileData = e.target.files[0];
    //name ned to be added
    formData.append(fileData.name, fileData);
    let self = this;
    //apiCall
    whiteBoardService.uploadImage(formData).then(url=>{
        let img = new Image();
        img.onload = function(){
            let imgObj = createPointsByOption({
                mouseX,
                mouseY,
                width:img.width,
                height: img.height,
                option: OPTION_TYPE,
                eventName: 'start',
                src: img.src
            })
            let dictionaryImage = {
                imgObj,
                img
            }
            imageDictionary[imgObj.id] = dictionaryImage;
            localShape.points.push(imgObj);
            liveDraw.bind(self, imgObj)();
            self.methods.addShape(localShape, clearLocalShape);
        }
        img.src = url;
    })

    // let reader = new FileReader();
    // let self = this;
    // reader.onload = (event)=>{
    //     let img = new Image();
    //     img.onload = function(){
    //         let imgObj = createPointsByOption({
    //             mouseX,
    //             mouseY,
    //             width:img.width,
    //             height: img.height,
    //             option: OPTION_TYPE,
    //             eventName: 'start',
    //             src: img.src
    //         })
    //         let dictionaryImage = {
    //             imgObj,
    //             img
    //         }
    //         imageDictionary[imgObj.id] = dictionaryImage;
    //         localShape.points.push(imgObj);
    //         liveDraw.bind(self, imgObj)();
    //         self.methods.addShape(localShape, clearLocalShape);
    //     }
    //     img.src = event.target.result;
    // }
    // reader.readAsDataURL(e.target.files[0]);
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