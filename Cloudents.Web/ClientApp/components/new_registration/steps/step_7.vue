<template>
    <!--step login-->
    <div class="step-login" >
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title"><b v-language:inner>login_welcome_back</b> <br/> <span v-language:inner>login_please_login</span> </p>
                </div>
            </div>
            <div slot="step-data" class="limited-width">
                <h1 v-if="!isMobile" class="step-title" v-language:inner>login_welcome_back</h1>
                <!--<h1 v-if="!isMobile" class="step-title" v-language:inner>login_please_login</h1>-->
                <!--<p v-if="!isMobile" class="inline" v-language:inner>login_please_login</p>-->
                <form @submit.prevent="submit">
                    <sb-input :errorMessage="errorMessage.email" :required="true" class="email-field" type="email"
                              name="email" id="input-url" v-model="userEmail"
                              placeholder="Enter your mobile or email "></sb-input>
                    <sb-input :errorMessage="errorMessage.password" :required="true" class="email-field mt-3"
                              :type="'password'"
                              name="user password"  v-model="password"
                              placeholder="Enter password"></sb-input>
                    <vue-recaptcha class="recaptcha-wrapper"
                                   ref="recaptcha"
                                   :sitekey="siteKey"
                                   @verify="onVerify"
                                   @expired="onExpired">

                    </vue-recaptcha>
                    <v-btn class="continue-btn loginBtn"
                           value="Login"
                           :loading="loading"
                           :disabled="!userEmail || !recaptcha || !password "
                           type="submit"
                    > <span v-language:inner>login_login</span>
                    </v-btn>
                </form>
                <div class="signin-strip">
                    <a @click="forgotPassword()" v-language:inner>login_forgot_password_link</a>
                </div>
            </div>
            <img slot="step-image" :src="require(`../img/signin.png`)"/>
        </step-template>
    </div>
    <!--step login end-->
</template>

<script>
    import stepTemplate from '../helpers/stepTemplate.vue'
    import VueRecaptcha from 'vue-recaptcha';
    import analyticsService from '../../../services/analytics.service';
    import SbInput from "../../question/helpers/sbInput/sbInput.vue";
    import { mapActions, mapMutations } from 'vuex'
    import registrationService from "../../../services/registrationService";
    export default {
        components: {stepTemplate, SbInput, VueRecaptcha},
        name: "step_7",
        data() {
            return {
                siteKey: '6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1',
                errorMessage: {
                    phone: '',
                    code: '',
                    password: '',
                    confirmPassword: ''
                },
                password: '',
                loading: false,
                userEmail: "",
                recaptcha: '',

            }

        },
        props: {
            isMobile: {
                type: Boolean,
                default: false
            },
            isCampaignOn: {
                type: Boolean,
                default: false
            },
            meta: {
            },
            lastActiveRoute: '',
        },
        methods: {
            onVerify(response) {
                this.recaptcha = response;
            },
            onExpired() {
                this.recaptcha = "";
            },
            submit() {
                let self = this;
                self.loading = true;
                registrationService.signIn(this.userEmail, this.recaptcha, this.password)
                    .then((response) => {
                        self.loading = false;
                        analyticsService.sb_unitedEvent('Login', 'Start');
                        let step = response.data.step;
                        self.$parent.$emit('updateEmail', self.userEmail);
                        self.$parent.$emit('changeStep', step);
                    }, function (reason) {
                        self.$refs.recaptcha.reset();
                        self.loading = false;
                        self.errorMessage.email = reason.response.data ? Object.values(reason.response.data)[0][0] : reason.message;
                    });
            },
            forgotPassword() {
                this.$parent.$emit('changeStep', 'emailpassword');

            },
        },
        created(){
            this.password = '';
        }
    }
</script>

<style scoped>

</style>