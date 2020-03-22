import insightService from '../services/insightService';
import * as routeNames from '../routes/routeNames.js';
import {twilio_SETTERS} from '../store/constants/twilioConstants.js';
import {studyRoom_SETTERS} from '../store/constants/studyRoomConstants.js';
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
   });

   // local participant events
   room.localParticipant.on('trackStopped',(track)=>{
      if(track.trackName === VIDEO_TRACK_NAME){
         store.commit(twilio_SETTERS.VIDEO_AVAILABLE,false)
      }
      if(track.trackName === AUDIO_TRACK_NAME){
         store.commit(twilio_SETTERS.AUDIO_AVAILABLE,false)
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
         store.commit(twilio_SETTERS.FULL_SCREEN_AVAILABLE,false);
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
         store.commit(twilio_SETTERS.FULL_SCREEN_AVAILABLE,true);
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
      if(store.getters.getRoomIsTutor){
         store.commit('setToaster', 'simpleToaster_userConnected');
      }
      _insightEvent('TwilioParticipantConnected', participant, null);
   })
   room.on('participantDisconnected', (participant) => {
      debugger
      if(store.getters.getRoomIsTutor){
         store.commit('setToaster', 'simpleToaster_userLeft');
      }
      _insightEvent('TwilioParticipantDisconnected', participant, null);
      _detachTracks(Array.from(participant.tracks.values()))
   })
   room.on('reconnecting', (reconnectingError) => {
      debugger
      _insightEvent('reconnecting', null, null);
   })
   room.on('disconnected', (dRoom, error) => {
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
      let _debugMode;
      store.subscribe((mutation) => {
         if (mutation.type === 'setRouteStack' && mutation.payload.name === routeNames.StudyRoom) {
            import('twilio-video').then(async (Twilio) => { 
               twillioClient = Twilio;
               dataTrack = new twillioClient.LocalDataTrack();
            });
            _debugMode = mutation.payload.query?.debug ? 'debug' : 'off';
         }
         if (mutation.type === twilio_SETTERS.JWT_TOKEN) {
            let jwtToken = mutation.payload;
            let options = {
               logLevel: _debugMode,
               tracks: [dataTrack],
               networkQuality: { // this is reserved down the road
                  local: 3,
                  remote: 3
               }
            };
            _insightEvent('connectToRoom', {'token': jwtToken}, null);
            twillioClient.connect(jwtToken, options).then((room) => {
               _activeRoom = room; // for global using in this plugin
               _insightEvent('TwilioConnect', _activeRoom, null);
               _twilioListeners(_activeRoom,store); // start listen to twilio events;
               store.commit(studyRoom_SETTERS.ROOM_ACTIVE,true)

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
                           _setLocalVideoTrack(value)
                        }
                        if(value.name === AUDIO_TRACK_NAME){
                           _setLocalAudioTrack(value);
                        }
                     }
                  })
               })
            })
         }
         if(_activeRoom){
            if (mutation.type === twilio_SETTERS.DATA_TRACK){
               dataTrack.send(mutation.payload);
            }
            if (mutation.type === twilio_SETTERS.VIDEO_TOGGLE){
               let tracks = [];
               _activeRoom.localParticipant.tracks.forEach(track=>{tracks.push(track)})
               _toggleTrack(tracks,'video');
            }
            if (mutation.type === twilio_SETTERS.AUDIO_TOGGLE){
               let tracks = [];
               _activeRoom.localParticipant.tracks.forEach(track=>{tracks.push(track)})
               _toggleTrack(tracks,'audio');
            }
            if (mutation.type === twilio_SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE){
               if(mutation.payload && !_localScreenTrack){ 
                  navigator.mediaDevices.getDisplayMedia({video:true,audio: false}).then(stream=>{
                     _localScreenTrack = new twillioClient.LocalVideoTrack(stream.getTracks()[0],{name:SCREEN_TRACK_NAME});
                     if(_localVideoTrack){
                        _unPublishTrack(_activeRoom,_localVideoTrack)
                     }
                     _publishTrack(_activeRoom,_localScreenTrack);
                     store.commit(twilio_SETTERS.VIDEO_AVAILABLE,true)
                     _localScreenTrack.on('stopped',(track)=>{
                        _unPublishTrack(_activeRoom,track)
                        store.commit(twilio_SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE,false);
                        if(_localVideoTrack){
                           _publishTrack(_activeRoom,_localVideoTrack)
                        }else{
                           //why it is not all the time
                           store.commit(twilio_SETTERS.VIDEO_AVAILABLE,false)
                        }
                     })
                  }).catch( error =>{
                     error = error || {};
                     let d = {...{
                        errorMessage:error.message,
                        errorname:error.name},
                        ...{id: _activeRoom.name}
                     };
                     _insightEvent('StudyRoom_ShareScreenBtn_showScreen', d, null);
                     if(error === "notBrowser") {
                        store.commit('setToaster', 'errorToaster_notBrowser');
                        return;
                     }
                     if(error.name === "NotAllowedError") {
                        if (error.message === "Permission denied") {
                           store.commit(twilio_SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE,false);
                           if(_localVideoTrack){
                              _publishTrack(_activeRoom,_localVideoTrack)
                           }
                           return;
                        }
                        if (error.message === "Permission denied by system") {
                           store.commit('setToaster', 'errorToaster_permissionDenied');
                        }
                        return
                     }
                     store.commit('setToaster', 'errorToaster_notScreen');
                  })
               }else{
                  if(_localScreenTrack){
                     _localScreenTrack.stop()
                  }
               }
            }
            if (mutation.type === twilio_SETTERS.CHANGE_VIDEO_DEVICE){
               if(_localVideoTrack){
                  _unPublishTrack(_activeRoom,_localVideoTrack)
               }
               let params = {deviceId: {exact: mutation.payload},name:VIDEO_TRACK_NAME}
               twillioClient.createLocalVideoTrack(params).then(track=>{
                  _setLocalVideoTrack(track);
               })
            }
            if (mutation.type === twilio_SETTERS.CHANGE_AUDIO_DEVICE){
               if(_localAudioTrack){
                  _unPublishTrack(_activeRoom,_localAudioTrack)
               }
               let params = {deviceId: {exact: mutation.payload},name:AUDIO_TRACK_NAME}
               twillioClient.createLocalAudioTrack(params).then(track=>{
                  _setLocalAudioTrack(track);
               })
            }
            if (mutation.type === studyRoom_SETTERS.ROOM_ACTIVE){
               if(!mutation.payload && _activeRoom){
                  _activeRoom.disconnect()
                  _localVideoTrack = null;
                  _localAudioTrack = null;
                  _localScreenTrack = null;
   
                  store.commit(twilio_SETTERS.VIDEO_AVAILABLE,false);
                  store.commit(twilio_SETTERS.AUDIO_AVAILABLE,false)
                  store.commit(twilio_SETTERS.FULL_SCREEN_AVAILABLE,false);
               }
            }
         }
      })

      // plugin functions:
      function _setLocalVideoTrack(track){
         const localMediaContainer = document.getElementById(LOCAL_TRACK_DOM_ELEMENT);
         let videoTag = localMediaContainer.querySelector("video");
         if (videoTag) {localMediaContainer.removeChild(videoTag)}
         localMediaContainer.appendChild(track.attach());
         _localVideoTrack = track;
         _publishTrack(_activeRoom,track)
         store.commit(twilio_SETTERS.VIDEO_AVAILABLE,true)
      }
      function _setLocalAudioTrack(track){
         _localAudioTrack = track;
         _publishTrack(_activeRoom,_localAudioTrack)
         store.commit(twilio_SETTERS.AUDIO_AVAILABLE,true)
      }
   }
}