<template>
    <!--!!!step terms and first screen-->
    <div class="step-terms-firstscreen">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="isCampaignOn ? campaignData.stepOne.text : meta.heading">

                    </p>
                </div>
                <div class="checkbox-terms" v-if="!isSignIn">
                    <input type="checkbox" v-model="agreeTerms" id="agreeTerm"/>
                    <label for="agreeTerm"></label>
                    <span><span v-language:inner>login_agree</span>&nbsp;<router-link
                            to="terms" v-language:inner> login_terms_of_services</router-link>&nbsp;<span v-language:inner>login_and</span>&nbsp;<router-link
                            to="privacy" v-language:inner>login_privacy_policy</router-link></span>
                </div>
                <span class="has-error" v-if="!isSignIn && confirmCheckbox"
                      style="background: white; display: block; color:red; text-align: center;">
                        <span v-language:inner>login_please_agree</span></span>
            </div>
            <div slot="step-data" class="limited-width form-wrap">
                <div class="text" v-if="!isMobile">
                    <h1 class="step-title"  v-html="isCampaignOn ? campaignData.stepOne.text : meta.heading" >
                    </h1>
                    <!--<p class="sub-title">{{ isCampaignOn ? campaignData.stepOne.text : meta.text }}</p>-->
                </div>
                <div class="checkbox-terms" v-if="!isMobile && !isSignIn">
                    <input type="checkbox" v-model="agreeTerms" id="agreeTermDesk"/>
                    <label for="agreeTermDesk"></label>
                    <span><span v-language:inner>login_agree</span>&nbsp;<router-link
                            to="terms" v-language:inner>login_terms_of_services</router-link>&nbsp;
                    <span v-language:inner>login_and</span>&nbsp;<router-link
                            to="privacy" v-language:inner>login_privacy_policy</router-link>
                    <span v-language:inner>login_end_privacy_policy</span>
                    </span>
                </div>
                <div class="has-error" v-if="confirmCheckbox && !isMobile && !isSignIn"
                     style="background: white; display: block; color:red; text-align: center;">
                    <span v-language:inner>login_please_agree</span>
                </div>
                <v-btn :loading="googleLoading" v-show="isSignIn" class="google-signin" @click="googleLogIn">
                    <span v-language:inner>login_sign_in_with_google</span>
                    <span>
                            <v-icon>sbf-google-icon</v-icon>
                        </span>
                </v-btn>
                <v-btn :loading="googleLoading" v-show="!isSignIn" class="google-signin" @click="googleLogIn">
                    <span v-language:inner>login_sign_up_with_google</span>
                    <span>
                            <v-icon>sbf-google-icon</v-icon>
                        </span>
                </v-btn>
                <span class="has-error" v-if="errorMessage.gmail"
                      style="background: white; display: block; color:red; text-align: center;">
                        {{errorMessage.gmail}}</span>
                <div class="seperator-text"><span v-language:inner>login_or</span></div>
                <v-btn v-show="isSignIn" class="sign-with-email"
                       value="Login"
                       :loading="loading"
                       @click="goToValidateEmail()">
                    <span v-language:inner>login_signin_your_email</span>
                </v-btn>
                <v-btn v-show="!isSignIn" class="sign-with-email"
                       value="Login"
                       :loading="loading"
                       @click="goToEmailLogin()">
                    <span v-language:inner>login_signup_your_email</span>
                </v-btn>
                <div class="signin-strip">
                    <div v-show="isSignIn">
                        <span v-language:inner>login_need_account_text</span>&nbsp;
                        <a class="click" @click="goTosignUp()" v-language:inner>login_need_account_link</a>
                    </div>
                    <div v-show="!isSignIn">
                        <span v-language:inner>login_already_have_account</span>&nbsp;
                        <a class="click" @click="redirectToSignIn()" v-language:inner>login_sign_in</a>
                    </div>
                </div>
            </div>
            <div slot="step-image">
                <img :src="require(`../img/registerEmail.png`)"/>
            </div>
        </step-template>


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
    const isIl = global.country.toLowerCase() === 'il';
    const defaultSubmitRoute = isIl ? {path: '/note'} : {path: '/ask'};

    export default {
        components: {stepTemplate, SbInput},

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
                signInScreeen: false,
                registerScreeen: false,
                googleLoading: false
            }
        },
        props: {
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
            confirmCheckbox() {
                return !this.agreeTerms && this.agreeError
            },
        },
        methods: {
            ...mapMutations({updateLoading: "UPDATE_LOADING"}),
            ...mapActions({updateToasterParams: 'updateToasterParams', updateCampaign: 'updateCampaign'}),
            googleLogIn() {
                if(!this.isSignIn){
                    if (!this.agreeTerms) {
                        return this.agreeError = true;
                    }
                }
                var self = this;
                self.googleLoading = true;
                this.updateLoading(true);
                let authInstance = gapi.auth2.getAuthInstance();
                authInstance.signIn().then((googleUser) => {
                    let idToken = googleUser.getAuthResponse().id_token;
                    registrationService.googleRegistration(idToken)
                        .then((resp) => {
                            self.updateLoading(false);
                            let newUser = resp.data.isNew;
                            self.googleLoading = false;
                            if (newUser) {
                                analyticsService.sb_unitedEvent('Registration', 'Start Google');
                                self.$parent.$emit('updateIsNewUser', newUser);
                                self.$parent.$emit('changeStep', 'enterphone');
                            } else {
                                analyticsService.sb_unitedEvent('Login', 'Start Google');
                                //user existing
                                global.isAuth = true;
                                let url = self.toUrl || defaultSubmitRoute;
                                self.$router.push({path: `${url.path }`});
                            }
                        }, (error) => {
                            self.updateLoading(false);
                            self.googleLoading = false;
                            self.errorMessage.gmail = error.response.data["Google"] ? error.response.data["Google"][0] : '';
                        });
                }, (error) => {
                    self.updateLoading(false);
                });
            },
            goToEmailLogin() {
                if (!this.agreeTerms && !this.isSignIn) {
                    return this.agreeError = true
                }
                this.$parent.$emit('changeStep', 'startstep');
            },
            goTosignUp(){
                if (!this.agreeTerms && !this.isSignIn) {
                    return this.agreeError = true
                }
                this.$router.push({path: '/register'});
            },
            redirectToSignIn(){
                this.$router.push({path: '/signin'});

            },

            goToValidateEmail() {
                this.$parent.$emit('changeStep', 'validateemail');
            },
        },
        created() {

        }
    }

</script>

<style>

</style>