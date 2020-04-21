<template>
    <div class="studyRoomVideoSettings">
        <div class="studyRoom-video-settings-title">
            <h4 class="studyRoom-video-settings-label mb-4" v-language:inner='"studyRoomSettings_camera_label"'></h4>
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
                        if(!self.singleCameraId){
                            self.singleCameraId = self.camerasList[0].deviceId;
                        }
                        self.createVideoQualityPreview();
                        return
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
                    self.videoEl = document.getElementById('local-video-test-track1');
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
@import '../../../../../../styles/colors.less';

.studyRoomVideoSettings{
    width: 100%;
    margin-top: 36px;
    .studyRoom-video-settings-title {
        .studyRoom-video-settings-label{
            color: @global-auth-text;
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
        #local-video-test-track1 {
            height: 100%;
            video {
                width: 100%;
                overflow: hidden;
            }
        }
    }
    
}

</style>
