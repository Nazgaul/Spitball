import { mapGetters } from 'vuex';
import remoteVideoTrack from './components/remoteVideoTrack.vue';







import timerIcon from '../images/timer.svg';
import stopIcon from '../images/stop-icon.svg';
import fullScreenIcon from '../images/fullscreen.svg';
import insightService from '../../../services/insightService';
import microphoneImage from '../images/outline-mic-none-24-px-copy-2.svg'
import microphoneImageIgnore from '../images/mic-ignore.svg'
import videoCameraImage from '../images/video-camera.svg'
import videoCameraImageIgnore from '../images/camera-ignore.svg'
import videoCameraImageIgnore2 from '../images/camera-ignore-big.svg'
import castIcon from "../images/cast.svg";
export default {
    name: "videoStream",
    components: { 
        remoteVideoTrack,
        videoCameraImageIgnore2,
        timerIcon,
        stopIcon, 
        fullScreenIcon,
        microphoneImage,
        videoCameraImage,
        microphoneImageIgnore,
        videoCameraImageIgnore,
        castIcon },
    data() {
        return {
            videoEl: null,
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
        ...mapGetters([
            'getIsShareScreen',


            'getLocalVideoTrack',
            'getLocalAudioTrack',
            'getIsVideoActive',
            'getIsAudioActive',
            'activeRoom',
            'getCurrentVideoTrack'
        ]),
        extandLocalVideoScreen(){
            return !this.getCurrentVideoTrack;
          },
        localVideoTrack(){
            return this.getLocalVideoTrack;
        },
        localAudioTrack(){
            return this.getLocalAudioTrack;
        },
        isVideoActive(){
            return this.getIsVideoActive;
        },
        isAudioActive(){
            return this.getIsAudioActive;
        }
    },
    watch:{
        localVideoTrack(videoTrack){
            if(videoTrack){
                insightService.track.event(insightService.EVENT_TYPES.LOG, 'StudyRoom_VideoStream_localVideoCreated', videoTrack, null);
                this.videoEl = document.getElementById('localTrack');
                this.videoEl.innerHTML = "";
                if(videoTrack.attach){
                    this.videoEl.appendChild(videoTrack.attach());
                }                
            }else{
                insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoStream_localVideoFailed', '', null);
            }
        }
    },
    methods: {
        toggleAudio(){
            this.$ga.event("tutoringRoom", "toggleAudio");
            this.$store.dispatch('updateAudioToggle')
        },
        toggleVideo(){
            this.$ga.event("tutoringRoom", "toggleVideo");
            this.$store.dispatch('updateVideoToggle')
        }
    }
};