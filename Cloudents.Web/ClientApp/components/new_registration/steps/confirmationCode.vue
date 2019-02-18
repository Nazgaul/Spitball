<template>
    <!--step verify phone number-->
    <div class="step-phone-confirm">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="meta.heading"></p>
                    <p class="text-block-sub-title" style="direction:ltr;" v-if="phone.phoneNum">(+{{phone.countryCode}})
                        {{phone.phoneNum }} &nbsp;<span class="phone-change" @click="changePhone()" v-language:inner>login_Change</span>
                    </p>
                </div>
            </div>
            <div slot="step-data" class="limited-width wide">
                <h1 v-if="!isMobile" class="step-title" v-html="meta.heading"></h1>
                <p v-if="phone.phoneNum && !isMobile" class="sub-title" style="direction:ltr;">
                    (+{{phone.countryCode}})
                    {{phone.phoneNum}}&nbsp;<span class="phone-change" @click="changePhone()"
                                             v-language:inner>login_Change</span></p>
                <sb-input class="code-field" icon="sbf-key" :errorMessage="errorMessage.code" :bottomError="true"
                          v-model="confirmationCode" placeholder="login_placeholder_enter_confirmation_code"
                          :type="'number'"
                          :autofocus="true" @keyup.enter.native="smsCodeVerify()" v-language:placeholder></sb-input>
                <v-btn class="continue-btn submit-code"
                       value="Login"
                       :loading="loading"
                       :disabled="!confirmationCode || confirmationCode.length !==6 "
                       @click="smsCodeVerify()"
                ><span v-language:inner>login_continue</span></v-btn>
                <div class="bottom-text column">
                    <p class="inline" style="width: 100%; max-width: 100%;">
                        <span class="email-text inline click" @click="resendSms()" v-language:inner>login_click_here_to_send</span>
                        <v-divider style="min-height: 24px;"
                                class="mx-3"
                                inset
                                vertical
                        ></v-divider>
                        <span class="email-text inline click" @click="voiceCall()" v-language:inner>login_click_here_to_call</span>
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
    import {LanguageService} from "../../../services/language/languageService";

    const defaultSubmitRoute = {path: '/ask'};

    export default {

        name: "step_5",
        components: {stepTemplate, SbInput},
        data() {
            return {
                rules: {
                    required: value => !!value || 'Required.',

                },
                codePlaceholder: LanguageService.getValueByKey("login_placeholder_enter_confirmation_code"),
                errorMessage: {
                    phone: '',
                    code: '',
                    password: '',
                    confirmPassword: ''

                },
                confirmationCode: '',
                loading: false,
                bottomError: false
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
            meta: {},
            phone: {
                phoneNum: '',
                countryCode: ''
            },
            lastActiveRoute: '',
            userEmail: "",
            isNewUser: {
                type: Boolean,
                default: false
            }
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
                    .then(function (userId) {
                        //got to congratulations route if new user
                        if (self.isNewUser) {
                            self.$parent.$emit('changeStep', 'congrats');
                            analyticsService.sb_unitedEvent('Registration', 'Phone Verified');
                            if(!!userId){
                                analyticsService.sb_unitedEvent('Registration', 'User Id', userId.data.id);
                            }
                            self.loading = false;
                        } else {
                            self.loading = false;
                            analyticsService.sb_unitedEvent('Login', 'Phone Verified');
                            if(!!userId){
                                analyticsService.sb_unitedEvent('Registration', 'User Id', userId.data.id);
                            }
                            let url = self.lastActiveRoute || defaultSubmitRoute;
                            window.isAuth = true;
                            self.$router.push({path: `${url.path }`});
                        }
                    }, function (error) {
                        self.loading = false;
                        self.errorMessage.code = "Invalid code";
                    });
            },
            voiceCall(){
                this.updateLoading(true);
                analyticsService.sb_unitedEvent('Registration', 'Call Voice SMS');
                registrationService.voiceConfirmation()
                    .then((success) => {
                            this.updateLoading(false);
                            this.updateToasterParams({
                                toasterText: LanguageService.getValueByKey("login_call_code"),
                                showToaster: true,
                            });
                        },
                        error => {
                            this.errorMessage = error.text;
                            console.error(error, 'sign in resend error')
                        })
            },
            resendSms() {
                this.updateLoading(true);
                analyticsService.sb_unitedEvent('Registration', 'Resend SMS');
                registrationService.resendCode()
                    .then((success) => {
                            this.updateLoading(false);
                            this.updateToasterParams({
                                toasterText: LanguageService.getValueByKey("login_verification_code_sent_to_phone"),
                                showToaster: true,
                            });
                        },
                        error => {
                            this.errorMessage = error.text;
                            console.error(error, 'sign in resend error')
                        })
            },
        },
    }
</script>

<style  lang="less">
    @import '../../../styles/mixin.less';

    .sb-field {
        &:not(.error--text){
            max-height: 48px;
        }
        &.v-text-field--solo {
            .v-input__slot {
                box-shadow: none !important; //vuetify
                border-radius: 4px; //vuetify
                border: 1px solid @colorGreyNew;
                font-family: @fontOpenSans;
                font-size: 14px;
                letter-spacing: -0.7px;
                color: @textColor;
            }
        }
    }
</style>