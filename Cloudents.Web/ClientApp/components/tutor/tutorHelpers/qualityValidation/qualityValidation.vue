<template>
    <v-stepper v-model="step">
        <v-stepper-header v-if="!isErorrGettingMedia">
            <v-stepper-step :complete-icon="'sbf-checkmark'" :complete="step > 1" step="1">Audio Input Test
            </v-stepper-step>
            <v-divider></v-divider>
            <v-stepper-step :complete-icon="'sbf-checkmark'" :complete="step > 2" step="2">Audio Output Test
            </v-stepper-step>
            <v-divider></v-divider>

            <v-stepper-step :complete-icon="'sbf-checkmark'" step="3">Video Test</v-stepper-step>
        </v-stepper-header>

        <v-stepper-items>
            <v-stepper-content v-for="n in steps"
                               :key="`${n}-content`"
                               :step="n">
                <v-card
                        class="mb-5 elevation-0"
                        color="grey lighten-1"
                        max-height="450px">
                    <!--if there is an error requesting user media show helper component-->
                    <not-allowed v-if="isErorrGettingMedia" :isNotFound="notAvaliableDevices"></not-allowed>
                    <!--in case devices are avaliable show regular test steps-->
                    <component  :is="'validation_step_'+step" v-if="n === step && !isErorrGettingMedia"></component>

                </v-card>
                <v-btn v-if="step !== steps && !isErorrGettingMedia"
                       color="primary"
                       @click="nextStep(n)"
                >
                    Continue
                </v-btn>
                <v-btn v-if="isErorrGettingMedia"
                       color="primary"
                       @click="closeDialog(n)"
                >
                    Got it
                </v-btn>

                <v-btn flat v-if="step === steps" @click="closeDialog()">Done</v-btn>
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
        components: {validation_step_1, validation_step_2, validation_step_3, notAllowed},
        data() {
            return {
                notAvaliableDevices: false,
                notAllowedDevices: false,
                steps: 3,
                step: 1
            }
        },
        computed: {
            isErorrGettingMedia() {
                return this.notAvaliableDevices || this.notAllowedDevices
            }
        },
        methods: {
            ...mapActions(['updateTestDialogState']),
            checkPermission() {
                let self =this;
                navigator.mediaDevices.getUserMedia({audio: true, video: true})
                    .then(stream => {
                            console.log('got permissins')
                        },
                    //handle all error types in catch
                    ).catch((err) => {
                    console.log(err.name + ": " + err.message, err);
                    if (err.name  === 'NotAllowedError') {
                        console.log('please grant permission Unable to access device.')
                        self.notAllowedDevices = true;
                        self.updateTestDialogState(true)
                    }else if(err.name === 'NotFoundError'){
                        self.notAvaliableDevices = true;
                        self.updateTestDialogState(true)
                    }
                }); // always check for errors at the end.
            },
            nextStep(n) {
                this.step = n + 1
            },
            closeDialog() {
                this.step = 1;
                this.updateTestDialogState(false);
            }
        },
        created() {
            this.checkPermission()
        }
    }
</script>

<style scoped>

</style>