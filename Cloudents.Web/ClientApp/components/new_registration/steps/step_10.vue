<template>
    <div class="step-email">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="camefromCreate ? meta.createtxt : meta.resettxt">
                        <!--{{ camefromCreate ? 'login_create_password_text' : 'login_reset_password_text' }}-->
                    </p>
                </div>
            </div>
            <div slot="step-data" class="limited-width form-wrap">
                <h1 v-if="!isMobile" class="step-title"  v-html="camefromCreate ? meta.createtxt : meta.resettxt"></h1>
                <p v-if="!camefromCreate" class="sub-title  mb-3" v-language:inner>login_happens_to_best</p>
                <form @submit.prevent="emailResetPassword" class="form-one">
                    <sb-input icon="sbf-email" class="email-field" :errorMessage="errorMessage.email" :bottomError="true"
                              placeholder="login_placeholder_email" v-model="userEmail" name="email" type="email"
                              :autofocus="true" v-language:placeholder></sb-input>
                    <div class="recaptcha-wrapper">
                    </div>
                    <v-btn class="continue-btn" value="Login"
                           :loading="loading"
                           :disabled="!userEmail"
                           type="submit"
                    >
                        <span>{{ camefromCreate ? meta.createbtn : meta.resetbtn }}</span></v-btn>
                </form>
                <div class="signin-strip">
                    <p class="click" @click="goToLogin" v-language:inner>{{camefromCreate ? 'login_already_have_password': 'login_remember_now'}}</p>
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
                bottomError: false
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
            },
            meta:{}
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
                        }, (error)=> {
                            self.loading = false;
                            self.errorMessage.email = error.response.data["Email"] ? error.response.data["Email"][0] : '';
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