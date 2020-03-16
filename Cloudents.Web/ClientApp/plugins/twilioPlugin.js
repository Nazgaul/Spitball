import insightService from '../services/insightService';
import analyticsService from '../services/analytics.service.js';
import * as routeNames from '../routes/routeNames.js';

function _insightEvent(...args) {
   insightService.track.event(insightService.EVENT_TYPES.LOG, ...args);
}

function _twilioListeners(room,store) {
   
   room.on('participantConnected', (participant) => {
      _insightEvent('StudyRoom_tutorService_TwilioParticipantConnected', participant, null);
   })
   room.on('participantDisconnected', (participant) => {
      _insightEvent('StudyRoom_tutorService_TwilioParticipantDisconnected', participant, null);
   })
   room.on('trackSubscribed', (track) => {
      _insightEvent('StudyRoom_tutorService_TwilioTrackSubscribed', track, null);
   })
   room.on('disconnected', (dRoom, error) => {
      if (error?.code) {
         _insightEvent('StudyRoom_tutorService_TwilioDisconnected', {'errorCode': error.code}, null);
         console.error(`Twilio Error: Code: ${error.code}, Message: ${error.message}`)
      }
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
      debugger
      if(track.kind === 'video'){
         let previewContainer = document.getElementById('remoteTrack');
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
   })

   room.localParticipant.on('networkQualityLevelChanged', (networkQualityLevel,networkQualityStats) => {
      _insightEvent('StudyRoom_tutorService_networkQuality',networkQualityStats, networkQualityLevel)
   });
   room.on('trackMessage', (message) => {
      let data = JSON.parse(message)
      store.dispatch('dispatchDataTrackJunk',data)
   })
   room.on('reconnecting', () => {
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
               let {LocalDataTrack,createLocalVideoTrack} = Twilio; 
               dataTrack = new LocalDataTrack();
               createLocalVideoTrack().then(track => {
                  const localMediaContainer = document.getElementById('localTrack');
                  localMediaContainer.appendChild(track.attach());
                  mediaTracks.push(track)
               });
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