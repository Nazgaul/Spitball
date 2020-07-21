<template>
    <div class="srVideoSettingsVideoContainerWrap mb-1 mb-sm-5 mb-md-0">
    <v-responsive :max-height="$vuetify.breakpoint.xsOnly?'24vh':''" :aspect-ratio="16/9">
        <v-row class="srVideoSettingsVideoContainer ma-md-0 ma-auto elevation-2">
            <div class="cameraTextWrap text-center">
                <div class="noCamera white--text" v-if="!cameraOn && (permissionDialogState || !videoBlockPermission)" v-t="'studyRoomSettings_no_camera'"></div>
                <i18n class="blockPermission inCamera white--text" v-if="videoBlockPermission && !permissionDialogState" path="studyRoomSettings_block_permission" tag="div">
                    <cameraBlock class="cameraBlock" width="20" />
                </i18n>
            </div>
            <template v-if="isLoggedIn">
                <div class="bottomIcons d-flex align-end justify-space-between">
                    <div class="micIconWrap d-flex align-center" v-if="microphoneOn">
                        <microphoneImage :class="{'audioIconVisible': !microphoneOn}" width="14" />
                        <v-progress-linear :style="`width:${audioLevel}px`" class="ms-5" absolute rounded color="#16eab1" height="6" :value="audioLevel" buffer-value="0"></v-progress-linear>
                    </div>
                    <v-icon class="settingIcon" color="#fff" @click="settingDialogState = true" size="22">sbf-settings</v-icon>
                </div>

                <div id="localTrackEnterRoom" :class="{'videoPlaceholderWrap': !cameraOn || !placeholder}">
                    <video ref="videoEl" autoplay playsinline></video>
                </div>
            </template>
            <div class="videoOverlay" :class="{'videoPlaceholderWrap': !cameraOn || !placeholder}"></div>
        </v-row>
    </v-responsive>
        <v-dialog v-model="settingDialogState" max-width="570px" content-class="studyRoomAudioVideoDialog"
                    :fullscreen="$vuetify.breakpoint.xsOnly" persistent
                    :transition="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition'">
            <studyRoomAudioVideoDialog :streamsArray="streamsArray" @closeAudioVideoSettingDialog="val => settingDialogState = val"/>
        </v-dialog>


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
        <mobilePermissionDialog @onClose="mobilePermissionDialogState = false" v-if="$vuetify.breakpoint.xsOnly && mobilePermissionDialogState"></mobilePermissionDialog>
    </div>
</template>

<script>
import insightService from '../../../../../services/insightService';

import studyRoomAudioVideoDialog from '../studyRoomAudioVideoDialog/studyRoomAudioVideoDialog.vue'

import microphoneImage from '../../../images/outline-mic-none-24-px-copy-2.svg'
import cameraBlock from '../images/cameraBlock.svg'
import { mapGetters } from 'vuex';
import settingMixin from '../settingMixin.js'
import mobilePermissionDialog from './mobilePermissionDialog.vue';
import pollAudioLevel from '../../../layouts/userPreview/pollaudiolevel.js';

export default {
    components: {
        studyRoomAudioVideoDialog,
        microphoneImage,
        cameraBlock,
        mobilePermissionDialog
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

            mobilePermissionDialogState:false,
            audioLevel:0
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
        ...mapGetters(['getVideoDeviceId','getAudioDeviceId']),
        isLoggedIn(){
            return this.$store.getters.getUserLoggedInStatus
        }
    },
    methods:{
        createTracks(){
            let videoDevices = [];
            let audioDevices = []
            let self = this;
            navigator.mediaDevices.enumerateDevices().then(devices=>{
                devices.forEach(device=>{
                    if(device.kind == 'audioinput'){
                        audioDevices.push(device)
                    }else{
                        videoDevices.push(device)
                    }
                })
                let params = {
                    audio: audioDevices.length?{deviceId:self.getAudioDeviceId}:false,
                    video: videoDevices.length?{deviceId:self.getVideoDeviceId}:false,
                }

                self.MIXIN_getMediaTrack(params)
                    .then(stream=>{
                        if(params.audio){
                            let audioTrack = new MediaStream()
                            audioTrack.addTrack(stream.getAudioTracks()[0])
                            self.connectAudioTrack(audioTrack)
                        }
                        if(params.video){
                            let videoTrack = new MediaStream()
                            videoTrack.addTrack(stream.getVideoTracks()[0])
                            self.connectVideoTrack(videoTrack)
                        }
                        return
                    })
                    .catch(err=>{
                        if(err.code === 0) {
                            self.mobilePermissionDialogState = true;
                        }
                        self.$store.dispatch('updateVideoDeviceId',null)
                        self.cameraOn = false
                        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoSettings_getVideoInputdevices', err, null);
                    })
            })
        },
        createVideoPreview(deviceId){
            let videoParams = {
                audio: false,
                video:{deviceId}
            }
            let self = this;
            this.MIXIN_getMediaTrack(videoParams)
                .then(stream=>{
                    self.connectVideoTrack(stream)
                    return
                })
                .catch(err=>{
                    if(err.code === 0) {
                        self.videoBlockPermission = true 
                        self.permissionDialogState = true
                    }
                    self.$store.dispatch('updateVideoDeviceId',null)
                    self.cameraOn = false
                    self.microphoneOn = false;
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoSettings_getVideoInputdevices', err, null);
                })
            self.cameraOn = false
        },
        checkAudio(deviceId){
            let audioParams = {
                audio: {deviceId},
                video: false
            }
            let self = this;
            this.MIXIN_getMediaTrack(audioParams)
                .then((stream)=>{
                    self.connectAudioTrack(stream)
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
        onAudioLevelChanged(level){
            this.audioLevel = level * 7;
        },
        validateMicrophone(audioTrack){
            let mediaStreamTrack = audioTrack.getAudioTracks()[0];
            let deviceId = mediaStreamTrack.getSettings().deviceId;
            pollAudioLevel({mediaStreamTrack},this.onAudioLevelChanged)
            this.$store.dispatch('updateAudioDeviceId',deviceId)
        },
        clearVideoTrack(){
            this.cameraOn = false
            this.placeholder = false
            this.$store.commit('settings_setIsVideo',false)
            this.MIXIN_cleanStreams(this.streamsArray)
        },
        startPreview(){
            let isMobile = this.$vuetify.breakpoint.xsOnly;
            if(isMobile){
                this.createTracks()
            }else{
                this.createVideoPreview(this.getVideoDeviceId)
                this.checkAudio(this.getAudioDeviceId) 
            }
        },
        connectVideoTrack(videoTrack){
            let deviceId;
            this.MIXIN_addStream(this.streamsArray,videoTrack)
            this.$refs.videoEl.srcObject = videoTrack;
            this.cameraOn = true
            this.placeholder = true
            this.videoBlockPermission = false
            this.permissionDialogState = false
            deviceId = videoTrack.getVideoTracks()[0].getSettings().deviceId
            this.$store.dispatch('updateVideoDeviceId',deviceId)
            this.$store.commit('settings_setIsVideo',true)
            return
        },
        connectAudioTrack(audioTrack){
            this.microphoneOn = true
            this.validateMicrophone(audioTrack);
        },
    },
    created(){
        if(!this.isLoggedIn) return ;
        this.startPreview()
        navigator.mediaDevices.ondevicechange = this.startPreview;
    },
    beforeDestroy() {
        this.MIXIN_cleanStreams(this.streamsArray)
        this.clearVideoTrack();
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
                    @media (max-width: @screen-sm) {
                        padding: 0;
                    }
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
