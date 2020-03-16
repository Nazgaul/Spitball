
// import Twilio, { LocalDataTrack } from 'twilio-video';
// import store from '../../store/index.js';
// import insightService from '../../services/insightService';

// import studyRoomRecordingService from './studyRoomRecordingService';

// const dataTrack = new LocalDataTrack();
// // Attach the Tracks to the DOM.
// const attachTracks = function (tracks, container) {
//     tracks.forEach((track) => {
//         if (track.attach) {
//             container.appendChild(track.attach());
//         }
//     });
// };
// // Attach the Participant's Tracks to the DOM.
// const attachParticipantTracks = function (participant, container) {
//     let tracks = Array.from(participant.tracks.values());
//     attachTracks(tracks, container);
// };

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

// // Detach the Participant's Tracks from the DOM.
// const detachParticipantTracks = function (participant) {
//     let tracks = Array.from(participant.tracks.values());
//     detachTracks(tracks);
// };

// const connectToRoom = function (token, options) {
//     console.warn('DEBUG: 27 tutorService: connectToRoom')
//     // disconnect the user from room if they already joined
//     store.dispatch('leaveRoomIfJoined');
//     insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_connectToRoom', {'token': token}, null);
    
//     Twilio.connect(token, options)
//         .then((room) => {

//             store.dispatch('updateRoomInstance', room);
//             console.log('Successfully joined a Room: ', room);
//             // Attach the Tracks of all the remote Participants.
//             store.getters['activeRoom'].participants.forEach((participant) => {
//                 let previewContainer = document.getElementById('remoteTrack');
//                 console.warn('DEBUG: 34 : tutorService attachParticipantTracks before ')
//                 attachParticipantTracks(participant, previewContainer);
//                 console.warn('DEBUG: 34 : tutorService attachParticipantTracks after ')
//             });
//             //disconnected room
//             store.getters['activeRoom'].on('disconnected', (room, error) => {
//                 console.warn('DEBUG: 28.6 tutorService: disconnected :ERROR: ',error)
//                   if(!error) return;
//                   store.dispatch('setSessionTimeEnd');
//                   let errorCode = error.code;
//                   insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioDisconnected', {'errorCode': errorCode}, null);
//                   if (errorCode === 20104) {
//                   console.error('Signaling reconnection failed due to expired AccessToken!');
//                   } else if (errorCode === 53000) {
//                   console.error('Signaling reconnection attempts exhausted!');
//                   } else if (errorCode === 53204) {
//                   console.error('Signaling reconnection took too long!');
//                   } else if (errorCode === 53205) {
//                   // TODO fix it with ram
//                   global.location.reload(true);
//                   }else {
//                   console.error('final disconnect');
//                   }
//                 if (store.getters['getStudyRoomData'].isTutor) {
//                     store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.ready);
//                     store.dispatch("setTutorDialogState", store.state.tutoringMain.startSessionDialogStateEnum.finished);
//                     store.dispatch('updateTutorStartDialog', true);
                    
//                 } else {
//                     store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.pending);
//                     if (store.getters['getAllowReview']) {
//                         store.dispatch('updateReviewDialog', true);
//                     }else{
//                         store.dispatch("setStudentDialogState", store.state.tutoringMain.startSessionDialogStateEnum.finished);
//                         store.dispatch('updateStudentStartDialog', true);
//                     }
//                 }
//                 //stop recording when disconnecting
//                 studyRoomRecordingService.stopRecord();
//                 store.dispatch('updateEndDialog', false);
//                 //detach all local tracks to prevent multiple added tracks
//                 store.getters['activeRoom'].localParticipant.tracks.forEach(function (track) {
//                     detachTracks([track]);
//                 });

//             });
//             // When a Participant adds a Track, attach it to the DOM.
//             store.getters['activeRoom'].on('trackSubscribed', (track) => {
//                 insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioTrackSubscribed', track, null);
//                 let previewContainer = document.getElementById('remoteTrack');
//                 if (track.kind === 'data') {
//                     track.on('message', transferObj => {
//                         // moved to plugin      
//                     });
//                 } else if (track.kind === 'video') {
//                     let videoTag = previewContainer.querySelector("video");
//                     if (videoTag) {
//                         previewContainer.removeChild(videoTag);
//                     }
//                     let updateObj = {
//                         type: "video",
//                         track,
//                         container: previewContainer
//                     };
//                     store.commit('releaseFullVideoButton',true);
//                     store.dispatch('updateRemoteTrack', updateObj);
//                 } else if (track.kind === 'audio') {
//                     let updateObj = {
//                         type: "audio",
//                         track,
//                         container: previewContainer
//                     };
//                     store.dispatch('updateRemoteTrack', updateObj);
//                 }

//                 console.log('track attached', " added track: " + track.kind, track);
//             });
//             // When a Participant's Track is unsubscribed from, detach it from the DOM.
//             store.getters['activeRoom'].on('trackUnsubscribed', function (track) {
//                 if(track.kind === 'video'){
//                     store.commit('releaseFullVideoButton',false);
//                 }
//                 insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioTrackUnsubscribed', track, null);
//                 console.log(" removed track: " + track.kind);
//                 detachTracks([track]);
//             });

//             // When a Participant leaves the Room, detach its Tracks.
//             store.getters['activeRoom'].on('participantDisconnected', (participant) => {
//                 insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioParticipantDisconnected', participant, null);
//                 let localIdentity = room.localParticipant?.identity;
//                 if (participant.identity !== localIdentity) {
//                     store.dispatch('setSesionClickedOnce', false);
//                     if (store.getters['getStudyRoomData'].isTutor) {
//                         store.dispatch("setTutorDialogState", store.state.tutoringMain.startSessionDialogStateEnum.disconnected);
//                         store.dispatch('updateTutorStartDialog', true);
//                     } else {
//                         store.dispatch("setStudentDialogState", store.state.tutoringMain.startSessionDialogStateEnum.disconnected);
//                     }
//                 }
//                 detachParticipantTracks(participant);
//             });
//         },
//             (error) => {
//                 console.error(error, 'error cant connect');

//             });
// };

// const validateUserMedia = async function() {
//     let devicesObj = store.getters['getDevicesObj'];
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