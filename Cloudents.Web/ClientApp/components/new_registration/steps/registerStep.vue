<template>
    <div class="step-email">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="isCampaignOn ? campaignData.stepOne.text : meta.heading">
                    </p>
                </div>
            </div>
            <div slot="step-data" class="limited-width form-wrap">
                <div class="text" v-if="!isMobile">
                    <h1 class="step-title" v-html="isCampaignOn ? campaignData.stepOne.text : meta.heading" ></h1>
                </div>
                <form @submit.prevent="emailSend" class="form-one">
                    <sb-input icon="sbf-email" class="email-field" :errorMessage="errorMessage.email" :bottomError="true"
                              placeholder="login_placeholder_email" v-model="email" name="email" type="email"
                              :autofocus="true" v-language:placeholder></sb-input>

                    <sb-input  :class="['mt-3', hintClass]"  :errorMessage="errorMessage.password" :bottomError="true"
                               placeholder="login_placeholder_choose_password" v-model="password" name="pass"
                               :hint="passZxcvbn"
                               :type="'password'"
                               :autofocus="true"  v-language:placeholder></sb-input>
                    <sb-input  class="mt-3" :errorMessage="errorMessage.confirmPassword" :bottomError="true"
                               placeholder="login_placeholder_confirm_password" v-model="confirmPassword" name="confirm" type="password"
                               :autofocus="true" v-language:placeholder></sb-input>
                    <vue-recaptcha class="recaptcha-wrapper"
                                   :sitekey="siteKey"
                                   ref="recaptcha"
                                   @verify="onVerify"
                                   @expired="onExpired()">
                    </vue-recaptcha>
                    <v-btn class="continue-btn" value="Login"
                           :loading="loading"
                           :disabled="!email  || !recaptcha  || !password || !confirmPassword"
                           type="submit"
                    ><span v-language:inner>login_continue</span></v-btn>
                    <div class="checkbox-terms">
                    </div>
                </form>
            </div>
            <div slot="step-image">
                <img :src="require(`../img/registerEmail.png`)"/>
            </div>
        </step-template>
    </div>

</template>

<script>
    //import zxcvbn from 'zxcvbn';
    import stepTemplate from '../helpers/stepTemplate.vue'
    import VueRecaptcha from 'vue-recaptcha';
    import analyticsService from '../../../services/analytics.service';
    import SbInput from "../../question/helpers/sbInput/sbInput.vue";
    import { mapActions, mapMutations } from 'vuex'
    import registrationService from "../../../services/registrationService";

    var auth2;
    export default {
        components: {stepTemplate, SbInput, VueRecaptcha},

        name: "step_2",
        data() {
            return {
                siteKey: '6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1',
                errorMessage: {
                    phone: "",
                    email: ""
                },
                score: {
                    default: 0,
                    required: false
                },
                recaptcha: '',
                loading: false,
                password: '',
                confirmPassword: '',
                bottomError: false,
                submitted: false,
            }
        },
        props: {
            passScoreObj:{},
            isMobile: {
                type: Boolean,
                default: false
            },
            isCampaignOn: {
                type: Boolean,
                default: false
            },
            meta: {},
            campaignData:{
            },
            userEmail: {
                type: String,
                default: ''
            },

        },
        computed:{
            passZxcvbn(){
                if(this.password.length !== 0){
                    this.score = global.zxcvbn(this.password).score;
                    return `${this.passScoreObj[this.score].name}`
                }
            },
            hintClass(){
                if(this.passZxcvbn){
                    return this.passScoreObj[this.score].className;
                }
            },
            email:{
                get(){
                    return this.userEmail
                },
                set(val){
                    this.$parent.$emit('updateEmail',val);
                }
            },
            isValidPass(){
                return this.password.length >= 8  && this.confirmPassword.length >= 8
            },

        },

        methods: {
            ...mapMutations({updateLoading: "UPDATE_LOADING"}),
            emailSend() {
                let email = this.email;
                let self = this;
                self.loading = true;
                self.sumbitted = true;
                registrationService.emailRegistration(email, this.recaptcha, this.password, this.confirmPassword)
                    .then(function (resp) {
                        let step = resp.data.step;
                        self.$parent.$emit('updateEmail',email);
                        self.$parent.$emit('changeStep', step);
                        analyticsService.sb_unitedEvent('Registration', 'Start');
                        self.loading = false;
                    }, function (error) {
                        self.recaptcha = "";
                        self.$refs.recaptcha.reset();
                        self.loading = false;
                        self.errorMessage.confirmPassword = error.response.data["ConfirmPassword"] ? error.response.data["ConfirmPassword"][0] : '';
                        self.errorMessage.password = error.response.data["Password"] ? error.response.data["Password"][0] : '';
                        self.errorMessage.email= error.response.data["Email"] ? error.response.data["Email"][0] : '';

                    });
            },

            // captcha events methods
            onVerify(response) {
                this.recaptcha = response;
            },
            onExpired() {
                this.recaptcha = "";
            },
        },
        created() {
        }
    }
</script>

<style scoped>

</style>