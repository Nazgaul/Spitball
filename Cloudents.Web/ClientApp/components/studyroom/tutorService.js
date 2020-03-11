
import Twilio, { LocalDataTrack } from 'twilio-video';
import { connectivityModule } from '../../services/connectivity.module';
import store from '../../store/index.js';
import whiteBoardService from "./whiteboard/whiteBoardService";
import insightService from '../../services/insightService';
import analyticsService from '../../services/analytics.service';

import studyRoomRecordingService from './studyRoomRecordingService';

const dataTrack = new LocalDataTrack();
// Attach the Tracks to the DOM.
const attachTracks = function (tracks, container) {
    tracks.forEach((track) => {
        if (track.attach) {
            container.appendChild(track.attach());
        }
    });
};
// Attach the Participant's Tracks to the DOM.
const attachParticipantTracks = function (participant, container) {
    let tracks = Array.from(participant.tracks.values());
    attachTracks(tracks, container);
};

// Detach the Tracks from the DOM.
const detachTracks = function (tracks) {
    tracks.forEach((track) => {
        if (!!track && track.detach) {
            track.detach().forEach((detachedElement) => {
                detachedElement.remove();
            });
        }
    });
};

// Detach the Participant's Tracks from the DOM.
const detachParticipantTracks = function (participant) {
    let tracks = Array.from(participant.tracks.values());
    detachTracks(tracks);
};

const printNetworkQuality = function (networkQualityLevel,networkQualityStats) {
    insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_networkQuality',networkQualityStats, networkQualityLevel);
    console.log({
        1: '▃',
        2: '▃▄',
        3: '▃▄▅',
        4: '▃▄▅▆',
        5: '▃▄▅▆▇'
    }[networkQualityLevel] || '');
    if (networkQualityStats) {
        // Print in console the networkQualityStats, which is non-null only if Network Quality
        // verbosity is 2 (moderate) or greater
        console.log('Network Quality statistics:', networkQualityStats);
      }
};

const connectToRoom = function (token, options) {
    console.warn('DEBUG: 27 tutorService: connectToRoom')
    // disconnect the user from room if they already joined
    insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_connectToRoom', {'token': token}, null);
    store.dispatch('leaveRoomIfJoined');
    Twilio.connect(token, options)
        .then((room) => {
            store.dispatch('updateTwilioConnection',room); // to start plugin listening



            console.warn('DEBUG: 28 tutorService: Twilio.connect')

            store.dispatch('setSessionTimeStart');
            insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioConnect', room, null);
            store.dispatch('updateRoomInstance', room);
            console.log('Successfully joined a Room: ', room);
            let localIdentity = room.localParticipant && room.localParticipant.identity ? room.localParticipant.identity : '';
            store.dispatch('updateUserIdentity', localIdentity);

            // Print the initial Network Quality Level
            if (!store.getters['getStudyRoomData'].isTutor) {
                console.warn('DEBUG: 28.2 tutorService: !istutor')
                store.dispatch('hideRoomToasterMessage');
            }

            //close start dialogs after reload page (by refresh).
                if (store.getters['getStudyRoomData'].isTutor) {
                    console.warn('DEBUG: 28.3 tutorService: close start dialogs after reload page (by refresh) TUTOR')

                    if(room.participants.size > 0){
                        console.warn('DEBUG: 28.4 tutorService: close start dialogs after reload page (by refresh) (room.participants.size > 0)')

                        if (store.getters['getTutorStartDialog']) {
                            store.dispatch('updateTutorStartDialog', false);
                        }
                    }
                } else {
                    console.warn('DEBUG: 28.5 tutorService: close start dialogs after reload page (by refresh) STUDENT')

                    if (store.getters['getStudentStartDialog']) {
                        store.dispatch('updateStudentStartDialog', false);
                    }
                }           
            
            
            // //event of network quality change
            // store.getters['localParticipant'].on('networkQualityLevelChanged', printNetworkQuality);
            
            // Attach the Tracks of all the remote Participants.
            store.getters['activeRoom'].participants.forEach((participant) => {
                let previewContainer = document.getElementById('remoteTrack');
                console.warn('DEBUG: 34 : tutorService attachParticipantTracks before ')
                attachParticipantTracks(participant, previewContainer);
                console.warn('DEBUG: 34 : tutorService attachParticipantTracks after ')
            });
            //disconnected room
            store.getters['activeRoom'].on('disconnected', (room, error) => {
                console.warn('DEBUG: 28.6 tutorService: disconnected :ERROR: ',error)
                if(!error) return;
                store.dispatch('setSessionTimeEnd');
                let errorCode = error.code;
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioDisconnected', {'errorCode': errorCode}, null);
                if (errorCode === 20104) {
                    console.error('Signaling reconnection failed due to expired AccessToken!');
                } else if (errorCode === 53000) {
                    console.error('Signaling reconnection attempts exhausted!');
                } else if (errorCode === 53204) {
                    console.error('Signaling reconnection took too long!');
                } else if (errorCode === 53205) {
                    // TODO fix it with ram
                    global.location.reload(true);
                }else {
                    console.error('final disconnect');
                }
                if (store.getters['getStudyRoomData'].isTutor) {
                    store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.ready);
                    store.dispatch("setTutorDialogState", store.state.tutoringMain.startSessionDialogStateEnum.finished);
                    store.dispatch('updateTutorStartDialog', true);
                    
                } else {
                    store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.pending);
                    if (store.getters['getAllowReview']) {
                        store.dispatch('updateReviewDialog', true);
                    }else{
                        store.dispatch("setStudentDialogState", store.state.tutoringMain.startSessionDialogStateEnum.finished);
                        store.dispatch('updateStudentStartDialog', true);
                    }
                }
                //stop recording when disconnecting
                studyRoomRecordingService.stopRecord();
                store.dispatch('updateEndDialog', false);
                //detach all local tracks to prevent multiple added tracks
                store.getters['activeRoom'].localParticipant.tracks.forEach(function (track) {
                    detachTracks([track]);
                });

            });
            // // //reconnecting room
            // store.getters['activeRoom'].on('reconnecting', () => {
            //     console.warn('DEBUG: 28.7 tutorService: reconnecting room')

            //     insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioReconnecting', null, null);
            //     console.log("ROOM - RECONNECTING");
            // });
            
            // Attach the Participant's Media to a <div> element.
            store.getters['activeRoom'].on('participantConnected', participant => {
                console.warn('DEBUG: 28.9 tutorService: Attach the Participant Media to a <div> element.')

                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioParticipantConnected', participant, null);
                console.log(`Participant "${participant.identity}" connected`);

                console.warn('DEBUG: 36 : tutorService updateCurrentRoomState before ')
                store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.active);
                console.warn('DEBUG: 37 : tutorService updateCurrentRoomState after ')
        
                if (store.getters['getStudyRoomData'].isTutor) {
                    console.warn('DEBUG: 40 : tutorService store.getters[getStudyRoomData].isTutor')
                    store.dispatch('hideRoomToasterMessage');
                    let studentName = !!store.getters['getStudyRoomData'] ? store.getters['getStudyRoomData'].studentName : '';
                    let studentId = !!store.getters['getStudyRoomData'] ? store.getters['getStudyRoomData'].studentId : '';
                    analyticsService.sb_unitedEvent('study_room', 'session_started', `studentName: ${studentName} studentId: ${studentId}`);
                    if (store.getters['getTutorStartDialog']) {
                        store.dispatch('updateTutorStartDialog', false);
                    }
                }
            });
            // When a Participant adds a Track, attach it to the DOM.
            store.getters['activeRoom'].on('trackSubscribed', (track) => {
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioTrackSubscribed', track, null);
                let previewContainer = document.getElementById('remoteTrack');
                if (track.kind === 'data') {
                    track.on('message', transferObj => {
                        let data = JSON.parse(transferObj);
                        let parsedData = data.data;
                        if (data.type === 'passData') {
                            whiteBoardService.passData(parsedData.canvasContext, parsedData.dataContext);
                        } else if (data.type === 'undoData') {
                            whiteBoardService.undo(parsedData, data.tab);
                        } else if (data.type === 'clearCanvas') {
                            whiteBoardService.clearData(parsedData, data.tab);
                        } else if(data.type === 'codeEditor_lang'){
                            store.commit('setLang',parsedData);
                        } else if (data.type === 'updateTab'){
                            store.dispatch('updateTab', parsedData);
                        } else if(data.type === 'updateTabById'){
                            store.commit('setTab',parsedData);
                        } 
                        else if(data.type === 'updateActiveNav'){
                            store.commit('setActiveNavIndicator',parsedData);
                        } 
                        else if(data.type === 'codeEditor_code'){
                            store.commit('setCode',parsedData);
                        }
                        
                    });
                } else if (track.kind === 'video') {
                    let videoTag = previewContainer.querySelector("video");
                    if (videoTag) {
                        previewContainer.removeChild(videoTag);
                    }
                    let updateObj = {
                        type: "video",
                        track,
                        container: previewContainer
                    };
                    store.commit('releaseFullVideoButton',true);
                    store.dispatch('updateRemoteTrack', updateObj);
                } else if (track.kind === 'audio') {
                    let updateObj = {
                        type: "audio",
                        track,
                        container: previewContainer
                    };
                    store.dispatch('updateRemoteTrack', updateObj);
                }

                console.log('track attached', " added track: " + track.kind, track);
            });
            // When a Participant's Track is unsubscribed from, detach it from the DOM.
            store.getters['activeRoom'].on('trackUnsubscribed', function (track) {
                if(track.kind === 'video'){
                    store.commit('releaseFullVideoButton',false);
                }
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioTrackUnsubscribed', track, null);
                console.log(" removed track: " + track.kind);
                detachTracks([track]);
            });

            // When a Participant leaves the Room, detach its Tracks.
            store.getters['activeRoom'].on('participantDisconnected', (participant) => {
                console.warn('DEBUG: 41 : tutorService participantDisconnected')
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioParticipantDisconnected', participant, null);
                let localIdentity = store.getters['userIdentity'];
                console.log("Participant '" + participant.identity + "' left the room");

                if (participant.identity !== localIdentity) {
                    console.warn('DEBUG: 41.5 : tutorService setSesionClickedOnce,false before')
                    store.dispatch('setSesionClickedOnce', false);
                    console.warn('DEBUG: 41.6 : tutorService setSesionClickedOnce,false after')

                    if (store.getters['getStudyRoomData'].isTutor) {
                        console.warn('DEBUG: 41.7 : tutorService setTutorDialogState before')
                        console.warn('DEBUG: 41.7 : tutorService setTutorDialogState data: ',store.state.tutoringMain.startSessionDialogStateEnum.disconnected)
                        store.dispatch("setTutorDialogState", store.state.tutoringMain.startSessionDialogStateEnum.disconnected);
                        console.warn('DEBUG: 41.8 : tutorService setTutorDialogState after')

                        console.warn('DEBUG: 41.9 : tutorService updateTutorStartDialog,true before')
                        store.dispatch('updateTutorStartDialog', true);
                        console.warn('DEBUG: 41.9.1 : tutorService updateTutorStartDialog,true after')

                    } else {
                        store.dispatch("setStudentDialogState", store.state.tutoringMain.startSessionDialogStateEnum.disconnected);
                    }
                }
                detachParticipantTracks(participant);
            });
        },
            (error) => {
                console.error(error, 'error cant connect');

            });
};
// TODO: move to studyroom service

const enterRoom = function (roomId) {
    
    return connectivityModule.http.post(`StudyRoom/${roomId}/enter`)
    .then(() => {
        console.warn('DEBUG: 29 tutorService: enterRoom')

            return true;
        });
};

function DevicesObject(){
    this.hasAudio= false,
    this.hasVideo= false,
    this.errors= {
        video: [],
        audio: []
    };
}

function createDevicesObj(){
    return new DevicesObject();
}

const validateUserMedia = async function() {
    let devicesObj = store.getters['getDevicesObj'];
    await navigator.mediaDevices.getUserMedia({ video: true }).then((y) => {
        console.log(y);
        devicesObj.hasVideo = true;
    }, err => {
        let insightErrorObj={
            error: err,
            userId: this.userId
        };
        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_validationDialog_getUserMedia_VIDEO', insightErrorObj, null);
        console.error(err.name + ":VIDEO!!!!!!!!!!!!!!!! " + err.message, err);
        devicesObj.errors.video.push(err.name);
    });

    await navigator.mediaDevices.getUserMedia({ audio: true }).then((y) => {
        console.log(y);
        devicesObj.hasAudio = true;
    }, err => {
        let insightErrorObj={
            error: err,
            userId: this.userId
        };
        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_validationDialog_getUserMedia_AUDIO', insightErrorObj, null);
        console.error(err.name + ":AUDIO!!!!!!!!!!!!!!!! " + err.message, err);
        devicesObj.errors.audio.push(err.name);
    });
};


function isRecordingSupported(){
     return store.getters.getStudyRoomData ? !store.getters.getStudyRoomData.isTutor : true;
}

export default {
    attachTracks, // remove from here cuz no one using outside this service.

    dataTrack,
    detachTracks,
    connectToRoom,
    enterRoom,
    validateUserMedia,
    createDevicesObj,
    isRecordingSupported
};