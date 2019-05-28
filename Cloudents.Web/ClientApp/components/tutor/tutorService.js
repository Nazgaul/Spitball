import Twilio, { connect, LocalDataTrack } from 'twilio-video';
import { connectivityModule } from '../../services/connectivity.module';
import { LanguageService } from '../../services/language/languageService';
import store from '../../store/index.js';
import whiteBoardService from "./whiteboard/whiteBoardService";

const dataTrack = new LocalDataTrack();

const uploadCanvasImage = function (formData) {
    return connectivityModule.http.post("StudyRoom/upload", formData);
};
// Attach the Tracks to the DOM.
const attachTracks = function (tracks, container) {
    tracks.forEach((track) => {
        if(track.attach) {
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
        if(track.detach) {
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

const printNetworkQuality = function (networkQualityLevel) {
    store.dispatch('updateLocalParticipantsNetworkQuality', networkQualityLevel);
    console.log({
        1: '▃',
        2: '▃▄',
        3: '▃▄▅',
        4: '▃▄▅▆',
        5: '▃▄▅▆▇'
    }[networkQualityLevel] || '');
};


const createAudioContext = function () {
    navigator.mediaDevices.getUserMedia({
                                            audio: true,
                                        }).then(stream => {
        const processInput = audioProcessingEvent => {
            let array = new Uint8Array(analyser.frequencyBinCount);
            analyser.getByteFrequencyData(array);
            let values = 0;

            let length = array.length;
            for (let i = 0; i < length; i++) {
                values += (array[i]);
            }

            let average = values / length;

            //console.log(Math.round(average - 40));
            //TODO fix style to class
            let micVolume = document.getElementById('micVolume_indicator');
            if(!micVolume)return
            micVolume.style.backgroundColor = 'rgba(66, 224, 113, 0.8)';
            micVolume.style.height = '6px';
            micVolume.style.maxWidth = '150px';
            micVolume.style.width = `${Math.round(average)}px`;

        };

        // Handle the incoming audio stream
        // Handle the incoming audio stream
        const audioContext = new (global.AudioContext || global.webkitAudioContext)();
        const input = audioContext.createMediaStreamSource(stream);
        const analyser = audioContext.createAnalyser();
        const scriptProcessor = audioContext.createScriptProcessor();

        // Some analyser setup
        analyser.smoothingTimeConstant = 0.3;
        analyser.fftSize = 1024;

        input.connect(analyser);
        analyser.connect(scriptProcessor);
        scriptProcessor.connect(audioContext.destination);
        scriptProcessor.onaudioprocess = processInput;

    }, error => {
        console.log('Something went wrong, or the browser does not support getUserMedia');
        // Something went wrong, or the browser does not support getUserMedia
    });

};

const connectToRoom = function (token, options) {
    // disconnect the user from room if they already joined
    store.dispatch('leaveRoomIfJoined');
    Twilio.connect(token, options)
          .then((room) => {
                    // add microphone indicator, comment if not in use, otherwise will throw errors cause cant get element
                    // createAudioContext();
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

                    if(store.getters['getStudentStartDialog']){
                        store.dispatch('updateStudentStartDialog', false);

                    }else if(store.getters['getTutorStartDialog']){
                        store.dispatch('updateTutorStartDialog', false);

                    }
                    //event of network quality change
                    store.getters['localParticipant'].on('networkQualityLevelChanged', printNetworkQuality);
                    // Attach the Tracks of all the remote Participants.
                    store.getters['activeRoom'].participants.forEach((participant, index) => {
                        let previewContainer = document.getElementById('remoteTrack');
                        attachParticipantTracks(participant, previewContainer);
                        store.dispatch('updateRemoteStatus', false);

                    });
                    //disconnected room
              store.getters['activeRoom'].on('disconnected', (room, error) => {
                        let errorCode = !!error && error.code ? error.code : "";
                        if (errorCode === 20104) {
                            console.error('Signaling reconnection failed due to expired AccessToken!');
                        } else if (errorCode=== 53000) {
                            console.error('Signaling reconnection attempts exhausted!');
                        } else if (errorCode === 53204) {
                            console.error('Signaling reconnection took too long!');
                        }else{
                            console.error('final disconnect')
                        }
                        if(store.getters['getStudyRoomData'].isTutor){
                            store.dispatch('updateCurrentRoomState', store.state.tutoringMainStore.roomStateEnum.ready);
                        }else{
                            store.dispatch('updateCurrentRoomState', store.state.tutoringMainStore.roomStateEnum.pending);
                            if(store.getters['getAllowReview']){
                                store.dispatch('updateReviewDialog', true);
                            }
                        }
                        let toasterParams = {
                            text: LanguageService.getValueByKey('studyRoom_session_ended'),
                            type: 'error-toaster'
                        };
                        store.dispatch('showRoomToasterMessage', toasterParams);
                        //detach all local tracks to prevent multiple added tracks
                        store.getters['activeRoom'].localParticipant.tracks.forEach(function(track) {
                            detachTracks([track]);
                            // track.stop()
                        });

                    });

                    // Attach the Participant's Media to a <div> element.
                    store.getters['activeRoom'].on('participantConnected', participant => {
                        console.log(`Participant "${participant.identity}" connected`);
                        participant.tracks.forEach(publication => {
                            if(publication.isSubscribed) {
                                const track = publication.track;
                                let previewContainer = document.getElementById('remoteTrack');
                                console.log('remote track attached', " added track: " + track.kind);
                                attachTracks([track], previewContainer);
                            }
                        });
                        store.dispatch('updateCurrentRoomState', store.state.tutoringMainStore.roomStateEnum.active);
                        store.dispatch('updateRemoteStatus', false);
                    });
                    // When a Participant adds a Track, attach it to the DOM.
                    store.getters['activeRoom'].on('trackSubscribed', (track, participant) => {
                        let previewContainer = document.getElementById('remoteTrack');
                        if(track.kind === 'data') {
                            track.on('message', transferObj => {
                                let Data = JSON.parse(transferObj);
                                let parsedData = Data.data;
                                if(Data.type === 'passData') {
                                    whiteBoardService.passData(parsedData.canvasContext, parsedData.dataContext);
                                } else if(Data.type === 'undoData') {
                                    whiteBoardService.undo(parsedData, Data.tab);
                                }
                            });
                            attachTracks([track], previewContainer);
                        } else if(track.kind === 'video') {
                            let videoTag = previewContainer.querySelector("video");
                            if(videoTag) {
                                previewContainer.removeChild(videoTag);
                            }
                            attachTracks([track], previewContainer);
                        } else if(track.kind === 'audio') {
                            attachTracks([track], previewContainer);
                        }
                        console.log('track attached', " added track: " + track.kind, track);
                    });
                    // When a Participant's Track is unsubscribed from, detach it from the DOM.
                    store.getters['activeRoom'].on('trackUnsubscribed', function (track) {
                        console.log(" removed track: " + track.kind);
                        detachTracks([track]);
                    });

                    // When a Participant leaves the Room, detach its Tracks.
                    store.getters['activeRoom'].on('participantDisconnected', (participant) => {
                        let localIdentity = store.getters['userIdentity'];
                        console.log("Participant '" + participant.identity + "' left the room");
                        if(participant.identity === localIdentity) {
                            store.dispatch('updateLocalStatus', true);
                        } else {
                            store.dispatch('updateRemoteStatus', true);
                            endTutoringSession(store.getters['getRoomId']);
                            if(store.getters['getStudyRoomData'].isTutor){
                                store.dispatch('updateCurrentRoomState', store.state.tutoringMainStore.roomStateEnum.ready);
                            }else{
                                store.dispatch('updateCurrentRoomState', store.state.tutoringMainStore.roomStateEnum.pending);
                            }
                        }
                        detachParticipantTracks(participant);
                    });
                },
                (error) => {
                    console.error(error, 'error cant connect');
                });
};

const getRoomInformation = function (roomId) {
    return connectivityModule.http.get(`StudyRoom/${roomId}`);
};

const enterRoom = function (roomId) {
    return connectivityModule.http.post(`StudyRoom/${roomId}/enter`)
                             .then(() => {
                                 return true
                             });
};
const endTutoringSession = function (roomId) {
    return connectivityModule.http.post(`StudyRoom/${roomId}/end`)
                             .then(() => {
                                 return true
                             });
};
function RoomProps(ObjInit) {
    this.conversationId = ObjInit.conversationId || '';
    this.onlineDocument = ObjInit.onlineDocument || '';
    this.isTutor = store.getters['accountUser'].id == ObjInit.tutorId;
    this.roomId = ObjInit.roomId || '';
    this.allowReview = ObjInit.allowReview;
    this.needPayment = ObjInit.needPayment;
    this.studentId = ObjInit.studentId || null;
    this.studentImage = ObjInit.studentImage || null;
    this.tutorImage = ObjInit.tutorImage || null;
    this.studentName = ObjInit.studentName || null;
    this.tutorName = ObjInit.tutorName || null;

}

const createRoomProps = function createLeaderBoardItem(ObjInit){
    return new RoomProps(ObjInit)
};

export default {
    dataTrack,
    attachTracks,
    detachTracks,
    uploadCanvasImage,
    connectToRoom,
    getRoomInformation,
    enterRoom,
    createRoomProps,
    endTutoringSession
};