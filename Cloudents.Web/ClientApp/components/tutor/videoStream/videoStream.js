import { mapActions, mapGetters } from 'vuex';
import { createLocalTracks, createLocalVideoTrack } from 'twilio-video';
import videoService from '../../../services/videoStreamService';
import { connectToRoom, createRoom, getToken, dataTrack } from '../tutorService';
import store from "../../../store";
 // import whiteBoardService from '../whiteboard/whiteBoardService';


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
            identity: '',
            roomLink: '',
            availableDevices: [],
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
        ...mapGetters(['sharedDocUrl', 'roomLinkID', 'isRoomFull', 'activeRoom', 'localOffline','remoteOffline', 'roomLoading'])
    },
    watch: {
        '$route': 'createChat'
    },
    methods: {
        ...mapActions([
            'updateRoomID',
            'updateRoomLoading'
        ]),
        publishTrackToRoom(track){
            this.activeRoom.localParticipant.publishTrack(track);
        },
        unPublishTrackfromRoom(track){
            this.activeRoom.localParticipant.unpublishTrack(track);
        },
        //screen share start
        showScreen() {
            let self = this;
            videoService.getUserScreen().then((stream) => {
                    self.screenShareTrack = stream.getVideoTracks()[0];
                    self.publishTrackToRoom(self.screenShareTrack);
                    self.isSharing = true;
                },
                (error) => {
                    console.log('error sharing screen')
                }
            );
        },
        stopSharing() {
            let self = this;
            self.unPublishTrackfromRoom(self.screenShareTrack);
            //create new video track
            createLocalVideoTrack().then((videoTrack)=> {
                    self.publishTrackToRoom(videoTrack);
                    // self.screenShareTrack = null;
                    self.isSharing = false;
                },
                (error)=>{
                    console.log('error creating video track')
                }
            );
        },
        biggerRemoteVideo() {
            let video = document.querySelectorAll("#remoteTrack video")[0];
            video.requestFullscreen()
        },
        minimize(type) {
            this.visible[`${type}`] = !this.visible[`${type}`];
        },
        createRoomFunc() {
            let self = this;
            createRoom().then(data => {
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
            return await getToken(this.id, identity);
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
                        connectToRoom(token, connectOptions);

                    }, (error) => {
                        console.log(error, 'error create tracks before connect')
                    })

                })
                .catch(function (err) {
                    console.log(err.name + ": " + err.message);
                });
        },

        // Create a new chat
        createChat() {
            this.updateRoomLoading(true);
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

