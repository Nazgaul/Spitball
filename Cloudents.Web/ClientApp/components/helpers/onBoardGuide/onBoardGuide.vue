<template>
    <div class="onboard-component">
        <div class="back-img"></div>
        <div class="guide-container">
            <v-stepper v-model="currentStep" class="on-board-stepper"
            >
                <!--<v-stepper-items class="step-items"-->
                                 <!--:class="{'last-step': isFinished}"-->
                                 <!--:style="[isFinished ? { 'background-image': 'url(' + require(`${imgSrc}`) + ')'} : '']"-->
                <!--&gt;-->
                <v-stepper-items class="step-items"
                                 :class="{'last-step': isFinished}"

                >
                    <v-stepper-content
                            v-for="n in steps"
                            :key="`${n}-content`"
                            :class="[isFinished && n === steps ? 'last-step-content' : 'step-content' ]"
                            :step="n">
                        <component  :is="`onBoard_step_${n}`"></component>
                    </v-stepper-content>

                </v-stepper-items>
                <div class="progress-background" :class="{'background-purple': isFinished}">
                    <div class="progress-wrap">
                        <v-btn class="btn sb-btn-flat close elevation-0" :class="{'visibility-hidden' : isFinished}"
                               @click="skipSteps()">
                            <span v-language:inner>onboard_btn_skip</span>
                        </v-btn>
                        <div class="steps-circle-wrap d-flex">
                            <v-stepper-step
                                    :complete-icon="''"
                                    :color="'#5158af'"
                                    v-for="n in steps"
                                    v-if="!$vuetify.breakpoint.xsOnly || !isFinished"
                                    :class="[currentStep === n ? 'active-step-progress' : 'inactive-step']"
                                    step=""></v-stepper-step>
                        </div>
                        <div class="actions-wrap">
                            <v-btn class="btn sb-btn-flat continue elevation-0" v-show="!isFinished"
                                   @click="nextStep()">
                                <span v-language:inner>onboard_btn_continue</span>
                            </v-btn>
                            <v-btn class="btn sb-btn-flat finish elevation-0" v-show="isFinished" @click="closeGuide()">
                                <span v-language:inner>onboard_btn_finish</span>
                            </v-btn>
                        </div>
                    </div>
                </div>

            </v-stepper>

        </div>
    </div>
</template>

<script>
    import { mapGetters, mapActions } from "vuex";
    import analyticsService from '../../../services/analytics.service';
    import onBoard_step_1 from "./onBoardSteps/welcomeStep.vue";
    import onBoard_step_2 from "./onBoardSteps/learnStep.vue";
    import onBoard_step_3 from "./onBoardSteps/earnStep.vue";
    import onBoard_step_4 from "./onBoardSteps/onBoardFinal.vue";

    export default {
        name: "onBoardGuide",
        components: {
            onBoard_step_1,
            onBoard_step_2,
            onBoard_step_3,
            onBoard_step_4
        },
        data() {
            return {
                currentStep: 1,
                steps: 4,

            }
        },
        computed: {
            ...mapGetters([]),
            activeStep(){

            },

            isFinished() {
                if (this.currentStep === this.steps) {
                    return true
                }
            },
            isMobile() {
                return this.$vuetify.breakpoint.xsOnly
            },
            isHebrew() {
                return global.lang.toLowerCase() === 'he';
            },
            imgSrc() {
                let imageSrc = '';
                let imagesSet = this.$vuetify.breakpoint.xsOnly ? this.mobile : this.desktop;
                imagesSet = this.isHebrew ? imagesSet.hebrew : imagesSet.english;
                return imagesSet[this.currentStep]
            },
        },
        methods: {
            ...mapActions(['updateOnBoardState']),
            nextStep() {
                analyticsService.sb_unitedEvent('WALKTHROUGH', `${this.currentStep}_CONTINUE`);
                if (this.currentStep === this.steps) {
                    return this.currentStep
                } else {
                    this.currentStep = this.currentStep + 1;
                }
            },
            closeGuide() {
                analyticsService.sb_unitedEvent('WALKTHROUGH', 'CLOSE');
                global.localStorage.setItem("sb-onboard-supressed", true);
                this.updateOnBoardState(false);
            },
            skipSteps() {
                analyticsService.sb_unitedEvent('WALKTHROUGH', 'SKIP_STEPS');
                this.currentStep = this.steps;
            }
        },
    }
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    @purpleOnBoard: #5158af;

    .onboard-component {
        //Start predefined sets
        .visibility-hidden {
            visibility: hidden;
        }
        .sb-btn-flat {
            display: flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            min-width: 104px;
            max-width: 104px;
            color: @purpleOnBoard;
            padding: 0 20px;
            border-radius: 24px;
            text-transform: capitalize;
            border: 1px solid @purpleOnBoard;
            font-weight: 400;
            font-size: 16px;
            outline: none;
            letter-spacing: -.5px;
            background-color: transparent !important; //vuetify
            box-shadow: none;
        }
        //End predefined sets
        position: relative;
        width: 100%;
        display: flex;
        margin: 0 auto;
        justify-content: center;
        height: 100%;
        .step-image {
            height: auto;
            width: 100%;
        }
        .last-step-content {
            height: 100%;
            .v-stepper__wrapper {
                height: 100%;
            }
        }
        .step-content {
            padding: 16px 24px 16px 24px;
            height:100%;
            @media(max-width: @screen-xs){
                padding: 50px 24px 16px 24px;
                height:unset;
            }
            .v-stepper__wrapper {
                display: flex;
                flex-direction: column;
                align-items: center;
                justify-content: center;
                height:100%;
                img{
                    height:100%;
                }
            }
        }
        .on-board-stepper {
            max-height: 100%;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            height: 100vh;
            @media (max-width: @screen-xs) {
                min-height: 100%;
                //can delete after background image replaced to be elements
            }
        }
        .step-items {
            height: 100%;
            background-size: contain;
            background-position: 50%;
            background-repeat: no-repeat;
            @media (max-width: @screen-xs) {
                background-size: cover;
                background-position: center;
            }
            &.last-step {
                background-size: cover;
                background-position: top;

            }
        }
        .guide-container {
            width: 100%;
            height: 100%;
            border-radius: 4px;
            box-shadow: 0 1px 17px 0 rgba(16, 27, 147, 0.39);
            background-color: @color-white;
            z-index: 9;
        }
        .progress-background {
            width: 100%;
            background-color: #f9f9f9;
            border-radius: 0 0 12px 12px;
            @media (max-width: @screen-xs) {
                background-color: transparent;
                &.background-purple {
                    background-color: @purpleOnBoard;
                    border-radius: 0;
                }
            }

        }
        .progress-wrap {
            display: flex;
            flex-direction: row;
            align-items: center;
            justify-content: center;
            width: 60%;
            margin: 0 auto;
            padding: 32px;
            @media (max-width: @screen-xs) {
                width: 100%;
                flex-direction: column;
                padding: 12px;
            }
            .btn {
                &.close {
                    margin-right: auto;
                    @media (max-width: @screen-xs) {
                        position: absolute;
                        top: 0;
                        right: 0;
                        border: none;
                        background-color: transparent;
                        box-shadow: none;
                        font-size: 16px;
                        letter-spacing: -0.3px;
                        color: rgba(0, 0, 0, 0.38);
                    }
                }
            }
            .actions-wrap {
                margin-left: auto;
                @media (max-width: @screen-xs) {
                    /*margin: 0 auto;*/
                    margin: unset;
                }
            }
            .continue, .finish {
                margin-left: auto;
            }
            .continue{
                @media (max-width: @screen-xs) {
                    /*margin: 0 auto;*/
                    margin: unset;
                }
            }
            .finish {
                @media (max-width: @screen-xs) {
                    color: @color-white;
                    border: 1px solid @color-white;
                }
            }
            .active-step-progress, .inactive-step {
                padding: 8px;
            }
            .active-step-progress {
                span {
                    width: 16px;
                    height: 16px;
                    min-width: 16px;
                    border-radius: 16px;
                    background: @purpleOnBoard !important;
                }
            }
            .inactive-step {
                span {
                    width: 8px;
                    height: 8px;
                    min-width: 8px;
                    border-radius: 8px;
                    background-color: #d8d8d8;
                }

            }
        }
    }
</style>