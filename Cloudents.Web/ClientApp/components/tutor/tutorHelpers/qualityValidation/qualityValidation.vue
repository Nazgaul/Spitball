<template>
    <v-stepper v-model="step" class="quality-test-container">
        <div class="header-text-wrap pt-3 pb-2 px-4">
            <span class="header-text 1" v-if="!getNotAvaliableDevices" v-language:inner="'tutor_quality_title'"></span>
            <span class="header-text 2" v-if="isErorrGettingMedia && !getNotAvaliableDevices" v-language:inner="'tutor_quality_permission'"></span>
            <span class="header-text 3" v-if="isErorrGettingMedia && getNotAvaliableDevices" v-language:inner="'tutor_quality_access_device'"></span>
            
        </div>

        <v-stepper-header v-if="!isErorrGettingMedia">
            <v-stepper-step class="step-indicator" color="#4452fc" :complete-icon="'sbf-checkmark'" :complete="step > 1" step="1">
                <span v-language:inner> tutor_quality_audio_test_in</span>
            </v-stepper-step>
            <v-divider></v-divider>
            <v-stepper-step class="step-indicator" color="#4452fc" :complete-icon="'sbf-checkmark'" :complete="step > 2" step="2">
                <span v-language:inner>tutor_quality_audio_test_out</span>
            </v-stepper-step>
            <v-divider></v-divider>

            <v-stepper-step class="step-indicator" color="#4452fc" :complete-icon="'sbf-checkmark'" step="3">
                <span v-language:inner>tutor_quality_video_test_title</span>
            </v-stepper-step>
        </v-stepper-header>
        <!--header unable to get device-->
        <v-stepper-header class="device-error-header px-4" v-if="isErorrGettingMedia && getNotAvaliableDevices">
            <video-image></video-image><microphone-image></microphone-image><span v-language:inner>tutor_quality_unable</span>
        </v-stepper-header>
        <v-stepper-items>
            <v-stepper-content v-for="n in steps"
                               :key="`${n}-content`"
                               :step="n">
                <v-card
                        class="mb-1 elevation-0"
                        color="grey lighten-1"
                        max-height="450px">
                    <!--if there is an error requesting user media show helper component-->
                    <not-allowed v-if="isErorrGettingMedia" :isNotFound="getNotAvaliableDevices"></not-allowed>
                    <!--in case devices are avaliable show regular test steps-->
                    <component :is="'validation_step_'+step" v-if="n === step && !isErorrGettingMedia"></component>

                </v-card>
                <v-layout align-center justify-end>
                    <v-flex class="d-flex align-center justify-center" style="max-width: 300px;">
                        <button class="blue-btn" v-if="step !== steps && !isErorrGettingMedia"
                                @click="nextStep(n)"
                        >
                            <span v-language:inner>tutor_quality_btn_continue</span>
                        </button>
                        <button class="blue-btn" v-if="isErorrGettingMedia"
                                @click="closeDialog(n)"
                        >
                            <span v-language:inner>tutor_quality_btn_got</span>
                        </button>
                        <button class="blue-btn" v-if="step === steps" @click="closeDialog()">
                            <span v-language:inner>tutor_quality_btn_done</span>
                        </button>

                    </v-flex>
                </v-layout>

            </v-stepper-content>
        </v-stepper-items>
    </v-stepper>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import validation_step_1 from './qualitySteps/audioInputValidation.vue';
    import validation_step_2 from './qualitySteps/audioOutputValidation.vue';
    import validation_step_3 from './qualitySteps/videoValidation.vue';
    import notAllowed from './qualitySteps/notAllowed.vue';
    import microphoneImage from '../../images/microphone.svg'
    import videoImage from '../../images/video-camera.svg'

    export default {
        name: "qualityValidation",
        components: {
            validation_step_1,
            validation_step_2,
            validation_step_3,
            notAllowed,
            microphoneImage,
            videoImage
        },
        data() {
            return {
                // notAvaliableDevices: false,
                // notAllowedDevices: false,
                steps: 3,
                step: 1
            };
        },
        computed: {
            ...mapGetters(['getNotAllowedDevices', 'getNotAvaliableDevices']),
            isErorrGettingMedia() {
                return this.getNotAllowedDevices || this.getNotAvaliableDevices;
            }

        },
        methods: {
            ...mapActions(['updateTestDialogState', 'setAllowedDevicesStatus', 'setAvaliableDevicesStatus']),
            checkPermission() {
                let self = this;
                navigator.mediaDevices.getUserMedia({audio: true, video: true})
                         .then(stream => {
                                   console.log('got permissins', stream);
                               },
                               //handle all error types in catch
                         ).catch((err) => {
                    console.log(err.name + ":!!!!!!!!!!!!!!!! " + err.message, err);
                    if(err.name === 'NotAllowedError') {
                        // self.notAllowedDevices = true;
                        self.setAllowedDevicesStatus(true)
                        self.updateTestDialogState(true);
                    } else if(err.name === 'NotFoundError') {
                        // self.notAvaliableDevices = true;
                        self.setAvaliableDevicesStatus(true);
                        self.updateTestDialogState(true);
                    }
                }); // always check for errors at the end.
            },
            nextStep(n) {
                this.step = n + 1;
            },
            closeDialog() {
                this.step = 1;
                this.updateTestDialogState(false);
            }
        },
        created() {
            this.checkPermission();
        }
    };
</script>

<style lang="less">
    .quality-test-container {
        .step-indicator{
            .v-stepper__step__step{
                margin-right: 8px;
            }
        }
        .device-error-header {
            display: flex;
            align-items: center;
            justify-content: flex-start;
            height: 32px;
            background-color: #43425d;
            font-size: 14px;
            font-weight: 600;
            color: rgba(255, 255, 255, 0.87);
            padding: 20px 0;
            height: unset;
            svg{
                fill: #fff;
                margin: 0 6px 0  0;
            }
            span {
                line-height: 1;
            }

        }
        .blue-btn {
            padding: 10px 16px;
            text-transform: uppercase;
            border-radius: 4px;
            box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.16);
            border: solid 1px #43425d;
            color: rgba(67, 66, 93, 0.87);
        }
        .header-text-wrap {
            display: flex;
            background-color: #f2f2f2;
            width: 100%;
            border-radius: 4px 4px 0 0;
            .header-text {
                font-size: 12px;
                font-weight: 600;
                color: #000000;
            }
        }
    }

</style>