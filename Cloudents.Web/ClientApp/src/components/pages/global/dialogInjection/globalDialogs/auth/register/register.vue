<template>
    <div class="registerDialog">
        <v-form @submit.prevent="submit" ref="form" class="registerForm pa-4">  
            <div>
                <div class="closeIcon" v-if="!isStudyRoomRoute">
                    <v-icon size="12" color="#aaa" @click="closeRegister">sbf-close</v-icon>
                </div>

                <div class="mainTitle text-center mb-8" v-t="'loginRegister_setemailpass_title'"></div>
<template v-if="cIsWebView">
                <v-btn 
                    
                    @click="gmailRegister"
                    depressed
                    block
                    height="40"
                    sel="gmail"
                    color="#da6156"
                    class="btns white--text mb-6"
                >
                    <gIcon class="mr-2" />
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
                    :teacher="teacher"
                >
                </component>
            </div>

            <div class="bottom">
                <v-btn
                    type="submit"
                    depressed
                    height="40"
                    :loading="btnLoading"
                    block
                    class="btns white--text"
                    color="#4452fc"
                >
                    <span v-t="'loginRegister_setemailpass_btn'"></span>
                </v-btn>

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

            </div>
        </v-form>

        <vue-recaptcha
            size="invisible"
            class="captcha"
            :sitekey="siteKey"
            ref="recaptcha"
            @verify="onVerify"
            @expired="onExpired"
        /> 
    </div>
</template>

<script>
import registrationService from '../../../../../../../services/registrationService2';
import analyticsService from '../../../../../../../services/analytics.service.js';
import authMixin from '../authMixin'
import VueRecaptcha from "vue-recaptcha";

const emailRegister = () => import('./emailRegister.vue');

import gIcon from '../images/g-icon.svg'

export default {
    mixins: [authMixin],
    components: {
        emailRegister,
        VueRecaptcha,
        gIcon
    },
    props: {
        params: {},
    },
  
    data() {
        return {
            component: 'emailRegister',
            googleLoading: false,
            recaptcha: "",
            siteKey: '6LfyBqwUAAAAAM-inDEzhgI2Cjf2OKH0IZbWPbQA',
        }
    },
    methods: {
        closeRegister() {
            if(this.$route.query.teacher) {
                this.$router.push('/')
            }
            this.$store.commit('setComponent', '')
            this.$store.commit('setRequestTutor')
        },
        submit() {
            let formValidate = this.$refs.form.validate()
            if(formValidate) {
                this.$refs.recaptcha.execute()
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
                password: childComp.password,
                captcha: this.recaptcha,
                userType: this.teacher ? 'tutor' : 'student'
            }
            let self = this
            registrationService.emailRegistration(emailRegister)
                .then(() => {
                    analyticsService.sb_unitedEvent('Registration', 'Start')

                    if(self.presetRouting()) return

                    window.location.reload()
                }).catch(error => {
                    let { response: { data } } = error

                    self.errors.email = data["Email"] ? data["Email"][0] : '',
                    self.errors.password = data["Password"] ? data["Password"][0] : ''

                    self.$appInsights.trackException(error);
                }).finally(() => {
                    self.$refs.recaptcha.reset()
                })
        }
    },
    created() {
        if(this.params.goTo) {
            this.component = this.params.goTo
        }
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
        height: 100% !important;
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