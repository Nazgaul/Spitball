<template>
    <div class="studyRoom-audio-settings-container">
        <div class="studyRoom-audio-settings-microphone-container">
            <h4 class="studyRoom-audio-settings-microphone-label" v-language:inner="'studyRoomSettings_audio_input'"></h4>
            <!-- <v-divider style="margin-bottom: 10px;"></v-divider> -->
            <div class="audioSelect">
                <v-select 
                class="minimum-width"
                :menu-props="{contentClass:'select-direction'}"
                v-model="singleMicrophoneId"
                :items="microphoneList"
                item-value="deviceId"
                item-text="label"
                :label="text.label"
                hide-details
                :prepend-icon="''"
                @change="validateMicrophone()"
                :placeholder="micPlaceholder"
                :append-icon="'sbf-arrow-down'"
                solo
                single-line
                ></v-select>
                <v-layout class="indicator-audio-meter" style="">
                    <div style="margin: 0 15px 0 0" v-language:inner='"studyRoomSettings_audio_indicator"'></div>
                    <div id="audio-input-meter"></div>
                </v-layout>
            </div>
            
        </div>
        <v-divider style="margin: 20px 0;"></v-divider>
        <div class="studyRoom-audio-settings-speaker-container">
            <h4 class="studyRoom-audio-settings-speaker-label" v-language:inner="'studyRoomSettings_audio_output'"></h4>
            
            <div class="audio-output-controls">
                <button @click="playTestSound" v-if="!isPlaying" v-language:inner='"studyRoomSettings_audio_test_sound"'></button>
                <button @click="stopSound" v-else v-language:inner='"studyRoomSettings_audio_stop_sound"'></button>
                <v-flex v-if="isPlaying" class="eq-image-container">
                    <img class="eq-image" src="../../../images/eq.gif" alt="">
                </v-flex>
            </div>
        </div>
        
    </div>
</template>

<script>
import studyRoomAudioSettingService from './studyRoomAudioSettingService';
import { LanguageService } from "../../../../../services/language/languageService";
export default {
    data(){
        return{
            microphoneList: [],
            singleMicrophoneId: global.localStorage.getItem('sb-audioTrackId'),
            micPlaceholder: LanguageService.getValueByKey("studyRoomSettings_mic_placeholder"),
            soundUrl: `https://zboxstorage.blob.core.windows.net/zboxhelp/new/music-check.mp3`,
            audio: null,
            isPlaying: false,
            text:{
                lable: LanguageService.getValueByKey('studyRoomSettings_audio_select_label')
            }
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
                        self.validateMicrophone('audio-input-meter', self.singleMicrophoneId);
                    })
            },
            validateMicrophone() {
                studyRoomAudioSettingService.createAudioContext('audio-input-meter', this.singleMicrophoneId);
                this.$store.dispatch('updateAudioTrack',this.singleMicrophoneId)
            }
        },
        created() {
            this.getDevices();
        },
        beforeDestroy() {
            studyRoomAudioSettingService.stopAudioContext();
            this.stopSound();
        }
}
</script>

<style lang="less">
.studyRoom-audio-settings-container{
    margin-top: 48px;
    .studyRoom-audio-settings-microphone-container{
        display: flex;
        .audioSelect{
            width: 100%;
        }
        .studyRoom-audio-settings-microphone-label{
            min-width: 100px;
        }
        .indicator-audio-meter{
                font-size: 14px;
                margin-top:20px;
                display:flex;
                align-items: center;
            }
        .v-input__control{
            min-height: unset;
            .v-select__selection{
                font-size: 14px;
            }
        }
    }
    .studyRoom-audio-settings-speaker-container{
        display: flex;
        margin-top: 25px;
        .studyRoom-audio-settings-speaker-label{
            font-size: 14px;
            width: 100px;
        }
        .audio-output-controls{
            font-size: 14px;
            display: flex;
            button{
                background-color: #5158af;
                padding: 5px;
                color: #FFF;
                border-radius: 4px;
                outline: none;
            }
            .eq-image-container{
                margin-left: 10px;
                display: flex;
            }
        }
    }
    
}
</style>
