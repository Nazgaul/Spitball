<template>
    <v-dialog :value="true" max-width="620px" content-class="registerDialog" persistent :fullscreen="$vuetify.breakpoint.xsOnly">
        <div class="text-right pa-4 pb-0">
            <v-icon size="14" color="grey" @click="$store.commit('setRegisterDialog', false)">sbf-close</v-icon>
        </div>
        <v-form @submit.prevent="submit" ref="form" class="form pa-4 pt-0">

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
            </template>
            
            <component :is="component" ref="childComponent" @goStep="goStep" class="mt-5"></component>

            <div class="text-left" v-if="isEmailRegister">
                <div class="mb-4">
                    <span class="paddingHelper" v-t="'loginRegister_getstarted_terms_i_agree'"></span>
                    <a :href="isFrymo ? 'https://help.frymo.com/en/article/terms' : 'https://help.spitball.co/en/article/terms-of-service'" class="terms paddingHelper" v-t="'loginRegister_getstarted_terms_terms'"></a>
                    <span class="paddingHelper" v-t="'loginRegister_getstarted_terms_and'"></span>
                    <a :href="isFrymo ? 'https://help.frymo.com/en/policies' : 'https://help.spitball.co/en/article/privacy-policy'" class="terms" v-t="'loginRegister_getstarted_terms_privacy'"></a>
                </div>
                
                <v-btn
                    type="submit"
                    depressed
                    large
                    :loading="btnLoading && !googleLoading"
                    block
                    rounded
                    class="ctnBtn white--text btn-login"
                    color="primary"
                >
                    <span v-t="'loginRegister_setemailpass_btn'"></span>
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
import registrationService from '../../../../../../services/registrationService2';

import storeService from "../../../../../../services/store/storeService";
import loginRegister from "../../../../../../store/loginRegister";

import * as routeNames from '../../../../../../routes/routeNames'

import authMixin from '../../../../../mixins/authMixin'

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
            recaptcha: "",
            siteKey: '6LfyBqwUAAAAAM-inDEzhgI2Cjf2OKH0IZbWPbQA',
            routeNames,
        }
    },
    computed: {
        isFrymo() {
            return this.$store.getters.isFrymo;
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
            if(form.validate()) {
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
                gender: childComp.gender,
                password: childComp.password,
                captcha: this.recaptcha
            }

            let self = this
            registrationService.emailRegistration2(emailRegister)
                .then(() => {
                    self.component = 'setPhone2'
                }).catch(error => {
                    self.$store.commit('setErrorMessages',{
                        email: error.response.data["Email"] ? error.response.data["Email"][0] : '',
                        password: error.response.data["Password"] ? error.response.data["Password"][0] : '',
                    });
                    self.$appInsights.trackException({exception: new Error(error)});
                }).finally(() => {
                    self.$refs.recaptcha.reset()
                })
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
};
</script>

<style lang="less">
@import '../../../../../../styles/mixin.less';

.registerDialog {
    background: #fff;
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
}
</style>