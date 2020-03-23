import store from  "../../store/index";
import insightService from '../../services/insightService';

let deviceValidationError = false;
let devicesObject = {
     hasAudio:false,
     hasVideo:false,
     errors:{
         video: [],
         audio: []
     },
  }
const validateUserMedia = async function() {
    await navigator.mediaDevices.getUserMedia({ video: true }).then((y) => {
        console.log(y);
        devicesObject.hasVideo = true;
    }, err => {
        let insightErrorObj={
            error: err,
            userId: this.userId
        };
        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_validationDialog_getUserMedia_VIDEO', insightErrorObj, null);
        console.error(err.name + ":VIDEO!!!!!!!!!!!!!!!! " + err.message, err);
        devicesObject.errors.video.push(err.name);
    });

    await navigator.mediaDevices.getUserMedia({ audio: true }).then((y) => {
        console.log(y);
        devicesObject.hasAudio = true;
    }, err => {
        let insightErrorObj={
            error: err,
            userId: this.userId
        };
        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_validationDialog_getUserMedia_AUDIO', insightErrorObj, null);
        console.error(err.name + ":AUDIO!!!!!!!!!!!!!!!! " + err.message, err);
        devicesObject.errors.audio.push(err.name);
    });
};
const isBrowserSupport = function(){
    let agent = navigator.userAgent;
    if(agent.match(/Edge/)){
      return false;
    }
    return agent.match(/Firefox|Chrome|Safari/);
};
const validateMedia = async function(){
  if(devicesObject.errors.video.length > 0){
    deviceValidationError = true;
    // store.dispatch("initLocalMediaTracks");  
  }else if(devicesObject.errors.audio.length > 0){
    deviceValidationError = true;
    // store.dispatch("initLocalMediaTracks");
  }else{
    // store.dispatch("initLocalMediaTracks");
  }
};

function firstPageObj(type, props){
  this.type = type; //String required
  this.props = props || null;
}

const determinFirstPage = function(){
  if(!isBrowserSupport()){
    return new firstPageObj("browserNotSupportedStep");
  }
  validateMedia();
  if(deviceValidationError){
    let notAllowedObj = {
      videoNotAllowed: false,
      audioNotAllowed: false,
    };
      if (!devicesObject.hasVideo) {
        notAllowedObj.videoNotAllowed = devicesObject.errors.video.indexOf("NotAllowedError") > -1;
      }
      if (!devicesObject.hasAudio) {
        notAllowedObj.audioNotAllowed = devicesObject.errors.audio.indexOf("NotAllowedError") > -1;
      }

      if(notAllowedObj.videoNotAllowed || notAllowedObj.audioNotAllowed){
        return new firstPageObj("notAllowedStep", notAllowedObj);
      }else{
        return new firstPageObj("unableToConnetStep", notAllowedObj);
      }
      
    
  }

  let isRecordingSupported = !store.getters.getRoomIsTutor || true;
  if(isRecordingSupported){
    return new firstPageObj("watchRecordedStep");
  }else{
    return new firstPageObj("studyRoom");
  }
  
};

export default {
    isBrowserSupport,
    determinFirstPage,
    validateMedia,
    validateUserMedia
}