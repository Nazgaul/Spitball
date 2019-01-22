<template>
        <div class="onboard-component">
            <div class="back-img"></div>
            <div  class="close-container">
                <v-icon>sbf-close</v-icon>
            </div>
            <div  class="guide-container">
                <v-stepper v-model="currentStep">
                    <v-stepper-header class="elevation-0">
                    </v-stepper-header>

                    <v-stepper-items>
                        <v-stepper-content step="1">
                            <v-card elevation="0"
                                    class="mb-5"
                                    color="grey lighten-1"
                                    height="200px"
                            ></v-card>
                        </v-stepper-content>

                        <v-stepper-content step="2">
                            <v-card elevation="0"
                                    class="mb-5"
                                    color="grey lighten-1"
                                    height="200px"
                            ></v-card>

                        </v-stepper-content>

                        <v-stepper-content step="3">
                            <v-card elevation="0"
                                    class="mb-5"
                                    color="grey lighten-1"
                                    height="200px"
                            ></v-card>
                        </v-stepper-content>
                    </v-stepper-items>
                    <div class="progress-wrap">
                        <v-btn @click="previousStep()">Continue</v-btn>

                        <v-stepper-step :complete="currentStep > 1" step="1"></v-stepper-step>

                        <v-stepper-step :complete="currentStep > 2" step="2"></v-stepper-step>

                        <v-stepper-step step="3"></v-stepper-step>


                        <v-btn @click="nextStep()" flat>Cancel</v-btn>
                    </div>

                </v-stepper>

            </div>
        </div>
</template>

<script>
    export default {
        name: "onBoardGuide",
        data() {
            return {
                currentStep: 0
            }
        },
        methods: {
            nextStep() {
                    return this.currentStep + 1;
            },
            previousStep(){
                return this.currentStep  - 1;
            }
        },
    }
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
    .onboard-component{
        position: relative;
        width: 100%;
        display: flex;
        margin: 0 auto;
        justify-content: center;
        height: 100%;
    }
    .guide-container {
        width: 100%;
        height: 100%;
        //opacity: 0.91;
        border-radius: 4px;
        box-shadow: 0 1px 17px 0 rgba(16, 27, 147, 0.39);
        //background-image: linear-gradient(140deg, #4b8ffe, #4452fc);
        background-color: @color-white;
        z-index: 9;
    }
    .progress-wrap{
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: center;
    }
</style>