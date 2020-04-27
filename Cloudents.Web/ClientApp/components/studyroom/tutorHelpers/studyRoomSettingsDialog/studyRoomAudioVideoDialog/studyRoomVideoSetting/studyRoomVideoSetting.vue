<template>
    <div class="studyRoomVideoSettings">
        <div class="studyRoom-video-settings-title">
            <h4 class="studyRoom-video-settings-label mb-3" v-language:inner='"studyRoomSettings_camera_label"'></h4>
            <v-select
                v-model="singleCameraId"
                class="videoSelect"
                :items="camerasList"
                @change="createVideoQualityPreview()"
                :label="$t('studyRoomSettings_video_select_label')"
                :placeholder="$t('studyRoomSettings_camera_placeholder')"
                :menu-props="{contentClass:'select-direction'}"
                :prepend-icon="''"
                :append-icon="'sbf-arrow-down'"
                item-value="deviceId"
                item-text="label"
                hide-details
                outlined
                dense
                single-line
            >
            </v-select>
        </div>
        
        <v-flex xs12  class="mt-4 mb-4 studyRoom-video-settings-video-container">
            <div id="local-video-test-track1"></div>
        </v-flex>
    </div>
</template>

<script>
import { createLocalVideoTrack, } from 'twilio-video';

import insightService from '../../../../../../services/insightService';

export default {
    data(){
        return {
            permissionDenied: false,
            camerasList:[],
            videoEl: null,
            localTrack: null,
            singleCameraId: global.localStorage.getItem('sb-videoTrackId')
        }
    },
    methods:{
        getVideoInputdevices() {
            let self = this;
            navigator.mediaDevices.enumerateDevices().then((mediaDevices) => {
                        mediaDevices.forEach((device) => {
                            if (device.kind === 'videoinput') {
                                self.camerasList.push(device)
                            }
                        })
                        if(self.camerasList.length > 0){
                            let isNotInList = self.camerasList.every(device=>device.deviceId !== self.singleCameraId)
                    
                            if(isNotInList || !self.singleCameraId){
                                self.singleCameraId = self.camerasList[0].deviceId;
                            }
                            self.createVideoQualityPreview();
                        }
                        if(self.permissionDenied) return self.camerasList = []
                },
                (error) => {
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoSettings_getVideoInputdevices', error, null);
                    console.log('error cant get video input devices', error)
                }
            )
        },
        async createVideoQualityPreview() {
            if (this.localTrack) {
                this.clearVideoTrack();
            }
            let self = this;
            let isNotInList = self.camerasList.every(device=>device.deviceId !== self.singleCameraId)
    
            // if what i have in LS === one of the ids in the list
            // if yes: this is my singleId
            // if not: take the first from the list
            let isDeviceInList = self.camerasList.some(device=>{
                if(device.deviceId === self.singleCameraId){
                    return true;
                }
            })
            if(isDeviceInList){
                debugger
                //
            }else{
                debugger
                self.singleCameraId = self.camerasList[0].deviceId;
            }

            // if(isNotInList || !self.singleCameraId){
            //     self.singleCameraId = self.camerasList[0].deviceId;
            // }
            let videoParams = {audio:false,video :{ deviceId: self.singleCameraId}}
            await navigator.mediaDevices.getUserMedia(videoParams)
                .then(track => {
                    self.videoEl = document.getElementById('local-video-test-track1');
                    self.localTrack = track;
                    let video = document.createElement('video');
                    video.srcObject = track;
                    video.onloadedmetadata = function() {
                        video.play();
                    };
                    self.videoEl.appendChild(video);
                    self.$store.dispatch('updateVideoTrack',self.singleCameraId)
                }).catch(err => {
                    debugger
                    self.permissionDenied = true
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoValidation_createVideoQualityPreview', err, null);
                })
        },
        clearVideoTrack() {
            if (this.localTrack) {
                this.localTrack.getTracks()[0].stop()
                let video = document.querySelector('#local-video-test-track1 video');
                if(video){
                    this.videoEl.removeChild(video)
                }
                this.localTrack = null;
            }
        }
    },
    created(){
        this.getVideoInputdevices();
    },
    beforeDestroy() {
        this.clearVideoTrack();
    }
}
</script>

<style lang="less">
@import '../../../../../../styles/colors.less';
@import '../../../../../../styles/mixin.less';

.studyRoomVideoSettings{
    width: 100%;
    margin-top: 36px;
    .studyRoom-video-settings-title {
        .studyRoom-video-settings-label{
            color: @global-auth-text;
            font-weight: 600;
        }
        .videoSelect {
            font-size: 14px;
                .v-input__slot {
                    min-height: 38px !important; // vuetify
                }
                ::placeholder { /* Chrome, Firefox, Opera, Safari 10.1+ */
                    color: @global-purple;
                    opacity: 1; /* Firefox */
                }
                :-ms-input-placeholder { /* Internet Explorer 10-11 */
                    color: @global-purple;
                }
                ::-ms-input-placeholder { /* Microsoft Edge */
                    color: @global-purple;
                }
        }
        .v-input__control{
            min-height: unset;
            .v-select__selection{
                font-size: 14px;
            }
        }
    }
    .studyRoom-video-settings-video-container{
        width: 100%;
        height: 300px;
        overflow: hidden;
        background-color: gray;

        @media (max-width: @screen-xs) {
            height: auto;
        }
        #local-video-test-track1 {
            height: 100%;

            video {
                width: 100%;
                overflow: hidden;
                @media (max-width: @screen-xs) {
                    display: block;
                }
            }
        }
    }
    
}

.select-direction {
    .v-list-item__title {
        color: #4c59ff;
    }
}

</style>
