import { mapActions, mapGetters, mapState } from 'vuex';
import { createLocalTracks, createLocalVideoTrack } from 'twilio-video';
import timerIcon from '../images/timer.svg';
import stopIcon from '../images/stop-icon.svg';
import fullScreenIcon from '../images/fullscreen.svg';
import walletService from '../../../services/walletService';

export default {
    name: "videoStream",
    components: { timerIcon, stopIcon, fullScreenIcon },
    data() {
        return {
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
        ...mapState(['tutoringMainStore']),
        ...mapGetters([
            'localOffline',
            'remoteOffline',
            'roomLoading',
            'getStudyRoomData',
            'accountUser'
        ]),

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
            'setSesionClickedOnce'
        ]),
        minimize(type) {
            this.visible[`${type}`] = !this.visible[`${type}`];
        },
        showLocalVideo(){
            let self = this
            createLocalVideoTrack({width: 100, height: 75}).then(track => {
                if(!!track){
                    self.videoEl = document.getElementById('localTrack');
                    self.videoEl.appendChild(track.attach());
                }
            });
        }
    },
    mounted() {
        this.showLocalVideo()
    },
};

