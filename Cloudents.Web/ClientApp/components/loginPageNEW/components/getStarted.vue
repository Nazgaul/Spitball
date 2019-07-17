<template>
    <v-layout column wrap class="getStartedContainer">
        <div class="getStarted-actions">
            <div class="getStarted-top">
                <p v-language:inner="'loginRegister_getstarted_title'"/>
                <span v-language:inner="'loginRegister_getstarted_subtitle'"/>
            </div>
            <div class="getStarted-form">
                <div v-if="isRegisterPath" class="getStarted-terms">
                    <div>
                        <input type="checkbox" @click="checkBoxConfirm" v-model="isTermsAgree" />
                        <span>
                            <span v-language:inner="'loginRegister_getstarted_terms_i_agree'"/>
                            <span class="terms" @click="redirectTo('terms')" v-language:inner="'loginRegister_getstarted_terms_terms'"/>
                            <span v-language:inner="'loginRegister_getstarted_terms_and'"/>
                            <span class="terms" @click="redirectTo('privacy')" v-language:inner="'loginRegister_getstarted_terms_privacy'"/>
                        </span>
                        <span v-if="isError" class="errorMsg" v-language:inner="'login_please_agree'"/>
                    </div>
                </div>
                <!-- <div> -->
                    <v-btn @click="goWithGoogle()" 
                        :loading="googleLoading" 
                        large round
                        class="google elevation-5 btn-login">
                        <v-icon class="pr-3">sbf-google-icon</v-icon>
                        <span v-language:inner="isRegisterPath? 'loginRegister_getstarted_btn_google_signup':'loginRegister_getstarted_btn_google_signin'"/>
                    </v-btn>
                    <!-- <span v-if="gmailError" class="errorMsg" v-language:inner="'gmailError'"/> -->
                <!-- </div> -->

                <span class="hidden-xs-only" hidden-xs-only v-language:inner="'loginRegister_getstarted_or'"/>

                <v-btn @click="goWithEmail()" 
                    large flat round 
                    class="email">
                    <v-icon class="pr-3">sbf-email</v-icon>
                    <span v-language:inner="isRegisterPath? 'loginRegister_getstarted_btn_email_signup':'loginRegister_getstarted_btn_email_signin'"/>
                </v-btn>

            </div>
        </div>
        <div class="getStarted-bottom">
            <span v-language:inner="isRegisterPath? 'loginRegister_getstarted_signin_text':'loginRegister_getstarted_signup_text'"/>
             &nbsp;
            <span class="link" @click="redirectTo(isRegisterPath? 'signin':'register')" v-language:inner="isRegisterPath? 'loginRegister_getstarted_signin_link':'loginRegister_getstarted_signup_link'"/>
        </div>
        
    </v-layout>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
let auth2;

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
        ...mapActions(['updateStep','googleSigning']),
        checkBoxConfirm(){
            if(this.isTermsAgree){
                this.showError = false
            } else this.showError = true
        },
        goWithGoogle(){
            if(this.isRegisterPath){
                if(!this.isTermsAgree){
                    this.showError = true;
                } else {
                    this.googleLoading = true;
                    this.googleSigning().then(res=>{},err=>{
                        
                        this.googleLoading = false
                        })
                    }
            } else {
                this.googleLoading = true;
                this.googleSigning().then(res=>{},err=>{
                        

                    this.googleLoading = false
                })
                }
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
        ...mapGetters(['getErrorMessages']),
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
        this.$nextTick(function () {
            this.$loadScript("https://apis.google.com/js/client:platform.js").then(()=>{
                gapi.load('auth2', function () {
                    auth2 = gapi.auth2.init({
                    client_id: '341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com',
                    });
                });
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
            text-align: center;
            .responsive-property(margin-bottom, 34px, null, 66px);
            span{
                padding: 0;
                color:#212121;
                .responsive-property(font-size, 13px, null, 12px);
                letter-spacing: -0.34px;
                line-height: 1.23;
                font-weight: initial;
                &.terms{
                   cursor: pointer;
                   color: @color-login-text-link; 
                   letter-spacing: -0.28px;
                   text-decoration: underline;
                }
                &.errorMsg{
                    display: block; 
                    color:red; 
                    text-align: center;
                }
            }
        }
        span{
            padding: 10px 0;
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
                .responsive-property(margin-bottom, 0px, null, 20px);
                color: white;
                span{
                    font-size: 16px;
                    color: white;
                    font-weight: 600;
                }
                .v-btn__loading{
                    color: white;
                }
            }
            &.email{
                color: @color-login-text-link;
                span{
                    color: @color-login-text-link;
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
        span {
            .responsive-property(font-size, 16px, null, 14px);
            letter-spacing: -0.42px;
            color: @color-login-text-subtitle;
            &.link{                
                cursor: pointer;
                letter-spacing: -0.37px;
                color: @color-login-text-link;
            }
        }
    }
 
}
</style>
