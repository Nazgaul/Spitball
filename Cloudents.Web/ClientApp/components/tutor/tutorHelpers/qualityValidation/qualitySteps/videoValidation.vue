<template>
    <div class="video-quality-wrap">
        <v-container fluid>
            <v-layout wrap align-center justify-center column>
                <v-flex>
                    <h4>Video quality test</h4>
                </v-flex>
                <v-flex xs12 sm6 md6>
                    <v-select
                            class="minimum-width"
                            v-model="singleCameraId"
                            :items="avalCameras"
                            item-value="deviceId"
                            item-text="label"
                            label="Standard"
                            hide-details
                            :prepend-icon="''"
                            @change="createVideoQualityPreview()"
                            :placeholder="'Please select camera'"
                            :append-icon="'sbf-arrow-down'"
                            solo
                            single-line
                    ></v-select>
                </v-flex>

            </v-layout>
            <v-layout align-center justify-center>
                <v-flex xs12 sm6 md6 class="mt-3">
                    <div id="local-video-test-track"></div>
                </v-flex>
            </v-layout>
        </v-container>
    </div>
</template>

<script>
    import { createLocalVideoTrack, } from 'twilio-video';
    import tutorService from '../../../tutorService'
    export default {
        name: "videoValidation",
        data() {
            return {
                videoEl: null,
                localTrack: null,
                avalCameras :[],
                singleCameraId: ''
            }
        },
        methods: {
            getVideoInputdevices(){
                let self = this;
                navigator.mediaDevices.enumerateDevices()
                    .then((mediaDevices)=>{
                            mediaDevices.forEach((device)=>{
                                if(device.kind ==='videoinput'){
                                    self.avalCameras.push(device)
                                }
                            })
                    },
                        (error)=>{
                        console.log('error cant get video input devices', error)
                        }
                    )
            },

            createVideoQualityPreview() {
                //clear if exists
                if(this.localTrack){
                    this.clearVideoTrack();
                }
                let self = this;
                createLocalVideoTrack({width: 320, height: 180, deviceId: {exact : self.singleCameraId} })
                    .then(track => {
                    self.videoEl= document.getElementById('local-video-test-track');
                    self.localTrack = track;
                    self.videoEl.appendChild(self.localTrack.attach());
                    console.log('added track')
                });
            },
            clearVideoTrack(){
                    tutorService.detachTracks([this.localTrack])
            }
        },
        created(){
            this.getVideoInputdevices()
        },
        beforeDestroy(){
            this.clearVideoTrack();
            console.log('destroyed component')
        }
    }
</script>

<style scoped lang="less">
    .video-quality-wrap{
    .minimum-width{
        min-width: 340px;
    }
    }


</style>