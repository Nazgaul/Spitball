
// import Twilio, { LocalDataTrack } from 'twilio-video';
// import store from '../../store/index.js';

// import studyRoomRecordingService from './studyRoomRecordingService';

// const dataTrack = new LocalDataTrack();

// // Detach the Tracks from the DOM.
// const detachTracks = function (tracks) {
//     tracks.forEach((track) => {
//         if (!!track && track.detach) {
//             track.detach().forEach((detachedElement) => {
//                 detachedElement.remove();
//             });
//         }
//     });
// };

// const connectToRoom = function (token, options) {
//     // disconnect the user from room if they already joined
//     store.dispatch('leaveRoomIfJoined');    
//     Twilio.connect(token, options)
//         .then((room) => {
//             store.dispatch('updateRoomInstance', room);

//             //disconnected room
//             store.getters['activeRoom'].on('disconnected', (room, error) => {
//                 if(!error) return;
//                 store.dispatch('setSessionTimeEnd');

//                 //stop recording when disconnecting
//                 studyRoomRecordingService.stopRecord();
//             });
//         },
//             (error) => {
//                 console.error(error, 'error cant connect');
//             });
// };
// export default {
//     dataTrack,
//     detachTracks,
//     connectToRoom,
// };