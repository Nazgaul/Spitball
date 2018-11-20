<template>
    <v-flex :class="['result-cell', 'mb-3', 'ask-action-card', 'xs-12', isFloatingBtn ? 'floatingcard' : 'regularCard']">
        <a class="mb-5 ask-link" @click="goToAskQuestion()">
            <div :class="['ask-wrap', isFloatingBtn ? 'floating-ask' : '']">
                <div class="static-center">
                    <p v-show="$vuetify.breakpoint.smAndUp" :class="['ask-text',  isFloatingBtn ? 'hidden-text' : '']" v-language:inner>
                        faqBlock_ask
                    </p>
                    <button round :class="['ask-btn',  isFloatingBtn ? 'rounded-floating-button' : '']">
                        <span class="btn-text" v-language:inner>faqBlock_add_question_btn</span>
                        <v-icon class="sb-edit-icon" right>sbf-edit-icon</v-icon>
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
            }
        },
        props: {
            isAsk: {
                type: Boolean,
                default: false,
                required: false
            }
        },
        computed: {
            ...mapGetters(['accountUser', 'loginDialogState',  'showRegistrationBanner']),

            isFloatingBtn() {
                let offHeight = 0;
                if(!!this.showRegistrationBanner){
                    offHeight = 150; // header + banner + mobile filter btn
                }else{
                    offHeight = 150; //  header + mobile filter btn
                }
                return this.offsetTop2 >= offHeight && (this.$vuetify.breakpoint.smAndDown)
            },
        },
        methods: {
            ...mapActions([
                "updateLoginDialogState",
                'updateNewQuestionDialogState',
                'updateUserProfileData'
            ]),
            goToAskQuestion(){
                if(this.accountUser == null){
                    this.updateLoginDialogState(true);
                    //set user profile
                    this.updateUserProfileData('profileHWH')
                }else{
                    //ab test original do not delete
                    this.updateNewQuestionDialogState(true)
                }
            },
            transformToBtn() {
                this.offsetTop2 = window.pageYOffset || document.documentElement.scrollTop;
            },
        },

        beforeMount: function () {
            if (window) {
                window.addEventListener('scroll', this.transformToBtn)
            }
        },
        beforeDestroy: function () {
            if (window) {
                window.removeEventListener('scroll', this.transformToBtn)
            }
        }
    }
</script>

<style lang="less">
    @import "../../../../styles/mixin.less";

    .ask-action-card{
        @-webkit-keyframes fade {
            0% {opacity: 0.25}
            25% {opacity: 0.5}
            50% {opacity: 0.75}
            100% {opacity: 1}
        }

        @-moz-keyframes fade{
            0% {opacity: 0.25}
            25% {opacity: 0.5}
            50% {opacity: 0.75}
            100% {opacity:1}
        }

        @keyframes fade {
            0% {opacity: 0.25}
            25% {opacity: 0.5}
            50% {opacity: 0.75}
            100%{opacity: 1}
        }
        @-o-keyframes fade {
            0% {opacity: 0.25}
            25% {opacity: 0.5}
            50% {opacity: 0.75}
            100%{opacity: 1}

        }

        @-webkit-keyframes fadeIn {
            0% {opacity: 0.2}
            50% {opacity: 0.75}
            100% {opacity:1}
        }

        @-moz-keyframes fadeIn{
            0% {opacity: 0.2}
            50% {opacity: 0.75}
            100% {opacity:1}
        }

        @keyframes fadeIn {
            0% {opacity: 0.2}
            50% {opacity: 0.75}
            100% {opacity:1}
        }
        @-o-keyframes fadeIn {
            0% {opacity: 0.2}
            50% {opacity: 0.75}
            100% {opacity:1}

        }
        right: 0;
        top: 0;
        border: dashed 1px rgba(68, 82, 252, 0.43);
        z-index: 999;
        border-radius: 4px;
        &.floatingcard {
            right: 0px;
            top: 25px;
            border-radius: 50%;
            background-color: transparent;
            border: 0;
        }
        @media (max-width: @screen-xs) {
            z-index: 9;
            width: 100%;
            right: 0;
            top: 16px;
        }
        .ask-wrap {
            &.floating-ask {
                border-radius: 0%;
                background-color: transparent;
            }
            .static-center {
                background-color: @color-AB-test-background;
                padding: 16px 0;
                visibility: visible;
                display: flex;
                flex-direction: row;
                align-items: center;
                justify-content: space-around;
                @media (max-width: @screen-xs) {
                    flex-direction: column;
                }
                .ask-text {
                    color: @color-blue-new;
                    font-family: @fontFiraSans;
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
                    background: @color-blue-new;
                    color: @color-white;
                    height: 48px;
                    border-radius: 30px;
                    box-shadow: 0 3px 8px 0 @color-blue-new;
                    font-size: 20px;
                    line-height: 1.25;
                    letter-spacing: -0.5px;
                    text-transform: initial;
                    width: 192px;
                    display: flex;
                    flex-direction: row;
                    justify-content: space-evenly;
                    align-items: center;
                    @media (max-width: @screen-xs) {
                        margin-top: 0;
                        margin-bottom: 0;
                    }
                    .sb-edit-icon {
                        color: @colorWhiteBlured;
                        font-size: 16px;
                        margin-top: 4px;
                    }
                    &:not(.rounded-floating-button){
                        -webkit-animation: fadeIn 0.8s;
                        animation: fadeIn 0.8s;
                        -moz-animation: fadeIn 0.8s;
                        -o-animation: fadeIn 0.8s;
                    }
                    &.rounded-floating-button {
                        -webkit-animation: fade 0.8s;
                        animation: fade 0.8s;
                        -moz-animation: fade 0.8s;
                        -o-animation: fade 0.8s;
                        box-shadow: 0 2px 9px 0 rgba(0, 0, 0, 0.36);
                        position: fixed;
                        right: 25px;
                        bottom: 25px;
                        background-color: @color-blue-new;
                        height: 56px;
                        width: 56px;
                        display: flex;
                        justify-content: space-around;
                        align-items: center;
                        padding: 0 12px;
                        letter-spacing: -0.4px;
                        color: rgba(255, 255, 255, 0.81);
                        .v-icon {
                            color: @color-white;
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
                    }
                }
            }
        }
    }
</style>