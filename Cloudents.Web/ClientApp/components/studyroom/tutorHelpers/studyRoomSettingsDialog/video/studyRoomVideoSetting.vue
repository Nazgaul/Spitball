<template>
    <div class="studyRoom-video-settings-container mr-12">
        <v-row class="studyRoom-video-settings-video-container elevation-2">
            <div class="cameraListWrap text-center pt-3">
                <div class="noCamera white--text" v-if="!camerasList.length" v-t="'studyRoomSettings_no_camera'"></div>
                <div class="noPermission white--text" v-if="permissionDenied" v-t="permissionText"></div>
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
                <v-icon color="#fff" @click="openSettingDialog" size="26">sbf-settings</v-icon>
            </div>

            <div id="local-video-test-track"></div>
            <div class="videoOverlay"></div>
        </v-row>

        <studyRoomAudioVideoDialog
            v-if="settingDialogState"
            @updateSettingDialogState="val => settingDialogState = val"
        />
        <v-dialog v-model="permissionDialogState" width="512" persistent content-class="premissionDeniedDialog pa-6 pb-4">
            <div class="mb-6" v-t="'studyRoomSettings_block_title'"></div>
            <div v-t="'studyRoomSettings_block_description'"></div>
            <div class="text-right">
                <v-btn
                    @click="permissionDialogState = false"
                    color="#5360FC"
                    rounded
                    text
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

import studyRoomAudioVideoDialog from '../studyRoomAudioVideoDialog.vue'

import microphoneImage from '../../../images/outline-mic-none-24-px-copy-2.svg'
import microphoneImageIgnore from '../../../images/mic-ignore.svg';
import videoCameraImage from '../../../images/video-camera.svg';
import videoCameraImageIgnore from '../../../images/camera-ignore.svg';

export default {
    components: {
        videoCameraImage,
        videoCameraImageIgnore,
        microphoneImage,
        microphoneImageIgnore,
        studyRoomAudioVideoDialog
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
                return 'studyRoomSettings_block_description'
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
                        self.cameraOn = true
                    }
                })
                    if(self.camerasList.length > 0){
                        if(!self.singleCameraId){
                            self.singleCameraId = self.camerasList[0].deviceId;
                        }
                        self.createVideoQualityPreview();
                        return
                    }
                    self.cameraOn = false
                }).catch(error => {
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoSettings_getVideoInputdevices', error, null);
                    console.log('error cant get video input devices', error)
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
                    if(!document.getElementsByTagName('video').length) {
                        self.videoEl = document.getElementById('local-video-test-track');
                        self.localTrack = track;
                        self.videoEl.appendChild(self.localTrack.attach());
                        self.$store.dispatch('updateVideoTrack',self.singleCameraId)
                    }
                }).catch(err => {
                    self.permissionDenied = true
                    self.permissionDialogState = true
                    self.cameraOn = false
                    self.microphoneOn = false
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoValidation_createVideoQualityPreview', err, null);
                    console.error(err);
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
        },
        openSettingDialog() {
            this.settingDialogState = true;
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
.studyRoom-video-settings-container{
    .studyRoom-video-settings-title{
        display:flex;
        justify-content: space-between;
        .studyRoom-video-settings-label{
            font-size: 14px;
            min-width: 100px;
        }
        .v-input__control{
            min-height: unset;
            .v-select__selection{
                font-size: 14px;
            }
        }
    }
    .studyRoom-video-settings-video-container{
        position: relative;
        border-radius: 8px;
        background-color: #202124;
        .cameraListWrap {
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
            width: 740px;
            height: 400px;
            video{
                width: 100%;
                border-radius: 8px;
                height: 100%;
                text-align: center;
                display: block;
            }
        }
        .videoOverlay{
            background-image: -webkit-linear-gradient(bottom,rgba(0,0,0,0.7) 0,rgba(0,0,0,0.3) 50%,rgba(0,0,0,0) 100%);
            background-image: linear-gradient(bottom,rgba(0,0,0,0.7) 0,rgba(0,0,0,0.3) 50%,rgba(0,0,0,0) 100%);
            height: 80px;
            margin-top: -80px;
            position: relative;
            width: 100%;
            border-radius: 6px
        }
    }
}
.premissionDeniedDialog {
    background: #fff !important;
}

</style>
