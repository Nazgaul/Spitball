import { mapActions, mapGetters, mapState } from 'vuex';
import { createLocalTracks, createLocalVideoTrack, createLocalAudioTrack } from 'twilio-video';
import timerIcon from '../images/timer.svg';
import stopIcon from '../images/stop-icon.svg';
import fullScreenIcon from '../images/fullscreen.svg';
//import walletService from '../../../services/walletService';
import insightService from '../../../services/insightService';
import microphoneImage from '../images/outline-mic-none-24-px-copy-2.svg'
import microphoneImageIgnore from '../images/mic-ignore.svg'
import videoCameraImage from '../images/video-camera.svg'
import videoCameraImageIgnore from '../images/camera-ignore.svg'
import videoCameraImageIgnore2 from '../images/camera-ignore copy.svg'
export default {
    name: "videoStream",
    components: { videoCameraImageIgnore2,timerIcon, stopIcon, fullScreenIcon,microphoneImage,videoCameraImage,microphoneImageIgnore,videoCameraImageIgnore },
    data() {
        return {
            isActive: false,
            videoEl: null,
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
            }
        };
    },
    props: {
        id: ''
    },
    computed: {
        ...mapState(['tutoringMain']),
        ...mapGetters([
            'localOffline',
            'remoteOffline',
            'roomLoading',
            'getStudyRoomData',
            'accountUser',
            'getLocalVideoTrack',
            'getLocalAudioTrack'
        ]),
        localVideoTrack(){
            return this.getLocalVideoTrack
        },
        localAudioTrack(){
            return this.getLocalAudioTrack
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
    methods: {
        ...mapActions([
            'updateReviewDialog',
            'updateToasterParams',
            'setSesionClickedOnce',
            'toggleVideoTrack',
            'toggleAudioTrack',
            'setLocalVideoTrack',
        ]),
        toggleAudio(){
            this.toggleAudioTrack()
        },
        toggleVideo(){
            if(this.localVideoTrack){
                this.isActive = false;
            } else {
                this.isActive = true
            }
            this.toggleVideoTrack()
        },
        showLocalVideo(){
            let self = this;
            insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_VideoStream_showLocalVideo', null, null);
            
            createLocalVideoTrack({width: 100, height: 75}).then(track => {
                self.isActive = true;
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_VideoStream_localVideoCreated', track, null);
                if(!!track){
                    self.videoEl = document.getElementById('localTrack');
                    self.videoEl.appendChild(track.attach());
                }
            }, (err)=>{
                insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoStream_localVideoFailed', err, null);
                console.error(err);
            });
        }
    },
    mounted() {
        this.$nextTick(function () {
            this.showLocalVideo()
        });
    },
};

