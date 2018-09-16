<template>
    <div class="step-email">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-language:inner>
                        {{ camefromCreate ? 'login_create_password_text' : 'login_reset_password_text' }}
                    </p>
                </div>
            </div>
            <div slot="step-data" class="limited-width form-wrap">
                <h1 v-if="!isMobile" class="step-title" v-language:inner>{{ camefromCreate ? 'login_create_password_text' : 'login_reset_password_text' }}</h1>
                <p v-if="!isMobile" class="sub-title  mb-3"
                   v-language:inner>login_happens_to_best
                </p>
                <form @submit.prevent="emailResetPassword" class="form-one">
                    <sb-input icon="sbf-email" class="email-field" :errorMessage="errorMessage.email"
                              placeholder="Enter your email address" v-model="userEmail" name="email" type="email"
                              :autofocus="true"></sb-input>
                    <div class="recaptcha-wrapper">
                    </div>
                    <v-btn class="continue-btn" value="Login"
                           :loading="loading"
                           :disabled="!userEmail"
                           type="submit"
                    >
                        <span v-language:inner>{{ camefromCreate ? 'login_create_password_button' : 'login_reset_password_button' }}</span></v-btn>
                </form>
                <div class="signin-strip">
                    <p class="click" @click="goToLogin">I remember now!</p>
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
        name: "step_10",
        components: {stepTemplate, SbInput},

        data() {
            return {
                errorMessage: {
                    code: '',
                    password: '',
                    confirmPassword: ''
                },
                userEmail: '',
                loading: false,

            }
        },
        props: {
            camefromCreate:{
                type: Boolean,
                default: false
            },
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
            }
        },
        methods: {
            emailResetPassword() {
                if (!!this.userEmail) {
                    let self = this;
                    self.loading = true;
                    registrationService.forgotPasswordReset(this.userEmail)
                        .then((response) => {
                            self.loading = false;
                            analyticsService.sb_unitedEvent('Forgot Password', 'Reset email send');
                            self.$parent.$emit('changeStep', 'emailconfirmedPass');
                        }, function (reason) {
                            self.loading = false;
                            self.errorMessage.confirmPassword = reason.response.data ? Object.values(reason.response.data)[0][0] : reason.message;
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