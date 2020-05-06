import insightService from '../services/insightService';
import * as routeNames from '../routes/routeNames.js';
import {twilio_SETTERS} from '../store/constants/twilioConstants.js';
import {studyRoom_SETTERS} from '../store/constants/studyRoomConstants.js';
import STORE from '../store/index.js'
//https://media.twiliocdn.com/sdk/js/video/releases/2.2.0/docs
// const REMOTE_TRACK_DOM_ELEMENT = 'remoteTrack';
// const LOCAL_TRACK_DOM_ELEMENT = 'localTrack';
const AUDIO_TRACK_NAME = 'audioTrack';
const VIDEO_TRACK_NAME = 'videoTrack';
const SCREEN_TRACK_NAME = 'screenTrack';
const CURRENT_STATE_UPDATE = '1';
const CURRENT_STATE_UPDATED = '2';
let isTwilioStarted = false;

let intervalTime = null;

   // STORE.commit(twilio_SETTERS.ADD_REMOTE_VIDEO_TRACK,track)
function _changeState(localParticipant) {
   if(!STORE.getters.getRoomIsTutor) return;

   let stuffToSend =  {
      type: CURRENT_STATE_UPDATE,
      // tab : STORE.getters.getActiveNavEditor,
      // canvasTab : {
      //    tab: STORE.getters.getCurrentSelectedTab,
      //    canvas: STORE.getters.canvasDataStore
      // },
      // fullScreen: null
   };
   localParticipant.tracks.forEach((track) => {
      if(track.trackName === VIDEO_TRACK_NAME && STORE.getters.getIsFullScreen 
         || track.trackName === SCREEN_TRACK_NAME){
         stuffToSend.fullScreen = localParticipant.identity.split('_')[0]
      }
   });
   // if(!STORE.getters.getRoomIsTutor) return;
   // let stuffToSend =  {
   //    type: CURRENT_STATE_UPDATE,
   //    tab : STORE.getters.getActiveNavEditor,
   //    canvasTab : {
   //       tab: STORE.getters.getCurrentSelectedTab,
   //       canvas: STORE.getters.canvasDataStore
   //    },
   //   // mute : STORE.getters.getIsAudioParticipants,
   //    fullScreen: null
   // };
   // localParticipant.tracks.forEach((track) => {
   //    if(track.trackName === VIDEO_TRACK_NAME && STORE.getters.getIsFullScreen 
   //       || track.trackName === SCREEN_TRACK_NAME){
   //       stuffToSend.fullScreen = track.trackSid;
   //    }
   // });
   STORE.dispatch('sendDataTrack',JSON.stringify(stuffToSend));
}

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
   _addParticipantTrack(track,activeRoom.localParticipant)

   //On share screen we want to update all the users in the room
   _changeState( activeRoom.localParticipant);
}
function _unPublishTrack(activeRoom,track){
   activeRoom.localParticipant.unpublishTrack(track);
   _deleteParticipantTrack(track,activeRoom.localParticipant)

   //On share screen we want to update all the users in the room
   _changeState( activeRoom.localParticipant);
}
function _toggleTrack(tracks,trackType,value){
   let {track} = tracks.find(track=>track.kind === trackType);
   if(track){
      // value: FLASE - USER TURNED OFF / TRUE - USER TURNED ON
      if(!value){
         track.disable()
      }
      if(value){
         track.enable()
      }
   }
}
function _twilioListeners(room,store) { 
   store.commit(studyRoom_SETTERS.ROOM_PARTICIPANT_COUNT,room.participants.size)
   if(!store.getters.getRoomIsBroadcast && room.localParticipant.tracks.size){
      store.commit(studyRoom_SETTERS.ROOM_ACTIVE,true)
      _changeState(room.localParticipant);
      if(store.getters.getRoomIsTutor){
         if(!isTwilioStarted && store.getters.getIsShareScreen){
            store.commit(twilio_SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE,true)
            isTwilioStarted = true;
         }
      }
   }
   store.commit(studyRoom_SETTERS.ADD_ROOM_PARTICIPANT,room.localParticipant)
   // romote participants events:
   room.participants.forEach((participant) => {
      store.commit(studyRoom_SETTERS.ADD_ROOM_PARTICIPANT,participant)
      let tracks = Array.from(participant.tracks.values());
      tracks.forEach((track) => {
         if(track.kind === 'video'){
            _addParticipantTrack(track,participant)
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
      store.commit(studyRoom_SETTERS.ROOM_ACTIVE,true)
      _changeState(room.localParticipant);
      if(store.getters.getRoomIsTutor){
         if(track.kind === 'video' && store.getters.getIsFullScreen){
            document.getElementById('openFullTutor').click();
         }
         if(!isTwilioStarted && store.getters.getIsShareScreen){
            store.commit(twilio_SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE,true)
            isTwilioStarted = true;
         }
      }
   })
   // room tracks events: 
   room.on('trackSubscribed', (track) => {
      _insightEvent('TwilioTrackSubscribed', track, null);
   })
   room.on('trackUnsubscribed', (track,trackPublication,participant) => {
      _insightEvent('TwilioTrackUnsubscribed', track, null);
      _deleteParticipantTrack(track,participant)
      _detachTracks([track],store);
   })
   room.on('trackStarted', (track,remoteParticipant) => {
      if(track.kind === 'audio' || track.kind === 'video'){
         _addParticipantTrack(track,remoteParticipant)
      }
   })

   // room tracks events: 
   room.on('trackMessage', (message) => {
      let data = JSON.parse(message);
      _insightEvent('trackMessage', data, null);
      if (data.type === CURRENT_STATE_UPDATE) {
         if(data.fullScreen){
            store.dispatch('updateFullScreen',data.fullScreen)
         }
         //store.dispatch('updateAudioToggleByRemote',data.mute)
         // store.dispatch('updateActiveNavEditor', data.tab)
         // store.dispatch('tempWhiteBoardTabChanged', data.canvasTab)
         store.dispatch('sendDataTrack', JSON.stringify({type : CURRENT_STATE_UPDATED}));
         return;
      }
      if (data.type === "muteAll") {
         store.dispatch('updateAudioToggleByRemote',data.val)
      }
      if (data.type === CURRENT_STATE_UPDATED) {
         clearInterval(intervalTime)
         return;
      }
      store.dispatch('dispatchDataTrackJunk',data)
   })
   // room connections events:
   room.on('participantConnected', (participant) => {
      store.commit(studyRoom_SETTERS.ADD_ROOM_PARTICIPANT,participant);

      store.commit(studyRoom_SETTERS.ROOM_PARTICIPANT_COUNT,room.participants.size)
      if(store.getters.getRoomIsTutor){
         store.commit('setComponent', 'simpleToaster_userConnected');
         participant.on('trackSubscribed',(track)=>{
            if(track.kind === 'data'){
               intervalTime= setInterval(() => {
                  _changeState(room.localParticipant);
               },500);
            }
         })
      }
      _insightEvent('TwilioParticipantConnected', participant, null);
   })
   room.on('participantDisconnected', (participant) => {
      store.commit(studyRoom_SETTERS.DELETE_ROOM_PARTICIPANT,participant)

      store.commit(studyRoom_SETTERS.ROOM_PARTICIPANT_COUNT,room.participants.size)
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
      store.commit(studyRoom_SETTERS.ROOM_ACTIVE,false)
   })
}
export default () => {
   return store => {
      let twillioClient;
      let dataTrack = null;
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
         if (mutation.type === twilio_SETTERS.JWT_TOKEN && mutation.payload) {
            if(_activeRoom?.state == 'connected'){
               return
            }
            if(!store.getters.getRoomIsJoined){
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
               let {createLocalVideoTrack,createLocalAudioTrack} = twillioClient;

               // TODO: fix it audio & video
               // let videoDeviceId = localStorage.getItem('sb-videoTrackId');
               // let videoParams = videoDeviceId ? {deviceId: {exact: videoDeviceId}} : {};

               // let devicesList = []
               // if(store.getters.getAudioDeviceId){
               //    devicesList.push(createLocalAudioTrack({name:AUDIO_TRACK_NAME,deviceId:store.getters.getAudioDeviceId}))
               // }
               // if(store.getters.getVideoDeviceId){
               //    devicesList.push(createLocalVideoTrack({name:VIDEO_TRACK_NAME,deviceId: {exact: store.getters.getVideoDeviceId}}))
               // }
               Promise.allSettled([
                  createLocalVideoTrack({name:VIDEO_TRACK_NAME,deviceId: {exact: store.getters.getVideoDeviceId}}),
                  createLocalAudioTrack({name:AUDIO_TRACK_NAME}),
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
            }).catch(()=>{
               store.dispatch('updateRoomIsJoined',false)
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
                  if(!mutation.payload && _localScreenTrack){
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
                  _activeRoom = null;
                  dataTrack = null;
                  _localVideoTrack = null;
                  _localAudioTrack = null;
                  _localScreenTrack = null;
                  isTwilioStarted = false;
                  store.dispatch('updateRoomDisconnected');
                  store.dispatch('updateRoomIsJoined',null);
               }
            }
            if (mutation.type === twilio_SETTERS.TOGGLE_TUTOR_FULL_SCREEN) {
               _changeState(_activeRoom.localParticipant)
            }
            if(mutation.type === twilio_SETTERS.TOGGLE_AUDIO_PARTICIPANTS){
               STORE.dispatch('sendDataTrack',JSON.stringify(
                  {
                     type:'muteAll',
                     val:  STORE.getters.getIsAudioParticipants
                  }
               ));
            }
         }
      })

      // plugin functions:
      function _setLocalVideoTrack(track){
         // const localMediaContainer = document.getElementById(LOCAL_TRACK_DOM_ELEMENT);
         // let videoTag = localMediaContainer.querySelector("video");
         // if (videoTag) {localMediaContainer.removeChild(videoTag)}
         // localMediaContainer.appendChild(track.attach());
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
function _addParticipantTrack(track,participant){
   track.identity = participant.identity;
   STORE.commit(studyRoom_SETTERS.ADD_ROOM_PARTICIPANT_TRACK,track)
}
function _deleteParticipantTrack(track,participant){
   track.identity = participant.identity;
   STORE.commit(studyRoom_SETTERS.DELETE_ROOM_PARTICIPANT_TRACK,track)
}
