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
const PRIORITY = {
   HIGH:'high',
   STANDARD:'standard',
   LOW:'low',
}
let isTwilioStarted = false;

let intervalTime = null;

function _changeState(localParticipant) {
   if(!STORE.getters.getRoomIsTutor) return;

   let stuffToSend =  {
      type: CURRENT_STATE_UPDATE,
      tab : STORE.getters.getActiveNavEditor,
      canvasTab : {
         tab: STORE.getters.getCurrentSelectedTab,
         canvas: STORE.getters.canvasDataStore
      },
      mute : STORE.getters.getIsAudioParticipants,
      // fullScreen: null
   };
   localParticipant.tracks.forEach((track) => {
      if(track.trackName === VIDEO_TRACK_NAME && STORE.getters.getIsFullScreen 
         || track.trackName === SCREEN_TRACK_NAME){
         stuffToSend.fullScreen = {
            participantId:localParticipant.identity.split('_')[0],
            trackType: track.trackName
         };
      }
   });
   STORE.dispatch('sendDataTrack',JSON.stringify(stuffToSend));
}
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
   //let x = {...args};
   //let string = JSON.parse(x);
   //use https://www.npmjs.com/package/vue-application-insights
   insightService.track.event(insightService.EVENT_TYPES.LOG, ...args);
}
function _twilioListeners(room,store) { 
   let isStateInit = false; // to prevent multiple initials 
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
      participant.on('trackSubscriptionFailed',(error, trackPublication)=>{
         // this participant publish a track the local participant couldn't subscribe
         let params = {
            errorCode: error?.code,
            errorMessage: error?.message,
            publisher: participant?.identity,
            subscriber: room.localParticipant?.identity,
            priority: trackPublication?.publishPriority,
            kind: trackPublication?.trackName,
         }
         _insightEvent('trackSubscriptionFailed', params, null);
      })
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
      let params = {
         participant: room.localParticipant.identity,
         level:networkQualityLevel,
         audio: networkQualityStats?.audio? 'recv:'+networkQualityStats.audio.recv+' send:'+networkQualityStats.audio.send : undefined,
         video: networkQualityStats?.video? 'recv:'+networkQualityStats.video.recv+' send:'+networkQualityStats.video.send : undefined,
      }
      _insightEvent('networkQuality',params,null)
      let localNetworkStats = 0;

      let isAudioTrack = room.localParticipant.audioTracks.size != 0;
      let isVideoTrack = room.localParticipant.videoTracks.size != 0;

      localNetworkStats += isAudioTrack? networkQualityStats?.audio.send : 0;
      localNetworkStats += isVideoTrack? networkQualityStats?.video.send : 0;
      
      if(isAudioTrack && isVideoTrack){
         localNetworkStats = Math.floor(localNetworkStats / 2);
      }
      if(!localNetworkStats){
         localNetworkStats = networkQualityLevel
      }
      let stats = {
         level: localNetworkStats || 0,
         stats: {
            localAudioSend: isAudioTrack? networkQualityStats?.audio.send : undefined,
            localVideoSend: isVideoTrack? networkQualityStats?.video.send : undefined,
            remoteAudioReceive: networkQualityStats?.audio.recv,
            remoteVideoReceive: networkQualityStats?.video.recv,
         }
      }
      store.commit(studyRoom_SETTERS.ROOM_NETWORK_QUALITY,stats)
   });
   room.localParticipant.on('trackPublicationFailed',(error,localTrack)=>{
      let params = {
         errorCode: error?.code,
         errorMessage: error?.message,
         publisher: room.localParticipant?.identity,
         ...localTrack,
      }
      _insightEvent('trackPublicationFailed', params, null);
   })
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
   room.on('trackSubscribed', (track, publication, participant) => {
      let params = {
         kind: track?.name,
         subscriber: room.localParticipant.identity,
         publisher: participant?.identity,
         priority: publication?.publishPriority,
      }
      _insightEvent('TwilioTrackSubscribed', params, null);
   })
   // room.on('dominantSpeakerChanged', participant => {
   //    _insightEvent('dominantSpeakerChanged', {participant: participant?.identity}, null);
   //  });
   room.on('trackUnsubscribed', (track,trackPublication,participant) => {
      let params = {
         kind: track?.name,
         unSubscriber: room.localParticipant.identity,
         publisher: participant?.identity,
         priority: trackPublication?.publishPriority,
      }
      _insightEvent('TwilioTrackUnsubscribed', params, null);
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
      if (data.type === CURRENT_STATE_UPDATE && !isStateInit) {
         if(data.fullScreen){
            store.dispatch('updateFullScreen',data.fullScreen)
         }
         store.dispatch('updateActiveNavEditor', data.tab)
         store.dispatch('updateAudioToggleByRemote',data.mute);
         store.dispatch('tempWhiteBoardTabChanged', data.canvasTab)
         store.dispatch('sendDataTrack', JSON.stringify({type : CURRENT_STATE_UPDATED}));
         isStateInit = true; // to prevent multiple initials 
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
      store.commit(studyRoom_SETTERS.ROOM_PARTICIPANT_COUNT,room.participants.size);
      participant.on('trackSubscriptionFailed',(error, trackPublication)=>{
         // this participant publish a track the local participant couldn't subscribe
         let params = {
            errorCode: error?.code,
            errorMessage: error?.message,
            publisher: participant?.identity,
            subscriber: room.localParticipant?.identity,
            priority: trackPublication?.publishPriority,
            kind: trackPublication?.trackName,
         }
         _insightEvent('trackSubscriptionFailed', params, null);
      })
      if(store.getters.getRoomIsTutor){
         store.commit('setComponent', 'simpleToaster_userConnected');
         participant.on('trackSubscribed',(track)=>{
            if(track.kind === 'data'){
               intervalTime= setInterval(() => {
                  _changeState(room.localParticipant);
               },500);
            }
         })
      }else{
         if(store.getters.getRoomTutor.tutorId == participant.identity.split('_')[0]){
            store.commit('setComponent', 'simpleToaster_tutorConnected');
         }
      }
      _insightEvent('TwilioParticipantConnected', {participant: participant.identity}, null);
   })
   room.on('participantDisconnected', (participant) => {
      store.commit(studyRoom_SETTERS.DELETE_ROOM_PARTICIPANT,participant)

      store.commit(studyRoom_SETTERS.ROOM_PARTICIPANT_COUNT,room.participants.size)
      if(store.getters.getRoomIsTutor){
         store.commit('setComponent', 'simpleToaster_userLeft');
      }else{
         if(store.getters.getRoomTutor.tutorId == participant.identity.split('_')[0]){
            store.commit('setComponent', 'simpleToaster_tutorLeft');
         }
      }
      let params = {
         participant: participant.identity,
         networkQualityLevel:participant.networkQualityLevel
      }
      _insightEvent('TwilioParticipantDisconnected', params, null);
      _detachTracks(Array.from(participant.tracks.values()),store)
   })
   room.on('reconnecting', (error) => {
      let params = {
         errorCode: error?.code,
         errorMessage: error?.message
      }
      _insightEvent('reconnecting', params, null);
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
               if(!twillioClient.isSupported){
                  store.commit(studyRoom_SETTERS.BROWSER_NOT_SUPPORT,true)
               }
            });
            _debugMode = mutation.payload.query?.debug ? 'debug' : 'off';
         }
         if (mutation.type === twilio_SETTERS.JWT_TOKEN && mutation.payload && store.getters.getRouteStack[0].name === routeNames.StudyRoom) {
            if(store.getters.getIsBrowserNotSupport){
               return
            }
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

            let roomOptions = _getRoomConfigByTopologyType(store.getters.getRoomTopologyType);

            _insightEvent('connectToRoom', {'token': jwtToken}, null);
            twillioClient.connect(jwtToken, roomOptions).then((room) => {
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
                  createLocalVideoTrack(
                     {
                        name:VIDEO_TRACK_NAME,
                        deviceId: 
                        {
                           exact: store.getters.getVideoDeviceId
                        },
                        width: {ideal: 1280, max: 1920},
                        height: {ideal: 720, max: 1080}
                     }),
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
                     _localScreenTrack = new twillioClient.LocalVideoTrack(stream.getTracks()[0],{
                        name:SCREEN_TRACK_NAME,
                        // video: document.body.clientWidth < 960 ?
                        // {
                        //     height: 480,
                        //     frameRate: 15,
                        //     width: 640 
                        // } 
                        //    : 
                        // { height: 720,
                        //   frameRate: 15,
                        //   width: 1280 
                        // },

                     });
                     let isRoomStudent = !store.getters.getRoomIsTutor;
                     if(isRoomStudent && _localVideoTrack){
                        _unPublishTrack(_localVideoTrack);
                     }
                     _publishTrack(_localScreenTrack);
                     store.commit(twilio_SETTERS.VIDEO_AVAILABLE,true)
                     _localScreenTrack.on('stopped',(track)=>{
                        _unPublishTrack(track)
                        store.commit(twilio_SETTERS.SCREEN_SHARE_BROADCAST_TOGGLE,false);
                        if(_localVideoTrack){
                           _publishTrack(_localVideoTrack)
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
                              _publishTrack(_localVideoTrack)
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
                  _unPublishTrack(_localVideoTrack)
               }
               let params = {deviceId: {exact: mutation.payload},name:VIDEO_TRACK_NAME}
               twillioClient.createLocalVideoTrack(params).then(track=>{
                  _setLocalVideoTrack(track);
               })
            }
            if (mutation.type === twilio_SETTERS.CHANGE_AUDIO_DEVICE){
               if(_localAudioTrack){
                  _unPublishTrack(_localAudioTrack)
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
         _localVideoTrack = track;
         _publishTrack(_localVideoTrack)
         store.commit(twilio_SETTERS.VIDEO_AVAILABLE,true)
      }
      function _setLocalAudioTrack(track){
         _localAudioTrack = track;
         _publishTrack(_localAudioTrack)
         store.commit(twilio_SETTERS.AUDIO_AVAILABLE,true)
      }
      function _publishTrack(track){
         console.log(store.getters.getRoomIsTutor? PRIORITY.HIGH : PRIORITY.LOW);
         _activeRoom.localParticipant.publishTrack(track,{priority: store.getters.getRoomIsTutor? PRIORITY.HIGH : PRIORITY.LOW});
         _addParticipantTrack(track,_activeRoom.localParticipant)
      
         //On share screen we want to update all the users in the room
         _changeState( _activeRoom.localParticipant);
      }
      function _unPublishTrack(track){
         _activeRoom.localParticipant.unpublishTrack(track);
         _deleteParticipantTrack(track,_activeRoom.localParticipant)
      
         //On share screen we want to update all the users in the room
         _changeState( _activeRoom.localParticipant);
      }
      function _toggleTrack(tracks,trackType,value){
         let {track} = tracks.find(track=>track.kind === trackType);
         if(track){
            // if user was false by local dont change it by remote
            // value: FALSE - USER TURNED OFF / TRUE - USER TURNED ON
            if(!value){
               track.disable()
            }
            if(value){
               track.enable()
            }
            let params = {
               participant: track?.identity,
               kind: track?.name,
               status: track?.isEnabled
            }
            _insightEvent('toggleTrack', params, null);
         }
      }
      function _getRoomConfigByTopologyType(roomTopologyType){
         // SmallGroup,
         // PeerToPeer,
         // GroupRoom
         let isMobileMode = document.body.clientWidth < 960;
         let defaultRoomSettings = {
            logLevel: _debugMode,
            tracks: [dataTrack],
            networkQuality: {local:2, remote: 2}, //https://www.twilio.com/docs/video/using-network-quality-api
            maxAudioBitrate:16000,//For music remove this line
            maxVideoBitrate : isMobileMode ? 500000 : undefined, // problem with screen share maybe we should put in on the student side only cuz he have only 1 video track 
            // video: isMobileMode? { height: 480, frameRate: 24, width: 640 } : { height: 720, frameRate: 24, width: 1280 }, // makes problems with share screen... choppy
         }
         if(roomTopologyType == 'PeerToPeer'){
            return defaultRoomSettings
         }else{ 
            let groupRoomSettings = {
               ...defaultRoomSettings,
               
               bandwidthProfile: {
                  video: {
                     mode: 'collaboration',
                     trackSwitchOffMode:'predicted', //DO NOT CHANGE THAT
                     maxSubscriptionBitrate: isMobileMode? 2500000 : 0,
                     dominantSpeakerPriority: PRIORITY.STANDARD,
                     maxTracks: isMobileMode ? 3 : 10,
                     renderDimensions: isMobileMode? undefined : {
                        [PRIORITY.HIGH]: {height:720, width:1280}, //{height:1080, width:1920},
                        [PRIORITY.STANDARD]: {height:720, width:1280},
                        [PRIORITY.LOW]: {height:720, width:1280} //{height:236, width:149}
                     }
                  }
               },
               dominantSpeaker: true,
               preferredVideoCodecs: [
                  { codec: 'VP8', simulcast: true },
                  { codec: 'H264', simulcast: true } // this is for iphone and Safari
               ],
            }
            return groupRoomSettings;
         }
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
