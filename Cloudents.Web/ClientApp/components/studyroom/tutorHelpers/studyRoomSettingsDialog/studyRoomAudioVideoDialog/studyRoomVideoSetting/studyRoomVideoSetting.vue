<template>
    <div class="studyRoomVideoSettings">
        <div class="studyRoom-video-settings-title">
            <h4 class="studyRoom-video-settings-label mb-3" v-language:inner='"studyRoomSettings_camera_label"'></h4>
            <v-select
                :value="getVideoDeviceId"
                class="videoSelect"
                :items="videoDevicesList"
                @change="createVideoPreview"
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
            <div id="localTrackSetting">
                <video ref="videoElPopUp" autoplay playsinline></video>
            </div>
        </v-flex>
    </div>
</template>

<script>

import insightService from '../../../../../../services/insightService';
import { mapGetters } from 'vuex';
import settingMixin from '../../settingMixin.js'

export default {
    data(){
        return {
            videoDevicesList: [],
            streams:[],
            permissionDenied: false,
        }
    },
    mixins: [settingMixin],
    computed: {
        ...mapGetters(['getVideoDeviceId'])
    },
    props:['streamsArray'],
    methods:{
        clearVideoTrack() {
            if(this.streamsArray){
                this.MIXIN_cleanStreams(this.streamsArray)
            }else{
                this.streams.forEach(t => {
                    t.getTracks()[0].stop()
                });
            }
        },
        createVideoPreview(deviceId){
            this.clearVideoTrack()
            let videoParams = {
                audio: false,
                video:{deviceId}
            }
            let self = this;
            this.MIXIN_getMediaTrack(videoParams)
                .then(stream=>{
                    if(self.streamsArray){
                        self.MIXIN_addStream(self.streamsArray,stream)
                    }else{
                        self.streams.push(stream)
                    }
                    self.$refs.videoElPopUp.srcObject = stream;
                    deviceId = stream.getVideoTracks()[0].getSettings().deviceId
                    self.$store.dispatch('updateVideoDeviceId',deviceId)
                    self.$store.dispatch('updateVideoTrack',deviceId)
                    return
                })
                .catch(err=>{
                    insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoSettings_getVideoInputdevices', err, null);
                })
                self.cameraOn = false
        },
        getVideoDevices(){
            let self = this;
            navigator.mediaDevices.enumerateDevices()
                .then(devices => {
                    self.videoDevicesList = devices.filter(device => device.kind === 'videoinput');
                });
        }
    },
    created(){
        this.createVideoPreview(this.getVideoDeviceId)
        this.getVideoDevices()
    },
    beforeDestroy() {
        this.clearVideoTrack()
    },
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
        #localTrackSetting {
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
