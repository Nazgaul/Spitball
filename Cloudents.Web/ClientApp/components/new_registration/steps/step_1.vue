<template>
    <!--!!!step terms and first screen-->
    <div class="step-terms-firstscreen">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="isCampaignOn ? campaignData.stepOne.text : meta.heading">

                    </p>
                </div>
                <div class="checkbox-terms">
                    <input type="checkbox" v-model="agreeTerms" id="agreeTerm"/>
                    <label for="agreeTerm"></label>
                    <span><span v-language:inner>login_agree</span>&nbsp;<router-link
                            to="terms" v-language:inner> login_terms_of_services</router-link>&nbsp;<span v-language:inner>login_and</span>&nbsp;<router-link
                            to="privacy" v-language:inner>login_privacy_policy</router-link></span>
                </div>
                <span class="has-error" v-if="confirmCheckbox"
                      style="background: white; display: block; color:red; text-align: center;">
                        <span v-language:inner>login_please_agree</span></span>
            </div>
            <div slot="step-data" class="limited-width form-wrap">
                <div class="text" v-if="!isMobile">
                    <h1 class="step-title"  v-html="isCampaignOn ? campaignData.stepOne.text : meta.heading" >
                    </h1>
                    <!--<p class="sub-title">{{ isCampaignOn ? campaignData.stepOne.text : meta.text }}</p>-->
                </div>
                <div class="checkbox-terms" v-if="!isMobile">
                    <input type="checkbox" v-model="agreeTerms" id="agreeTermDesk"/>
                    <label for="agreeTermDesk"></label>
                    <span><span v-language:inner>login_agree</span>&nbsp;<router-link
                            to="terms" v-language:inner>login_terms_of_services</router-link>&nbsp;<span v-language:inner>login_and</span>&nbsp;<router-link
                            to="privacy" v-language:inner>login_privacy_policy</router-link></span>
                </div>
                <div class="has-error" v-if="confirmCheckbox && !isMobile"
                     style="background: white; display: block; color:red; text-align: center;">
                    <span v-language:inner>login_please_agree</span>
                </div>
                <button v-if="isSignIn" class="google-signin" @click="googleLogIn">
                    <span v-language:inner>login_sign_in_with_google</span>
                    <span>
                            <v-icon>sbf-google-icon</v-icon>
                        </span>
                </button>
                <button v-else class="google-signin" @click="googleLogIn">
                    <span v-language:inner>login_sign_up_with_google</span>
                    <span>
                            <v-icon>sbf-google-icon</v-icon>
                        </span>
                </button>
                <span class="has-error" v-if="errorMessage.gmail"
                      style="background: white; display: block; color:red; text-align: center;">
                        {{errorMessage.gmail}}</span>
                <div class="seperator-text"><span v-language:inner>login_or</span></div>
                <v-btn v-if="isSignIn" class="sign-with-email"
                       value="Login"
                       :loading="loading"
                       @click="showDialogPass()">
                    <span v-language:inner>login_signin_your_email</span>
                </v-btn>
                <v-btn v-else class="sign-with-email"
                       value="Login"
                       :loading="loading"
                       @click="goToEmailLogin()">
                    <span v-language:inner>login_signup_your_email</span>
                </v-btn>
                <div class="signin-strip">
                    <div v-if="isSignIn">
                        <span v-language:inner>login_need_account_text</span>&nbsp;
                        <a class="click" @click="goToEmailLogin()" v-language:inner>login_need_account_link</a>
                    </div>
                    <div v-else>
                        <span v-language:inner>login_already_have_account</span>&nbsp;
                        <a class="click" @click="showDialogPass()" v-language:inner>login_sign_in</a>
                    </div>
                </div>
            </div>
            <div slot="step-image">
                <img :src="require(`../img/registerEmail.png`)"/>
            </div>
        </step-template>
        <v-dialog v-model="passDialog" max-width="600px" :fullscreen="isMobile" content-class="registration-dialog">
            <v-card>
                <button class="close-btn" @click="passDialog = false">
                    <v-icon>sbf-close</v-icon>
                </button>
                <v-card-text class="limited-width">
                    <h1 v-if="isMobile">

                        <span v-language:inner >login_passsword_dialog_title</span>
                        <!--<span v-language:inner>login_sure_exit1</span><br/>-->
                        <!--<span v-language:inner>login_sure_exit2</span><br/>-->
                        <!--<span v-language:inner>login_sure_exit3</span><br/>-->
                        <!--<span v-language:inner>login_sure_exit4</span>&nbsp;<b>-->
                        <!--<span v-language:inner>login_sure_exit5</span></b>-->
                    </h1>
                    <p v-language:inner>login_passsword_dialog_text_1</p>
                    <p v-language:inner>login_passsword_dialog_text_2</p>
                    <v-btn  class="continue-registr"
                            @click="goToCreatePassword()">
                        <span v-language:inner>login_password_dialog_action_create_password</span>
                    </v-btn>
                    <button class="continue-btn" @click="goToLogin()" v-language:inner>login_password_dialog_action_have_password</button>
                </v-card-text>
            </v-card>
        </v-dialog>

    </div>
    <!--!!!end terms and first screen-->
</template>

<script>
    import stepTemplate from '../helpers/stepTemplate.vue'
    import analyticsService from '../../../services/analytics.service';
    import SbInput from "../../question/helpers/sbInput/sbInput.vue";
    import { mapActions,  mapMutations } from 'vuex'
    import registrationService from "../../../services/registrationService";
    var auth2;
    const defaultSubmitRoute = {path: '/ask'};

    export default {
        components: {stepTemplate, SbInput },

        name: 'step_1',
        data() {
            return {
                siteKey: '6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1',
                agreeTerms: false,
                agreeError: false,
                loading: false,
                passDialog: false,
                isCampaignOn: false,
                errorMessage: {
                    gmail: '',
                },
            }
        },
        props:{
            isMobile: {
                type: Boolean,
                default: false
            },
            meta: {},
            isSignIn: {
                type: Boolean,
                default: false
            },
            toUrl: {},
        },
        computed: {
            confirmCheckbox(){
                return !this.agreeTerms && this.agreeError
            },
        },
        methods: {
            ...mapMutations({updateLoading: "UPDATE_LOADING"}),
            ...mapActions({updateToasterParams: 'updateToasterParams', updateCampaign: 'updateCampaign'}),
            googleLogIn() {
                if(!this.agreeTerms){
                    return  this.agreeError = true;
                }
                var self = this;
                this.updateLoading(true);
                let authInstance = gapi.auth2.getAuthInstance();
                authInstance.signIn().then((googleUser) => {
                    let idToken = googleUser.getAuthResponse().id_token;
                    registrationService.googleRegistration(idToken)
                        .then((resp)=> {
                            self.updateLoading(false);
                            let newUser = resp.data.isNew;
                            if (newUser) {
                                analyticsService.sb_unitedEvent('Registration', 'Start Google');
                                self.$parent.$emit('changeStep', 'enterphone');
                            } else {
                                analyticsService.sb_unitedEvent('Login', 'Start Google');
                                //user existing
                                global.isAuth = true;
                                let url = self.toUrl || defaultSubmitRoute;
                                self.$router.push({path: `${url.path }`});
                            }
                        },  (error)=> {
                            self.updateLoading(false);
                            self.errorMessage.gmail = error.response.data["Google"] ? error.response.data["Google"][0] : '';
                        });
                },  (error)=> {
                    self.updateLoading(false);
                });
            },
            goToEmailLogin(){
                if(!this.agreeTerms){
                    return this.agreeError = true
                }
                this.$parent.$emit('changeStep', 'startstep');
            },
            goToLogin() {
                this.passDialog = false;
                this.$parent.$emit('changeStep', 'loginStep');

            },
            showDialogPass(){
                this.passDialog = true;
            },
            goToCreatePassword() {
                this.passDialog = false;
                this.$parent.$emit('fromCreate', 'create');
                this.$parent.$emit('changeStep', 'emailpassword');
            },
        }
    }
</script>

<style>

</style>