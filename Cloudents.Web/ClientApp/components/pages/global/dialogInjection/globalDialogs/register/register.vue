<template>
    <v-dialog :value="true" max-width="620px" persistent>

        <v-form @submit.prevent="submit" ref="form" class="form pa-4">

            <template v-if="isEmailRegister">
                <v-btn 
                    @click="gmailRegister"
                    depressed
                    :loading="googleLoading"
                    rounded
                    sel="gmail"
                    color="primary"
                    class="google btn-login"
                >
                    <img width="40" src="../../../../authenticationPage/images/G icon@2x.png" />
                    <span class="btnText" v-t="'loginRegister_getstarted_btn_google_signup'"></span>
                </v-btn>

                <div class="getStartedTerms">
                    <div class="lineTerms">
                        <v-checkbox
                            v-model="isTermsAgree"
                            @click="checkBoxConfirm"
                            class="checkboxUserinfo"
                            :ripple="false"
                            name="checkBox"
                            sel="check"
                            off-icon="sbf-check-box-un"
                            on-icon="sbf-check-box-done"
                            id="checkBox"
                        >
                        </v-checkbox>
                        <label for="checkBox">
                            <span>
                                <span class="paddingHelper" v-t="'loginRegister_getstarted_terms_i_agree'"></span>
                                <a :href="isFrymo ? 'https://help.frymo.com/en/article/terms' : 'https://help.spitball.co/en/article/terms-of-service'" class="terms paddingHelper" v-t="'loginRegister_getstarted_terms_terms'"></a>
                                <span class="paddingHelper" v-t="'loginRegister_getstarted_terms_and'"></span>
                                <a :href="isFrymo ? 'https://help.frymo.com/en/policies' : 'https://help.spitball.co/en/article/privacy-policy'" class="terms" v-t="'loginRegister_getstarted_terms_privacy'"></a>
                            </span>
                        </label>
                    </div>
                    <span v-if="isError" class="errorMsg" v-t="'login_please_agree'"></span>
                </div>
            </template>
            
            <component :is="component" ref="childComponent" @goStep="goStep"></component>

            <div class="text-right">
                <v-btn
                    v-if="isEmailRegister"
                    type="submit"
                    depressed
                    large
                    :loading="btnLoading"
                    block
                    rounded
                    class="ctnBtn white--text btn-login"
                    color="primary"
                >
                    <span>{{$t('loginRegister_setemailpass_btn')}}</span>
                </v-btn>
            </div>

            <vue-recaptcha
                size="invisible"
                class="captcha"
                :sitekey="siteKey"
                ref="recaptcha"
                @verify="onVerify"
                @expired="onExpired()"
            />

        </v-form>        
    </v-dialog>
</template>

<script>
import analyticsService from '../../../../../../services/analytics.service.js';
import registrationService from '../../../../../../services/registrationService';

import storeService from "../../../../../../services/store/storeService";
import loginRegister from "../../../../../../store/loginRegister";

import * as routeNames from '../../../../../../routes/routeNames'

import VueRecaptcha from "vue-recaptcha";

const emailRegister = () => import('./emailRegister.vue');
const setPhone2 = () => import('./setPhone2.vue');
const verifyPhone = () => import('./verifyPhone.vue');

export default {
    components: { emailRegister, setPhone2, verifyPhone, VueRecaptcha },
    data() {
        return {
            component: 'emailRegister',
            isTermsAgree: false,
            googleLoading: false,
            showError: false,
            recaptcha: "",
            siteKey: '6LfyBqwUAAAAAM-inDEzhgI2Cjf2OKH0IZbWPbQA',
            routeNames,
        }
    },
    computed: {
        isFrymo() {
            return this.$store.getters.isFrymo;
        },
        isError(){
            return !this.isTermsAgree && this.showError
        },
        btnLoading() {
            return this.$store.getters.getGlobalLoading
        },
        isEmailRegister() {
            return this.component === 'emailRegister'
        }
    },
    methods: {
        submit() {
            let form = this.$refs.form

            if(form.validate() && this.isTermsAgree) {
                this.$refs.recaptcha.execute()
            } else {
                this.showError = true;
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

        gmailRegister() {
            if(!this.isTermsAgree) return this.showError = true;

            this.googleLoading = true;
            let self = this
            registrationService.googleRegistration().then(({data}) => {
                if (!data.isSignedIn) {
                analyticsService.sb_unitedEvent('Registration', 'Start Google');
                self.component = 'setPhone2'
            } else {
                analyticsService.sb_unitedEvent('Login', 'Start Google');
                self.$store.dispatch('updateLoginStatus',true)
            }
            }).catch(error => {
                console.log(error);
                self.$store.commit('setErrorMessages', { gmail: error.response.data["Google"] ? error.response.data["Google"][0] : '' });
                self.$appInsights.trackException({exception: new Error(error)});
            }).finally(() => {
                self.$store.commit('setRegisterDialog', false)
            })
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
            registrationService.emailRegistration2(emailRegister).then(res => {
                console.log(res);
                self.component = 'setPhone2'
            }).catch(ex => {
                console.log(ex);
                self.$appInsights.trackException({exception: new Error(ex)});
            })
        },
        checkBoxConfirm(){
            if(this.isTermsAgree) return this.showError = false

            this.showError = true
        },
        goStep(step) {
            this.component = step
        }
    },
    beforeDestroy() {
        storeService.unregisterModule(this.$store, "loginRegister");
    },
    created() {
        storeService.registerModule(this.$store, "loginRegister", loginRegister);
        let captchaLangCode = global.lang === "he" ? "iw" : "en";
        this.$loadScript(`https://www.google.com/recaptcha/api.js?onload=vueRecaptchaApiLoaded&render=explicit&hl=${captchaLangCode}`);
    },
    mounted() {
        let self = this;
        this.$nextTick(function () {
            this.$loadScript("https://apis.google.com/js/client:platform.js").then(()=>{
                self.$store.dispatch('gapiLoad');
            }).catch(ex => {
                console.log(ex);
                self.$appInsights.trackException({exception: new Error(ex)});
            })
        });
    }
};
</script>

<style lang="less">
@import '../../../../../../styles/mixin.less';

.form {
    background: #fff;
    .responsive-property(width, 100%, null, 72%);
    &.google {
        .responsive-property(margin-bottom, 0px, null, 20px);
        color: white;
        .btnText{
            font-size: 16px;
            color: white;
            font-weight: 600;
            margin: 0 22px 0 8px;
        }
        .v-btn__loading{
            color: white;
        }
        .v-btn__content {
            margin: 0;
        }
    }
    .getStartedTerms{
        margin-bottom: 34px;
        align-items: center;
        flex-direction: column;
        .responsive-property(margin-bottom, 34px, null, 66px);
        .errorMsg {
            display: block; 
            font-weight: normal;
            color:red; 
            text-align: center;
            font-size: 14px;
            letter-spacing: -0.36px;
        }
        .lineTerms {
            display: flex;
            align-items: inherit;
            .checkboxUserinfo {
                .v-input__slot {
                    display: flex;
                    align-items: unset;
                    margin-bottom: 6px;
                    .v-icon{
                        color: @global-blue !important;
                    }
                    .v-messages{
                        display: none;
                    }
                }
            }
            input{
                padding-left: 12px;
                width: 25px;
                height: 25px;
            }
            span {
                padding: 0;
                color:#212121;
                .responsive-property(font-size, 13px, null, 12px);
                font-weight: initial;
                &.paddingHelper{
                    padding-right: 2px;
                }
                .terms {
                    color: @global-blue; 
                    text-decoration: underline;
                }
            }
        }
    }
}
</style>