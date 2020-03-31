<template>
    <div class="registerDialog">
        <v-form @submit.prevent="submit" ref="form" class="registerForm pa-4">  
            <div>
                <div class="closeIcon">
                    <v-icon size="12" color="#aaa" @click="$store.commit('setComponent', '')">sbf-close</v-icon>
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
                        <div class="or" v-t="'loginRegister_or'"></div>
                        <div class="divider"></div>
                    </div>
                </template>

                <component 
                    :is="component"
                    ref="childComponent"
                    :errors="errors"
                    :phone="phoneNumber"
                    :code="localCode"
                    :teacher="teacher"
                    @goStep="goStep"
                    @updatePhone="updatePhone"
                    @updateCode="updateCode"
                >
                </component>
            </div>


            <div class="bottom">

                <template v-if="isVerifyPhone">
                    <div class="verifyPhone mb-11">
                        <div class="d-flex justify-center text-center mb-6">
                            <div class="divider"></div>
                            <div class="otherMethod" v-t="'loginRegister_choose_other_method'"></div>
                            <div class="divider"></div>
                        </div>

                        <div class="methods d-flex justify-space-between">
                            <div class="linkAction d-flex" @click="phoneCall">
                                <phoneCall />
                                <div class="ml-2" v-t="'loginRegister_change_number'"></div>
                            </div>
                            <div class="linkAction d-flex">
                                <changeNumber />
                                <div @click="goStep('setPhone2')" class="ml-2" v-t="'loginRegister_change_numb'"></div>
                            </div>
                        </div>
                    </div>
                </template>

                <v-btn
                    type="submit"
                    depressed
                    height="40"
                    :loading="btnLoading && !googleLoading"
                    block
                    class="btns white--text"
                    color="#4452fc"
                >
                    <span v-t="globalBtnText"></span>
                </v-btn>

                <template v-if="isEmailRegister">
                    <div class="termsWrap text-center">
                        <div class="my-3">
                            <span v-t="'loginRegister_getstarted_terms_i_agree'"></span>
                            <a class="link" :href="termsLink" target="_blank" v-t="'loginRegister_getstarted_terms_terms'"></a>
                            <span class="" v-t="'loginRegister_getstarted_terms_and'"></span>
                            <a class="link" :href="policyLink" target="_blank" v-t="'loginRegister_getstarted_terms_privacy'"></a>
                        </div>
                    </div>

                    <div class="getStartedBottom mt-2">    
                        <div class="text-center">
                            <span class="needAccount" v-t="'loginRegister_getstarted_signin_text'"></span>
                            <span class="link" v-t="'loginRegister_getstarted_signin_link'" @click="$emit('goTo', 'login')"></span>
                        </div>
                    </div>

                </template>
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

import changeNumber from '../images/changeNumber.svg'
import phoneCall from '../images/phoneCall.svg'

export default {
    mixins: [authMixin],
    components: {
        emailRegister,
        setPhone2,
        verifyPhone,
        VueRecaptcha,
        changeNumber,
        phoneCall
    },
    props: {
        params: {},
        teacher: {
            type: Boolean,
            default: false
        }
    },
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
        globalBtnText() {
            return this.isVerifyPhone ? 'loginRegister_setemailpass_btn_verify' : 'loginRegister_setemailpass_btn'
        },
        isEmailRegister() {
            return this.component === 'emailRegister'
        },
        isVerifyPhone() {
            return this.component === 'verifyPhone'
        },
        isFromTutorReuqest() {
            return this.$store.getters.getIsFromTutorStep
        }
    },
    methods: {
        openLoginDialog() {
            this.$store.commit('setComponent', 'login')
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
                    self.errors.phone = ''
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

					commit('setComponent', '')
                    commit('changeLoginStatus', true)

                    // this is when user statr register from tutorRequest
                    if(self.isFromTutorReuqest) {
                        self.$store.dispatch('updateRequestDialog', true);
                        self.$store.dispatch('updateTutorReqStep', 'tutorRequestSuccess')
                        return
                    }

					dispatch('userStatus').then(user => {
                        // when user is register and pick teacher, redirect him to his profile page
                        if(self.teacher) {
                            self.$router.push({
                                name: self.routeNames.Profile,
                                params: {
                                    id: user.id,
                                    name: user.name,
                                },
                                query: {
                                    dialog: 'becomeTutor'
                                }
                            })
                        }
                    })
				}).catch(error => {
                    self.errors.code = self.$t('loginRegister_invalid_code')
                    self.$appInsights.trackException({exception: new Error(error)});
                })
        },
        phoneCall(){
			let self = this
			registrationService.voiceConfirmation()
            	.then(() => {
					self.$store.dispatch('updateToasterParams',{
						toasterText: self.$t("login_call_code"),
						showToaster: true,
					});
				}).catch(error => {
                    self.$appInsights.trackException({exception: new Error(error)});
                })
		},
        goStep(step) {
            this.component = step
        },
        updatePhone(phone) {
            this.phoneNumber = phone
        },
        updateCode(code) {
            this.localCode = code
        }
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
        .divider {
            width: 140px;
            border-bottom: 1px solid #ddd;
            margin: 0 10px 7px;
        }
        .or {
            color: @global-purple;
            font-weight: 600;
        }
        .otherMethod {
            color: @global-purple;
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
                color: @global-auth-text;
                text-decoration: underline;
            }
        }
        .verifyPhone {
            color: @global-auth-text;
            .methods {
                .linkAction {
                    cursor: pointer;
                }
            }
        }
        .getStartedBottom {
            .responsive-property(font-size, 14px, null, 14px);
                .link {
                    cursor: pointer;  
                    color: @global-auth-text;
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
            bottom: 135px !important;
        }
    }
}
</style>