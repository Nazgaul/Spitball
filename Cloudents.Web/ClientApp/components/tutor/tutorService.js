import Twilio, { connect, createLocalTracks, createLocalVideoTrack, LocalDataTrack } from 'twilio-video';
import { connectivityModule } from '../../services/connectivity.module';
import store from '../../store/index.js'
import whiteBoardService from "./whiteboard/whiteBoardService";

const dataTrack = new LocalDataTrack();

const uploadCanvasImage = function(formData){
    return connectivityModule.http.post("Tutoring/upload", formData);
};
const getSharedDoc = async function(docName){
   return connectivityModule.http.post("Tutoring/document", docName)
       .then((resp)=>{
        return  resp.data.link
   })
};
const passSharedDocLink = function (docUrl) {
    let transferDataObj = {
        type: "sharedDocumentLink",
        data: docUrl
    };
    let normalizedData = JSON.stringify(transferDataObj);
    console.log('service data track', dataTrack);
    dataTrack.send(normalizedData);
};

const createRoom = () => {
    return connectivityModule.http.post("tutoring/create");
};
// Token get
const getToken = function (name, identityName){
    let userIdentity = identityName || '';
    return connectivityModule.http.get(`tutoring/join?roomName=${name}&identityName=${userIdentity}`)
        .then((data) => {
            return data.data.token
        });
};


// Attach the Tracks to the DOM.
const attachTracks = function(tracks, container){
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
const detachTracks = function(tracks){
    tracks.forEach((track) => {
        if (track.detach) {
            track.detach().forEach((detachedElement) => {
                detachedElement.remove();
            });
        }
    });
};

// Detach the Participant's Tracks from the DOM.
const detachParticipantTracks = function(participant){
    let tracks = Array.from(participant.tracks.values());
    detachTracks(tracks);
};

const printNetworkQuality = function (networkQualityLevel) {
    console.log({
        1: '▃',
        2: '▃▄',
        3: '▃▄▅',
        4: '▃▄▅▆',
        5: '▃▄▅▆▇'
    }[networkQualityLevel] || '');
};



const connectToRoom = function(token, options) {
    // disconnect the user from they joined already
    store.dispatch('leaveRoomIfJoined');
    Twilio.connect(token, options)
        .then((room) => {
                store.dispatch('updateRoomInstance', room);
                console.log('Successfully joined a Room: ');
                store.dispatch('updateRoomLoading', false);
                //update room status in store of chat to use for show/hide shareBtn
                store.dispatch('updateRoomStatus', true);
                let localIdentity = room.localParticipant && room.localParticipant.identity ? room.localParticipant.identity : '';
                store.dispatch('updateUserIdentity', localIdentity);
                store.dispatch('updateLocalStatus', false);
                localStorage.setItem("identity", localIdentity);


                // Print the initial Network Quality Level
                printNetworkQuality(room.localParticipant.networkQualityLevel);

                store.getters['activeRoom'].localParticipant.on('networkQualityLevelChanged', printNetworkQuality);

                //shared google document
                if ( store.getters['activeRoom'].participants && store.getters['activeRoom'].participants.size < 1) {
                    let shareLink = localStorage.getItem(`sb_share_link_${store.getters['roomLinkID']}`);
                    if(!shareLink){
                        store.dispatch('updateRoomIsFull', true);
                            getSharedDoc({name: `${store.getters['roomLinkID']}`})
                                .then((link) => {
                                        localStorage.setItem(`sb_share_link_${store.getters['roomLinkID']}`, `${link}&embedded=true`);
                                        store.dispatch('updateSharedDocLink',`${link}&embedded=true`);
                                    },
                                    (error) => {
                                    }
                                );

                    }else{
                        store.dispatch('updateSharedDocLink',`${shareLink}`);
                    }
                }
                // Attach the Tracks of all the remote Participants.
                store.getters['activeRoom'].participants.forEach((participant, index) => {
                    let previewContainer = document.getElementById('remoteTrack');
                    attachParticipantTracks(participant, previewContainer);
                    store.dispatch('updateRemoteStatus', false);

                });
                // Attach the Participant's Media to a <div> element.
                store.getters['activeRoom'].on('participantConnected', participant => {
                    console.log(`Participant "${participant.identity}" connected`);
                    participant.tracks.forEach(publication => {
                        if (publication.isSubscribed) {
                            const track = publication.track;
                            let previewContainer = document.getElementById('remoteTrack');
                            console.log('remote track attached', " added track: " + track.kind);
                            attachTracks([track], previewContainer);
                        }
                    });
                    store.dispatch('updateRemoteStatus', false);
                });
                // When a Participant adds a Track, attach it to the DOM.
                store.getters['activeRoom'].on('trackSubscribed', (track, participant) => {
                    let previewContainer = document.getElementById('remoteTrack');
                    if (track.kind === 'data') {
                        let shareDocUrl = store.getters['sharedDocUrl'];
                        passSharedDocLink(shareDocUrl);
                        track.on('message', transferObj => {
                            let Data = JSON.parse(transferObj);
                            let parsedData = Data.data;
                            if (Data.type === 'passData') {
                                whiteBoardService.passData(parsedData.canvasContext, parsedData.dataContext);
                            } else if (Data.type === 'undoData') {
                                whiteBoardService.undo(parsedData);
                            } else if (Data.type === 'tutoringChatMessage') {
                                store.dispatch('addMessage', Data);
                            } else if (Data.type === 'sharedDocumentLink') {
                                if(!shareDocUrl){
                                    store.dispatch('updateSharedDocLink',parsedData)
                                }
                            }
                        });
                        attachTracks([track], previewContainer);
                    }else if(track.kind === 'video'){
                        let videoTag = previewContainer.querySelector("video");
                        if(videoTag){
                            previewContainer.removeChild(videoTag);
                        }
                        attachTracks([track], previewContainer);
                    }else if(track.kind === 'audio'){
                        attachTracks([track], previewContainer);
                    }
                    console.log('track attached', " added track: " + track.kind);
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
                    if (participant.identity === localIdentity) {
                        store.dispatch('updateLocalStatus', true);
                    } else {
                        store.dispatch('updateRemoteStatus', true);
                    }
                    detachParticipantTracks(participant);
                });
            },
            (error) => {
                console.log(error, 'error cant connect')
            });
};


export {
    dataTrack,
    uploadCanvasImage,
    getToken,
    createRoom,
    connectToRoom
}