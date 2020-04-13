<template>
    <div class="studyRoom-video-settings-container">
        <v-flex class="mt-4 mb-4 studyRoom-video-settings-video-container">

            <!-- <div class="cameraListWrap text-center pt-3">
                <div class="" v-if="camerasList.length">
                    <v-select
                        v-model="singleCameraId"
                        @change="createVideoQualityPreview()"
                        :items="camerasList"
                        :label="$t('studyRoomSettings_video_select_label')"
                        :placeholder="$t('studyRoomSettings_camera_placeholder')"
                        :append-icon="'sbf-arrow-down'"
                        :menu-props="{contentClass:'select-direction'}"
                        background-color="rgba(0,0,0,.7)"
                        item-text="label"
                        item-value="deviceId"
                        solo
                        dense
                        rounded
                        single-line
                        hide-details
                    >
                        <template v-slot:prepend-inner>
                            <videoCameraImage class="videoIcon" width="20" />
                        </template>
                    </v-select>
                </div>
                <div class="noCamera" v-t="'studyRoomSettings_no_camera'" v-else></div>
            </div> -->

            <div class="bottomIcons d-flex align-end justify-space-between">
                <microphoneImage width="14" />
                <div class="centerIcons d-flex align-center">
                    <v-btn class="mx-2" :class="{'noBorder': !microphoneOn}" fab :color="microphoneOn ? 'transparent' : 'red'" @click="toggleMic">
                        <microphoneImage width="14" v-if="microphoneOn" />
                        <microphoneImageIgnore width="18" v-else />
                    </v-btn>
                    <v-btn class="mx-2" :class="{'noBorder': !cameraOn}" fab :color="cameraOn ? 'transparent' : 'red'" @click="toggleCamera">
                        <videoCameraImage class="videoIcon" width="22" v-if="cameraOn" />
                        <videoCameraImageIgnore width="18" v-else />
                    </v-btn>
                </div>
                <v-icon color="#fff" @click="openSettingDialog">sbf-settings</v-icon>
            </div>

            <div id="local-video-test-track"></div>
        </v-flex>
    </div>
</template>

<script>
// import {LanguageService} from '../../../../../services/language/languageService';
import { createLocalVideoTrack } from 'twilio-video';
import insightService from '../../../../../services/insightService';
import videoCameraImage from '../../../images/video-camera.svg';

import microphoneImage from '../../../images/outline-mic-none-24-px-copy-2.svg'
import microphoneImageIgnore from '../../../images/mic-ignore.svg';
import videoCameraImageIgnore from '../../../images/camera-ignore.svg';

export default {
    components: {
        videoCameraImage,
        microphoneImage,
        microphoneImageIgnore,
        videoCameraImageIgnore,
    },
    data(){
        return{
            microphoneList: [],
            camerasList:[],
            videoEl: null,
            localTrack: null,
            singleCameraId: global.localStorage.getItem('sb-videoTrackId'),
            cameraOn: true,
            microphoneOn: true
            
            // placeCamera: LanguageService.getValueByKey("studyRoomSettings_camera_placeholder"),
            // text:{
            //     label: LanguageService.getValueByKey("studyRoomSettings_video_select_label"),
            // }
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
        },
        toggleMic() {
            this.microphoneOn = !this.microphoneOn
        },
        toggleCamera() {
            this.cameraOn = !this.cameraOn
        },
        openSettingDialog() {
            this.$store.commit('setComponent', 'studyRoomSetting')
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
        border-radius: 6px;
        background-color: #000;
        .cameraListWrap {
            margin: 0 220px;
            position: absolute;
            right: 0;
            left: 0;
            z-index: 2;
            .v-select__selections {
                .v-select__selection--comma {
                    color: #fff;
                    font-size: 13px;
                }
            }
            .sbf-arrow-down {
                color: #fff
            }
            .videoIcon {
                fill: #fff;
            }
        }
        .bottomIcons {
            position: absolute;
            bottom: 10px;
            right: 10px;
            left: 10px;
            z-index: 2;
            .videoIcon {
                fill: #fff;
            }
            .centerIcons {
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
                // width: 100%;
                // border-radius: 6px;
                height: 100%;
                width: 70%;
                object-fit: fill;
                margin: 0 auto;
                text-align: center;
                display: block;
            }
        }
    }
    
}

</style>
