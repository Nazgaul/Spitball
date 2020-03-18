import insightService from '../services/insightService';
import analyticsService from '../services/analytics.service.js';
import * as routeNames from '../routes/routeNames.js';
import {SETTERS} from '../store/constants/twilioConstants.js';

const REMOTE_TRACK = 'remoteTrack';
const LOCAL_TRACK = 'localTrack';

function _detachTracks(tracks){
   tracks.forEach((track) => {
      if (track?.detach) {
         track.detach().forEach((detachedElement) => {
            detachedElement.remove();
         });
      }
   });
}
function _attachTracks(tracks,container){
   tracks.forEach((track) => {
      if (track.attach) {
         container.appendChild(track.attach());
      }
   });
}
function _insightEvent(...args) {
   insightService.track.event(insightService.EVENT_TYPES.LOG, ...args);
}

function _twilioListeners(room,store) {
   _insightEvent('TwilioConnect', room, null);

   debugger

   room.participants.forEach((participant) => {
         participant.on('trackUnpublished',(track)=>{
            debugger
         })
      
      
      let previewContainer = document.getElementById(REMOTE_TRACK);
      let tracks = Array.from(participant.tracks.values());
      _attachTracks(tracks, previewContainer)





   });









   room.on('participantConnected', (participant) => {
      debugger
      _insightEvent('TwilioParticipantConnected', participant, null);
   })
   room.on('participantDisconnected', (participant) => {
      debugger
      _insightEvent('TwilioParticipantDisconnected', participant, null);
      _detachTracks(Array.from(participant.tracks.values()))
   })
   room.on('trackSubscribed', (track) => {
      debugger
      _insightEvent('TwilioTrackSubscribed', track, null);
      _detachTracks([track])
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
   room.on('participantReconnected', () => {
      debugger
   })
   room.on('participantReconnecting', () => {
      debugger
   })
   room.on('recordingStarted', () => {
      debugger
   })
   room.on('recordingStopped', () => {
      debugger
   })
   // room.on('trackDimensionsChanged', () => {
   //    debugger
   // })
   room.on('trackDisabled', () => {
      debugger
   })
   room.on('trackEnabled', () => {
      debugger
   })
   room.on('trackPublishPriorityChanged', () => {
      debugger
   })
   room.on('trackRemoved',()=>{
      debugger
   })
   room.on('trackStarted', (track) => {
      debugger

      let previewContainer = document.getElementById(REMOTE_TRACK);
      if(track.kind === 'video'){
         let videoTag = previewContainer.querySelector("video");
         if (videoTag) {
            previewContainer.removeChild(videoTag);
         }
         previewContainer.appendChild(track.attach());
      }
      if(track.kind === 'audio'){   
         track.detach().forEach((detachedElement) => {
            detachedElement.remove();
         });
         previewContainer.appendChild(track.attach());
      }
   })
   room.on('trackSwitchedOff', () => {
      debugger
   })
   room.on('trackSwitchedOn', () => {
      debugger
   })
   room.on('trackPublished', () => {
      debugger
   })
   room.on('trackUnpublished', () => {
      debugger
   })
   room.on('trackUnsubscribed', (RemoteDataTrack,RemoteDataTrackPublication,RemoteParticipant) => {
      debugger
      _insightEvent('TwilioTrackUnsubscribed', RemoteDataTrack, null);
      _detachTracks([RemoteDataTrack]);
   })
   room.localParticipant.on('trackStopped',(track)=>{
      if(track.kind === 'video'){
         store.commit(SETTERS.VIDEO_AVAILABLE,false)
      }
      if(track.kind === 'audio'){
         store.commit(SETTERS.AUDIO_AVAILABLE,false)
      }
   })
   room.localParticipant.on('networkQualityLevelChanged', (networkQualityLevel,networkQualityStats) => {
      _insightEvent('networkQuality',networkQualityStats, networkQualityLevel)
   });
   room.on('trackMessage', (message,RemoteDataTrack,RemoteParticipant) => {
      let data = JSON.parse(message)
      _insightEvent('trackMessage', data, null);
      store.dispatch('dispatchDataTrackJunk',data)
   })
   room.on('reconnecting', (reconnectingError) => {
      _insightEvent('reconnecting', null, null);
   })
}
export default () => {
   return store => {
      let twillioClient;
      let dataTrack;
      let mediaTracks = [];
      let _room = null;
      store.subscribe((mutation) => {
         if (mutation.type === 'setRouteStack' && mutation.payload === routeNames.StudyRoom) {
            import('twilio-video').then(async (Twilio) => { 
               twillioClient = Twilio;
               let {LocalDataTrack,createLocalVideoTrack,createLocalAudioTrack} = Twilio; 
               dataTrack = new LocalDataTrack();
               Promise.allSettled([createLocalVideoTrack(),createLocalAudioTrack()]).then((tracks) => {
                  tracks.forEach(({value}) => {
                     if(value){
                        if(value.kind === 'video'){
                           const localMediaContainer = document.getElementById(LOCAL_TRACK);
                           localMediaContainer.appendChild(value.attach());
                           mediaTracks.push(value);
                           store.commit(SETTERS.VIDEO_AVAILABLE,true)
                        }
                        if(value.kind === 'audio'){                         
                           mediaTracks.push(value);
                           store.commit(SETTERS.AUDIO_AVAILABLE,true)
                        }
                     }
                  })
               })
            });
         }
         if (mutation.type === SETTERS.JWT_TOKEN) {
            let jwtToken = mutation.payload;
            let options = {
               logLevel: 'debug',
               tracks: [dataTrack,...mediaTracks],
               networkQuality: {
                  local: 3,
                  remote: 3
               }
            };
            _insightEvent('connectToRoom', {'token': jwtToken}, null);
            twillioClient.connect(jwtToken, options).then((room) => {
               _room = room;
               _twilioListeners(room,store);
            })
         }
         if (mutation.type === SETTERS.DATA_TRACK){
            dataTrack.send(mutation.payload);
         }
         if (mutation.type === SETTERS.VIDEO_TOGGLE){
            let videoTrack = mediaTracks.find(track=>track.kind === 'video');
            if(videoTrack){
               if(videoTrack.isEnabled){
                  videoTrack.disable()
               }else{
                  videoTrack.enable()
               }
            }
         }
         if (mutation.type === SETTERS.AUDIO_TOGGLE){
            let audioTrack = mediaTracks.find(track=>track.kind === 'audio');
            if(audioTrack){
               if(audioTrack.isEnabled){
                  audioTrack.disable()
               }else{
                  audioTrack.enable()
               }
            }
         }
         if (mutation.type === SETTERS.SCREEN_SHARE){
            const share = async()=>{
               let stream = await navigator.mediaDevices.getDisplayMedia();
               const screenTrack = new twillioClient.LocalVideoTrack(stream.getTracks()[0]);
               _room.localParticipant.publishTrack(screenTrack);
            }
            share()
         }
      })


      // store.subscribeAction((action) => {
      //    // let room = action.payload;
      //    // let {isTutor,studentName,studentId} = store.getters.getStudyRoomData;
      //    // _insightEvent('TwilioConnect', room, null);
      //    // store.dispatch('setSessionTimeStart');

      //    // //close start dialogs after reload page (by refresh).
      //    // if (isTutor) {
      //    //    if(room.participants.size > 0){
      //    //       store.dispatch('updateTutorStartDialog', false);
      //    //    }
      //    // } else {
      //    //       store.dispatch('updateStudentStartDialog', false);
      //    // } 
      //    // // event of network quality change


      //    // // Attach the Participant's Media to a <div> element.
      //    // room.on('participantConnected', participant => {
      //    //    store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.active);
      //    //    if (isTutor) {
      //    //       analyticsService.sb_unitedEvent('study_room', 'session_started', `studentName: ${studentName} studentId: ${studentId}`);
      //    //       if (store.getters.getTutorStartDialog) {
      //    //          store.dispatch('updateTutorStartDialog', false);
      //    //       }
      //    //    }
      //    // });
      // }
      // })
   }
}