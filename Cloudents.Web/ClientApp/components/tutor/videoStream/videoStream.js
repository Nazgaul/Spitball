import { mapActions, mapGetters, mapState } from 'vuex';
import { createLocalTracks, createLocalVideoTrack } from 'twilio-video';
import tutorService from '../tutorService';
import timerIcon from '../images/timer.svg';
import stopIcon from '../images/stop-icon.svg';
import fullScreenIcon from '../images/fullscreen.svg';

export default {
    name: "videoStream",
    components: {timerIcon, stopIcon, fullScreenIcon},
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
            availableDevices: [],
            visible: {
                'local_player': true,
                'remote_player': true
            },
            btnLoading : false
        };
    },
    props: {
        id: ''
    },
    computed: {
        ...mapState(['tutoringMainStore']),
        ...mapGetters([
                          'sharedDocUrl',
                          'activeRoom',
                          'localOffline',
                          'remoteOffline',
                          'roomLoading',
                          'getCurrentRoomState',
                          'getStudyRoomData',
                          'getJwtToken',
                          'accountUser'
                      ]),
        roomIsPending() {
            return this.getCurrentRoomState === this.tutoringMainStore.roomStateEnum.pending;
        },
        roomIsActive() {
            return this.getCurrentRoomState === this.tutoringMainStore.roomStateEnum.active;
        },
        waitingStudent() {
            return this.getCurrentRoomState === this.tutoringMainStore.roomStateEnum.loading;
        },
        isTutor() {
            return this.getStudyRoomData ? this.getStudyRoomData.isTutor : false;
        },
        accountUserID(){
            if(this.accountUser && this.accountUser.id){
                return this.accountUser.id
            }
        }
    },
    watch: {
        '$route': 'createVideoSession'
    },
    methods: {
        ...mapActions([
                          'updateRoomLoading',
                          'updateCurrentRoomState'
                      ]),

        biggerRemoteVideo() {
            let video = document.querySelectorAll("#remoteTrack video")[0];
            video.requestFullscreen();
        },
        minimize(type) {
            this.visible[`${type}`] = !this.visible[`${type}`];
        },
        enterRoom() {
            if(this.isTutor) {
                this.btnLoading = true;
                tutorService.enterRoom(this.id).then(() => {
                    this.createVideoSession();
                });
            } else {
                //join
                this.createVideoSession();
            }
        },
        endSession() {
            tutorService.endTutoringSession(this.id)
                        .then((resp) => {
                            console.log('ended session', resp);
                        }, (error) => {
                            console.log('error', error);
                        });
        },
        async isHardawareAvaliable() {
            let self = this;
            if(!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
                console.log("enumerateDevices() not supported.");
                return;
            }
            const token = this.getJwtToken; //get jwt from store
            // List cameras and microphones.
            navigator.mediaDevices.enumerateDevices()
                     .then(function (devices) {
                         devices.forEach(function (device) {
                             console.log(device.kind + ": " + device.label +
                                 " id = " + device.deviceId);
                             self.availableDevices.push(device.kind);
                         });
                         let connectOptions;
                         //create local track with custom names
                         let audioTrackName = `audio_${self.isTutor ? 'tutor' : 'student'}_${self.accountUserID}`;
                         let videoTrackName = `video_${self.isTutor ? 'tutor' : 'student'}_${self.accountUserID}`;
                         createLocalTracks({
                                               audio: {audio: self.availableDevices.includes('audioinput'), name: `${audioTrackName}`},
                                               video: {video: self.availableDevices.includes('videoinput'), name: `${videoTrackName}`}
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
                             self.isTutor ?  self.updateCurrentRoomState(self.tutoringMainStore.roomStateEnum.loading) :  self.updateCurrentRoomState(self.tutoringMainStore.roomStateEnum.active);

                         }, (error) => {
                             console.log(error, 'error create tracks before connect');
                         });

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
            if(clearEl) {
                clearEl.innerHTML = "";
            }
            self.isHardawareAvaliable();
        },
    },
    created() {

    }

};

