import store from  "../../store/index";
import tutorService from '../studyroom/tutorService';

const isBrowserSupport = function(){
    let agent = navigator.userAgent;
    if(agent.match(/Edge/)){
      return false;
    }
    return agent.match(/Firefox|Chrome|Safari/);
};
const validateMedia = async function(){
  let deviceValidationObj = store.getters.getDevicesObj;
  if(deviceValidationObj.errors.video.length > 0){
    store.dispatch("setDeviceValidationError", true);    
  }else if(deviceValidationObj.errors.audio.length > 0){
    store.dispatch("setDeviceValidationError", true);
    //allow video to be shown locally.
    store.dispatch("initLocalMediaTracks");
  }else{
    store.dispatch("initLocalMediaTracks");
  }
};

function firstPageObj(type, props){
  this.type = type; //String required
  this.props = props || null;
}

const determinFirstPage = function(){
  //test browser support
  if(!isBrowserSupport()){
    return new firstPageObj("browserNotSupportedStep");
  }
  //test validate user media
  validateMedia();
  if(store.getters.showDeviceValidationError){
    let notAllowedObj = {
      videoNotAllowed: false,
      audioNotAllowed: false,
    };
    let deviceValidationObj = store.getters.getDevicesObj;
      if (!deviceValidationObj.hasVideo) {
        notAllowedObj.videoNotAllowed = deviceValidationObj.errors.video.indexOf("NotAllowedError") > -1;
      }
      if (!deviceValidationObj.hasAudio) {
        notAllowedObj.audioNotAllowed = deviceValidationObj.errors.audio.indexOf("NotAllowedError") > -1;
      }

      if(notAllowedObj.videoNotAllowed || notAllowedObj.audioNotAllowed){
        return new firstPageObj("notAllowedStep", notAllowedObj);
      }else{
        return new firstPageObj("unableToConnetStep", notAllowedObj);
      }
      
    
  }
  //else recording instruction page
  if(tutorService.isRecordingSupported()){
    return new firstPageObj("watchRecordedStep");
  }else{
    return new firstPageObj("studyRoom");
  }
  
};

export default {
    isBrowserSupport,
    determinFirstPage,
    validateMedia
}