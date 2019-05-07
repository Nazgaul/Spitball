<template>
    <div class="become-tutor-wrap d-flex" >
        <v-stepper v-model="currentStep" class="elevation-0 stepper" :class="[isLastStep ?  'back-image' : '']">
            <v-layout align-center justify-center class="become-header" v-show="!isLastStep">
                <v-flex xs12 sm12 md12 class="text-xs-center mt-1">
                    <v-icon class="become-title face-icon mr-2">sbf-face-icon</v-icon>
                    <span class="subheading font-weight-bold become-title" v-language:inner>becomeTutor_title_become</span>
                </v-flex>
            </v-layout>
            <v-stepper-header class="sb-box" v-show="!isLastStep" :class="[$vuetify.breakpoint.smAndUp ? '' : '' ]">
                <v-stepper-step class="step-control justify-center"
                                color="#4452FC"
                                :complete="currentStep > 1"
                                :complete-icon="'sbf-checkmark'" step="1">
                    <span v-language:inner>becomeTutor_personal_details</span>
                </v-stepper-step>
                <v-stepper-step class="step-control justify-center" color="#4452FC"
                                :complete="currentStep > 2"
                                :complete-icon="'sbf-checkmark'" step="2">
                    <span v-language:inner>becomeTutor_who</span>
                </v-stepper-step>
            </v-stepper-header>
            <v-stepper-items>
                <v-stepper-content v-for="n in steps" class="mt-3"
                                   :key="`step_${n}`"
                                   :step="n">
                    <keep-alive>
                        <component :is="`step_${currentStep}`"></component>
                    </keep-alive>
                </v-stepper-content>
            </v-stepper-items>
        </v-stepper>
    </div>
</template>

<script>
    import step_1 from './helpers/firstStep.vue';
    import step_2 from './helpers/secondStep.vue';
    import step_3 from './helpers/finalStep.vue';

    export default {
        name: "becomeTutor",
        components: {step_1, step_2, step_3},
        data() {
            return {
                steps: 3,
                currentStep: 1,
            };
        },
        computed: {
            isLastStep() {
                return this.currentStep === this.steps;
            },
        },
        methods: {
            nextStep(n) {
                this.currentStep = n;
            },
        },
        created(){
            this.$root.$on('becomeTutorStep', (step) => {
                this.nextStep(step);
            });
        }

    };
</script>

<style lang="less">
    @import '../../styles/mixin.less';

    .v-dialog {
        &.become-tutor {
            border-radius: 4px;
            box-shadow: 0 3px 13px 0 rgba(0, 0, 0, 0.22);
        }
    }

    .become-tutor-wrap {
        width: 100%;
        .stepper{
            overflow-y: auto;
        }
        .theme--light.v-btn.v-btn--disabled:not(.v-btn--icon):not(.v-btn--flat):not(.v-btn--outline){
            background-color: rgba(68, 82, 252, 0.5)!important; //vuetify overwrite
            color: @color-white!important;
        }
        .back-image{
            background-image: url('./images/conffeti_desktop.png')!important;
            background-repeat: no-repeat;
            background-position: top;
            background-size: contain;
            @media(max-width: @screen-xs){
                background-image: url('./images/conffeti_mobile.png');

            }
        }
        .cancel-btn {
            border: solid 1px @colorBlue;
            color: @colorBlue;
        }
        .step-control {
            padding: 0 80px;
            width: 50%;
            @media(max-width: @screen-xs){
                padding: 0 24px;
            }
        }
        .white-text {
            color: @color-white;
        }
        .become-header {
            background-color: @systemBackgroundColor;
            padding:6px 0;
        }
        .v-stepper__step--active {
            border-bottom: solid 2px @colorBlue;
        }
        .sb-box {
            box-shadow: 0 3px 6px 0 rgba(0, 0, 0, 0.16);
            height:  48px;
        }
        .v-stepper:not(.v-stepper--vertical) .v-stepper__label{
            display: flex;
            padding-left: 12px;
            @media(max-width: @screen-xs){
                padding-left: 6px;
            }
        }
        .become-title {
            color: @profileTextColor;
        }
        .face-icon {
            vertical-align: middle;
        }

    }
</style>