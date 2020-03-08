import store from '../../store/index';
// import tutorService from './tutorService';
import {Decoder, tools, Reader} from 'ts-ebml';
import insightService from '../../services/insightService';
import { LanguageService } from '../../services/language/languageService';


const MIME_TYPE = getBestMimeType();

function getBestMimeType(){
  if(!!global.MediaRecorder){
    if(MediaRecorder.isTypeSupported('video/webm; codecs=vp9,opus')){
      return 'video/webm; codecs=vp9,opus';
    }else{
      return 'video/webm; codecs=vp8,opus';
    }
  }
}

let recordingChunks = [];

const readAsArrayBuffer = async function(blob) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.readAsArrayBuffer(blob);
        reader.onloadend = () => { resolve(reader.result); };
        reader.onerror = (ev) => { reject(ev.error); };
    });
};

const injectMetadata = async function(blob) {
    const decoder = new Decoder();
    const reader = new Reader();
    reader.logging = false;
    reader.drop_default_duration = false;

   return await readAsArrayBuffer(blob).then((buffer) => {
        const elms = decoder.decode(buffer);
        elms.forEach((elm) => { reader.read(elm); });
        reader.stop();

        var refinedMetadataBuf = tools.makeMetadataSeekable(
            reader.metadatas, reader.duration, reader.cues);
        var body = buffer.slice(reader.metadataSize);

        const result = new Blob([refinedMetadataBuf, body],
            {type: blob.type});

       return result;
    });
};

let wasCancelled = false;

const getDisplayMedia = function(){
    return navigator.mediaDevices.getDisplayMedia({video:true});
};
const getUserMedia = async function(){
  try{
    let userMedia = await navigator.mediaDevices.getUserMedia({audio:true});
    return userMedia;
  }catch(err){
    return null;
  }
};

const downloadRecording = async function(e, recordingData){
// usage: pass in a webm blob
  let updatedBlob = await injectMetadata(recordingData);
  
  let a = document.createElement("a");
  document.body.appendChild(a);
  a.style = "display: none";
  // let url = URL.createObjectURL(e.data);
  let url = URL.createObjectURL(updatedBlob);
  a.href = url;
  a.download = 'Recorded_File.webm';
  a.click();
  window.URL.revokeObjectURL(url);
};

const stackChunks = function(e){
  recordingChunks.push(e.data);
};

const handleRecording = function(e){
  if(!wasCancelled){
    let options = {mimeType: MIME_TYPE};
    const recordingData = new Blob(recordingChunks, options);
        if(store.getters.getRoomId && recordingData.size < 209715199){
          downloadRecording(e, recordingData);
        }else{
          downloadRecording(e, recordingData);
        }
        
        stopRecord();
  }

  /* if error dialog is open and stopsharing button was pressed,
   then dialog should be closed anyway. */
  store.dispatch('setShowAudioRecordingError', false);
};
const registerRecorderEvents = function(){
    // store.getters.getRecorder.removeEventListener('dataavailable', stackChunks);
    // store.getters.getRecorder.addEventListener('dataavailable', stackChunks);
    // store.getters.getRecorder.removeEventListener('stop', handleRecording);
    // store.getters.getRecorder.addEventListener('stop', handleRecording);
    store.getters.getRecorder.ondataavailable = stackChunks;
    store.getters.getRecorder.onstop = handleRecording;
    let streams = store.getters.getRecorderStream.getTracks();
    streams.forEach(stream=>{
      stream.removeEventListener('ended', toggleRecord);
      stream.addEventListener('ended', toggleRecord);
    });
};

function createRemoteAudioStream(){
  let remoteAudioTrack = store.getters.getCurrentAudioTrack;
  if(remoteAudioTrack){
    let remoteAudioTrackStream = remoteAudioTrack.mediaStreamTrack;
    let remoteMediaStream = new store.getters.getCurrentAudioTrack._MediaStream();
    remoteMediaStream.addTrack(remoteAudioTrackStream);
    return remoteMediaStream;
  }else{
    return null;
  }
}

async function activateRecord(){
  let roomId = store.getters.getRoomId ? store.getters.getRoomId : 'testRoom';
  let userId = store.getters.accountUser ? store.getters.accountUser.id : 'GUEST';
  insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_Recording_Start', {'roomId': roomId, 'userId': userId}, null);
  recordingChunks = [];
  wasCancelled = false;
  //start record
  
  let displayMedia = await getDisplayMedia();
  store.dispatch('setLocalDisplayMedia', displayMedia);
  let userMedia = await getUserMedia();  
  store.dispatch('setLocalUserMedia', userMedia);
  
  //if session is alreadyActive than set the remote tracks (can be null)
  let remoteMediaStream = createRemoteAudioStream();
  createCombinedMediaStreams(remoteMediaStream);
}

function combineAudioStreams(streams) {
  let audioContext = new (window.AudioContext || window.webkitAudioContext)();
  let userMedia = store.getters.getLocalUserMedia;
  let newStreams = [userMedia, ...streams];
  const dest = audioContext.createMediaStreamDestination();
  newStreams.forEach(stream => {
    if(stream){
      const source = audioContext.createMediaStreamSource(stream);
      source.connect(dest);
    }
  });
  return dest.stream;
}

function createCombinedMediaStreams(remoteMediaStream){
  let displayMedia = store.getters.getLocalDisplayMedia;
  let combinedAudioStreams = combineAudioStreams([remoteMediaStream]);
  let combinedMediaStreams = new MediaStream([...combinedAudioStreams.getTracks(), ...displayMedia.getVideoTracks()]);
  store.dispatch('setRecorderStearm', combinedMediaStreams);
  createMediaRecorder(); 
}

function createMediaRecorder (){
  //record true only after recorderStream selected
  store.dispatch('setIsRecording', true);
  let options = {mimeType: MIME_TYPE};
  if(!!global.MediaRecorder){
    store.dispatch('setRecorder', new MediaRecorder(store.getters.getRecorderStream, options));
  }
  console.log(store.getters.getRecorder);
  registerRecorderEvents();
  store.getters.getRecorder.start();
}

function stopRecord(cancelled){
  let roomId = store.getters.getRoomId ? store.getters.getRoomId : 'testRoom';
  let userId = store.getters.accountUser ? store.getters.accountUser.id : 'GUEST';
  insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_Recording_End', {'roomId': roomId, 'userId': userId}, null);
  if(cancelled){
    wasCancelled = true;
  }
  store.dispatch('setIsRecording', false);
  if(store.getters.getRecorder){
    store.getters.getRecorder.stop();
    let tracks = store.getters.getRecorderStream.getTracks();
    tracks.forEach((track)=>{
      track.stop();
    });
  }
}

async function toggleRecord(isTutor){
    if(!store.getters.getIsRecording){
      if(global.location.pathname === '/studyroom'){
        let userMedia = await getUserMedia();
        if(!userMedia){
          let msg = LanguageService.getValueByKey('tutor_microphone_blocked')
          alert(msg)
          return
        }  
      }
      if(isTutor){
        store.dispatch('setShowUserConsentDialog', true);
      }else{
        activateRecord();
      }
    }else{
      stopRecord();
    }
  }
  export default{
    toggleRecord,
    stopRecord,
    createRemoteAudioStream,
    createCombinedMediaStreams,
    activateRecord
  }