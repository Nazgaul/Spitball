import insightService from '../services/insightService';
function _insightEvent(...args){
   insightService.track.event(insightService.EVENT_TYPES.LOG,...args);
}
export default () => {
   return store => {
      store.subscribeAction((action) => {
         if (action.type === 'updateTwilioConnection') {
            let room = action.payload;

            room.on('reconnecting', () => {
               _insightEvent('StudyRoom_tutorService_TwilioReconnecting', null, null)
            });
            room.localParticipant.on('networkQualityLevelChanged', (networkQualityLevel,networkQualityStats) => {
               _insightEvent('StudyRoom_tutorService_networkQuality',networkQualityStats, networkQualityLevel)
            });
         }
      })
   }
}
/*
 * @emits Room#disconnected
 * @emits Room#participantConnected
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
*/