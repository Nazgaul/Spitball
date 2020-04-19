<template>
    <div class="studyRoomVideoSettings">
        <div class="studyRoom-video-settings-title">
            <h4 class="studyRoom-video-settings-label" v-language:inner='"studyRoomSettings_camera_label"'></h4>
            <v-select
                :menu-props="{contentClass:'select-direction'}"
                v-model="singleCameraId"
                :items="camerasList"
                item-value="deviceId"
                item-text="label"
                :label="text.label"
                hide-details
                :prepend-icon="''"
                @change="createVideoQualityPreview()"
                :placeholder="placeCamera"
                :append-icon="'sbf-arrow-down'"
                solo
                single-line
                ></v-select>
        </div>
        
        <v-flex xs12  class="mt-4 mb-4 studyRoom-video-settings-video-container">
            <div id="local-video-test-track"></div>
        </v-flex>
    </div>
</template>

<script>
import {LanguageService} from '../../../../../services/language/languageService';
import { createLocalVideoTrack, } from 'twilio-video';
import insightService from '../../../../../services/insightService';

export default {
    data(){
        return{
            camerasList:[],
            videoEl: null,
            localTrack: null,
            singleCameraId: global.localStorage.getItem('sb-videoTrackId'),
            placeCamera: LanguageService.getValueByKey("studyRoomSettings_camera_placeholder"),
            text:{
                label: LanguageService.getValueByKey("studyRoomSettings_video_select_label"),
            }
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
                        if(!self.singleCameraId){
                            self.singleCameraId = self.camerasList[0].deviceId;
                        }
                        self.createVideoQualityPreview();
                    }
                },
                (error) => {
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoSettings_getVideoInputdevices', error, null);
                    console.log('error cant get video input devices', error)
                }
            )
        },
        createVideoQualityPreview() {
            if (this.localTrack) {
                this.clearVideoTrack();
            }
            let self = this;
            createLocalVideoTrack({width: 490, height: 368, deviceId: {exact: self.singleCameraId}})
                .then(track => {
                    self.videoEl = document.getElementById('local-video-test-track');
                    self.localTrack = track;
                    self.videoEl.appendChild(self.localTrack.attach());
                    self.$store.dispatch('updateVideoTrack',self.singleCameraId)
                }, (err)=>{
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoValidation_createVideoQualityPreview', err, null);
                    console.error(err);
                });
        },
        clearVideoTrack() {
            if (this.localTrack?.detach) {
                this.localTrack.detach().forEach((detachedElement) => {
                    detachedElement.remove();
                });
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
.studyRoomVideoSettings{
    margin-top: 48px;
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
        height: 300px;
        background-color: gray;
        #local-video-test-track {
            video {
                width:100%;
            }
        }
    }
    
}

</style>
