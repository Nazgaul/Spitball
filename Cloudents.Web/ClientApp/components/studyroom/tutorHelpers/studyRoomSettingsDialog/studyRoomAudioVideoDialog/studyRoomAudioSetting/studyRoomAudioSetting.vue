<template>
    <div class="studyRoom-audio-settings-container">
        <div class="studyRoom-audio-settings-microphone-container">
            <h4 class="studyRoom-audio-settings-microphone-label mb-3" v-language:inner="'studyRoomSettings_audio_input'"></h4>
            <div class="audioSelect cameraListWrap d-sm-flex d-block align-center">
                <v-select 
                    v-model="singleMicrophoneId"
                    :items="microphoneList"
                    @change="validateMicrophone()"
                    class="selectAudio flex-grow-1"
                    :label="$t('studyRoomSettings_audio_select_label')"
                    :placeholder="$t('studyRoomSettings_mic_placeholder')"
                    item-value="deviceId"
                    item-text="label"
                    :menu-props="{contentClass:'select-direction'}"
                    :prepend-icon="''"
                    :append-icon="'sbf-arrow-down'"
                    small
                    hide-details
                    outlined
                    dense
                    single-line
                ></v-select>                      
                <v-layout class="indicator-audio-meter ml-sm-4 mt-3 mt-sm-0">
                    <microphoneImage class="image mr-1" width="24" />
                    <div id="audio-input-meter1"></div>
                </v-layout>
            </div>
        </div>
        <div class="studyRoom-audio-settings-speaker-container mt-8">
            <h4 class="studyRoom-audio-settings-speaker-label mb-3" v-t="'studyRoomSettings_audio_output'"></h4>
            <div class="audio-output-controls">
                <speakerImage width="20" class="mr-2" />
                <button @click="playTestSound" v-if="!isPlaying" v-t='"studyRoomSettings_audio_test_sound"'></button>
                <button @click="stopSound" v-else v-t='"studyRoomSettings_audio_stop_sound"'></button>
                <v-flex v-if="isPlaying" class="eq-image-container">
                    <img class="eq-image" src="../../images/speakers.gif" alt="">
                </v-flex>
            </div>
        </div>
    </div>
</template>

<script>
import studyRoomAudioSettingService from './studyRoomAudioSettingService';

import microphoneImage from '../../../../images/microphone.svg';
import speakerImage from '../../images/speaker.svg';

export default {
    components: {
        microphoneImage,
        speakerImage
    },
    data(){
        return{
            microphoneList: [],
            singleMicrophoneId: global.localStorage.getItem('sb-audioTrackId'),
            soundUrl: `https://zboxstorage.blob.core.windows.net/zboxhelp/new/music-check.mp3`,
            audio: null,
            isPlaying: false,
        }
    },
    methods: {
            playTestSound() {
                this.audio = new Audio(`${this.soundUrl}`);
                this.audio.play()
                this.isPlaying = true;
            },
            stopSound(){
                if(this.audio && this.audio.pause){
                    this.audio.pause();
                }
                this.isPlaying = false;
            },
            getDevices() {
                let self = this;
                navigator.mediaDevices.enumerateDevices()
                    .then(function (devices) {
                        devices.forEach(function (device) {
                            if (device.kind === 'audioinput') {
                                self.microphoneList.push(device);
                                if(!self.singleMicrophoneId){
                                    //check if any default micrphone, if so set as default for test
                                    if (device && device.label.toLowerCase().includes('default')) {
                                        self.singleMicrophoneId = device.deviceId;
                                    }
                                }
                            }
                        });
                        self.validateMicrophone('audio-input-meter1', self.singleMicrophoneId);
                    })
            },
            validateMicrophone() {
                studyRoomAudioSettingService.createAudioContext('audio-input-meter1', this.singleMicrophoneId);
                this.$store.dispatch('updateAudioTrack',this.singleMicrophoneId)
            }
        },
        created() {
            this.getDevices();
        },
        beforeDestroy() {
            // studyRoomAudioSettingService.stopAudioContext();
            this.stopSound();
        }
}
</script>

<style lang="less">
@import '../../../../../../styles/colors.less';
@import '../../../../../../styles/mixin.less';

.studyRoom-audio-settings-container{
    margin-top: 36px;
    width: 100%;
    .studyRoom-audio-settings-microphone-container{
        .audioSelect{
            .selectAudio {
                min-width: 420px;
                flex-basis: 0;
                font-size: 14px;

                @media (max-width: @screen-xs) {
                    min-width: 100%;
                }
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
                .v-select__selection--comma {
                    max-width: 100%;
                }
                .v-list-item__title {
                    color: #4c59ff !important;
                }
            }
        }
        .studyRoom-audio-settings-microphone-label {
            color: @global-auth-text;
            font-weight: 600;
        }
        .indicator-audio-meter{
                font-size: 14px;
                display:flex;
                align-items: center;

                .image {
                    fill: #7a798c;
                    flex-shrink: 0;
                }
                .audio-input-meter1 {
                    background: #16eab1;
                    height: 6px;
                    max-width: 150px;
                }
            }
        .v-input__control{
            min-height: unset;
            .v-select__selection{
                font-size: 14px;
            }
        }
    }
    .studyRoom-audio-settings-speaker-container {
        .studyRoom-audio-settings-speaker-label {
            color: @global-auth-text;
            font-weight: 600;
        }
        .audio-output-controls {
            color: @global-purple;
            font-size: 14px;
            display: flex;
            align-items: center;
            button {
                outline: none;
            }
            .eq-image-container{
                margin-left: 10px;
                display: flex;
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
