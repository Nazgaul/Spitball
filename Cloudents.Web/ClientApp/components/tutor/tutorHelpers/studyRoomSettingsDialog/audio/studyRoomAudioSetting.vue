<template>
    <div class="studyRoom-audio-settings-container">
        <div>
            <h3>Audio Input:</h3>
            <v-divider style="margin-bottom: 10px;"></v-divider>
            <v-select
                class="minimum-width"
                :menu-props="{contentClass:'select-direction'}"
                v-model="singleMicrophoneId"
                :items="microphoneList"
                item-value="deviceId"
                item-text="label"
                label="Select Audio"
                hide-details
                :prepend-icon="''"
                @change="validateMicrophone()"
                :placeholder="micPlaceholder"
                :append-icon="'sbf-arrow-down'"
                solo
                single-line
            ></v-select>
            <v-layout class="indicator-audio-meter" style="">
                <div style="margin: 0 15px 0 0">Indicator:</div>
                <div id="audio-input-meter"></div>
            </v-layout>
        </div>
        <div>
            <h3>Audio Output:</h3>
            <v-divider style="margin-bottom: 10px;"></v-divider>
            <v-layout class="audio-output-controls">
            <button @click="playTestSound" v-if="!isPlaying">Test Sound</button>
            <button @click="stopSound" v-else>Stop Sound</button>
            <v-flex v-if="isPlaying" style="margin-left: 10px; display: flex;">
                <img class="eq-image" src="../../../images/eq.gif" alt="">
            </v-flex>
        </v-layout>
        </div>
        
    </div>
</template>

<script>
import studyRoomAudioSettingService from './studyRoomAudioSettingService';
import { LanguageService } from "../../../../../services/language/languageService";
import {mapState, mapActions} from 'vuex';
export default {
    data(){
        return{
            microphoneList: [],
            singleMicrophoneId: global.localStorage.getItem('sb-audioTrackId'),
            micPlaceholder: LanguageService.getValueByKey("tutor_quality_mic_placeholder"),
            soundUrl: `https://zboxstorage.blob.core.windows.net/zboxhelp/new/music-check.mp3`,
            audio: null,
            isPlaying: false
        }
    },
    computed:{
        ...mapState(['studyRoomTracks_store']),
    },
    methods: {
        ...mapActions(['changeAudioTrack']),
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
                        console.log('mics:::', self.microphoneList)
                    })
            },
            validateMicrophone() {
                studyRoomAudioSettingService.createAudioContext('audio-input-meter', this.singleMicrophoneId);
                global.localStorage.setItem(this.studyRoomTracks_store.storageENUM.audio, this.singleMicrophoneId);
                this.changeAudioTrack(this.singleMicrophoneId);
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
    .indicator-audio-meter{
        margin-top:10px; 
        display:flex;
        align-items: center;
    }
    .audio-output-controls{
        button{
            background-color: #3dc2ba;
            padding: 5px;
            color: #FFF;
            border-radius: 4px;
            outline: none;
        }
    }
}
</style>
