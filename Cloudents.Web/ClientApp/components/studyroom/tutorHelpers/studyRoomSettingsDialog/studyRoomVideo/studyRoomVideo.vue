<template>
    <div class="srVideoSettingsVideoContainerWrap mb-1 mb-sm-5 mb-md-0">
        <v-row class="srVideoSettingsVideoContainer ma-md-0 ma-auto elevation-2">
            <div class="cameraTextWrap text-center">
                <div class="noCamera white--text" v-if="!cameraOn && (permissionDialogState || !videoBlockPermission)" v-t="'studyRoomSettings_no_camera'"></div>
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
                        :loading="cameraInitLoader"
                        class="mx-2"
                        :class="{'noBorder': !cameraOn}"
                        :color="cameraOn ? 'transparent' : '#cb4243 !important'" @click="toggleCamera"
                        depressed
                        fab 
                    >
                        <videoCameraImage class="videoIcon" width="22" v-if="cameraOn" />
                        <videoCameraImageIgnore width="18" v-else />
                    </v-btn>
                </div> -->
                <v-icon class="settingIcon" color="#fff" @click="settingDialogState = true" size="22">sbf-settings</v-icon>
            </div>

            <div id="localTrackEnterRoom" :class="{'videoPlaceholderWrap': !cameraOn || !placeholder}">
                <video ref="videoEl" autoplay playsinline></video>
            </div>
            <div class="videoOverlay" :class="{'videoPlaceholderWrap': !cameraOn || !placeholder}"></div>
        </v-row>

        <studyRoomAudioVideoDialog :streamsArray="streamsArray" v-if="settingDialogState" @closeAudioVideoSettingDialog="val => settingDialogState = val"/>

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
import settingMixin from '../settingMixin.js'

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
            streamsArray:[],
            microphoneOn: true,
            cameraOn: false,
            placeholder: false,
            videoBlockPermission: false,
            audioBlockPermission: false,
            audioDeviceNotFound: false,
            permissionDialogState: false,
            settingDialogState: false,
        }
    },
    mixins: [settingMixin],
    watch: {
        settingDialogState(){
            this.createVideoPreview(this.getVideoDeviceId) 
        },
        getVideoDeviceId(newVal,oldVal){
            if(newVal && newVal !== oldVal){
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
            this.MIXIN_getMediaTrack(videoParams)
                .then(stream=>{
                    self.MIXIN_addStream(self.streamsArray,stream)
                    self.$refs.videoEl.srcObject = stream;
                    self.cameraOn = true
                    self.placeholder = true
                    self.videoBlockPermission = false
                    self.permissionDialogState = false
                    deviceId = stream.getVideoTracks()[0].getSettings().deviceId
                    self.$store.dispatch('updateVideoDeviceId',deviceId)
                    self.$store.commit('settings_setIsVideo',true)
                    return
                })
                .catch(err=>{
                    if(err.code === 0) {
                        self.videoBlockPermission = true 
                        self.permissionDialogState = true
                    }
                    self.$store.dispatch('updateVideoDeviceId',null)
                    self.cameraOn = false
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoSettings_getVideoInputdevices', err, null);
                })
                .finally(()=>{
                    self.cameraInitLoader = false;
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
            this.MIXIN_getMediaTrack(audioParams)
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
        clearVideoTrack(){
            this.cameraOn = false
            this.placeholder = false
            this.$store.commit('settings_setIsVideo',false)
            this.MIXIN_cleanStreams(this.streamsArray)
        },
        toggleMic() {
            if(this.audioBlockPermission) return

            this.microphoneOn = !this.microphoneOn   
            if(!this.microphoneOn) {
                studyRoomAudioSettingService.stopAudioContext()
                this.$store.dispatch('updateAudioDeviceId',null)
                return
            }else{
                this.checkAudio() 
            }

        },
        toggleCamera() {
            if(this.videoBlockPermission) return

            this.cameraOn = !this.cameraOn

            if(!this.cameraOn) {
                this.clearVideoTrack()
                this.$store.dispatch('updateVideoDeviceId',null)
                return
            }else{
                this.cameraInitLoader = true;
                this.createVideoPreview(this.getVideoDeviceId)
            }
        },
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
        this.MIXIN_cleanStreams(this.streamsArray)
        this.clearVideoTrack();
        studyRoomAudioSettingService.stopAudioContext();
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
        #localTrackEnterRoom {
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
