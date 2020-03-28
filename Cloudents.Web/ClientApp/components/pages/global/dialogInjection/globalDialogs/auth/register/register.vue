<template>
    <v-dialog :value="true" max-width="620px" content-class="authDialog" persistent :fullscreen="$vuetify.breakpoint.xsOnly">

        <v-form @submit.prevent="submit" ref="form" class="registerForm pa-4">  
            <div>
                <div class="text-right">
                    <v-icon size="14" color="" @click="$store.commit('setRegisterDialog', false)">sbf-close</v-icon>
                </div>

                <component 
                    :is="component"
                    ref="childComponent"
                    class=""
                    :errors="errors"
                    :phone="phoneNumber"
                    :code="localCode"
                    @goStep="goStep"
                >
                </component>

                <template v-if="isEmailRegister">
                    <v-btn 
                        @click="gmailRegister"
                        depressed
                        :loading="googleLoading"
                        rounded
                        sel="gmail"
                        color="#304FFE"
                        class="white--text mt-4"
                    >
                        <img width="40" src="../../../../../authenticationPage/images/G icon@2x.png" />
                        <span class="googleBtn" v-t="'loginRegister_getstarted_btn_google_signup'"></span>
                    </v-btn>
                </template>
            </div>

            <div class="bottom mt-8">
                <div class="text-center" v-if="isEmailRegister">
                    <div class="mb-4">
                        <span v-t="'loginRegister_getstarted_terms_i_agree'"></span>
                        <a :href="termsLink" v-t="'loginRegister_getstarted_terms_terms'"></a>
                        <span class="" v-t="'loginRegister_getstarted_terms_and'"></span>
                        <a :href="policyLink" v-t="'loginRegister_getstarted_terms_privacy'"></a>
                    </div>
                </div>
                <v-btn
                    type="submit"
                    depressed
                    large
                    :loading="btnLoading && !googleLoading"
                    v-t="'loginRegister_setemailpass_btn'"
                    block
                    rounded
                    class="white--text"
                    color="#304FFE"
                >
                </v-btn>
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
    </v-dialog>
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
            component: 'emailRegister',
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
                gender: childComp.gender,
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

                    if(data.Email) self.errors.email = self.$t('loginRegister_invalid_email')
                    if(data.Password) self.errors.password = self.$t('loginRegister_invalid_password')
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
                    
                    if(data.Phone) self.errors.phone = self.$t('loginRegister_invalid_phone_number')
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

					commit('setRegisterDialog', false)
					commit('changeLoginStatus', true)
					dispatch('userStatus');
				}).catch(error => {
                    let { response: { data } } = error

                    if(data.Code) self.errors.code = self.$t('loginRegister_invalid_code')
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

.authDialog {
    background: #fff;
    .registerForm {
        height: 100%;
        display: flex;
        flex-direction: column;
        justify-content: space-between;

        .googleBtn {
            margin-bottom: 2px;
        }
    }
    .captcha {
        .grecaptcha-badge {
            @media (max-width: @screen-xs) {
                bottom: 150px !important;
            }
        }
    }
}
</style>