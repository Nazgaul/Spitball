<template>
    <div class="audio-input-quality-wrap">
        <v-container fluid grid-list-xl>
            <v-layout wrap align-center column>
                <v-flex xs12 sm6 md6>
                    <h4>Audio Input Quality test</h4>
                    <div class="audio-input-validation">
                        <div id="audio-input-meter"></div>
                    </div>
                </v-flex>
                <v-flex xs12 sm6 md6 d-flex>
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
    import qualityValidationService from '../qualityValidationService'

    export default {
        name: "audioInputValidation",
        data() {
            return {
                avalMics: [],
                singleMicrophoneId: 'none'
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
    .audio-input-quality-wrap{
        .minimum-width{
            min-width: 340px;
        }
    }


</style>