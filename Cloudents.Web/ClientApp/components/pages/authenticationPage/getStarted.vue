<template>
    <div class="getStartedContainer text-center">
        <div class="getStartedActions">
            <div class="getStartedTop">
                <p class="getStartedTitle" v-language:inner="isRegisterPath? 'loginRegister_getstarted_title':'loginRegister_getstarted_title_login'"></p>
                <span class="getStartedSubtitle" v-language:inner="isRegisterPath? 'loginRegister_getstarted_subtitle': '' "></span>
            </div>
            <div class="getStartedForm">
                <div v-if="isRegisterPath" class="getStartedTerms">
                    <div class="lineTerms">
                        <v-checkbox
                            v-model="isTermsAgree"
                            @click="checkBoxConfirm"
                            class="checkboxUserinfo"
                            :ripple="false"
                            name="checkBox"
                            sel="check"
                            off-icon="sbf-check-box-un"
                            on-icon="sbf-check-box-done"
                            id="checkBox"
                        >
                        </v-checkbox>
                        <label for="checkBox">
                            <span>
                                <span class="paddingHelper" v-language:inner="'loginRegister_getstarted_terms_i_agree'"></span>
                                <a :href="isFrymo ? 'https://help.frymo.com/en/article/terms' : 'https://help.spitball.co/en/article/terms-of-service'" class="terms paddingHelper" v-language:inner="'loginRegister_getstarted_terms_terms'"></a>
                                    <span class="paddingHelper" v-language:inner="'loginRegister_getstarted_terms_and'"></span>
                                <a :href="isFrymo ? 'https://help.frymo.com/en/policies' : 'https://help.spitball.co/en/article/privacy-policy'" class="terms" v-language:inner="'loginRegister_getstarted_terms_privacy'"></a>
                            </span>
                        </label>
                    </div>
                    <span v-if="isError" class="errorMsg" v-language:inner="'login_please_agree'"></span>
                </div>
                
                <v-btn 
                    @click="goWithGoogle()"
                    depressed
                    :loading="googleLoading"
                    large rounded
                    sel="gmail"
                    block
                    class="google btn-login"
                >
                        <img src="./images/G icon@2x.png" />
                        <span class="btnText" v-language:inner="isRegisterPath? 'loginRegister_getstarted_btn_google_signup':'loginRegister_getstarted_btn_google_signin'"></span>
                </v-btn>

                <span v-if="gmailError" class="errorMsg">{{gmailError}}</span>

                <span class="or d-none d-sm-flex" v-language:inner="'loginRegister_getstarted_or'"></span>

                <v-btn 
                    @click="goWithEmail()"
                    class="email"
                    large
                    text
                    rounded
                    block
                    sel="email"
                >
                    <img src="./images/np_email@2x.png">
                    <span class="btnText" v-language:inner="isRegisterPath? 'loginRegister_getstarted_btn_email_signup' : 'loginRegister_getstarted_btn_email_signin'"></span>
                </v-btn>
            </div>
        </div>
        <div class="getStartedBottom">
            <span class="needAccount" v-language:inner="isRegisterPath ? 'loginRegister_getstarted_signin_text' : 'loginRegister_getstarted_signup_text'"></span>
            &nbsp;
            <router-link
                :to="{name: isRegisterPath ? 'login' : 'register'}" 
                exact
                class="link" 
                v-language:inner="isRegisterPath ? 'loginRegister_getstarted_signin_link' : 'loginRegister_getstarted_signup_link'"
            >
            </router-link>
        </div>
        
    </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import insightService from '../../../services/insightService';

export default {
    data() {
        return {
            isTermsAgree: false,
            showError: false,
            googleLoading: false
        }
    },
    methods: {
        ...mapActions(['updateStep','googleSigning','gapiLoad']),
        checkBoxConfirm(){
            if(this.isTermsAgree){
                this.showError = false
            } else this.showError = true
        },
        goWithGoogle(){
            if(this.isRegisterPath && !this.isTermsAgree){
                this.showError = true;
                return;
            }
            this.googleLoading = true;
            this.googleSigning().then(() => {}, err => {
                insightService.track.event(insightService.EVENT_TYPES.ERROR, 'signInWithGoogle', err);
                this.googleLoading = false
            })
        },
        goWithEmail(){
            if(this.isRegisterPath){
                if(!this.isTermsAgree){
                    this.showError = true;
                } else  {
                    this.$router.push({name: 'setEmailPassword'})
                }
            } else {
                this.$router.push({name: 'setEmail'})
            }
        }
    },
    computed: {
        ...mapGetters(['getErrorMessages','isFrymo']),

        isError(){
            return !this.isTermsAgree && this.showError
        },
        gmailError(){
            return this.getErrorMessages.gmail
        },
        isRegisterPath(){
            return (this.$route.name === 'register')
        }
    },
    mounted() {
        let self = this;
        this.$nextTick(function () {
            this.$loadScript("https://apis.google.com/js/client:platform.js").then(()=>{
                self.gapiLoad();
                // gapi.load('auth2', function () {
                //     auth2 = gapi.auth2.init({
                //     client_id: global.client_id,
                //     });
                // });
            })
        });
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
@import '../../../styles/colors.less';

.getStartedContainer{
    .getStartedActions{
        .getStartedTop {
            padding-bottom: 64px;
            @media (max-width: @screen-xs) {
                margin-top: 42px;
            }
            .getStartedTitle {
                .responsive-property(font-size, 28px, null, 22px);
                .responsive-property(line-height, 1.54, null, 1.95);
                color: @color-login-text-title;
            }
            .getStartedSubtitle {
                .responsive-property(font-size, 16px, null, 14px);
                .responsive-property(padding-top,  8px, null, 0px);
                color: @color-login-text-subtitle;
            }
        }
        .getStartedForm {
            display: flex;
            flex-direction: column;
            align-items: center;
            .getStartedTerms{
                margin-bottom: 34px;
                display: flex;
                align-items: inherit;
                flex-direction: column;
                .responsive-property(margin-bottom, 34px, null, 66px);
                        .errorMsg{
                            display: block; 
                            font-weight: normal;
                            color:red; 
                            text-align: center;
                            font-size: 14px;
                            letter-spacing: -0.36px;
                        }
                .lineTerms {
                    display: flex;
                    align-items: inherit;
                    .checkboxUserinfo {
                        .v-input__slot {
                            display: flex;
                            align-items: unset;
                            margin-bottom: 6px;
                            .v-icon{
                                color: @global-blue !important;
                            }
                            .v-messages{
                                display: none;
                            }
                        }
                    }
                    input{
                        padding-left: 12px;
                        width: 25px;
                        height: 25px;
                    }
                    span {
                        padding: 0;
                        color:#212121;
                        .responsive-property(font-size, 13px, null, 12px);
                        font-weight: initial;
                        &.paddingHelper{
                            padding-right: 2px;
                        }
                        .terms {
                            color: @global-blue; 
                            text-decoration: underline;
                        }
                    }
                }
            }
            .or{
                    padding: 15px 0;
                }
                font-size: 12px;
                font-weight: bold;
                letter-spacing: -0.36px;
                text-align: center;
                color: @color-login-text-subtitle;
                &.errorMsg{
                    display: block; 
                    color:red; 
                    text-align: center;
                }
            }
            button{
                margin: 0;
                text-transform: none !important;
                .responsive-property(width, 100%, null, 72%);
                &.google {
                    .responsive-property(margin-bottom, 0px, null, 20px);
                    color: white;
                    img{
                        width: 48px;
                        height: 48px;  
                    }
                    .btnText{
                        font-size: 16px;
                        color: white;
                        font-weight: 600;
                        margin: 0 22px 0 8px;
                    }
                    .v-btn__loading{
                        color: white;
                    }
                    .v-btn__content {
                        margin: 0;
                    }
                }
                &.email{
                    color: @global-blue;
                    background-color:white;
                    border: solid 1px #3751ff;
                    img{
                        width: 32px;
                        height: 32px;
                    }
                    .btnText{
                        margin: 0 34px 0 13px;
                        color: @global-blue;
                        font-size: 16px;
                        font-weight: normal;
                    }
                    .v-btn__content {
                        margin: 0;
                    }
                }
            }
        }
        .getStartedBottom {
            .responsive-property(margin, 48px auto, null, 10px auto);
            .responsive-property(font-size, 16px, null, 14px);
                .link{      
                    color: @global-blue;
                }
            .needAccount {
                color: @color-login-text-subtitle;
            }
        }
    }
</style>
