import insightService from '../services/insightService';
import * as routeNames from '../routes/routeNames.js';
import {SETTERS} from '../store/constants/twilioConstants.js';
//https://media.twiliocdn.com/sdk/js/video/releases/2.2.0/docs
const REMOTE_TRACK_DOM_ELEMENT = 'remoteTrack';
const LOCAL_TRACK_DOM_ELEMENT = 'localTrack';
const AUDIO_TRACK_NAME = 'audioTrack';
const VIDEO_TRACK_NAME = 'videoTrack';
const SCREEN_TRACK_NAME = 'screenTrack';

function _detachTracks(tracks){
   tracks.forEach((track) => {
      if (track?.detach) {
         track.detach().forEach((detachedElement) => {
            detachedElement.remove();
         });
      }
   });
}
// function _attachTracks(tracks,container){
  
// }
function _insightEvent(...args) {
   //use https://www.npmjs.com/package/vue-application-insights
   insightService.track.event(insightService.EVENT_TYPES.LOG, ...args);
}
function _publishTrack(activeRoom,track){
   activeRoom.localParticipant.publishTrack(track);
}
function _unPublishTrack(activeRoom,track){
   activeRoom.localParticipant.unpublishTrack(track);
}
function _toggleTrack(tracks,trackType){
   let {track} = tracks.find(track=>track.kind === trackType);
   if(track){
      if(track.isEnabled){
         track.disable()
      }else{
         track.enable()
      }
   }
}
function _twilioListeners(room,store) { 
   // romote participants
   room.participants.forEach((participant) => {
      let previewContainer = document.getElementById(REMOTE_TRACK_DOM_ELEMENT);
      let tracks = Array.from(participant.tracks.values());
      tracks.forEach((track) => {
         if (track.attach) {
            previewContainer.appendChild(track.attach());
         }
      });
     // _attachTracks(tracks, previewContainer)
   });

   // local participant events
   room.localParticipant.on('trackStopped',(track)=>{
      if(track.trackName === VIDEO_TRACK_NAME){
         store.commit(SETTERS.VIDEO_AVAILABLE,false)
      }
      if(track.trackName === AUDIO_TRACK_NAME){
         store.commit(SETTERS.AUDIO_AVAILABLE,false)
      }
   })
   room.localParticipant.on('networkQualityLevelChanged', (networkQualityLevel,networkQualityStats) => {
      _insightEvent('networkQuality',networkQualityStats, networkQualityLevel)
   });
   room.localParticipant.on('trackPublished',(track)=>{
      debugger;
   })


   // room events
   room.on('trackSubscribed', (track) => {
      debugger
      _insightEvent('TwilioTrackSubscribed', track, null);
      _detachTracks([track])
   })
   room.on('trackUnsubscribed', (track, publication, participant) => {
      debugger
      if(track.kind === 'video'){
         store.commit(SETTERS.FULL_SCREEN_AVAILABLE,false);
     }
      _insightEvent('TwilioTrackUnsubscribed', track, null);
      _detachTracks([track]);
   })
   room.on('participantReconnected', () => {
      debugger
   })
   room.on('participantReconnecting', () => {
      debugger
   })
   room.on('trackDisabled', () => {
      debugger
   })
   room.on('trackEnabled', () => {
      debugger
   })
   room.on('trackRemoved',()=>{
      debugger
   })
   room.on('trackStarted', (track) => {
      debugger
      let previewContainer = document.getElementById(REMOTE_TRACK_DOM_ELEMENT);
      if(track.kind === 'video'){
         let videoTag = previewContainer.querySelector("video");
         if (videoTag) {
            previewContainer.removeChild(videoTag);
         }
         previewContainer.appendChild(track.attach());
         store.commit(SETTERS.FULL_SCREEN_AVAILABLE,true);
      }
      if(track.kind === 'audio'){   
         track.detach().forEach((detachedElement) => {
            detachedElement.remove();
         });
         previewContainer.appendChild(track.attach());
      }
   })















   // room tracks events: 
   room.on('trackMessage', (message) => {
      let data = JSON.parse(message)
      _insightEvent('trackMessage', data, null);
      store.dispatch('dispatchDataTrackJunk',data)
   })
   room.on('trackPublished', (track) => {
      debugger
   })
   room.on('trackUnpublished', (track) => {
      debugger
   })
   // room connections events:
   room.on('participantConnected', (participant) => {
      debugger
      _insightEvent('TwilioParticipantConnected', participant, null);
   })
   room.on('participantDisconnected', (participant) => {
      debugger
      _insightEvent('TwilioParticipantDisconnected', participant, null);
      _detachTracks(Array.from(participant.tracks.values()))
   })
   room.on('reconnecting', (reconnectingError) => {
      debugger
      _insightEvent('reconnecting', null, null);
   })
   room.on('disconnected', (dRoom, error) => {
      debugger
      if (error?.code) {
         _insightEvent('TwilioDisconnected', {'errorCode': error.code}, null);
         console.error(`Twilio Error: Code: ${error.code}, Message: ${error.message}`)
      }
      dRoom.localParticipant.tracks.forEach(function (track) {
         _detachTracks([track]);
      });
   })
}
export default () => {
   return store => {
      let twillioClient;
      let dataTrack;
      let _activeRoom = null;
      let _localVideoTrack = null;
      let _localAudioTrack = null;
      let _localScreenTrack = null;
      store.subscribe((mutation) => {
         if (mutation.type === 'setRouteStack' && mutation.payload === routeNames.StudyRoom) {
            import('twilio-video').then(async (Twilio) => { 
               twillioClient = Twilio;
               dataTrack = new twillioClient.LocalDataTrack();
            });
         }
         if (mutation.type === SETTERS.JWT_TOKEN) {
            let jwtToken = mutation.payload;
            let options = {
               logLevel: 'debug', // from query string
               tracks: [dataTrack],
               networkQuality: { // this is reserved down the road
                  local: 3,
                  remote: 3
               }
            };
            _insightEvent('connectToRoom', {'token': jwtToken}, null);
            //do the end session
            twillioClient.connect(jwtToken, options).then((room) => {
               _activeRoom = room; // for global using in this plugin
               _insightEvent('TwilioConnect', _activeRoom, null);
               _twilioListeners(_activeRoom,store); // start listen to twilio events;

               let {createLocalVideoTrack,createLocalAudioTrack} = twillioClient;

               // TODO: fix it audio & video
               // let videoDeviceId = localStorage.getItem('sb-videoTrackId');
               // let videoParams = videoDeviceId ? {deviceId: {exact: videoDeviceId}} : {};


               
               Promise.allSettled([
                  createLocalVideoTrack({name:VIDEO_TRACK_NAME}),
                  createLocalAudioTrack({name:AUDIO_TRACK_NAME})
               ]).then((tracks) => {
                  tracks.forEach(({value}) => {
                     if(value){
                        if(value.name === VIDEO_TRACK_NAME){
                           //#1 why not the same code
                           const localMediaContainer = document.getElementById(LOCAL_TRACK_DOM_ELEMENT);
                           localMediaContainer.appendChild(value.attach());
                           _localVideoTrack = value;
                           _publishTrack(_activeRoom,value)
                           store.commit(SETTERS.VIDEO_AVAILABLE,true)
                        }
                        if(value.name === AUDIO_TRACK_NAME){  
                           _localAudioTrack = value;
                           _publishTrack(_activeRoom,value)
                           store.commit(SETTERS.AUDIO_AVAILABLE,true)
                        }
                     }
                  })
               })
            })
         }
         if (mutation.type === SETTERS.DATA_TRACK){
            if(_activeRoom){
               dataTrack.send(mutation.payload);
            }
         }
         if (mutation.type === SETTERS.VIDEO_TOGGLE){
            let tracks = [];
            _activeRoom.localParticipant.tracks.forEach(track=>{tracks.push(track)})
            _toggleTrack(tracks,'video');
         }
         if (mutation.type === SETTERS.AUDIO_TOGGLE){
            let tracks = [];
            _activeRoom.localParticipant.tracks.forEach(track=>{tracks.push(track)})
            _toggleTrack(tracks,'audio');
         }
         //screen share broadcast toggle
         if (mutation.type === SETTERS.SCREEN_SHARE){
            if(mutation.payload){ 
               //copy the logic from the previous to add toaster
               // remove the tooltip
               navigator.mediaDevices.getDisplayMedia({video:true,audio: false}).then(stream=>{
                  _localScreenTrack = new twillioClient.LocalVideoTrack(stream.getTracks()[0],{name:SCREEN_TRACK_NAME});
                  if(_localVideoTrack){
                     _unPublishTrack(_activeRoom,_localVideoTrack)
                  }
                  _publishTrack(_activeRoom,_localScreenTrack);
                  store.commit(SETTERS.VIDEO_AVAILABLE,true)
                  _localScreenTrack.on('stopped',(track)=>{
                     _unPublishTrack(_activeRoom,track)
                     store.commit(SETTERS.SCREEN_SHARE,false);
                     if(_localVideoTrack){
                        _publishTrack(_activeRoom,_localVideoTrack)
                     }else{
                        //why it is not all the time
                        store.commit(SETTERS.VIDEO_AVAILABLE,false)
                     }
                  })
               })
            }else{
               _localScreenTrack.stop()
            }
         }
         //change video devices
         if (mutation.type === SETTERS.VIDEO_TRACK){
            if(_localVideoTrack){
               _unPublishTrack(_activeRoom,_localVideoTrack)
            }
            let params = {deviceId: {exact: mutation.payload},name:VIDEO_TRACK_NAME}
            twillioClient.createLocalVideoTrack(params).then(track=>{
               //#1 why not the same code
               _localVideoTrack = track;
               _publishTrack(_activeRoom,_localVideoTrack)

               const localMediaContainer = document.getElementById(LOCAL_TRACK_DOM_ELEMENT);
               let videoTag = localMediaContainer.querySelector("video");
               if (videoTag) {
                  localMediaContainer.removeChild(videoTag);
               }
               localMediaContainer.appendChild(track.attach());

               store.commit(SETTERS.FULL_SCREEN_AVAILABLE,true);
               store.commit(SETTERS.VIDEO_AVAILABLE,true)
            })
         }
         if (mutation.type === SETTERS.AUDIO_TRACK){
            if(_localAudioTrack){
               _unPublishTrack(_activeRoom,_localAudioTrack)
            }
            let params = {deviceId: {exact: mutation.payload},name:AUDIO_TRACK_NAME}
            twillioClient.createLocalAudioTrack(params).then(track=>{
               _localAudioTrack = track;
               _publishTrack(_activeRoom,_localAudioTrack)
               store.commit(SETTERS.AUDIO_AVAILABLE,true)
            })
         }
      })
   }
}