<template>
    <v-stepper v-model="step">
        <v-stepper-header>
            <v-stepper-step :complete-icon="'sbf-checkmark'" :complete="step > 1" step="1">Audio Input Test</v-stepper-step>
            <v-divider></v-divider>
            <v-stepper-step  :complete-icon="'sbf-checkmark'"  :complete="step > 2" step="2">Audio Output Test</v-stepper-step>
            <v-divider></v-divider>

            <v-stepper-step :complete-icon="'sbf-checkmark'"  step="3">Video Test</v-stepper-step>
        </v-stepper-header>

        <v-stepper-items>
            <v-stepper-content  v-for="n in steps"
                                :key="`${n}-content`"
                                :step="n">
                <v-card
                        class="mb-5 elevation-0"
                        color="grey lighten-1"
                        max-height="450px"
                >
                    <component :is="'validation_step_'+step" v-if="n === step"></component>

                </v-card>

                <v-btn v-if="step !== steps"
                        color="primary"
                        @click="nextStep(n)"
                >
                    Continue
                </v-btn>

                <v-btn flat v-if="step === steps" @click="closeDialog()">Done</v-btn>
            </v-stepper-content>
        </v-stepper-items>
    </v-stepper>
</template>

<script>
    import {mapActions} from 'vuex';
    import validation_step_1 from './qualitySteps/audioInputValidation.vue';
    import validation_step_2 from './qualitySteps/audioOutputValidation.vue';
    import validation_step_3 from './qualitySteps/videoValidation.vue';

    export default {
        name: "qualityValidation",
        components: {validation_step_1, validation_step_2, validation_step_3},
        data() {
            return {
                steps: 3,
                step: 1
            }
        },
        methods: {
            ...mapActions(['updateTestDialogState']),
            nextStep(n) {
                this.step =  n+1
            },
            closeDialog(){
                this.step = 1;
                this.updateTestDialogState(false);

            }
        },
    }
</script>

<style scoped>

</style>