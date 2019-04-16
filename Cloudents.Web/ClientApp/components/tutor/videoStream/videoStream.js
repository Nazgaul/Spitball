import { mapActions, mapGetters, mapState } from 'vuex';
import { createLocalTracks, createLocalVideoTrack } from 'twilio-video';
import tutorService  from '../tutorService';
 import timerIcon from '../images/timer.svg';
 import fullScreenIcon from '../images/fullscreen.svg';

export default {
    name: "videoStream",
    components: {timerIcon, fullScreenIcon},
    data() {
        return {
            loading: false,
            loaded: false,
            data: {},
            isCopied: false,
            localTrackAval: false,
            remoteTrack: '',
            screenShareTrack: null,
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
        ...mapState(['tutoringMainStore']),
        ...mapGetters(['sharedDocUrl', 'roomLinkID', 'isRoomFull', 'activeRoom', 'localOffline','remoteOffline', 'roomLoading', 'getCurrentRoomState']),
        roomIsPending(){
            return this.getCurrentRoomState === this.tutoringMainStore.roomStateEnum.pending;
        },
        roomIsActive(){
            return this.getCurrentRoomState === this.tutoringMainStore.roomStateEnum.active;
        }
    },
    watch: {
        '$route': 'createVideoSession'
    },
    methods: {
        ...mapActions([
            'updateRoomID',
            'updateRoomLoading'
        ]),
        
        biggerRemoteVideo() {
            let video = document.querySelectorAll("#remoteTrack video")[0];
            video.requestFullscreen()
        },
        minimize(type) {
            this.visible[`${type}`] = !this.visible[`${type}`];
        },
        createRoomFunc() {
            this.createVideoSession()
            // let self = this;
            // tutorService.createRoom().then(resp => {
            //     self.roomLink = resp.data.name;
            //     self.$router.push({name: 'tutoring', params: {id: self.roomLink}});
            //     self.updateRoomID(self.roomLink)
            // })

        },
        startVideo() {
            this.createVideoSession()
        },
        // Generate access token
        async getAccessToken() {
            let identity = localStorage.getItem("identity");
            return await tutorService.getJwtToken(this.id, identity);
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
                        video: self.availableDevices.includes('videoinput'),
                    }).then((tracksCreated) => {
                        let localMediaContainer = document.getElementById('localTrack');
                        tracksCreated.forEach((track) => {
                            localMediaContainer.appendChild(track.attach());
                            self.localTrackAval = true;
                        });
                            tracksCreated.push(tutorService.dataTrack);
                            connectOptions = {
                                tracks: tracksCreated,
                                networkQuality: true
                            };
                        tutorService.connectToRoom(token, connectOptions);

                    }, (error) => {
                        console.log(error, 'error create tracks before connect')
                    })

                })
                .catch(function (err) {
                    console.log(err.name + ": " + err.message);
                });
        },

        // Create a new chat
        createVideoSession() {
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
            // this.updateRoomID(this.id);
            // this.startVideo();
        }
    }

}

