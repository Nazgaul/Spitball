
import Twilio, { connect, LocalDataTrack } from 'twilio-video';
import { connectivityModule } from '../../services/connectivity.module';
import { LanguageService } from '../../services/language/languageService';
import store from '../../store/index.js';
import whiteBoardService from "./whiteboard/whiteBoardService";
import insightService from '../../services/insightService';
import analyticsService from '../../services/analytics.service';



const dataTrack = new LocalDataTrack();
const uploadCanvasImage = function (formData) {
    return connectivityModule.http.post("StudyRoom/upload", formData);
};
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
    store.dispatch('updateLocalParticipantsNetworkQuality', networkQualityLevel);
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


const createAudioContext = function () {
    navigator.mediaDevices.getUserMedia({
        audio: true
    }).then(stream => {
        const processInput = audioProcessingEvent => {
            let array = new Uint8Array(analyzer.frequencyBinCount);
            analyzer.getByteFrequencyData(array);
            let values = 0;

            let length = array.length;
            for (let i = 0; i < length; i++) {
                values += (array[i]);
            }

            let average = values / length;

            //console.log(Math.round(average - 40));
            //TODO fix style to class
            let micVolume = document.getElementById('micVolume_indicator');
            if (!micVolume) return;
            micVolume.style.backgroundColor = 'rgba(66, 224, 113, 0.8)';
            micVolume.style.height = '6px';
            micVolume.style.maxWidth = '150px';
            micVolume.style.width = `${Math.round(average)}px`;

        };

        // Handle the incoming audio stream
        // Handle the incoming audio stream
        const audioContext = new (global.AudioContext || global.webkitAudioContext)();
        const input = audioContext.createMediaStreamSource(stream);
        const analyzer = audioContext.createAnalyser();
        const scriptProcessor = audioContext.createScriptProcessor();

        // Some analyser setup
        analyzer.smoothingTimeConstant = 0.3;
        analyzer.fftSize = 1024;

        input.connect(analyzer);
        analyzer.connect(scriptProcessor);
        scriptProcessor.connect(audioContext.destination);
        scriptProcessor.onaudioprocess = processInput;

    }, error => {
        console.log('Something went wrong, or the browser does not support getUserMedia');
        // Something went wrong, or the browser does not support getUserMedia
    });

};

const connectToRoom = function (token, options) {
    // disconnect the user from room if they already joined
    insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_connectToRoom', {'token': token}, null);
    store.dispatch('leaveRoomIfJoined');
    Twilio.connect(token, options)
        .then((room) => {
            // add microphone indicator, comment if not in use, otherwise will throw errors cause cant get element
            // createAudioContext();
            insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioConnect', room, null);
            store.dispatch('updateRoomInstance', room);
            console.log('Successfully joined a Room: ', room);
            store.dispatch('updateRoomLoading', false);
            //update room status in store of chat to use for show/hide shareBtn
            store.dispatch('updateRoomStatus', true);
            // TODO persistent
            let localIdentity = room.localParticipant && room.localParticipant.identity ? room.localParticipant.identity : '';
            store.dispatch('updateUserIdentity', localIdentity);
            store.dispatch('updateLocalStatus', false);
            //set local participant in store
            store.dispatch('updateLocalParticipant', room.localParticipant);

            // Print the initial Network Quality Level
            // printNetworkQuality(store.getters['localParticipant'].networkQualityLevel);
            let toasterParams = {};
            if (store.getters['getStudyRoomData'].isTutor) {
                toasterParams.text = LanguageService.getValueByKey('studyRoom_waiting_for_student_toaster');
                toasterParams.timeout = 3600000;
                store.dispatch('showRoomToasterMessage', toasterParams);
            } else {
                store.dispatch('hideRoomToasterMessage');
            }
            

            if (store.getters['getStudentStartDialog']) {
                store.dispatch('updateStudentStartDialog', false);

            } else if (store.getters['getTutorStartDialog']) {
                store.dispatch('updateTutorStartDialog', false);
            }
            //event of network quality change
            store.getters['localParticipant'].on('networkQualityLevelChanged', printNetworkQuality);
            // Attach the Tracks of all the remote Participants.
            store.getters['activeRoom'].participants.forEach((participant) => {
                let previewContainer = document.getElementById('remoteTrack');
                attachParticipantTracks(participant, previewContainer);
                store.dispatch('updateRemoteStatus', false);

            });
            //disconnected room
            store.getters['activeRoom'].on('disconnected', (room, error) => {
                if(!error) return;
                
                let errorCode = !!error && error.code ? error.code : "";
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioDisconnected', {'errorCode': errorCode}, null);
                if (errorCode === 20104) {
                    console.error('Signaling reconnection failed due to expired AccessToken!');
                } else if (errorCode === 53000) {
                    console.error('Signaling reconnection attempts exhausted!');
                } else if (errorCode === 53204) {
                    console.error('Signaling reconnection took too long!');
                } else {
                    console.error('final disconnect');
                }
                if (store.getters['getStudyRoomData'].isTutor) {
                    store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.ready);
                } else {
                    store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.pending);
                    if (store.getters['getAllowReview']) {
                        store.dispatch('updateReviewDialog', true);
                    }
                }
                let toasterParams = {
                    text: LanguageService.getValueByKey('studyRoom_session_ended'),
                    type: 'error-toaster'
                };
                store.dispatch('showRoomToasterMessage', toasterParams);
                        store.dispatch('updateEndDialog', false);
                //detach all local tracks to prevent multiple added tracks
                store.getters['activeRoom'].localParticipant.tracks.forEach(function (track) {
                    detachTracks([track]);
                    // track.stop()
                });

            });
            //reconnecting room
            store.getters['activeRoom'].on('reconnecting', (error) => {
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioReconnecting', null, null);
                console.log("ROOM - RECONNECTING");
            });
            
            //reconnected room
            store.getters['activeRoom'].on('reconnected', () => {
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioReconnected', null, null);
                console.log("ROOM - RECONNECTED");
                /* Update the application UI here */
            });

            // Attach the Participant's Media to a <div> element.
            store.getters['activeRoom'].on('participantConnected', participant => {
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioParticipantConnected', participant, null);
                console.log(`Participant "${participant.identity}" connected`);
                store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.active);
                store.dispatch('updateRemoteStatus', false);
                if (store.getters['getStudyRoomData'].isTutor) {
                    store.dispatch('hideRoomToasterMessage');
                    let studentName = !!store.getters['getStudyRoomData'] ? store.getters['getStudyRoomData'].studentName : ''
                    let studentId = !!store.getters['getStudyRoomData'] ? store.getters['getStudyRoomData'].studentId : ''
                    analyticsService.sb_unitedEvent('study_room', 'session_started', `studentName: ${studentName} studentId: ${studentId}`);
                }
            });
            // When a Participant adds a Track, attach it to the DOM.
            store.getters['activeRoom'].on('trackSubscribed', (track, participant) => {
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioTrackSubscribed', track, null);
                let previewContainer = document.getElementById('remoteTrack');
                if (track.kind === 'data') {
                    track.on('message', transferObj => {
                        let Data = JSON.parse(transferObj);
                        let parsedData = Data.data;
                        if (Data.type === 'passData') {
                            whiteBoardService.passData(parsedData.canvasContext, parsedData.dataContext);
                        } else if (Data.type === 'undoData') {
                            whiteBoardService.undo(parsedData, Data.tab);
                        } else if (Data.type === 'clearCanvas') {
                            whiteBoardService.clearData(parsedData, Data.tab);
                        } else if(Data.type === 'codeEditor_lang'){
                            store.commit('setLang',parsedData)
                        } else if (Data.type === 'updateTab'){
                            store.dispatch('updateTab', parsedData);
                        } else if(Data.type === 'updateTabById'){
                            store.commit('setTab',parsedData)
                        } 
                        else if(Data.type === 'updateActiveNav'){
                            store.commit('setActiveNavIndicator',parsedData)
                        } 
                        else if(Data.type === 'codeEditor_code'){
                            store.commit('setCode',parsedData)
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
                    }
                    store.commit('releaseFullVideoButton',true)
                    store.dispatch('updateRemoteTrack', updateObj);
                } else if (track.kind === 'audio') {
                    let updateObj = {
                        type: "audio",
                        track,
                        container: previewContainer
                    }
                    store.dispatch('updateRemoteTrack', updateObj);
                    // attachTracks([track], previewContainer);
                }

                console.log('track attached', " added track: " + track.kind, track);
            });
            // When a Participant's Track is unsubscribed from, detach it from the DOM.
            store.getters['activeRoom'].on('trackUnsubscribed', function (track) {
                if(track.kind === 'video'){
                    store.commit('releaseFullVideoButton',false)
                }
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioTrackUnsubscribed', track, null);
                console.log(" removed track: " + track.kind);
                detachTracks([track]);
            });

            // When a Participant leaves the Room, detach its Tracks.
            store.getters['activeRoom'].on('participantDisconnected', (participant) => {
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_tutorService_TwilioParticipantDisconnected', participant, null);
                let localIdentity = store.getters['userIdentity'];
                console.log("Participant '" + participant.identity + "' left the room");
                if (participant.identity === localIdentity) {
                    store.dispatch('updateLocalStatus', true);
                } else {
                    store.dispatch('updateRemoteStatus', true);
                    // endTutoringSession(store.getters['getRoomId']);
                    store.dispatch('setSesionClickedOnce', false);
                    // if (store.getters['getStudyRoomData'].isTutor) {
                    //     store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.ready);
                    // } else {
                    //     store.dispatch('updateCurrentRoomState', store.state.tutoringMain.roomStateEnum.pending);
                    // }
                }
                detachParticipantTracks(participant);
            });
        },
            (error) => {
                console.error(error, 'error cant connect');

            });
};

const getRoomInformation = function (roomId) {
    return connectivityModule.http.get(`StudyRoom/${roomId}`).then(({data})=>{
        data.roomId = roomId;
        return new RoomProps(data)
    })
};

const enterRoom = function (roomId) {
    return connectivityModule.http.post(`StudyRoom/${roomId}/enter`)
        .then(() => {
            return true;
        });
};
const endTutoringSession = function (roomId) {
    return connectivityModule.http.post(`StudyRoom/${roomId}/end`)
        .then(() => {
            return true;
        });
};

function RoomProps(objInit) {
    this.allowReview = true;
    this.conversationId = objInit.conversationId || '';
    this.needPayment = objInit.needPayment;
    this.onlineDocument = objInit.onlineDocument || '';
    this.studentId = objInit.studentId || null;
    this.studentImage = objInit.studentImage || null;
    this.studentName = objInit.studentName || null;
    this.tutorId = objInit.tutorId || null;
    this.tutorImage = objInit.tutorImage || null;
    this.tutorName = objInit.tutorName || null;
    this.isTutor = store.getters['accountUser'].id == objInit.tutorId;
    this.roomId = objInit.roomId || '';
}

const createRoomProps = function (objInit) {
    return new RoomProps(objInit);
};

function DevicesObject(){
    this.hasAudio= false,
    this.hasVideo= false,
    this.errors= {
        video: [],
        audio: []
    }
}

function createDevicesObj(){
    return new DevicesObject();
}

const validateUserMedia = async function(audioCheck, videoCheck) {
    // let self = this;
    // let devices = await navigator.mediaDevices.enumerateDevices();
    let devicesObj = store.getters['getDevicesObj'];
    await navigator.mediaDevices.getUserMedia({ video: true }).then((y) => {
        console.log(y);
        devicesObj.hasVideo = true;
    }, err => {
        let insightErrorObj={
            error: err,
            userId: this.userId
        }
        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_validationDialog_getUserMedia_VIDEO', insightErrorObj, null);
        console.error(err.name + ":VIDEO!!!!!!!!!!!!!!!! " + err.message, err);
        devicesObj.errors.video.push(err.name)
    });

    await navigator.mediaDevices.getUserMedia({ audio: true }).then((y) => {
        console.log(y);
        devicesObj.hasAudio = true;
    }, err => {
        let insightErrorObj={
            error: err,
            userId: this.userId
        }
        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_validationDialog_getUserMedia_AUDIO', insightErrorObj, null);
        console.error(err.name + ":AUDIO!!!!!!!!!!!!!!!! " + err.message, err);
        devicesObj.errors.audio.push(err.name)
    });
}

export default {
    dataTrack,
    attachTracks,
    detachTracks,
    uploadCanvasImage,
    connectToRoom,
    getRoomInformation,
    enterRoom,
    createRoomProps,
    endTutoringSession,
    validateUserMedia,
    createDevicesObj
};