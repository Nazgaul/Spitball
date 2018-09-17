<template>
    <!--step verify phone number-->
    <div class="step-phone-confirm">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="meta.heading"></p>
                    <p class="text-block-sub-title" v-if="phone.phoneNum">(+{{phone.countryCode}})
                        {{phone.phoneNum }} <span class="phone-change" @click="changePhone()" v-language:inner>login_Change</span>
                    </p>
                </div>
            </div>
            <div slot="step-data" class="limited-width wide">
                <h1 v-if="!isMobile" class="step-title" v-html="meta.heading"></h1>
                <p v-if="phone.phoneNum" class="sub-title">
                    <span v-language:inner>login_sent_code_by_sms</span>
                    (+{{phone.countryCode}})
                    {{phone.phoneNum}} </p>
                <!--<p v-if="!isMobile" class="confirm-title" v-language:inner>login_sent_confirmation_code</p>-->
                <sb-input class="code-field" icon="sbf-key" :errorMessage="errorMessage.code"
                          v-model="confirmationCode" placeholder="Enter confirmation code" type="number"
                          :autofocus="true" @keyup.enter.native="smsCodeVerify()"></sb-input>
                <v-btn class="continue-btn submit-code"
                       value="Login"
                       :loading="loading"
                       :disabled="!confirmationCode"
                       @click="smsCodeVerify()"
                ><span v-language:inner>login_continue</span></v-btn>
                <div class="bottom-text">
                    <p class="inline"><span v-language:inner>login_didnt_get_sms</span> &nbsp;
                        <span class="email-text inline click" @click="resendSms()" v-language:inner>login_click_here_to_send</span>
                    </p>
                </div>
            </div>
            <img slot="step-image" :src="require(`../img/confirm-phone.png`)"/>
        </step-template>
    </div>
    <!--step verify phone number end-->
</template>

<script>
    import stepTemplate from '../helpers/stepTemplate.vue'
    import { mapActions, mapGetters, mapMutations } from 'vuex'
    import registrationService from '../../../services/registrationService'
    import analyticsService from '../../../services/analytics.service';
    import SbInput from "../../question/helpers/sbInput/sbInput.vue";
    const defaultSubmitRoute = {path: '/ask'};

    export default {

        name: "step_5",
        components: {stepTemplate, SbInput},
        data() {
            return {
                errorMessage: {
                    phone: '',
                    code: '',
                    password: '',
                    confirmPassword: ''

                },
                confirmationCode: '',
                loading: false,
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
            phone: {
                phoneNum: '',
                countryCode: ''
            },
            lastActiveRoute: '',
            userEmail: ""
        },
        methods: {
            ...mapMutations({updateLoading: "UPDATE_LOADING"}),
            ...mapActions({updateToasterParams: 'updateToasterParams'}),
            changePhone() {
                this.$parent.$emit('changeStep', 'enterphone');
            },
            smsCodeVerify() {
                let self = this;
                self.loading = true;
                registrationService.smsCodeVerification(this.confirmationCode)
                    .then(function () {
                        //got to congratulations route if new user
                        if (self.isNewUser) {
                            this.$parent.$emit('changeStep', 'congrats');
                            analyticsService.sb_unitedEvent('Registration', 'Phone Verified');
                            self.loading = false;

                        } else {
                            self.loading = false;
                            analyticsService.sb_unitedEvent('Login', 'Phone Verified');
                            let url = self.lastActiveRoute || defaultSubmitRoute;
                            window.isAuth = true;
                            self.$router.push({path: `${url.path }`});
                        }
                    }, function (error) {
                        self.loading = false;
                        self.errorMessage.code = "Invalid code";
                    });
            },
            resendSms() {
                let self = this;
                self.updateLoading(true);
                analyticsService.sb_unitedEvent('Registration', 'Resend SMS');
                registrationService.resendCode()
                    .then((success) => {
                            self.updateLoading(false);
                            self.updateToasterParams({
                                toasterText: 'A verification code was sent to your phone',
                                showToaster: true,
                            });
                        },
                        error => {
                            self.errorMessage = error.text;
                            console.error(error, 'sign in resend error')
                        })
            },
        },
    }
</script>

<style scoped>

</style>