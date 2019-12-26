import {
    createPointsByOption,
    createShape
} from '../utils/factories'
import whiteBoardService from '../whiteBoardService';
import canvasFinder from '../utils/canvasFinder';
import store from '../../../../store/index';
import {LanguageService} from '../../../../services/language/languageService.js'

const optionType = 'imageDraw';

let localShape = createShape({
    type: optionType,
    points: []
});

const clearLocalShape = function () {
    localShape = createShape({
        type: optionType,
        points: []
    });
};

let imageXDefaultPosition;
const imageYDefaultPosition = 75;
let imageDictionary = {};

const init = function () {
    let imageElm = document.getElementById('imageUpload');
    imageElm.removeEventListener('change', handleImage.bind(this), false);
    imageElm.addEventListener('change', handleImage.bind(this), false);
};

const imgSizeFit = function(imgWidth, imgHeight, maxWidth, maxHeight) {
    let ratio = Math.min(1, maxWidth / imgWidth, maxHeight / imgHeight);
    let width = imgWidth * ratio;
    let height = imgHeight * ratio;
    return {width, height};
};

const draw = function (imgObj) {
    if (!!imageDictionary[imgObj.id]) {
        let img = imageDictionary[imgObj.id].img;
        console.log(`img newX ${imgObj.mouseX} img newY ${imgObj.mouseY} img width ${imgObj.width} img height ${imgObj.height}`);
        whiteBoardService.getContext().drawImage(img, imgObj.mouseX, imgObj.mouseY, imgObj.width, imgObj.height);
    } else {
        let img = new Image();
        // img.crossOrigin="anonymous";
        img.onload = function () {
            let imageSize = imgSizeFit(img.width, img.height, 600, 800);
            img.width = imageSize.width;
            img.height = imageSize.height;
            let dictionaryImage = {
                imgObj,
                img
            };
            imageDictionary[imgObj.id] = dictionaryImage;
            whiteBoardService.getContext().drawImage(img, imgObj.mouseX, imgObj.mouseY, img.width, img.height);
        };
        img.src = imgObj.src;
    }
};
const liveDraw = function (imgObj) {
    draw.bind(this, imgObj)();
};

const handleImage = function (e,isDragged) {
    if(e.target.value === "" && !isDragged) return;

    let formData = new FormData();
    let fileData;
    if(!isDragged){
        fileData = e.target.files[0];
    } else{
        fileData = e.dataTransfer.files[0];
    }

    if(!fileData) return;
    store.dispatch("updateShowBoxHelper", false);
    store.dispatch("updateImgLoader", true);
    formData.append("file", fileData);
    let self = this;
    //apiCall
    whiteBoardService.uploadImage(formData).then(url => {
        let img = new Image();
        // img.crossOrigin="anonymous";
        img.onload = function () {
            let imageSize = imgSizeFit(img.width, img.height, 600, 800);
            imageXDefaultPosition = (window.innerWidth / 2) - (imageSize.width / 2);
            let {mouseX, mouseY} = canvasFinder.getRelativeMousePoints(whiteBoardService.getContext(), imageXDefaultPosition, imageYDefaultPosition);
            
            img.width = imageSize.width;
            img.height = imageSize.height;
            let imgObj = createPointsByOption({
                mouseX,
                mouseY,
                width: img.width,
                height: img.height,
                option: optionType,
                eventName: 'start',
                src: img.src
            });
            let dictionaryImage = {
                imgObj,
                img
            };
            imageDictionary[imgObj.id] = dictionaryImage;
            localShape.points.push(imgObj);
            liveDraw.bind(self, imgObj)();
            // self.methods.addShape(localShape, clearLocalShape);
            let addImageObject = {
                dragObj: localShape,
                callback: clearLocalShape
            };
            store.dispatch("setAddImage", addImageObject);
            setTimeout(()=>{
                store.dispatch("setAddImage", null);
            }, 500);
            // self.methods.addShape(localShape, clearLocalShape);
        };
        img.src = url;
    },()=>{
        store.dispatch("updateImgLoader", false);
        store.dispatch('updateToasterParams', {
            toasterText: LanguageService.getValueByKey("upload_multiple_error_extension_title"),
            showToaster: true,
            toasterType: 'error-toaster'
        });
    });
    //reset the element to allow same image to be uploaded
    e.target.value = "";
};

const mousedown = function () {
    return;
};
const mousemove = function () {
    return;
};

const defineEndPosition = function () {
    return;
};

const mouseup = function (e) {
    console.log('mouseUp');
    defineEndPosition.bind(this, e)();
};

const mouseleave = function (e) {
    console.log('mouseLeave');
    defineEndPosition.bind(this, e)();
};

export default {
    mousedown,
    mouseup,
    mousemove,
    mouseleave,
    draw: liveDraw,
    init,
    handleImage
}