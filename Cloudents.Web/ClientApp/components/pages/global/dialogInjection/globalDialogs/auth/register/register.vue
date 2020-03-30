<template>
    <div class="registerDialog">
        <v-form @submit.prevent="submit" ref="form" class="registerForm pa-4">  
            <div>
                <div class="closeIcon">
                    <v-icon size="12" color="#aaa" @click="$store.commit('setToaster', '')">sbf-close</v-icon>
                </div>

                <template v-if="isEmailRegister">
                    <div class="mainTitle text-center mb-8" v-t="'loginRegister_setemailpass_title'"></div>

                    <v-btn 
                        @click="gmailRegister"
                        depressed
                        :loading="googleLoading"
                        block
                        height="40"
                        sel="gmail"
                        color="#da6156"
                        class="btns white--text mb-6"
                    >
                        <img width="40" src="../../../../../authenticationPage/images/G icon@2x.png" />
                        <span class="googleBtnText" v-t="'loginRegister_getstarted_btn_google_signup'"></span>
                    </v-btn>

                    <div class="d-flex justify-center text-center mb-6">

                        <div class="divider"></div>
                        <div class="or">OR</div>
                        <div class="divider"></div>

                    </div>
                </template>

                <component 
                    :is="component"
                    ref="childComponent"
                    :errors="errors"
                    :phone="phoneNumber"
                    :code="localCode"
                    @goStep="goStep"
                >
                </component>

            </div>

            <div class="bottom">
                <v-btn
                    type="submit"
                    depressed
                    height="40"
                    :loading="btnLoading && !googleLoading"
                    block
                    class="btns white--text"
                    color="#4452fc"
                >
                    <span v-t="'loginRegister_setemailpass_btn'"></span>
                </v-btn>

                <div class="termsWrap text-center" v-if="isEmailRegister">
                    <div class="my-3">
                        <span v-t="'loginRegister_getstarted_terms_i_agree'"></span>
                        <a class="link" :href="termsLink" v-t="'loginRegister_getstarted_terms_terms'"></a>
                        <span class="" v-t="'loginRegister_getstarted_terms_and'"></span>
                        <a class="link" :href="policyLink" v-t="'loginRegister_getstarted_terms_privacy'"></a>
                    </div>
                </div>

                <div class="getStartedBottom mt-2" v-if="isEmailRegister">    
                    <div class="text-center">
                        <span class="needAccount" v-t="'loginRegister_getstarted_signin_text'"></span>
                        <span class="link" v-t="'loginRegister_getstarted_signin_link'" @click="$emit('goTo', 'login')"></span>
                    </div>
                </div>
            </div>
        </v-form>

        <vue-recaptcha
            size="invisible"
            class="captcha"
            :sitekey="siteKey"
            ref="recaptcha"
            @verify="onVerify"
            @expired="onExpired()"
        /> 
    </div>
</template>

<script>
import registrationService from '../../../../../../../services/registrationService2';

import analyticsService from '../../../../../../../services/analytics.service.js';

import authMixin from '../authMixin'

import VueRecaptcha from "vue-recaptcha";

const emailRegister = () => import('./emailRegister.vue');
const setPhone2 = () => import('./setPhone2.vue');
const verifyPhone = () => import('./verifyPhone.vue');

export default {
    components: { emailRegister, setPhone2, verifyPhone, VueRecaptcha },
    mixins: [authMixin],
    data() {
        return {
            component: 'setPhone2',
            googleLoading: false,
            recaptcha: "",
            siteKey: '6LfyBqwUAAAAAM-inDEzhgI2Cjf2OKH0IZbWPbQA',
            localCode: '',
            phoneNumber: '',
            errors: {
                gmail: '',
                phone: '',
                code: '',
                email: '',
                password: '',
            }
        }
    },
    computed: {
        isEmailRegister() {
            return this.component === 'emailRegister'
        },
        termsLink() {
            let isFrymo = this.$store.getters.isFrymo
            return isFrymo ? 'https://help.frymo.com/en/article/terms' : 'https://help.spitball.co/en/article/terms-of-service'
        },
        policyLink() {
            let isFrymo = this.$store.getters.isFrymo
            return isFrymo ? 'https://help.frymo.com/en/policies' : 'https://help.spitball.co/en/article/privacy-policy'
        }
    },
    methods: {
        openLoginDialog() {
            this.$store.commit('setToaster', 'login')
        },
        submit() {
            let formValidate = this.$refs.form.validate()
            if(formValidate) {
                switch(this.component) {
                    case 'emailRegister':
                        this.$refs.recaptcha.execute()
                        break;
                    case 'setPhone2':
                        this.sendSms()
                        break;
                    case 'verifyPhone':
                        this.verifyPhone()
                        break;
                    default:
                        return                       
                }
            }
        },
        onVerify(response) {
            this.recaptcha = response
            this.emailRegister()
        },
        onExpired() {
            this.recaptcha = ''
            this.$refs.recaptcha.reset();
        },
        emailRegister() {
            let childComp = this.$refs.childComponent
            let emailRegister = {
                firstName: childComp.firstName,
                lastName: childComp.lastName,
                email: childComp.email,
                // gender: childComp.gender,
                password: childComp.password,
                captcha: this.recaptcha
            }

            let self = this
            registrationService.emailRegistration(emailRegister)
                .then(({data}) => {
                    analyticsService.sb_unitedEvent('Registration', 'Start')
                    if (data && data.param && data.param.phoneNumber) {
                        self.component = data.step.name
                        self.phoneNumber = data.param.phoneNumber
                        return
                    }
                    self.component = 'setPhone2'
                }).catch(error => {
                    let { response: { data } } = error

                    // if(data.Email) self.errors.email = self.$t('loginRegister_invalid_email')
                    // if(data.Password) self.errors.password = self.$t('loginRegister_invalid_password')

                    self.errors.email = data["Email"] ? data["Email"][0] : '', // TODO
                    self.errors.password = data["Password"] ? data["Password"][0] : '' // TODO
                    self.$appInsights.trackException({exception: new Error(error)});
                }).finally(() => {
                    self.$refs.recaptcha.reset()
                })
        },
        sendSms(){
            let childComp = this.$refs.childComponent
            let smsObj = {
                countryCode: childComp.localCode,
                phoneNumber: childComp.phoneNumber
            }

            let self = this
            registrationService.smsRegistration(smsObj)
                .then(function (){
                    let { dispatch } = self.$store

                    dispatch('updateToasterParams',{
                        toasterText: self.$t("login_verification_code_sent_to_phone"),
                        showToaster: true,
                    });
                    analyticsService.sb_unitedEvent('Registration', 'Phone Submitted');
                    self.component = 'verifyPhone'
                }).catch(error => {
                    let { response: { data } } = error
                    
                    // if(data.Phone) self.errors.phone = self.$t('loginRegister_invalid_phone_number')

                    self.errors.phone = data["PhoneNumber"] ? data["PhoneNumber"][0] : '' // TODO:
                    self.$appInsights.trackException({exception: new Error(error)});
                })
        },
        verifyPhone(){
            let childComp = this.$refs.childComponent

			let self = this
			registrationService.smsCodeVerification({number: childComp.smsCode})
				.then(userId => {
                    let { commit, dispatch } = self.$store

                    analyticsService.sb_unitedEvent('Registration', 'Phone Verified');
                    if(!!userId){
                        analyticsService.sb_unitedEvent('Registration', 'User Id', userId.data.id);
                    }

					commit('setToaster', '')
					commit('changeLoginStatus', true)
					dispatch('userStatus');
				}).catch(error => {
                    self.errors.code = self.$t('loginRegister_invalid_code')
                    self.$appInsights.trackException({exception: new Error(error)});
                })
        },
        goStep(step) {
            this.component = step
        },
    },
    created() {      
        let captchaLangCode = global.lang === "he" ? "iw" : "en";
        this.$loadScript(`https://www.google.com/recaptcha/api.js?onload=vueRecaptchaApiLoaded&render=explicit&hl=${captchaLangCode}`);
    },
};
</script>

<style lang="less">
@import '../../../../../../../styles/mixin.less';

.registerDialog {
    background: #fff;
    position: relative;
    @media (max-width: @screen-xs) {
        height: 100%;
    }
    .closeIcon {
        position: absolute;
        right: 16px;
    }
    .divider {
        width: 140px;
        border-bottom: 1px solid #ddd;
        margin: 0 10px 7px;
    }
    .or {
        color: @global-purple;
        font-weight: 600;
    }
    .registerForm {
        height: inherit;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        .mainTitle {
            .responsive-property(font-size, 20px, null, 22px);
            color: @color-login-text-title;
            font-weight: 600;
        }
        .googleBtnText {
            margin-bottom: 2px;
        }
        .v-label {
            color: @global-purple;
        }
    }
    .btns {
        border-radius: 6px;
    }
    .bottom {
        .termsWrap {
            color: @global-purple;
            font-size: 10px;
            border-bottom: 1px solid #dddddd;

            .link {
                color: #4c59ff;
                text-decoration: underline;
            }
        }
        .getStartedBottom {
            .responsive-property(font-size, 14px, null, 14px);
                .link {
                    cursor: pointer;  
                    color: #4c59ff;
                    font-weight: 600;
                }
            .needAccount {
                color: @global-purple;
            }
        }
    }
}
.captcha {
    .grecaptcha-badge {
        @media (max-width: @screen-xs) {
            bottom: 150px !important;
        }
    }
}
</style>