<template>
    <v-layout column wrap class="getStartedContainer">
        <div class="getStarted-actions">
            <div class="getStarted-top">
                <p v-language:inner="isRegisterPath? 'loginRegister_getstarted_title':'loginRegister_getstarted_title_login'"/>
                <span v-language:inner="isRegisterPath? 'loginRegister_getstarted_subtitle': '' "/>
            </div>
            <div class="getStarted-form">
                <div v-if="isRegisterPath" class="getStarted-terms">
                        <div class="line-terms">

            <v-checkbox @click="checkBoxConfirm" :ripple="false" class="checkbox-userinfo" 
                        v-model="isTermsAgree" off-icon="sbf-check-box-un" sel="check"
                        on-icon="sbf-check-box-done" name="checkBox" id="checkBox"/>
                            <label for="checkBox">
                                <span>
                                    <span class="padding-helper" v-language:inner="'loginRegister_getstarted_terms_i_agree'"/>
                                    <a :href="isFrymo? 'https://help.frymo.com/en/article/terms': 'https://help.spitball.co/en/article/terms-of-service'" class="terms padding-helper" v-language:inner="'loginRegister_getstarted_terms_terms'"/>
                                    <span class="padding-helper" v-language:inner="'loginRegister_getstarted_terms_and'"/>
                                    <a :href="isFrymo? 'https://help.frymo.com/en/policies':'https://help.spitball.co/en/article/privacy-policy'" class="terms" v-language:inner="'loginRegister_getstarted_terms_privacy'"/>
                                </span>
                            </label>
                        </div>
                        <span v-if="isError" class="errorMsg" v-language:inner="'login_please_agree'"/>
                </div>
                    <v-btn @click="goWithGoogle()" 
                        :loading="googleLoading" 
                        large rounded
                        sel="gmail"
                        class="google elevation-5 btn-login">
                        <img src="../images/G icon@2x.png">
                        <span v-language:inner="isRegisterPath? 'loginRegister_getstarted_btn_google_signup':'loginRegister_getstarted_btn_google_signin'"/>
                    </v-btn>
                        <span v-if="gmailError" class="errorMsg">{{gmailError}}</span>

                <span class="hidden-xs-only or" hidden-xs-only v-language:inner="'loginRegister_getstarted_or'"/>

                <v-btn @click="goWithEmail()" 
                    large text rounded 
                    sel="email"
                    class="email">
                    <img src="../images/np_email@2x.png">
                    <span v-language:inner="isRegisterPath? 'loginRegister_getstarted_btn_email_signup':'loginRegister_getstarted_btn_email_signin'"/>
                </v-btn>

            </div>
        </div>
        <div class="getStarted-bottom">
            <span v-language:inner="isRegisterPath? 'loginRegister_getstarted_signin_text':'loginRegister_getstarted_signup_text'"/>
             &nbsp;
            <router-link 
            to="" 
            class="link" 
            @click.native="redirectTo(isRegisterPath? 'signin':'register')" 
            v-language:inner="isRegisterPath? 'loginRegister_getstarted_signin_link':'loginRegister_getstarted_signup_link'"/>
        </div>
        
    </v-layout>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import insightService from '../../../services/insightService';
// let auth2;

export default {
    name: 'getStarted',
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
                    this.googleSigning().then(res=>{},err=>{
                        insightService.track.event(insightService.EVENT_TYPES.ERROR, 'signInWithGoogle', err);
                        this.googleLoading = false
                        })
                    
        },
        goWithEmail(){
            if(this.isRegisterPath){
                if(!this.isTermsAgree){
                    this.showError = true;
                } else this.updateStep('setEmailPassword')
            } else this.updateStep('setEmail')
        },
        redirectTo(toPath){
            this.$router.push({path: `/${toPath}`});
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
            return (this.$route.path === '/register')
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
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
@import '../../../styles/colors.less';
.getStartedContainer{
    justify-content: center;
    @media (max-width: @screen-xs) {
        height: -webkit-fill-available;
        justify-content: space-between;
    }
    .getStarted-actions{

   

    .getStarted-top{
        display: flex;
        flex-direction: column;
        padding-bottom: 64px;
        p{
            .responsive-property(font-size, 28px, null, 22px);
            .responsive-property(line-height, 1.54, null, 1.95);
            .responsive-property(letter-spacing, -0.51px, null, -0.4px);
            margin: 0;
            text-align: center;
            color: @color-login-text-title;
        }
        span{
            .responsive-property(font-size, 16px, null, 14px);
            .responsive-property(padding-top,  8px, null, 0px);
            letter-spacing: -0.42px;
            text-align: center;
            color: @color-login-text-subtitle;
        }
    }
    .getStarted-form{
        display: flex;
        flex-direction: column;
        align-items: center;
        .getStarted-terms{
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
            .line-terms{
                .checkbox-userinfo{
                .v-input__slot{
                display: flex;
                align-items: unset;
                    .v-icon{
                        color: @global-blue !important;
                    }
                    .v-messages{
                        display: none;
                    }
                }
            }
                display: flex;
                align-items: inherit;
                input{
                    padding-left: 12px;
                    width: 25px;
                    height: 25px;
                }
                span{
                    &.padding-helper{
                        padding-right: 2px;
                    }
                    padding: 0;
                    color:#212121;
                    .responsive-property(font-size, 13px, null, 12px);
                    letter-spacing: -0.34px;
                    line-height: 1.23;
                    font-weight: initial;
                    .terms{
                    cursor: pointer;
                    color: @global-blue; 
                    letter-spacing: -0.28px;
                    text-decoration: underline;
                    }
                }
            }
        }
        span{
            &.or{
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
                &.google{
                    img{
                        width: 48px;
                        height: 48px;  
                    }
                    .responsive-property(margin-bottom, 0px, null, 20px);
                    color: white;
                    span{
                        font-size: 16px;
                        color: white;
                        font-weight: 600;
                        margin: 0 22px 0 8px;
                    }
                    .v-btn__loading{
                        color: white;
                    }

            }
            &.email{
                img{
                    width: 32px;
                    height: 32px;
                }
                color: @global-blue;
                span{
                    
                    margin: 0 34px 0 13px;
                    color: @global-blue;
                    font-size: 16px;
                    font-weight: normal;
                }
                background-color: rgba(68, 82, 252, 0.06);
                border: solid 1px rgba(55, 81, 255, 0.29);
            }
        }
    }
     }

    .getStarted-bottom{
        display: flex;
        justify-content: center;
        .responsive-property(margin-top, 48px, null, 0px);
        .responsive-property(font-size, 16px, null, 14px);
            .link{      
                cursor: pointer;
                letter-spacing: -0.37px;
                color: @global-blue !important;
            }
        span {
            letter-spacing: -0.42px;
            color: @color-login-text-subtitle;
        }
    }
 
}
</style>
