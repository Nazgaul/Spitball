
// import Twilio, { LocalDataTrack } from 'twilio-video';
// import store from '../../store/index.js';
// import insightService from '../../services/insightService';

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

// const validateUserMedia = async function() {
//     let devicesObj = store.getters['getDev icesObj'];
//     await navigator.mediaDevices.getUserMedia({ video: true }).then((y) => {
//         console.log(y);
//         devicesObj.hasVideo = true;
//     }, err => {
//         let insightErrorObj={
//             error: err,
//             userId: this.userId
//         };
//         insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_validationDialog_getUserMedia_VIDEO', insightErrorObj, null);
//         console.error(err.name + ":VIDEO!!!!!!!!!!!!!!!! " + err.message, err);
//         devicesObj.errors.video.push(err.name);
//     });

//     await navigator.mediaDevices.getUserMedia({ audio: true }).then((y) => {
//         console.log(y);
//         devicesObj.hasAudio = true;
//     }, err => {
//         let insightErrorObj={
//             error: err,
//             userId: this.userId
//         };
//         insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_validationDialog_getUserMedia_AUDIO', insightErrorObj, null);
//         console.error(err.name + ":AUDIO!!!!!!!!!!!!!!!! " + err.message, err);
//         devicesObj.errors.audio.push(err.name);
//     });
// };

// export default {
//     dataTrack,
//     detachTracks,
//     connectToRoom,
//     validateUserMedia,
// };