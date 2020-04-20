<template>
    <div class="srVideoSettingsVideoContainerWrap">
        <v-row class="srVideoSettingsVideoContainer ma-md-0 ma-auto elevation-2">
            <div class="cameraTextWrap text-center">
                <div class="noCamera white--text" v-if="!camerasList.length" v-t="'studyRoomSettings_no_camera'"></div>
                <i18n class="blockPermission inCamera white--text" v-if="permissionDenied" :path="permissionText" tag="div">
                    <cameraBlock class="cameraBlock" width="20" />
                </i18n>
            </div>

            <div class="bottomIcons d-flex align-end justify-space-between">
                <microphoneImage v-if="!permissionDenied" width="14" />
                <div class="centerIcons d-flex align-center">
                    <v-btn
                        class="mx-2"
                        :class="{'noBorder': !microphoneOn}"
                        :color="microphoneOn ? 'transparent' : '#cb4243'" @click="toggleMic"
                        fab
                    >
                        <microphoneImage width="14" v-if="microphoneOn" />
                        <microphoneImageIgnore width="18" v-else />
                    </v-btn>
                    <v-btn 
                        class="mx-2"
                        :class="{'noBorder': !cameraOn || !singleCameraId}"
                        :color="cameraOn ? 'transparent' : '#cb4243'" @click="toggleCamera"
                        fab 
                    >
                        <videoCameraImage class="videoIcon" width="22" v-if="cameraOn && singleCameraId" />
                        <videoCameraImageIgnore width="18" v-else />
                    </v-btn>
                </div>
                <v-icon color="#fff" @click="settingDialogState = true" size="26">sbf-settings</v-icon>
            </div>

            <div id="local-video-test-track">
                <video id="videoPlaceholder" width="600" height="400" v-show="!cameraOn"></video>
            </div>
            <div class="videoOverlay"></div>
        </v-row>

        <studyRoomAudioVideoDialog
            v-if="settingDialogState"
            @closeAudioVideoSettingDialog="val => settingDialogState = val"
        />

        <v-dialog v-model="permissionDialogState" width="512" persistent content-class="premissionDeniedDialog pa-6 pb-4">
            <div class="mb-6 mainTitle" v-t="'studyRoomSettings_block_title'"></div>
            <i18n path="studyRoomSettings_block_permission" tag="div" class="blockPermission mb-6">
                <cameraBlock class="cameraBlock" width="20" />
            </i18n>
            <div class="text-right">
                <v-btn
                    @click="permissionDialogState = false"
                    class="white--text"
                    color="#5360FC"
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
import { createLocalVideoTrack } from 'twilio-video';

import insightService from '../../../../../services/insightService';

import studyRoomAudioVideoDialog from '../studyRoomAudioVideoDialog/studyRoomAudioVideoDialog.vue'

import microphoneImage from '../../../images/outline-mic-none-24-px-copy-2.svg'
import microphoneImageIgnore from '../../../images/mic-ignore.svg';
import videoCameraImage from '../../../images/video-camera.svg';
import videoCameraImageIgnore from '../../../images/camera-ignore.svg';
import cameraBlock from '../images/cameraBlock.svg'

export default {
    components: {
        videoCameraImage,
        videoCameraImageIgnore,
        microphoneImage,
        microphoneImageIgnore,
        studyRoomAudioVideoDialog,
        cameraBlock
    },
    data(){
        return{
            videoEl: null,
            localTrack: null,
            cameraOn: true,
            microphoneOn: true,
            permissionDenied: false,
            permissionDialogState: false,
            settingDialogState: false,
            camerasList:[],
            singleCameraId: global.localStorage.getItem('sb-videoTrackId')
        }
    },
    computed: {
        permissionText() {
            if(this.permissionDenied && !this.permissionDialogState) {
                return 'studyRoomSettings_block_permission'
            }
            return 'studyRoomSettings_camera_permission_denied'
        }
    },
    methods:{
        getVideoInputdevices() {
            this.camerasList = []

            let self = this;
            navigator.mediaDevices.enumerateDevices().then((mediaDevices) => {
                mediaDevices.forEach((device) => {
                    if (device.kind === 'videoinput') {
                        self.camerasList.push(device)
                    }
                })
                    if(self.camerasList.length > 0){
                        if(!self.singleCameraId){
                            self.singleCameraId = self.camerasList[0].deviceId;
                        }
                        self.cameraOn = true
                        self.createVideoQualityPreview();
                        return
                    }
                    self.cameraOn = false
                }).catch(error => {
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoSettings_getVideoInputdevices', error, null);
                })
        },
        createVideoQualityPreview() {
            if (this.localTrack) {
                this.clearVideoTrack();
            }
            
            let self = this;
            createLocalVideoTrack({width: 600, height: 400})
                .then(track => {
                    // Checking whether a video tag is already have been attached to dom.
                    // Reason: duplicate video attached when pluggin device on/off
                    let videoPlaceholderExist = document.getElementById('videoPlaceholder')

                    if(videoPlaceholderExist) {
                        self.videoEl = document.getElementById('local-video-test-track');
                        self.localTrack = track;
                        self.videoEl.appendChild(self.localTrack.attach());
                        self.$store.dispatch('updateVideoTrack',self.singleCameraId)
                        videoPlaceholderExist
                    }
                }).catch(err => {
                    self.permissionDenied = true
                    self.permissionDialogState = true
                    self.cameraOn = false
                    self.microphoneOn = false
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoValidation_createVideoQualityPreview', err, null);
                })
        },
        clearVideoTrack() {
            if (this.localTrack?.detach) {
                this.localTrack.detach().forEach((detachedElement) => {
                    detachedElement.remove();
                });
            }
        },
        toggleMic() {
            if(this.permissionDenied) return

            this.microphoneOn = !this.microphoneOn
        },
        toggleCamera() {
            if(this.permissionDenied) return

            this.cameraOn = !this.cameraOn

            if(!this.cameraOn) {
                this.clearVideoTrack()
                this.camerasList = []
                return
            }

            this.getVideoInputdevices()
        }
    },
    created(){
        this.getVideoInputdevices();

        navigator.mediaDevices.ondevicechange = this.getVideoInputdevices
    },
    beforeDestroy() {
        this.clearVideoTrack();
    }
}
</script>

<style lang="less">
@import '../../../../../styles/mixin';
@import '../../../../../styles/colors';
.srVideoSettingsVideoContainerWrap {
    max-width: 600px;
    // height: 400px;
    // width: 100%;
    .srVideoSettingsVideoContainer {
        width: 100%;
        position: relative;
        border-radius: 8px;
        background-color: #202124;

        // @media (max-width: @screen-sm) {
        //     max-width: 400px;
        // }
        .cameraTextWrap {
            margin: 0 100px;
            position: absolute;
            top: calc(50% - 36px); // center text
            right: 0;
            left: 0;
            z-index: 2;
            .noCamera, .noPermission {
                font-size: 20px;
                font-weight: 600;
            }
            .blockPermission {
                color: rgba(0,0,0,0.541);

                &.inCamera {
                    font-size: 22px;
                    line-height: 30px;
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
            .videoIcon {
                fill: #fff;
            }
            .centerIcons {
                width: 100%;
                text-align: center;
                justify-content: center;
                button {
                    border: 1px solid #fff !important;
                    &.noBorder {
                        border: none !important;
                    }
                }
            }
        }
        #local-video-test-track{
            width: 100%;

            .videoPlaceHolder {
                    width: 100%;
                    border-radius: 8px;
                    height: 100%;
                    text-align: center;
                    display: block;
                }
            video {
                width: 100%;
                border-radius: 8px;
                height: 100%;
                text-align: center;
                display: block;
            }
        }
        .videoOverlay {
            background-image: -webkit-linear-gradient(bottom, rgba(0, 0, 0, 0.7) 0, rgba(0, 0, 0, 0.3) 50%, rgba(0, 0, 0, 0) 100%);
            background-image: linear-gradient(bottom, rgba(0, 0, 0, 0.7) 0, rgba(0, 0, 0, 0.3) 50%, rgba(0, 0, 0, 0) 100%);
            /* margin-top: -80px; */
            width: 100%;
            border-radius: 6px;
            position: absolute;
            bottom: 0;
            height: 100px;
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
        color: rgba(0,0,0,0.541);
        line-height: 22px;

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
