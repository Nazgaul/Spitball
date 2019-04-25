<template>
    <v-stepper v-model="step" class="quality-test-container">
        <div class="header-text-wrap pt-3 pb-2 px-4">
            <span class="header-text" v-if="!isErorrGettingMedia" v-language:inner>tutor_quality_title</span>
            <span class="header-text" v-if="isErorrGettingMedia && !notAvaliableDevices" v-language:inner>tutor_quality_permission</span>
            <span class="header-text" v-if="isErorrGettingMedia && notAvaliableDevices" v-language:inner>tutor_quality_access_device</span>
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

            <v-stepper-step class="step-indicator" color="#4452fc" :complete-icon="'sbf-checkmark'" step="3">Video Test</v-stepper-step>
        </v-stepper-header>
        <!--header unable to get device-->
        <v-stepper-header class="device-error-header px-4 py-2" v-if="isErorrGettingMedia && notAvaliableDevices">
            <span v-language:inner>tutor_quality_unable</span>
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
                    <not-allowed v-if="isErorrGettingMedia" :isNotFound="notAvaliableDevices"></not-allowed>
                    <!--in case devices are avaliable show regular test steps-->
                    <component :is="'validation_step_'+step" v-if="n === step && !isErorrGettingMedia"></component>

                </v-card>
                <v-layout align-center justify-center>
                    <v-flex xs2 sm2 md2 class="d-flex align-center justify-center">
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
    import { mapActions } from 'vuex';
    import validation_step_1 from './qualitySteps/audioInputValidation.vue';
    import validation_step_2 from './qualitySteps/audioOutputValidation.vue';
    import validation_step_3 from './qualitySteps/videoValidation.vue';
    import notAllowed from './qualitySteps/notAllowed.vue';

    export default {
        name: "qualityValidation",
        components: {
            validation_step_1,
            validation_step_2,
            validation_step_3,
            notAllowed
        },
        data() {
            return {
                notAvaliableDevices: false,
                notAllowedDevices: false,
                steps: 3,
                step: 1
            };
        },
        computed: {
            isErorrGettingMedia() {
                return this.notAvaliableDevices || this.notAllowedDevices;
            }

        },
        methods: {
            ...mapActions(['updateTestDialogState']),
            checkPermission() {
                let self = this;
                navigator.mediaDevices.getUserMedia({audio: true, video: true})
                         .then(stream => {
                                   console.log('got permissins');
                               },
                               //handle all error types in catch
                         ).catch((err) => {
                    console.log(err.name + ": " + err.message, err);
                    if(err.name === 'NotAllowedError') {
                        console.log('please grant permission Unable to access device.');
                        self.notAllowedDevices = true;
                        self.updateTestDialogState(true);
                    } else if(err.name === 'NotFoundError') {
                        self.notAvaliableDevices = true;
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
        /*rtl:begin:ignore*/
        direction: ltr;
        .step-indicator{
            .v-stepper__step__step{
                margin-right: 8px/*rtl:ignore*/;
            }
        }
        .device-error-header {
            display: flex;
            align-items: center;
            justify-content: flex-start;
            height: 32px;
            background-color: #ff5a5a;
            font-size: 14px;
            font-weight: 600;
            color: rgba(255, 255, 255, 0.87);
            span {
                line-height: 1;
            }

        }
        .blue-btn {
            border-radius: 4px;
            padding: 10px 16px;
            text-transform: uppercase;
            box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.16);
            background-color: #4452fc;
            font-size: 12px;
            font-weight: 600;
            letter-spacing: 0.5px;
            color: rgba(255, 255, 255, 0.87);
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
        /*rtl:end:ignore*/
    }

</style>