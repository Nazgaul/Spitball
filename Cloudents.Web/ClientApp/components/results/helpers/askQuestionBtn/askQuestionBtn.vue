<template>
    <v-flex :class="['result-cell', $vuetify.breakpoint.smAndUp ? 'mb-3': '', 'ask-action-card', 'xs-12', isFloatingBtn ? 'floatingcard' : 'regularCard']">
        <a class="mb-5 ask-link">
            <div :class="['ask-wrap', isFloatingBtn ? 'floating-ask' : '']">
                <div class="static-center">
                    <button round
                            :class="['ask-btn',  isFloatingBtn ? 'rounded-floating-button' : '', {'raised': raiseFloatingButtonPosition}]"
                            @click="goToAskQuestion()">
                        <v-icon class="sb-edit-icon">sbf-edit-icon</v-icon>
                        <span class="btn-text" v-language:inner>faqBlock_add_question_btn</span>
                    </button>
                </div>
            </div>
        </a>
    </v-flex>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';

    export default {
        data() {
            return {
                offsetTop: 0,
                offsetTop2: 0,
            };
        },
        props: {
            isAsk: {
                type: Boolean,
                default: false,
                required: false
            }
        },
        computed: {
            ...mapGetters(['accountUser', 'loginDialogState', 'getCookieAccepted']),

            isFloatingBtn() {
                if(this.$vuetify.breakpoint.smAndDown) {
                    return true;
                } else {
                    return false;
                }

                return this.offsetTop2 >= offHeight && (this.$vuetify.breakpoint.smAndDown);
            },
            raiseFloatingButtonPosition() {
                return !this.getCookieAccepted;
            }
        },
        methods: {
            ...mapActions([
                              "updateLoginDialogState",
                              'updateNewQuestionDialogState',
                              'updateUserProfileData'
                          ]),
            goToAskQuestion() {
                if(this.accountUser == null) {
                    this.updateLoginDialogState(true);
                    //set user profile
                    this.updateUserProfileData('profileHWH');
                } else {
                    //ab test original do not delete
                    this.updateNewQuestionDialogState(true);
                }
            },
            transformToBtn() {
                this.offsetTop2 = window.pageYOffset || document.documentElement.scrollTop;
            },
        },

        beforeMount: function () {
            if(window) {
                window.addEventListener('scroll', this.transformToBtn);
            }
        },
        beforeDestroy: function () {
            if(window) {
                window.removeEventListener('scroll', this.transformToBtn);
            }
        }
    };
</script>

<style lang="less">
    @import "../../../../styles/mixin.less";
    @import "../../../../styles/animation.less";

    .ask-action-card {
        right: 0;
        top: 0;
        //z-index: 999; @gaby: why do we need z-index here??? (button appears above the menu is small screens)
        border-radius: 4px;
        &.floatingcard {
            right: 0;
            top: 25px;
            border-radius: 50%;
            background-color: transparent;
            border: 0;
        }
        @media (max-width: @screen-xs) {
            z-index: 4;
            width: 100%;
            right: 0;
            border: none;
            order: 2;
        }
        .ask-wrap {
            &.floating-ask {
                border-radius: 0;
                background-color: transparent;
            }
            .static-center {
                padding: 0 0 8px 0;
                visibility: visible;
                display: flex;
                flex-direction: row;
                align-items: center;
                justify-content: space-around;
                @media (max-width: @screen-xs) {
                    flex-direction: column;
                    background-color: transparent;
                    padding: 0;
                }
                .ask-text {
                    color: @color-blue-new;
                    
                    font-size: 18px;
                    font-weight: 600;
                    margin-bottom: 0;
                    text-transform: capitalize;
                    &.hidden-text {
                    }
                    @media (max-width: @screen-xs) {
                        font-size: 26px;
                        font-weight: 600;
                        margin-bottom: 14px;
                        text-transform: none;
                    }
                }
                .ask-btn {
                    .sb-rounded-medium-btn(@color-blue-new, @colorWhiteBlured, @color-white);
                    @media (max-width: @screen-xs) {
                        .sb-rounded-btn(@color-blue-new, @colorWhiteBlured, @color-white);
                        margin-top: 0;
                        margin-bottom: 0;
                    }

                    &:not(.rounded-floating-button) {
                        -webkit-animation: fadeIn 0.8s;
                        animation: fadeIn 0.8s;
                        -moz-animation: fadeIn 0.8s;
                        -o-animation: fadeIn 0.8s;
                        //margin-right: 30px;
                    }
                    &.rounded-floating-button {
                        animation: sb_bounce_animation 1000ms linear both;

                        /*-webkit-animation: fade 0.8s;*/
                        /*animation: fade 0.8s;*/
                        /*-moz-animation: fade 0.8s;*/
                        /*-o-animation: fade 0.8s;*/
                        box-shadow: 0 2px 9px 0 rgba(0, 0, 0, 0.36);
                        position: fixed;
                        right: 25px;
                        bottom: 60px;
                        background-color: @color-blue-new;
                        border-radius: 50%;
                        height: 56px;
                        width: 56px;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        padding: 0 12px;
                        letter-spacing: -0.4px;
                        color: rgba(255, 255, 255, 0.81);
                        .v-icon {
                            color: @color-white;
                            margin-right: 0;
                            height: 22px;
                            font-size: 20px;
                            opacity: 1;
                        }
                        .btn-text {
                            opacity: 0;
                            position: absolute;
                            font-size: 0px;
                            visibility: hidden;
                        }
                        &.raised {
                            bottom: 65px;
                        }
                    }
                }
            }
        }
    }
</style>