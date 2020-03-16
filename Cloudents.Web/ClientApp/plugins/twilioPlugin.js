import insightService from '../services/insightService';
import analyticsService from '../services/analytics.service.js';
import * as routeNames from '../routes/routeNames.js';

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
   insightService.track.event(insightService.EVENT_TYPES.LOG, ...args);
}

function _twilioListeners(room,store) {
   debugger
   
   room.on('participantConnected', (participant) => {
      debugger
      _insightEvent('StudyRoom_tutorService_TwilioParticipantConnected', participant, null);
   })
   room.on('participantDisconnected', (participant) => {
      _insightEvent('StudyRoom_tutorService_TwilioParticipantDisconnected', participant, null);
      _detachTracks(Array.from(participant.tracks.values()))
   })
   room.on('trackSubscribed', (track) => {
      _insightEvent('StudyRoom_tutorService_TwilioTrackSubscribed', track, null);
      _detachTracks([track])
   })
   room.on('disconnected', (dRoom, error) => {
      if (error?.code) {
         _insightEvent('StudyRoom_tutorService_TwilioDisconnected', {'errorCode': error.code}, null);
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
   room.on('trackDimensionsChanged', () => {
      debugger
   })
   room.on('trackDisabled', () => {
      debugger
   })
   room.on('trackEnabled', () => {
      debugger
   })
   room.on('trackPublished', () => {
      debugger
   })
   room.on('trackPublishPriorityChanged', () => {
      debugger
   })
   room.on('trackStarted', (track) => {
      let previewContainer = document.getElementById('remoteTrack');
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
   room.on('trackUnpublished', () => {
      debugger
   })
   room.on('trackUnsubscribed', (RemoteDataTrack,RemoteDataTrackPublication,RemoteParticipant) => {
      _insightEvent('StudyRoom_tutorService_TwilioTrackUnsubscribed', RemoteDataTrack, null)
      debugger
   })

   room.localParticipant.on('networkQualityLevelChanged', (networkQualityLevel,networkQualityStats) => {
      _insightEvent('StudyRoom_tutorService_networkQuality',networkQualityStats, networkQualityLevel)
   });
   room.on('trackMessage', (message) => {
      let data = JSON.parse(message)
      store.dispatch('dispatchDataTrackJunk',data)
   })
   room.on('reconnecting', () => {
      debugger
      _insightEvent('StudyRoom_tutorService_TwilioReconnecting', null, null);
   })
}
export default () => {
   return store => {
      let twillioClient;
      let dataTrack;
      let mediaTracks = [];
      store.subscribe((mutation) => {
         if (mutation.type === 'setRouteStack' && mutation.payload === routeNames.StudyRoom) {
            import('twilio-video').then(Twilio => { 
               twillioClient = Twilio;
               let {LocalDataTrack,createLocalVideoTrack,createLocalAudioTrack} = Twilio; 
               dataTrack = new LocalDataTrack();
               Promise.allSettled([createLocalVideoTrack(),createLocalAudioTrack()]).then((tracks) => {
                  tracks.forEach(({value}) => {
                     if(value){
                        if(value.kind === 'video'){
                           const localMediaContainer = document.getElementById('localTrack');
                           localMediaContainer.appendChild(value.attach());
                           mediaTracks.push(value);
                        }
                        if(value.kind === 'audio'){                         
                           mediaTracks.push(value);
                        }
                     }
                  })
               })
            });
         }
         if (mutation.type === 'setJwtToken') {
            let jwtToken = mutation.payload;
            let options = {
               logLevel: 'debug',
               tracks: [dataTrack,...mediaTracks],
               networkQuality: {
                  local: 3,
                  remote: 3
               }
            };
            twillioClient.connect(jwtToken, options).then((room) => {
               _insightEvent('StudyRoom_tutorService_TwilioConnect', room, null);
               _twilioListeners(room,store);
            })
         }
         if (mutation.type === 'setDataTrack'){
            dataTrack.send(mutation.payload);
         }
         if (mutation.type === 'setVideoToggle'){
            let videoTrack = mediaTracks.find(track=>track.kind === 'video');
            if(videoTrack){
               if(videoTrack.isEnabled){
                  videoTrack.disable()
               }else{
                  videoTrack.enable()
               }
            }
         }
         if (mutation.type === 'setAudioToggle'){
            let audioTrack = mediaTracks.find(track=>track.kind === 'audio');
            if(audioTrack){
               if(audioTrack.isEnabled){
                  audioTrack.disable()
               }else{
                  audioTrack.enable()
               }
            }
         }
      })


      // store.subscribeAction((action) => {
      //    // let room = action.payload;
      //    // let {isTutor,studentName,studentId} = store.getters.getStudyRoomData;
      //    // _insightEvent('StudyRoom_tutorService_TwilioConnect', room, null);
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