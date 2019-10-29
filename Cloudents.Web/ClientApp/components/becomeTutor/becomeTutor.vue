<template>
    <div class="become-tutor-wrap d-flex" >
        <v-stepper v-model="currentStep" class="elevation-0 stepper" :class="{'back-image': isLastStep}">
            <v-layout align-center justify-center class="become-header" v-show="!isLastStep">
                <v-flex xs12 sm12  class="text-xs-center">
                    <v-icon class="face-icon mr-2">sbf-face-icon</v-icon>
                    <span class="become-title" v-language:inner>becomeTutor_title_become</span>
                </v-flex>
            </v-layout>
            <v-stepper-header class="sb-box" v-show="!isLastStep" v-if="$vuetify.breakpoint.smAndUp">
                <v-stepper-step class="step-control justify-center"
                                color="#4452FC"
                                :complete="currentStep > 1"
                                :complete-icon="'sbf-checkmark'" step="1">
                    <span v-language:inner>becomeTutor_personal_details</span>
                </v-stepper-step>
                <v-stepper-step class="step-control justify-center who" color="#4452FC"
                                :complete="currentStep > 2"
                                :complete-icon="'sbf-checkmark'" step="2">
                    <span v-language:inner>becomeTutor_who</span>
                </v-stepper-step>
                <v-stepper-step class="step-control justify-center" color="#4452FC"
                                :complete="currentStep > 3"
                                :complete-icon="'sbf-checkmark'" step="3">
                    <span v-language:inner>becomeTutor_calendar</span>
                </v-stepper-step>
            </v-stepper-header>
            <v-stepper-items>
                <v-stepper-content v-for="n in steps" 
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
    import step_3 from './helpers/calendarStep.vue'
    import step_4 from './helpers/confirmationStep.vue'
    import step_5 from './helpers/finalStep.vue';

    export default {
        name: "becomeTutor",
        components: {step_1, step_2, step_3,step_4,step_5},
        data() {
            return {
                steps: 5,
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
            border-radius: 6px;
            box-shadow: 0 3px 13px 0 rgba(0, 0, 0, 0.22);
        }
    }

    .become-tutor-wrap {
        width: 100%;
        .stepper{
            overflow-y: auto;
            @media(max-width: @screen-xs){
                display: flex;
                flex-direction: column;
                justify-content: space-between;
            }
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
            border: solid 1px @global-blue;
            color: @global-blue;
        }
          
        .step-control {
            padding: 0 20px;
            width: 33%;
            justify-content: start;
            @media(max-width: @screen-xs){
                padding: 0 18px;
                justify-content: center;
            }
            @media(max-width: @screen-xss){
                padding: 0 6px;
                justify-content: center;
            }

        }
        .who{
            @media(max-width: @screen-xs){
                padding: 0 24px;
            }
        }
        .white-text {
            color: @color-white;
        }
        .become-header {
            height: 56px;
            border-bottom: solid 1px #dddddd;

            @media(max-width: @screen-xs){
                max-height: 54px;
            }
        }
        .v-stepper__step{
            @media(max-width: @screen-xs){
                flex-direction: column;
                justify-content: start;
                margin-top: 8px;
            }      
            
        }
        .v-stepper__step--active {
            border-bottom: solid 6px @global-blue;
        }

        .v-stepper__items{
            @media(max-width: @screen-xs){
                height: 100%;
                overflow: auto;
                
                .v-stepper__content{
                    padding: 18px 16px 0px;
                    height: 100%;
                    .v-stepper__wrapper{
                       height: 100%;
                       overflow: initial;
                    }
                }
            } 
        }
        
        
        .sb-box {
            border-bottom: solid 1px #dddddd;
            height: 62px;
            box-shadow: none;
            @media(max-width: @screen-xs){
                height: 96px;
            }
        }
        .v-stepper__step__step{
            height: 30px;
            width: 30px;
            font-size: 16px;
        }
        .v-stepper__step--complete{
            .v-stepper__step__step{
                background-color: #2ec293 !important;
                .v-icon{
                    font-size: 24px;
                }
            }
        }

        .v-stepper:not(.v-stepper--vertical) .v-stepper__label{
            font-size: 16px;
            letter-spacing: -0.3px;
            color: @global-purple;
            display: flex;
            padding-left: 6px;
            @media(max-width: @screen-xs){
                padding-left: 0;
                font-size: 14px;
                text-align: center;
                padding-top: 8px;
            }
        }
        .become-title {
            color: @global-purple;
            font-size: 18px;
            font-weight: bold;
        }
        .face-icon {
            vertical-align: bottom;
            color: @global-purple;
            font-size: 20px;
            
        }
        .v-btn__content{
            padding: 0 20px;
        }

    }
</style>