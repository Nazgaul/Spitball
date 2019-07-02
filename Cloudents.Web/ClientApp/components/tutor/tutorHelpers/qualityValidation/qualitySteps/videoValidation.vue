<template>
    <v-layout class="video-quality-wrap" align-center justify-center column  wrap>
        <v-layout align-center justify-center row>
            <v-flex>
                <videoCameraImage class="video-cam-icon"></videoCameraImage>
            </v-flex>
            <v-flex class="ml-2">
                <h4 class="video-title subheading" v-language:inner>tutor_quality_video_title</h4>

            </v-flex>
        </v-layout>
        <v-flex xs12   class="mt-3 mb-4" >
            <div id="local-video-test-track"></div>
        </v-flex>
        <v-flex xs12   d-flex class="width-force mb-4">
            <v-select
                    class="minimum-width"
                    :menu-props="{contentClass:'select-direction'}"
                    v-model="singleCameraId"
                    :items="avalCameras"
                    item-value="deviceId"
                    item-text="label"
                    label="Standard"
                    hide-details
                    :prepend-icon="''"
                    @change="createVideoQualityPreview()"
                    :placeholder="placeCamera"
                    :append-icon="'sbf-arrow-down'"
                    solo
                    single-line
            ></v-select>
        </v-flex>
    </v-layout>

</template>

<script>
    import { createLocalVideoTrack, } from 'twilio-video';
    import tutorService from '../../../tutorService';
    import videoCameraImage from '../../../images/video-camera.svg'
    import { LanguageService } from "../../../../../services/language/languageService";
    import insightService from '../../../../../services/insightService';

    export default {
        name: "videoValidation",
        components: {videoCameraImage},
        data() {
            return {
                videoEl: null,
                localTrack: null,
                avalCameras: [],
                singleCameraId: '',
                placeCamera: LanguageService.getValueByKey("tutor_quality_camera_placeholder")
            }
        },
        methods: {
            getVideoInputdevices() {
                let self = this;
                navigator.mediaDevices.enumerateDevices()
                    .then((mediaDevices) => {
                            mediaDevices.forEach((device) => {
                                if (device.kind === 'videoinput') {
                                    self.avalCameras.push(device)
                                }
                            })
                        },
                        (error) => {
                            insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoValidation_getVideoInputdevices', err, null);
                            console.log('error cant get video input devices', error)
                        }
                    )
            },

            createVideoQualityPreview() {
                //clear if exists
                if (this.localTrack) {
                    this.clearVideoTrack();
                }
                let self = this;
                createLocalVideoTrack({width: 320, height: 180, deviceId: {exact: self.singleCameraId}})
                    .then(track => {
                        self.videoEl = document.getElementById('local-video-test-track');
                        self.localTrack = track;
                        self.videoEl.appendChild(self.localTrack.attach());
                        console.log('added track')
                    }, (err)=>{
                insightService.track.event(insightService.EVENT_TYPES.ERROR, 'StudyRoom_VideoValidation_createVideoQualityPreview', err, null);
                console.error(err);
            });
            },
            clearVideoTrack() {
                tutorService.detachTracks([this.localTrack])
            }
        },
        created() {
            this.getVideoInputdevices()
        },
        beforeDestroy() {
            this.clearVideoTrack();
        }
    }
</script>

<style lang="less">
    /*.select-direction{*/
        /*.v-select-list{*/
            /*direction: ltr!*rtl:ignore*!;*/
        /*}*/
    /*}*/
    .video-quality-wrap {
        .width-force{
            width: 98%;
        }
        #local-video-test-track{
            min-height: 168px;
            min-width: 298px;
            box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.16);
            border: solid 6px #ffffff;
        }
        .video-title{
            font-weight: 600;
            color: rgba(0, 0, 0, 0.87);
        }
        .video-cam-icon {
                height: 40px;
                width: 40px;
                fill: #a8a8a8;
        }
        .minimum-width {
            min-width: 340px;
        }
    }


</style>