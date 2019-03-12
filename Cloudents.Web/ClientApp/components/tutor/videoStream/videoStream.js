
import { mapActions, mapGetters } from 'vuex';
import Twilio, { connect, createLocalTracks, createLocalVideoTrack, LocalDataTrack } from 'twilio-video';
import videoService from '../../../services/videoStreamService';
import { dataTrack, getSharedDoc, passSharedDocLink } from '../tutorService';
import whiteBoardService from '../whiteboard/whiteBoardService';


export default {
    name: "videoStream",
    data() {
        return {
            loading: false,
            loaded: false,
            data: {},
            isCopied: false,
            localTrackAval: false,
            remoteTrack: '',
            screenShareTrack: null,
            isSharing: false,
            activeRoom: '',
            previewTracks: '',
            identity: '',
            roomName: '',
            roomLink: '',
            username: '',
            members: [],
            localOffline: true,
            remoteOffline: true,
            availableDevices: [],
            videoTracksQuantatyAttached: [],
            trackToReDetach: {},
            visible: {
                'local_player': true,
                'remote_player': true
            }
        }
    },
    props: {
        id: ''
    },
    computed: {
        ...mapGetters(['sharedDocUrl', 'roomLinkID', 'isRoomFull'])
    },
    watch: {
        '$route': 'createChat'
    },
    methods: {
        ...mapActions(['addMessage', 'updateUserIdentity', 'updateRoomStatus', 'updateRoomID', 'updateSharedDocLink', 'updateRoomIsFull']),
        stopSharing() {
            let self = this;
            self.unPublishTrackfromRoom(self.screenShareTrack);
            //create new track
            createLocalVideoTrack().then(function(videoTrack) {
                    self.publishTrackToRoom(videoTrack);
                    // self.screenShareTrack = null;
                    self.isSharing = false;
                },
                (error)=>{
                    console.log('error creating video track')
                }
            );
        },
        publishTrackToRoom(track){
            this.activeRoom.localParticipant.publishTrack(track);
            // this.screenShareTrack = null;
        },
        unPublishTrackfromRoom(track){
            this.activeRoom.localParticipant.unpublishTrack(track);

        },
        //screen share functionality
        showScreen() {
            let self = this;
            videoService.getUserScreen().then((stream)=> {
                    self.screenShareTrack = stream.getVideoTracks()[0];
                    self.publishTrackToRoom(self.screenShareTrack);
                    self.isSharing = true;
                },
                (error) => {
                    console.log('error sharing screen')
                }
            );
        },
        //end screen share functions

        biggerRemoteVideo() {
            let video = document.querySelectorAll("#remoteTrack video")[0];
            video.requestFullscreen()
        },
        minimize(type) {
            this.visible[`${type}`] = !this.visible[`${type}`];
        },
        generateRoom() {
            let self = this;
            videoService.generateRoom().then(data => {
                self.roomLink = data.data.name;
                self.$router.push({name: 'tutoring', params: {id: self.roomLink}});
                self.updateRoomID(self.roomLink)
            })

        },
        startChat() {
            this.createChat()
        },
        // Generate access token
        async getAccessToken() {
            let identity = localStorage.getItem("identity");
            return await videoService.getToken(this.id, identity);
        },

        // Attach the Tracks to the DOM.
        attachTracks(tracks, container) {
            tracks.forEach((track) => {
                if (track.attach) {
                    container.appendChild(track.attach());
                }
            });
        },
        // Attach the Participant's Tracks to the DOM.
        attachParticipantTracks(participant, container) {
            let tracks = Array.from(participant.tracks.values());
            this.attachTracks(tracks, container);
        },

        // Detach the Tracks from the DOM.
        detachTracks(tracks) {
            tracks.forEach((track) => {
                if (track.detach) {
                    track.detach().forEach((detachedElement) => {
                        detachedElement.remove();
                    });
                }
            });
        },

        // Detach the Participant's Tracks from the DOM.
        detachParticipantTracks(participant) {
            let tracks = Array.from(participant.tracks.values());
            this.detachTracks(tracks);
        },
        // Leave Room.
        leaveRoomIfJoined() {
            if (this.activeRoom) {
                this.activeRoom.disconnect();
            }
        },
        //delete members from array when left
        removeMember(mem) {
            let index = this.members.indexOf(mem);
            this.members.splice(index, 1);
        },
        async isHardawareAvaliable() {
            let self = this;
            if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
                console.log("enumerateDevices() not supported.");
                return;
            }
            const token = await self.getAccessToken();
            // List cameras and microphones.
            navigator.mediaDevices.enumerateDevices()
                .then(function (devices) {
                    devices.forEach(function (device) {
                        console.log(device.kind + ": " + device.label +
                            " id = " + device.deviceId);
                        self.availableDevices.push(device.kind);
                    });
                    let connectOptions;
                    createLocalTracks({
                        audio: self.availableDevices.includes('audioinput'),
                        video: self.availableDevices.includes('videoinput') ? {width: 720, height: 480} : false,
                    }).then((tracksCreated) => {
                        let localMediaContainer = document.getElementById('localTrack');
                        tracksCreated.forEach((track) => {
                            localMediaContainer.appendChild(track.attach());
                            self.localTrackAval = true;
                        });
                        if (self.availableDevices) {
                            tracksCreated.push(dataTrack);
                            connectOptions = {
                                tracks: tracksCreated
                            }
                        }
                        self.connect(token, connectOptions);

                    }, (error) => {
                        console.log(error, 'error create tracks before connect')
                    })

                })
                .catch(function (err) {
                    console.log(err.name + ": " + err.message);
                });
        },
        getSharedocUrl() {
            let self = this;
            self.updateRoomIsFull(true);
            getSharedDoc({name: self.roomLinkID})
                .then((link) => {
                        localStorage.setItem(`sb_share_link_${self.roomLinkID}`, `${link}&embedded=true`);
                        self.updateSharedDocLink(`${link}&embedded=true`);
                    },
                    (error) => {
                    }
                );
        },
        connect(token, options) {
            let self = this;
            // disconnect the user from they joined already
            self.leaveRoomIfJoined();
            Twilio.connect(token, options)
                .then((room) => {
                        console.log('Successfully joined a Room: ');
                        // set active toom
                        self.activeRoom = room;
                        self.roomName = room.name;
                        self.loading = false;
                        self.loaded = true;
                        //update room status in store of chat to use for show/hide shareBtn
                        self.updateRoomStatus(true);
                        let localIdentity = room.localParticipant && room.localParticipant.identity ? room.localParticipant.identity : '';
                        self.members.push(localIdentity);
                        self.updateUserIdentity(localIdentity);
                        self.localOffline = false;
                        localStorage.setItem("identity", localIdentity);

                        //shared google document
                        if (self.activeRoom.participants && self.activeRoom.participants.size < 1) {
                            let shareLink = localStorage.getItem(`sb_share_link_${self.roomLinkID}`);
                            if(!shareLink){
                                self.getSharedocUrl()
                            }else{
                                self.updateSharedDocLink(`${shareLink}`);
                            }
                        }
                        // Attach the Tracks of all the remote Participants.
                        self.activeRoom.participants.forEach((participant, index) => {
                            let previewContainer = document.getElementById('remoteTrack');
                            self.members.push(participant);
                            self.attachParticipantTracks(participant, previewContainer);
                            self.remoteOffline = false;
                        });
                        // Attach the Participant's Media to a <div> element.
                        self.activeRoom.on('participantConnected', participant => {
                            console.log(`Participant "${participant.identity}" connected`);
                            self.members.push(participant);
                            participant.tracks.forEach(publication => {
                                if (publication.isSubscribed) {
                                    const track = publication.track;
                                    let previewContainer = document.getElementById('remoteTrack');
                                    console.log('remote track attached', " added track: " + track.kind);
                                    self.attachTracks([track], previewContainer);
                                }
                            });
                            self.remoteOffline = false;
                        });
                        // When a Participant adds a Track, attach it to the DOM.
                        self.activeRoom.on('trackSubscribed', (track, participant) => {
                            let previewContainer = document.getElementById('remoteTrack');
                            if (track.kind === 'data') {
                                passSharedDocLink(self.sharedDocUrl);
                                track.on('message', transferObj => {
                                    let Data = JSON.parse(transferObj);
                                    let parsedData = Data.data;
                                    if (Data.type === 'passData') {
                                        whiteBoardService.passData(parsedData.canvasContext, parsedData.dataContext);
                                    } else if (Data.type === 'undoData') {
                                        whiteBoardService.undo(parsedData);
                                    } else if (Data.type === 'tutoringChatMessage') {
                                        self.addMessage(Data);
                                    } else if (Data.type === 'sharedDocumentLink') {
                                        console.log('!!!got shared link event', Data.type, Data);
                                        if(!self.sharedDocUrl){
                                            self.updateSharedDocLink(parsedData)
                                        }
                                    }
                                });
                                self.attachTracks([track], previewContainer);
                            }else if(track.kind === 'video'){
                                let videoTag = previewContainer.querySelector("video");
                                if(videoTag){
                                    previewContainer.removeChild(videoTag);
                                }
                                self.attachTracks([track], previewContainer);
                            }else if(track.kind === 'audio'){
                                self.attachTracks([track], previewContainer);
                            }
                            console.log('track attached', " added track: " + track.kind);
                        });
                        // When a Participant's Track is unsubscribed from, detach it from the DOM.
                        self.activeRoom.on('trackUnsubscribed', function (track) {
                            console.log(" removed track: " + track.kind);
                            self.detachTracks([track]);
                        });

                        // When a Participant leaves the Room, detach its Tracks.
                        self.activeRoom.on('participantDisconnected', (participant) => {
                            self.removeMember(participant);
                            console.log("Participant '" + participant.identity + "' left the room");
                            if (participant.identity === self.userIdentity) {
                                self.localOffline = true;
                            } else {
                                self.remoteOffline = true
                            }
                            self.detachParticipantTracks(participant);
                        });
                    },
                    (error) => {
                        console.log(error, 'error cant connect')
                    });
        },
        // Create a new chat
        createChat() {
            this.loading = true;
            const self = this;
            // remove any remote track when joining a new room
            let clearEl = document.getElementById('remoteTrack');
            if (clearEl) {
                clearEl.innerHTML = "";
            }
            self.isHardawareAvaliable();
        },
    },
    created() {
        if (this.id) {
            this.updateRoomID(this.id);
            this.startChat();
        }
    }

}

