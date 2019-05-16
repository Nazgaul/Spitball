import { mapActions, mapGetters, mapState } from 'vuex';
import { createLocalTracks, createLocalVideoTrack } from 'twilio-video';
import tutorService from '../tutorService';
import timerIcon from '../images/timer.svg';
import stopIcon from '../images/stop-icon.svg';
import fullScreenIcon from '../images/fullscreen.svg';
import walletService from '../../../services/walletService';

export default {
    name: "videoStream",
    components: { timerIcon, stopIcon, fullScreenIcon },
    data() {
        return {
            loading: false,
            loaded: false,
            data: {},
            isCopied: false,
            sessionStartClickedOnce: false,
            localTrackAval: false,
            remoteTrack: '',
            screenShareTrack: null,
            identity: '',
            availableDevices: [],
            visible: {
                'local_player': true,
                'remote_player': true
            },
        };
    },
    props: {
        id: ''
    },
    computed: {
        ...mapState(['tutoringMainStore']),
        ...mapGetters([
            'activeRoom',
            'localOffline',
            'remoteOffline',
            'roomLoading',
            'getCurrentRoomState',
            'getStudyRoomData',
            'getJwtToken',
            'accountUser',
            'getNotAllowedDevices',
            'getAllowReview',
            'getNotAvaliableDevices',
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
        needPayment() {
            return this.getStudyRoomData ? this.getStudyRoomData.needPayment : false;
        },
        accountUserID() {
            if (this.accountUser && this.accountUser.id) {
                return this.accountUser.id;
            }
        }
    },
    watch: {
        '$route': 'createVideoSession'
    },
    methods: {
        ...mapActions([
            'updateCurrentRoomState',
            'updateTestDialogState',
            'updateReviewDialog',
            'setRoomId',
            "updateToasterParams",
        ]),

        biggerRemoteVideo() {
            //check browser support
            let video = document.querySelector("#remoteTrack video");
            if (video.requestFullscreen) {
                video.requestFullscreen();
            } else if (video.webkitRequestFullscreen) {
                video.webkitRequestFullscreen();
            } else if (video.mozRequestFullScreen) {
                video.mozRequestFullScreen();
            } else if (video.msRequestFullscreen) {
                video.msRequestFullscreen();
            }
            console.log();
        },
        minimize(type) {
            this.visible[`${type}`] = !this.visible[`${type}`];
        },
        enterRoom() {
            if (!!this.accountUser && this.accountUser.needPayment && !this.isTutor) {
                walletService.getPaymeLink().then(({ data }) => {
                    global.open(data.link, '_blank', 'height=520,width=440');
                })
                return;
            }
            //if blocked or not available  use of media devices do not allow session start
            if (this.getNotAllowedDevices && this.getNotAvaliableDevices) {
                this.updateTestDialogState(true);
                return;
            }
            if (!this.sessionStartClickedOnce) {
                this.sessionStartClickedOnce = true;
                if (this.isTutor) {
                    this.updateCurrentRoomState('loading');
                    tutorService.enterRoom(this.id).then(() => {
                        setTimeout(() => {
                            this.createVideoSession();
                            this.sessionStartClickedOnce = false;
                        }, 1000);
                    });
                } else {
                    //join
                    this.createVideoSession();
                    this.sessionStartClickedOnce = false;
                }
            }
        },
        endSession() {
            tutorService.endTutoringSession(this.id)
                .then((resp) => {
                    console.log('ended session', resp);
                    this.sessionStartClickedOnce = false;
                    if (!this.isTutor && this.getAllowReview) {
                        this.updateReviewDialog(true);
                    }
                }, (error) => {
                    console.log('error', error);
                });
        },
        async isHardawareAvaliable() {
            let self = this;
            if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
                console.log("enumerateDevices() not supported.");
                return;
            }
            const token = this.getJwtToken; //get jwt from store
            // List cameras and microphones.
            let devices = await navigator.mediaDevices.enumerateDevices();
            //navigator.mediaDevices.enumerateDevices()
            //.then(function (devices) {
            devices.forEach(function (device) {
                console.log(device.kind + ": " + device.label +
                    " id = " + device.deviceId);
                self.availableDevices.push(device.kind);
            });
            let connectOptions;
            //create local track with custom names
            let audioTrackName = `audio_${self.isTutor ? 'tutor' : 'student'}_${self.accountUserID}`;
            let videoTrackName = `video_${self.isTutor ? 'tutor' : 'student'}_${self.accountUserID}`;
            let audioSetObj = {
                audio: self.availableDevices.includes('audioinput'),
                name: audioTrackName
            };
            let videoSetObj = {
                video: self.availableDevices.includes('videoinput'),
                name: videoTrackName
            };
            let audioDevice = await navigator.mediaDevices.getUserMedia({ audio: true }).then(y => audioSetObj, z => false);
            let videoDevice = await navigator.mediaDevices.getUserMedia({ video: true }).then(y => videoSetObj, z => false);
            createLocalTracks({
                audio: audioDevice,
                video:videoDevice
            }).then((tracksCreated) => {
                let localMediaContainer = document.getElementById('localTrack');
                tracksCreated.forEach((track) => {
                    localMediaContainer.innerHTML = "";
                    localMediaContainer.appendChild(track.attach());
                    self.localTrackAval = true;
                });
                tracksCreated.push(tutorService.dataTrack);
                connectOptions = {
                    tracks: tracksCreated,
                    networkQuality: true
                };
                tutorService.connectToRoom(token, connectOptions);
                if (!self.isTutor) {
                    self.updateCurrentRoomState(self.tutoringMainStore.roomStateEnum.active)
                }

            }, (error) => {
                self.updateToasterParams({
                    toasterText: "We having trouble connection you to the room",
                    showToaster: true,
                    toasterType: 'error-toaster' //c
                  });

            });

            //                     })
            //.catch(function (err) {
            //console.log(err.name + ": " + err.message);
            //});
        },

        // Create a new chat
        createVideoSession() {
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
        this.setRoomId(this.id);
    }
};

