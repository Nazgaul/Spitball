<template>
    <div class="audio-input-quality-wrap mt-3">
        <v-container fluid grid-list-xl>
            <v-layout wrap align-center column>
                <v-flex xs12 sm6 md6>
                    <microphoneImage class="mic-image"></microphoneImage>
                </v-flex>
                <v-flex xs12 sm6 md6>
                    <h4 class="audio-title subheading">Audio Input Quality Test</h4>
                    <div class="audio-input-validation mt-3">
                        <div id="audio-input-meter"></div>
                    </div>
                </v-flex>
                <v-flex xs12 sm6 md6 d-flex class="mt-5">
                    <v-select
                            class="minimum-width"
                            v-model="singleMicrophoneId"
                            :items="avalMics"
                            item-value="deviceId"
                            item-text="label"
                            label="Standard"
                            hide-details
                            :prepend-icon="''"
                            @change="validateMicrophone()"
                            :placeholder="'Please select microphone'"
                            :append-icon="'sbf-arrow-down'"
                            solo
                            single-line
                    ></v-select>
                </v-flex>
            </v-layout>
        </v-container>

    </div>
</template>

<script>
    import qualityValidationService from '../qualityValidationService';
    import microphoneImage from '../../../images/microphone.svg'


    export default {
        name: "audioInputValidation",
        components: {microphoneImage},
        data() {
            return {
                avalMics: [],
                singleMicrophoneId: ''
            }
        },
        methods: {
            getAvalDevices() {
                let self = this;
                navigator.mediaDevices.enumerateDevices()
                    .then(function (devices) {
                        devices.forEach(function (device) {
                            console.log(device.kind + ": " + device.label +
                                " id = " + device.deviceId);
                            if (device.kind === 'audioinput') {
                                self.avalMics.push(device);
                                //check if any default micrphone, if so set as default for test
                                if (device && device.label.toLowerCase().includes('default')) {
                                    self.singleMicrophoneId = device.deviceId;
                                    self.validateMicrophone('audio-input-meter', self.singleMicrophoneId);
                                }
                            }
                        });
                        console.log('mics:::', self.avalMics)
                    })
            },
            validateMicrophone() {
                qualityValidationService.createAudioContext('audio-input-meter', this.singleMicrophoneId);
            }
        },
        created() {
            this.getAvalDevices();
            // this.validateMicrophone()
        },
        beforeDestroy() {
            qualityValidationService.stopAudioContext()
        }
    }
</script>

<style scoped lang="less">
    .audio-input-quality-wrap {
        .audio-title{
            font-weight: 600;
            color: rgba(0, 0, 0, 0.87);
        }
        .mic-image{
            height: 40px;
            width: 40px;
            fill: #a8a8a8;
        }
        .audio-input-validation{
            background-color:#ededed;
        }
        .minimum-width {
            min-width: 340px;
        }
    }


</style>