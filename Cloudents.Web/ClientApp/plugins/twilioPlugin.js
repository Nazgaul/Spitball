import insightService from '../services/insightService';
import * as routeNames from '../routes/routeNames.js';
import {twilio_SETTERS} from '../store/constants/twilioConstants.js';
import {studyRoom_SETTERS} from '../store/constants/studyRoomConstants.js';
import STORE from '../store/index.js'
//https://media.twiliocdn.com/sdk/js/video/releases/2.2.0/docs
const REMOTE_TRACK_DOM_ELEMENT = 'remoteTrack';
const LOCAL_TRACK_DOM_ELEMENT = 'localTrack';
const AUDIO_TRACK_NAME = 'audioTrack';
const VIDEO_TRACK_NAME = 'videoTrack';
const SCREEN_TRACK_NAME = 'screenTrack';

const CURRENT_STATE="1";
let intervalTime = null;

function _changeState(localParticipant) {
   let stuffToSend =  {
      type: CURRENT_STATE,
      tab : STORE.getters.getActiveNavEditor,
      canvasTab : {
         tab: STORE.getters.getCurrentSelectedTab,
         canvas: STORE.getters.canvasDataStore
      },
      mute : STORE.getters.getIsAudioParticipants,
      fullScreen: null
   };
   localParticipant.tracks.forEach((track) => {
      if(track.trackName === VIDEO_TRACK_NAME && STORE.getters.getIsFullScreen 
         || track.trackName === SCREEN_TRACK_NAME){
         stuffToSend.fullScreen = track.trackSid;
         // let shareVideoCamera = {
         //    type: "openFullScreen",
         //    data: `remoteTrack_${track.trackSid}`

         // };
         // STORE.dispatch('sendDataTrack',JSON.stringify(shareVideoCamera))
      }
      // if(track.trackName === SCREEN_TRACK_NAME){
      //    let shareScreen = {
      //       type: "openFullScreen",
      //       data: `remoteTrack_${track.trackSid}`
      //    };
      //    STORE.dispatch('sendDataTrack',JSON.stringify(shareScreen))
      // }
   });
   STORE.dispatch('sendDataTrack',JSON.stringify(stuffToSend));
}

// function _initStudentJoined(localParticipant){
//    // update editor tab:

//    let editorTab = {
//       type: "updateActiveNav",
//       data: STORE.getters.getActiveNavEditor
//    };
//    STORE.dispatch('sendDataTrack',JSON.stringify(editorTab));

//    // update canvas tab:
//    let canvasTab = {
//       type: "updateTabById",
//       data:{
//          tab: STORE.getters.getCurrentSelectedTab,
//          canvas: STORE.getters.canvasDataStore
//       }
//    }
//    STORE.dispatch('sendDataTrack',JSON.stringify(canvasTab));

//    // share screen & video full screen:
//    Array.from(localParticipant.tracks.values()).forEach((track) => {
//          if(track.trackName === VIDEO_TRACK_NAME && STORE.getters.getIsFullScreen){
//             let shareVideoCamera = {
//                type: "openFullScreen",
//                data: `remoteTrack_${track.trackSid}`

//             };
//             STORE.dispatch('sendDataTrack',JSON.stringify(shareVideoCamera))
//          }
//          if(track.trackName === SCREEN_TRACK_NAME){
//             let shareScreen = {
//                type: "openFullScreen",
//                data: `remoteTrack_${track.trackSid}`
//             };
//             STORE.dispatch('sendDataTrack',JSON.stringify(shareScreen))
//          }
//    });
//    // room muted by tutor:
//    if(!STORE.getters.getIsAudioParticipants){
//       let normalizedData = {
//          type: "toggleParticipantsAudio",
//          data: STORE.getters.getIsAudioParticipants
//       };
//       STORE.dispatch('sendDataTrack',JSON.stringify(normalizedData))
//    }
// }
function _detachTracks(tracks){
   tracks.forEach((track) => {
      if (track?.detach) {
         // store.commit(twilio_SETTERS.DELETE_REMOTE_VIDEO_TRACK,track)
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
function _toggleTrack(tracks,trackType,value){
   let {track} = tracks.find(track=>track.kind === trackType);
   if(track){
      // value: FLASE - USER TURNED OFF / TRUE - USER TURNED ON
      if(!value && track.isEnabled){
         track.disable()
      }
      if(value && !track.isEnabled){
         track.enable()
      }
   }
}
function _twilioListeners(room,store) { 
   // romote participants
   room.participants.forEach((participant) => {
      let tracks = Array.from(participant.tracks.values());
      tracks.forEach((track) => {
         if(track.kind === 'video'){
            console.log(participant,track)
            store.commit(twilio_SETTERS.ADD_REMOTE_VIDEO_TRACK,track)
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
      console.log('trackPublished')
      if(store.getters.getRoomIsTutor && track.trackName === SCREEN_TRACK_NAME){
         let videoElementId = `remoteTrack_${track.trackSid}`
         let transferDataObj = {
            type: "openFullScreen",
            data: videoElementId
         };
         let normalizedData = JSON.stringify(transferDataObj);
         store.dispatch('sendDataTrack',normalizedData)
      }
   })


   // room events
   room.on('trackSubscribed', (track) => {
      console.log('trackSubscribed',track)
      _insightEvent('TwilioTrackSubscribed', track, null);
      // _detachTracks([track],store)
   })
   room.on('trackUnsubscribed', (track) => {
      _insightEvent('TwilioTrackUnsubscribed', track, null);
      _detachTracks([track],store);
   })
   room.on('participantReconnected', () => {
   })
   room.on('participantReconnecting', () => {
   })
   room.on('trackDisabled', () => {
   })
   room.on('trackEnabled', () => {
      console.log('trackEnabled')
   })
   room.on('trackRemoved',()=>{
   })
   room.on('trackStarted', (track) => {
      if(track.kind === 'video'){
         console.log(track)
         store.commit(twilio_SETTERS.ADD_REMOTE_VIDEO_TRACK,track)
      }
      if(track.kind === 'audio'){   
         let previewContainer = document.getElementById(REMOTE_TRACK_DOM_ELEMENT);
         track.detach().forEach((detachedElement) => {
            detachedElement.remove();
         });
         previewContainer.appendChild(track.attach());
      }
   })














   // room tracks events: 
   room.on('trackMessage', (message) => {
      console.log(message);
      let data = JSON.parse(message);
      if (data.type == CURRENT_STATE) {
         //We put our code in here
         store.dispatch('updateAudioToggleByRemote',data.mute)
         store.dispatch('updateFullScreen',data.fullScreen)
         store.dispatch('updateActiveNavEditor', data.tab)
        // store.dispatch('tempWhiteBoardTabChanged',data.canvasTab)
         store.dispatch('sendDataTrack', JSON.stringify( {
            type : 'ACK'
         }));
         return;
      }
      console.log(message);
      if (data.type == 'ACK') {
         clearInterval(intervalTime)
         return;
         //Remove the interval
      }
      store.dispatch('dispatchDataTrackJunk',data)
   })
   // room.on('trackPublished', (track) => {
     

   //    // if(store.getters.getRoomIsTutor){
   //    //    if(track.kind === 'data'){
   //    //          console.log('y')
   //    //          _initStudentJoined(room.localParticipant)
   //    //    }
   //    // }
       
   //    console.log('x',track)
   // })
   room.on('trackUnpublished', () => {
   })
   // room connections events:
   room.on('participantConnected', (participant) => {
     
     

      

      if(store.getters.getRoomIsTutor){
         store.commit('setComponent', 'simpleToaster_userConnected');
         console.log(participant);
         participant.on('trackSubscribed',(track)=>{
            if(track.kind === 'data'){
               intervalTime= setInterval(() => {
                  _changeState(room.localParticipant);

               },100);
               //_initStudentJoined(room.localParticipant)
            }
         })
        
        // participant.tracks.forEach(publication => trackPublished(participant, publication));
      }
      _insightEvent('TwilioParticipantConnected', participant, null);
   })


   room.on('participantDisconnected', (participant) => {
      if(store.getters.getRoomIsTutor){
         store.commit('setComponent', 'simpleToaster_userLeft');
      }
      _insightEvent('TwilioParticipantDisconnected', participant, null);
      _detachTracks(Array.from(participant.tracks.values()),store)
   })
   room.on('reconnecting', () => {
      _insightEvent('reconnecting', null, null);
   })
   room.on('disconnected', (dRoom, error) => {
      if (error?.code) {
         _insightEvent('TwilioDisconnected', {'errorCode': error.code}, null);
         console.error(`Twilio Error: Code: ${error.code}, Message: ${error.message}`)
      }
      dRoom.localParticipant.tracks.forEach(function (track) {
         _detachTracks([track],store);
      });
      if(!store.getters.getRoomIsTutor){
         store.commit(studyRoom_SETTERS.ROOM_ACTIVE,false)
         store.dispatch('updateReviewDialog',true)
      }
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
            });
            _debugMode = mutation.payload.query?.debug ? 'debug' : 'off';
         }
         if (mutation.type === twilio_SETTERS.JWT_TOKEN) {
            if(_activeRoom?.state == 'connected'){
               return
            }
            let isRoomNeedPayment = store.getters.getRoomIsNeedPayment;
            let isRoomStudent = !store.getters.getRoomIsTutor;
            if(isRoomStudent && isRoomNeedPayment){return}

            dataTrack = new twillioClient.LocalDataTrack();
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
               _toggleTrack(tracks,'video',mutation.payload);
            }
            if (mutation.type === twilio_SETTERS.AUDIO_TOGGLE){
               let tracks = [];
               _activeRoom.localParticipant.tracks.forEach(track=>{tracks.push(track)})
               _toggleTrack(tracks,'audio',mutation.payload);
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
                        store.commit('setComponent', 'errorToaster_notBrowser');
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
                           store.commit('setComponent', 'errorToaster_permissionDenied');
                        }
                        return
                     }
                     store.commit('setComponent', 'errorToaster_notScreen');
                  })
               }else{
                  if(_localScreenTrack){
                     _localScreenTrack.stop()
                     _localScreenTrack = null;
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
               }
            }
            if (mutation.type === twilio_SETTERS.TOGGLE_TUTOR_FULL_SCREEN){
               let normalizedData = {
                  type: "openFullScreen",
               };
               if(mutation.payload){
                  let videoTrack = Array.from(_activeRoom.localParticipant.videoTracks.values())[0];
                  let videoElementId = `remoteTrack_${videoTrack.trackSid}`
                  normalizedData.data = videoElementId
               }
               store.dispatch('sendDataTrack',JSON.stringify(normalizedData))
            }
            if(mutation.type === twilio_SETTERS.TOGGLE_AUDIO_PARTICIPANTS){
               let normalizedData = {
                  type: "toggleParticipantsAudio",
                  data: mutation.payload
               };
               store.dispatch('sendDataTrack',JSON.stringify(normalizedData))
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