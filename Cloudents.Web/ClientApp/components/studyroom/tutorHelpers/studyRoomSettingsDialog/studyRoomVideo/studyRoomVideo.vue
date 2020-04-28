<template>
    <div class="srVideoSettingsVideoContainerWrap mb-1 mb-sm-5 mb-md-0">
        <v-row class="srVideoSettingsVideoContainer ma-md-0 ma-auto elevation-2">
            <div class="cameraTextWrap text-center">
                <div class="noCamera white--text" v-if="!cameraOn && permissionDialogState" v-t="'studyRoomSettings_no_camera'"></div>
                <i18n class="blockPermission inCamera white--text" v-if="videoBlockPermission && !permissionDialogState" path="studyRoomSettings_block_permission" tag="div">
                    <cameraBlock class="cameraBlock" width="20" />
                </i18n>
            </div>

            <div class="bottomIcons d-flex align-end justify-space-between">
                <div class="micIconWrap d-flex align-center" v-if="microphoneOn">
                    <microphoneImage :class="{'audioIconVisible': !microphoneOn}" width="14" /> 
                    <div id="audio-input-meter" class="ml-2"></div>
                </div>
                <!-- <div class="centerIcons d-flex align-center">
                    <v-btn
                        class="mx-2"
                        :class="{'noBorder': !microphoneOn}"
                        :color="microphoneOn ? 'transparent' : '#cb4243 !important'" @click="toggleMic"
                        depressed
                        fab
                    >
                        <microphoneImage width="14" v-if="microphoneOn" />
                        <microphoneImageIgnore width="18" v-else />
                    </v-btn>
                    <v-btn 
                        class="mx-2"
                        :class="{'noBorder': !cameraOn || !singleCameraId}"
                        :color="cameraOn ? 'transparent' : '#cb4243 !important'" @click="toggleCamera"
                        depressed
                        fab 
                    >
                        <videoCameraImage class="videoIcon" width="22" v-if="cameraOn && singleCameraId" />
                        <videoCameraImageIgnore width="18" v-else />
                    </v-btn>
                </div> -->
                <v-icon class="settingIcon" color="#fff" @click="settingDialogState = true" size="22">sbf-settings</v-icon>
            </div>

            <div id="local-video-test-track" :class="{'videoPlaceholderWrap': !cameraOn || !placeholder}">
                <video ref="videoEl" autoplay playsinline controls="false"></video>
            </div>
            <div class="videoOverlay" :class="{'videoPlaceholderWrap': !cameraOn || !placeholder}"></div>
        </v-row>

        <studyRoomAudioVideoDialog
            v-if="settingDialogState"
            @closeAudioVideoSettingDialog="val => settingDialogState = val"
        />

        <v-dialog v-model="permissionDialogState" width="512" :fullscreen="$vuetify.breakpoint.xsOnly" persistent content-class="premissionDeniedDialog text-center pa-6 pb-4">
            <div class="mb-4 mainTitle" v-t="'studyRoomSettings_block_title'"></div>
            <i18n path="studyRoomSettings_block_permission" tag="div" class="blockPermission mb-6">
                <cameraBlock class="cameraBlock mx-1 mt-1" width="20" />
            </i18n>
            <div class="text-center">
                <v-btn
                    @click="permissionDialogState = false"
                    class="white--text"
                    color="#5360FC"
                    width="140"
                    rounded
                    depressed
                >
                    {{$t('studyRoomSettings_dismiss')}}
                </v-btn>
            </div>
        </v-dialog>
    </div>
</template>

<script>
import insightService from '../../../../../services/insightService';
import studyRoomAudioSettingService from '../studyRoomAudioVideoDialog/studyRoomAudioSetting/studyRoomAudioSettingService';

import studyRoomAudioVideoDialog from '../studyRoomAudioVideoDialog/studyRoomAudioVideoDialog.vue'

import microphoneImage from '../../../images/outline-mic-none-24-px-copy-2.svg'
// import microphoneImageIgnore from '../../../images/mic-ignore.svg';
// import videoCameraImage from '../../../images/video-camera.svg';
// import videoCameraImageIgnore from '../../../images/camera-ignore.svg';
import cameraBlock from '../images/cameraBlock.svg'
import { mapGetters } from 'vuex';

export default {
    components: {
        studyRoomAudioVideoDialog,
        // videoCameraImage,
        // videoCameraImageIgnore,
        microphoneImage,
        // microphoneImageIgnore,
        cameraBlock
    },
    data(){
        return {
            // camerasList: [],
            // videoEl: null,
            // localTrack: null,
            // audio: null,
            microphoneOn: true,
            cameraOn: false,
            placeholder: false,
            videoBlockPermission: false,
            audioBlockPermission: false,
            audioDeviceNotFound: false,
            permissionDialogState: false,
            settingDialogState: false,
            // singleCameraId: global.localStorage.getItem('sb-videoTrackId'),
            // singleMicrophoneId: global.localStorage.getItem('sb-audioTrackId')
        }
    },
    watch: {
        getVideoDeviceId(newVal,oldVal){
            if((newVal && oldVal) && newVal !== oldVal){
                this.createVideoPreview(newVal)
            }
        }
    },
    computed: {
        ...mapGetters(['getVideoDeviceId','getAudioDeviceId'])
    },
    methods:{
        createVideoPreview(deviceId){
            let videoParams = {
                audio: false,
                video:{deviceId}
            }
            let self = this;
            navigator.mediaDevices.getUserMedia(videoParams)
                .then(stream=>{
                    self.$refs.videoEl.srcObject = stream;
                    self.cameraOn = true
                    self.placeholder = true
                    self.videoBlockPermission = false
                    self.permissionDialogState = false
                    deviceId = stream.getVideoTracks()[0].getSettings().deviceId
                    self.$store.dispatch('updateVideoDeviceId',deviceId)
                    return
                })
                .catch(err=>{
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoSettings_getVideoInputdevices', err, null);
                })
                self.cameraOn = false
        },
        getAudioDevices(){
            let self = this;
            navigator.mediaDevices.enumerateDevices()
                .then(devices => {
                    let audioDevice = devices.find(device => device.kind === 'audioinput' && device.label.toLowerCase().includes('default'));
                    self.checkAudio(audioDevice.deviceId);
                });
        },
        checkAudio(deviceId){
            let audioParams = {
                audio: {deviceId},
                video: false
            }
            let self = this;
            navigator.mediaDevices.getUserMedia(audioParams)
                .then((stream)=>{
                    deviceId = stream.getAudioTracks()[0].getSettings().deviceId 
                    self.microphoneOn = true
                    self.validateMicrophone(deviceId);
                })
                .catch(err=>{
                    if(err.code === 0) {
                        self.audioBlockPermission = true 
                    }
                    if(err.code === 8) {
                        self.audioDeviceNotFound = true
                    }
                    self.microphoneOn = false;
                })
        },
        validateMicrophone(deviceId) {
            studyRoomAudioSettingService.createAudioContext('audio-input-meter', deviceId);
            this.$store.dispatch('updateAudioDeviceId',deviceId)
        },
        // getInputDevices() {
        //     let self = this;
        //     navigator.mediaDevices.enumerateDevices().then((mediaDevices) => {
        //         mediaDevices.forEach((device) => {
        //             if (device.kind === 'videoinput') {
        //                 self.camerasList.push(device)
        //             }
        //             if (device.kind === 'audioinput') {
        //                 if(!self.singleMicrophoneId){
        //                     if (device && device.label.toLowerCase().includes('default')) {
        //                         self.singleMicrophoneId = device.deviceId;
        //                     }
        //                 }
        //             }
        //         })

        //         self.checkAudioDevice()

        //         if(self.camerasList.length > 0){
        //             let isNotInList = self.camerasList.every(device=>device.deviceId !== self.singleCameraId)
            
        //             if(isNotInList || !self.singleCameraId){
        //                 self.singleCameraId = self.camerasList[0].deviceId;
        //             }
        //             self.createVideoQualityPreview();
        //             self.cameraOn = true
        //             return
        //         }
        //         self.cameraOn = false
        //     })
        // },
        // createVideoQualityPreview() {
        //     if (this.localTrack) {
        //         this.clearVideoTrack();
        //     }
        //     let self = this;
        //     let isNotInList = self.camerasList.every(device=>device.deviceId !== self.singleCameraId)
            
        //     if(self.camerasList.length){
        //         if(isNotInList || !self.singleCameraId){
        //             self.singleCameraId = self.camerasList[0].deviceId;
        //         }
        //     }

        //     let videoParams = {audio:false,video :{ deviceId: this.singleCameraId}}
        //     navigator.mediaDevices.getUserMedia(videoParams)
        //         .then(track => {
        //             self.setVideoTrack(track)
        //         }).catch(err => {
        //             let rateReviewDialogState = this.$store.getters.getReviewDialogState
        //             if(err.code === 0 && !rateReviewDialogState) {
        //                 self.videoBlockPermission = true
        //                 self.permissionDialogState = true
        //                 self.placeholder = false
        //             }
        //             self.cameraOn = false
        //             insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoValidation_createVideoQualityPreview', err, null);
        //         })
        // },
        // setVideoTrack(track) {
        //     if(document.querySelector('#local-video-test-track video')){
        //         return
        //     }
        //     this.videoEl = document.getElementById('local-video-test-track');
        //     this.localTrack = track;
        //     let video = document.createElement('video');
        //     video.srcObject = track;
        //     video.onloadedmetadata = function() {
        //         video.play();
        //     };
        //     this.videoEl.appendChild(video);
        //     this.$store.dispatch('updateVideoTrack',this.singleCameraId)
        //     this.$store.commit('settings_setIsVideo',true)

        //     this.cameraOn = true
        //     this.placeholder = true
        //     this.videoBlockPermission = false
        //     this.permissionDialogState = false
        // },
        // clearVideoTrack() {
        //     this.$store.commit('settings_setIsVideo',false)
        //     if (this.localTrack) {
        //         this.localTrack.getTracks().forEach(track => {
        //             track.stop();
        //         });
        //         let video = document.querySelector('#local-video-test-track video')
        //         if(video){
        //             this.videoEl.removeChild(video)
        //         }
        //         this.localTrack = null;
        //         this.cameraOn = false
        //         this.placeholder = false
        //     }
        // },
        // checkAudioDevice() {
        //     let self = this
        //     navigator.getUserMedia({ audio: true }, () => {
        //         self.microphoneOn = true
        //         self.validateMicrophone('audio-input-meter', self.singleMicrophoneId);
        //     }, (err) => {
        //         if(err.code === 0) {
        //             self.audioBlockPermission = true 
        //         }
        //         if(err.code === 8) {
        //             self.audioDeviceNotFound = true
        //         }
        //         self.microphoneOn = false;
        //     })
        // },
        // toggleMic() {
        //     if(this.audioBlockPermission) return

        //     this.microphoneOn = !this.microphoneOn   
        //     if(!this.microphoneOn) {
        //         this.stopSound()
        //         return
        //     }

        //     this.checkAudioDevice()
        // },
        // toggleCamera() {
        //     if(this.videoBlockPermission) return

        //     this.cameraOn = !this.cameraOn

        //     if(!this.cameraOn) {
        //         this.clearVideoTrack()
        //         this.camerasList = []
        //         return
        //     }
        //     this.getInputDevices()
        // },
        // validateMicrophone() {
        //     studyRoomAudioSettingService.createAudioContext('audio-input-meter', this.singleMicrophoneId);
        //     this.$store.dispatch('updateAudioTrack',this.singleMicrophoneId)
        // },
        // stopSound(){
        //     if(this.audio && this.audio.pause){
        //         this.audio.pause();
        //     }
        // },
        startPreview(){
            this.createVideoPreview(this.getVideoDeviceId)
            this.checkAudio(this.getAudioDeviceId) 
        }
    },
    created(){
        this.startPreview()
        navigator.mediaDevices.ondevicechange = this.startPreview
    },
    beforeDestroy() {
        // this.clearVideoTrack();
        studyRoomAudioSettingService.stopAudioContext();
        // this.stopSound();
    }
}
</script>

<style lang="less">
@import '../../../../../styles/mixin';
@import '../../../../../styles/colors';

.srVideoSettingsVideoContainerWrap {
    max-width: 680px;
    width: 100%;
    .srVideoSettingsVideoContainer {
        position: relative;
        border-radius: 8px;
        background-color: #212123;
        box-shadow: none !important;
        @media (max-width: @screen-xs) {
            border-radius: 0;
        }
        width: 100%;
        height: 100%;
        overflow: hidden;
        max-height: 380px;
        max-width: 680px;
        .cameraTextWrap {
            font-weight: 600;
            padding: 0 10px;
            position: absolute;
            top: calc(50% - 40px); // center text
            right: 0;
            left: 0;
            z-index: 2;
            flex-direction: column;
            display: flex;
            justify-content: center;
            align-items: center;
            .noCamera, .noPermission {
                font-size: 16px;
                font-weight: 600;
            }
            .blockPermission {
                color: rgba(0,0,0,0.541);

                &.inCamera {
                    padding: 0 100px;
                    line-height: 22px;
                }
                .cameraBlock {
                    vertical-align: middle;
                }
            }
        }
        .bottomIcons {
            position: absolute;
            bottom: 14px;
            right: 14px;
            left: 14px;
            z-index: 2;

            .micIconWrap {
                position: absolute;
                .audioIconVisible {
                    visibility: hidden;
                }
            }
            .videoIcon {
                fill: #fff;
            }
            .centerIcons {
                width: 100%;
                text-align: center;
                justify-content: center;
                button {
                     background-color: rgba(0, 0, 0, 0.15) !important;
                    &.noBorder {
                        border: none !important;
                    }
                }
            }
            .settingIcon {
                position: absolute;
                right: 0;
            }
        }
        #local-video-test-track {
            width: 100%;
            &.videoPlaceholderWrap {
            padding-top: 55.88%;
            }
            .videoPlaceHolder {
                    width: 100%;
                    border-radius: 8px;
                    height: 100%;
                    text-align: center;
                    display: block;
                    @media (max-width: @screen-xs) {
                        border-radius: 0;
                    }
                }
            video {
                width: 100%;
                border-radius: 8px;
                height: 100%;
                text-align: center;
                display: block;
                @media (max-width: @screen-xs) {
                    border-radius: 0;
                }
            }
        }
        .videoOverlay {
            background-image: -webkit-linear-gradient(bottom, rgba(0, 0, 0, 0.7) 0, rgba(0, 0, 0, 0.3) 50%, rgba(0, 0, 0, 0) 100%);
            background-image: linear-gradient(bottom, rgba(0, 0, 0, 0.7) 0, rgba(0, 0, 0, 0.3) 50%, rgba(0, 0, 0, 0) 100%);
            width: 100%;
            border-radius: 6px;
            position: absolute;
            bottom: 0;
            height: 100px;
            @media (max-width: @screen-xs) {
                border-radius: 0;
            }
        }
        .videoPlaceholderWrap {
            padding-top: 55.88%;
            width: 100%;
        }
    }
}
.premissionDeniedDialog {
    background: #fff !important;
    border-radius: 8px;
    .mainTitle {
        color: @global-purple;
        font-weight: 600;
        font-size: 22px;
    }
    .blockPermission {
        color: @global-purple;
        line-height: 22px;
        font-size: 16px;
        &.inCamera {
            font-size: 22px;
            line-height: 30px;
        }
        .cameraBlock {
            vertical-align: middle;
        }
    }
}

</style>
