<template>
    <v-layout column wrap class="getStartedContainer">
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
                    <span v-if="isError" class="errorMsg">Please agree to Terms And Services in order to proceed</span>
                </div>
            </div>

            <v-btn @click="goWithGoogle()" 
                   :loading="isGoogleLoading" 
                   color="#304FFE" large round 
                   class="google elevation-5">
                <v-icon class="pr-3">sbf-google-icon</v-icon>
                <span v-language:inner="isRegisterPath? 'loginRegister_getstarted_btn_google_signup':'loginRegister_getstarted_btn_google_signin'"/>
            </v-btn>

            <span v-language:inner="'loginRegister_getstarted_or'"/>

            <v-btn @click="goWithEmail()" 
                   large flat round 
                   class="email">
                <v-icon class="pr-3">sbf-email</v-icon>
                <span v-language:inner="isRegisterPath? 'loginRegister_getstarted_btn_email_signup':'loginRegister_getstarted_btn_email_signin'"/>
            </v-btn>

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
            isRegisterPath: false,
            isTermsAgree: false,
            showError: false
        }
    },
    methods: {
        ...mapActions(['updateStep','updateAnalytics','googleSigning']),
        checkBoxConfirm(){
            if(this.isTermsAgree){
                this.showError = false
            } else this.showError = true
        },
        goWithGoogle(){
            if(this.isRegisterPath){
                if(!this.isTermsAgree){
                    this.showError = true;
                } else this.googleSigning()
            } else this.googleSigning()
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
        ...mapGetters(['getGoogleLoading']),
        isGoogleLoading(){
            return this.getGoogleLoading
        },
        isError(){
            return !this.isTermsAgree && this.showError
        }
    },
    mounted() {
        this.$nextTick(function () {
            gapi.load('auth2', function () {
                auth2 = gapi.auth2.init({
                    client_id: '341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com',
                });
            });
        });
    },
    created() {
        this.isRegisterPath = this.$route.name === 'registerPageNEW'? true : false;
    },
}
</script>

<style lang="less">
.getStartedContainer{
    .getStarted-top{
        display: flex;
        flex-direction: column;
        padding-bottom: 64px;
        p{
            margin: 0;
            font-size: 28px;
            line-height: 1.54;
            letter-spacing: -0.51px;
            text-align: center;
            color: #434c5f;
        }
        span{
            padding-top: 8px;
            font-size: 16px;
            letter-spacing: -0.42px;
            text-align: center;
            color: #888b8e;
        }
    }
    .getStarted-form{
        display: flex;
        flex-direction: column;
        align-items: center;
        .getStarted-terms{
            padding: 0 24px 34px 24px;
            span{
                padding: 0;
                color:#212121;
                font-size: 13px;
                letter-spacing: -0.34px;
                line-height: 1.23;
                &.terms{
                   cursor: pointer;
                   color: #4452fc; 
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
            color: #888b8e;
        }
        button{
            margin: 0;
            width: 400px;
            text-transform: none !important;
            &.google{
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
                color: #4452fc;
                span{
                    color: #4452fc;
                    font-size: 16px;
                    font-weight: normal;
                }
                background-color: rgba(68, 82, 252, 0.06);
                border: solid 1px rgba(55, 81, 255, 0.29);
            }
        }
    }
    .getStarted-bottom{
        display: flex;
        justify-content: center;
        padding-top: 48px;
        span {
            font-size: 16px;
            letter-spacing: -0.42px;
            color: #888b8e;
            &.link{                
                cursor: pointer;
                letter-spacing: -0.37px;
                color: #4452fc;
            }
        }
    }
}
</style>
