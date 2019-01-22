<template>
    <div class="onboard-component">
        <div class="back-img"></div>
        <div class="guide-container">
            <v-stepper v-model="currentStep">
                <v-stepper-header class="elevation-0">
                </v-stepper-header>
                <v-stepper-items>
                    <v-stepper-content
                            v-for="n in steps"
                            :key="`${n}-content`"
                            :step="n">
                        <img :src="require(`${imgSrc}`)" alt="">
                    </v-stepper-content>

                </v-stepper-items>
                <div class="progress-background">
                    <div class="progress-wrap">
                        <v-btn class="btn sb-btn-flat close" :class="{'visibility-hidden' : isFinished}"
                               @click="closeGuide()">Skip
                        </v-btn>
                        <v-stepper-step
                                :complete-icon="''"
                                :color="'#5158af'"
                                v-for="n in steps"
                                :class="[currentStep === n ? 'active-step-progress' : 'inactive-step']"
                                step=""></v-stepper-step>
                        <v-btn class="btn sb-btn-flat continue" v-show="!isFinished" @click="nextStep()">Continue
                        </v-btn>
                        <v-btn class="btn sb-btn-flat finish" v-show="isFinished" @click="nextStep()">Finish</v-btn>
                    </div>
                </div>

            </v-stepper>

        </div>
    </div>
</template>

<script>
    import { mapGetters, mapActions } from "vuex";

    export default {
        name: "onBoardGuide",
        data() {
            return {
                currentStep: 1,
                steps: 4,
                desktop: {
                    hebrew: {
                        1: './images/mobile_Hebrew_LogedIn.png',
                        2: './images/mobile_Hebrew_LogedIn.png',
                        3: './images/mobile_Hebrew_LogedIn.png',
                        4: './images/mobile_Hebrew_LogedIn.png',

                    },
                    english: {
                        1: './images/desktop_English_LogedIn.png',
                        2: './images/desktop_English_LogedIn.png',
                        3: './images/mobile_Hebrew_LogedIn.png',
                        4: './images/mobile_Hebrew_LogedIn.png',
                    }
                },
                mobile: {
                    hebrew: {
                        1: './images/mobile_Hebrew_LogedIn.png',
                        2: './images/mobile_Hebrew_LogedIn.png',
                        3: './images/mobile_Hebrew_LogedIn.png',
                        4: './images/mobile_Hebrew_LogedIn.png',

                    },
                    english: {
                        1: './images/mobile_Hebrew_LogedIn.png',
                        2: './images/desktop_English_LogedIn.png',
                        3: './images/mobile_Hebrew_LogedIn.png',
                        4: './images/mobile_Hebrew_LogedIn.png',
                    }
                }
            }
        },
        computed: {
            ...mapGetters([]),
            isFinished() {
                if (this.currentStep === this.steps) {
                    return true
                }
            },
            isMobile() {
                return this.$vuetify.breakpoint.xsOnly
            },
            isIsrael() {
                return global.isIsrael;
            },
            imgSrc() {
                let imageSrc = '';
                let imagesSet = this.$vuetify.breakpoint.xsOnly ? this.mobile : this.desktop;
                imagesSet = this.isIsrael ? imagesSet.hebrew : imagesSet.english;
                return imagesSet[this.currentStep]
            },
        },
        methods: {
            ...mapActions(['updateOnBoardState']),
            nextStep() {
                if (this.currentStep === this.steps) {
                    return this.currentStep
                } else {
                    this.currentStep = this.currentStep + 1;
                }
                console.log('after', this.currentStep);
            },
            closeGuide() {
                this.updateOnBoardState(false);
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
        }
        //End predefined sets
        position: relative;
        width: 100%;
        display: flex;
        margin: 0 auto;
        justify-content: center;
        height: 100%;

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
            }
            .close {
                margin-right: auto;
            }
            .continue, .finish {
                margin-left: auto;
            }
            .active-step-progress, .inactive-step {
                padding: 8px;
            }
            .active-step-progress {
                span {
                    width: 26px;
                    height: 26px;
                    border-radius: 26px;
                    background: @purpleOnBoard !important;
                }
            }
            .inactive-step {
                span {
                    width: 16px;
                    height: 16px;
                    min-width: 16px;
                    border-radius: 16px;
                    background-color: #d8d8d8;
                }

            }
        }
    }
</style>