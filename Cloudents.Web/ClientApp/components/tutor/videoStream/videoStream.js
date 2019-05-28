import { mapActions, mapGetters, mapState } from 'vuex';
import { createLocalTracks, createLocalVideoTrack } from 'twilio-video';
import tutorService from '../tutorService';
import timerIcon from '../images/timer.svg';
import stopIcon from '../images/stop-icon.svg';
import fullScreenIcon from '../images/fullscreen.svg';
import walletService from '../../../services/walletService';
import videoStreamService from "../../../services/videoStreamService";

export default {
    name: "videoStream",
    components: { timerIcon, stopIcon, fullScreenIcon },
    data() {
        return {
            loading: false,
            loaded: false,
            data: {},
            isCopied: false,
            sessionEndClickedOnce: false,
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
            'updateToasterParams',
            'setSesionClickedOnce'
        ]),

        biggerRemoteVideo() {
            //check browser support
            let video = document.querySelector("#remoteTrack video");
            if (!video) {
                return;
            }
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
        // move all this function inside to service
        enterRoom() {
            videoStreamService.enterRoom();
                this.sessionEndClickedOnce = false; //unlock end session btn
        },
        endSession() {
            if(this.sessionEndClickedOnce) return;
            let self= this;
            self.sessionEndClickedOnce = true; //lock end session btn if already clicked
            tutorService.endTutoringSession(self.id)
                .then((resp) => {
                    self.sessionStartClickedOnce = false; //unlock start session btn
                    this.setSesionClickedOnce(false)
                    if (!self.isTutor && self.getAllowReview) {
                        self.updateReviewDialog(true);
                    }
                }, (error) => {
                    console.log('error', error);
                });
        },
         addDevicesToTrack() {
           videoStreamService.addDevicesTotrack();
        },
        // Create a new chat
        createVideoSession() {
            const self = this;
            // remove any remote track when joining a new room
            let clearEl = document.getElementById('remoteTrack');
            if (clearEl) {
                clearEl.innerHTML = "";
            }
            self.addDevicesToTrack();
        },
    },
    created() {
        this.setRoomId(this.id);
    }
};

