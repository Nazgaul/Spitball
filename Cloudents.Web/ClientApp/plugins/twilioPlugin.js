import insightService from '../services/insightService';
import analyticsService from '../services/analytics.service.js';

function _insightEvent(...args){
   insightService.track.event(insightService.EVENT_TYPES.LOG,...args);
}
export default () => {
   return store => {
      store.subscribeAction((action) => {
         if (action.type === 'updateTwilioConnection') {
            let room = action.payload;
            
            //reconnecting room
            room.on('reconnecting', () => {
               _insightEvent('StudyRoom_tutorService_TwilioReconnecting', null, null)
            });
            
            // event of network quality change
            room.localParticipant.on('networkQualityLevelChanged', (networkQualityLevel,networkQualityStats) => {
               _insightEvent('StudyRoom_tutorService_networkQuality',networkQualityStats, networkQualityLevel)
            });

            // Attach the Participant's Media to a <div> element.
            room.on('participantConnected', participant => {
               _insightEvent('StudyRoom_tutorService_TwilioParticipantConnected', participant, null)
               store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.active);
               if (store.getters.getStudyRoomData.isTutor) {
                  store.dispatch('hideRoomToasterMessage');
                  let {studentName,studentId} = store.getters.getStudyRoomData;
                  analyticsService.sb_unitedEvent('study_room', 'session_started', `studentName: ${studentName} studentId: ${studentId}`);
                  if (store.getters.getTutorStartDialog) {
                     store.dispatch('updateTutorStartDialog', false);
                  }
               }
            });



         }
      })
   }
}
/*
 * @emits Room#disconnected
 * @emits Room#participantDisconnected
 * @emits Room#participantReconnected
 * @emits Room#participantReconnecting
 * @emits Room#reconnected
 * @emits Room#recordingStarted
 * @emits Room#recordingStopped
 * @emits Room#trackDimensionsChanged
 * @emits Room#trackDisabled
 * @emits Room#trackEnabled
 * @emits Room#trackMessage
 * @emits Room#trackPublished
 * @emits Room#trackPublishPriorityChanged
 * @emits Room#trackStarted
 * @emits Room#trackSubscribed
 * @emits Room#trackSwitchedOff
 * @emits Room#trackSwitchedOn
 * @emits Room#trackUnpublished
 * @emits Room#trackUnsubscribed

 * @emits Room#reconnecting
 * @emits Room#participantConnected

*/