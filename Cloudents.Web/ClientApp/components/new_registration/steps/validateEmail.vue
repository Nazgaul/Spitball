<template>
    <div class="step-user-status-email">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="meta.heading"></p>
                </div>
            </div>
            <div slot="step-data" class="limited-width form-wrap">
                <h1 v-if="!isMobile" class="step-title" v-html="meta.heading"></h1>
                <form @submit.prevent="emailValidate()" class="form-one">
                    <sb-input icon="sbf-email" class="email-field" :errorMessage="errorMessage.forgotPass" :bottomError="true"
                              placeholder="login_placeholder_email" v-model="userEmail" name="email" :type="'email'"
                              :autofocus="true" v-language:placeholder></sb-input>
                    <div class="recaptcha-wrapper">
                    </div>
                    <v-btn class="continue-btn" value="Login"
                           :loading="loading"
                           :disabled="!userEmail"
                           type="submit"
                    >
                        <span v-language:inner>login_continue</span></v-btn>
                </form>
                <div class="signin-strip">
                    <!--<p class="click" @click="goToLogin" v-language:inner>{{camefromCreate ? 'login_already_have_password': 'login_remember_now'}}</p>-->
                </div>
            </div>
            <div slot="step-image">
                <img :src="require(`../img/signin.png`)"/>
            </div>
        </step-template>
    </div>
</template>

<script>
    import stepTemplate from '../helpers/stepTemplate.vue'
    import analyticsService from '../../../services/analytics.service';
    import SbInput from "../../question/helpers/sbInput/sbInput.vue";
    import registrationService from "../../../services/registrationService";

    export default {
        name: "step_11",
        components: {stepTemplate, SbInput},
        data() {
            return {
                errorMessage: {
                    code: '',
                    password: '',
                    confirmPassword: '',
                    forgotPass: ''
                },
                userEmail: '',
                loading: false,
                bottomError: false
            }
        },
        props: {
            isMobile: {
                type: Boolean,
                default: false
            },
            ID: {
                type: String,
                default: '',
                required: false
            },
            passResetCode: {
                type: String,
                default: '',
                required: false
            },
            meta:{}
        },
        methods: {
            emailValidate() {
                if (!!this.userEmail) {
                    let self = this;
                    self.loading = true;
                    registrationService.validateEmail(encodeURIComponent(self.userEmail))
                        .then((response) => {
                            self.loading = false;
                            let step = response.data.step;
                            analyticsService.sb_unitedEvent('Login Email validation', 'email send');
                            self.$parent.$emit('updateEmail', self.userEmail);
                            self.$parent.$emit('fromCreate', 'create');
                            self.$parent.$emit('changeStep', `${step}`);
                        }, (error)=> {
                            self.loading = false;
                            self.errorMessage.forgotPass = error.response.data["Email"] ? error.response.data["Email"][0] : '';
                        });
                }
            },
            goToLogin() {
                this.$parent.$emit('changeStep', 'loginStep');
            },

        },
    }
</script>


<style scoped>

</style>