import {
    createPointsByOption,
    createShape
} from '../utils/factories'
import whiteBoardService from '../whiteBoardService';

const OPTION_TYPE = 'imageDraw';

let localShape = createShape({
    type: OPTION_TYPE,
    points: []
});

const clearLocalShape = function () {
    localShape = createShape({
        type: OPTION_TYPE,
        points: []
    });
}

const imageXDefaultPosition = 100;
const imageYDefaultPosition = 75;

let imageDictionary = {};

const init = function () {
    let imageElm = document.getElementById('imageUpload');
    imageElm.removeEventListener('change', handleImage.bind(this), false);
    imageElm.addEventListener('change', handleImage.bind(this), false);
}

const imgSizeFit = function(imgWidth, imgHeight, maxWidth, maxHeight) {
    let ratio = Math.min(1, maxWidth / imgWidth, maxHeight / imgHeight);
    let width = imgWidth * ratio;
    let height = imgHeight * ratio;
    return {width, height};
}

const draw = function (imgObj) {
    if (!!imageDictionary[imgObj.id]) {
        let img = imageDictionary[imgObj.id].img;
        this.context.drawImage(img, imgObj.mouseX, imgObj.mouseY, img.width, img.height);
    } else {
        let img = new Image();
        let self = this;
        img.onload = function () {
            let dictionaryImage = {
                imgObj,
                img
            }
            imageDictionary[imgObj.id] = dictionaryImage;
            self.context.drawImage(img, imgObj.mouseX, imgObj.mouseY, img.width, img.height);
        }
        img.src = imgObj.src;
    }
}
const liveDraw = function (imgObj) {
    // this.context.beginPath();
    draw.bind(this, imgObj)();
    // this.context.closePath();
    // this.context.stroke();
}

const handleImage = function (e) {
    //Set Click Position
    let mouseX = imageXDefaultPosition;
    let mouseY = imageYDefaultPosition;
    this.methods.hideColorPicker();

    let formData = new FormData();
    let fileData = e.target.files[0];
    formData.append("file", fileData);
    let self = this;
    //apiCall
    whiteBoardService.uploadImage(formData).then(url => {
        let img = new Image();
        img.onload = function () {
            let imageSize = imgSizeFit(img.width, img.height, 600, 800);
            img.width = imageSize.width;
            img.height = imageSize.height;
            let imgObj = createPointsByOption({
                mouseX,
                mouseY,
                width: img.width,
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
}

const mousedown = function (e) {
    return;
}
const mousemove = function (e) {
    return;
}

const defineEndPosition = function (e) {
    return;
}

const mouseup = function (e) {
    console.log('mouseUp')
    defineEndPosition.bind(this, e)()
}

const mouseleave = function (e) {
    console.log('mouseLeave')
    defineEndPosition.bind(this, e)()
}

export default {
    mousedown,
    mouseup,
    mousemove,
    mouseleave,
    draw: liveDraw,
    init
}