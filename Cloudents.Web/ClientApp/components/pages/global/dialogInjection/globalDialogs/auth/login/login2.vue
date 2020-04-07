<template>
    <v-form class="loginForm pa-4" @submit.prevent="submit" ref="form">
        <div class="top">
            <div class="closeIcon">
                <v-icon size="12" color="#aaa" @click="closeRegister">sbf-close</v-icon>
            </div>

            <template v-if="isLoginDetails">
                <div class="loginTitle text-center mb-8"  v-t="'loginRegister_setemail_title'"></div>

                <v-btn
                    @click="gmailRegister"
                    :loading="googleLoading"
                    class="btns google white--text mb-6"
                    sel="gmail"
                    block
                    height="40"
                    color="#da6156"
                    depressed
                >
                    <gIcon class="mr-2" />
                    <span class="googleBtnText" v-t="'loginRegister_getstarted_btn_google_signin'"></span>
                </v-btn>
            </template>

            <component
                :is="component"
                ref="childComponent"
                :email="email"
                :phone="phoneNumber"
                :code="localCode"
                :errors="errors"
                @updateEmail="updateEmail"
                @updatePhone="updatePhone"
                @updateCode="updateCode"
            >
            </component>
        </div>

        <div class="bottom text-center mt-6">
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
            <!-- <span class="helpLinks" @click="linksAction" v-t="remmberForgotLink" v-if="component !== 'resetPassword'"></span> -->
            <span class="helpLinks" @click="linksAction" v-if="isLoginDetails" v-t="remmberForgotLink"></span>
            <v-btn
                type="submit"
                depressed
                height="40"
                :loading="btnLoading && !googleLoading"
                block
                class="btns white--text mt-6"
                color="#4452fc"
            >
                <span v-t="btnResource"></span>
            </v-btn>
            
            <div class="getStartedBottom mt-2" v-if="isLoginDetails">
                <div class="text-center mt-2">
                    <span class="needAccount" v-t="'loginRegister_getstarted_signup_text'"></span>
                    <span class="link" v-t="'loginRegister_getstarted_signup_link'" @click="$emit('goTo', 'register')"></span>
                </div>
            </div>
        </div>
    </v-form>
</template>

<script>
import registrationService from '../../../../../../../services/registrationService2';
import analyticsService from '../../../../../../../services/analytics.service.js';

import authMixin from '../authMixin'

const loginDetails = () => import('./loginDetails.vue')
const setPhone2 = () => import('../register/setPhone2.vue');
const verifyPhone = () => import('../register/verifyPhone.vue');

import phoneCall from '../images/phoneCall.svg'
import changeNumber from '../images/changeNumber.svg'

// const forgotPassword = () => import('./forgotPassword.vue')
// const resetPassword = () => import('./resetPassword2.vue')
import gIcon from '../images/g-icon.svg'

export default {
    mixins: [authMixin],
    components: {
        gIcon,
        loginDetails,
        setPhone2,
        verifyPhone,
        phoneCall,
        changeNumber
        // forgotPassword,
        // resetPassword
    },
    data() {
        return {
            email: '',
            component: 'loginDetails',
        }
    },
    computed: {
        isLoginDetails() {
            return this.component === 'loginDetails'
        },
        btnResource() {
            let resource = {
                loginDetails: 'loginRegister_setemail_btn',
                setPhone2: 'loginRegister_setemailpass_btn',
                verifyPhone: 'loginRegister_setemailpass_btn_verify'
                // forgotPassword: 'loginRegister_forgot_btn',
                // resetPassword: 'loginRegister_resest_btn',
            }
            return resource[this.component]
        },
        remmberForgotLink() {
            return this.isLoginDetails ? 'loginRegister_setpass_forgot' : 'loginRegister_forgot_remember'
        }
    },
    methods: {
        closeRegister() {
            this.$store.commit('setComponent', '')
            this.$store.commit('setRequestTutor')
        },
        submit() {
            let formValidate = this.$refs.form.validate()
            if(formValidate) {
                switch(this.component) {
                    case 'loginDetails':
                        this.login()
                        break;
                    case 'setPhone2':
                        this.sendSms()
                        break;
                    case 'verifyPhone':
                        this.verifyPhone()
                        break;
                    // case 'forgotPassword':
                    //     this.forgotPassword()
                    //     break;
                    // case 'resetPassword':
                    //     this.resetPassword()
                    //     break;
                    default:
                        return                        
                }
            }
        },
        login(){
            let childComp = this.$refs.childComponent
            let loginObj = {
                email: childComp.email,
                password: childComp.password
            }

            let self = this
            registrationService.emailLogin(loginObj)
                .then(({data}) => {
                    let { commit, dispatch } = self.$store

                    global.country = data.country; // should we need this? @idan
                    analyticsService.sb_unitedEvent('Login', 'Start');

                    commit('setComponent', '')
                    dispatch('updateLoginStatus', true)
                    
                    if(self.$route.path === '/' || self.$route.path === '/learn') {
                        self.$router.push({name: self.routeNames.LoginRedirect})
                        return
                    }

                    dispatch('userStatus')
                }).catch(error => {      
                    let { response: { data } } = error

                    self.errors.password = data["Password"] ? error.response.data["Password"][0] : ''
                    self.$appInsights.trackException({exception: new Error(error)})
                })
        },
        // forgotPassword() {
        //     let self = this
        //     registrationService.resetPassword({email: this.email})
        //         .then(() =>{
        //             analyticsService.sb_unitedEvent('Forgot Password', 'Reset email send');
        //             // TODO: what is the next step @idan
        //         }).catch(error => {
        //             let { response: { data } } = error

        //             self.errors.email = data["ForgotPassword"] ? data["ForgotPassword"][0] : data["Email"][0]
        //             // if(data.Password) {
        //             //     self.errors.password = self.$t('loginRegister_error_forgot_password')
        //             // }
        //             // if(data.Email) {
        //             //     self.errors.email = self.$t('loginRegister_error_forgot_email')
        //             // }
        //             self.$appInsights.trackException({exception: new Error(error)})
        //         })
        // },
        // resetPassword() {
        //     let childComp = this.$refs.childComponent
        //     let passwordObj = {
        //         id: childComp.id,
        //         code: childComp.code,
        //         password: childComp.password
        //     }
            
        //     let self = this
        //     registrationService.updatePassword(passwordObj)
        //         .then(() => {
        //             console.log();
        //         }).catch(error => {
        //             let { response: { data } } = error

        //             self.errors.password = data["Password"] ? data["Password"][0] : data["ConfirmPassword"][0]
        //             self.$appInsights.trackException({exception: new Error(error)})
        //         })
        // },
        updateEmail(email) {
            this.email = email
        },
        linksAction() {
            this.errors.email = ''
            this.errors.password = ''
            
            if(this.isLoginDetails) {
                // this.component = 'forgotPassword'
                this.$router.push({name: 'forgotPassword', params: { email: this.email }})
                this.$store.commit('setComponent', '')
                return
            }
            this.component = 'loginDetails'
        }
    },
};
</script>

<style lang="less">
@import '../../../../../../../styles/mixin.less';
@import '../../../../../../../styles/colors.less';

.loginForm {
    background: #ffffff;
    position: relative;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    @media (max-width: @screen-xs) {
        height: 100% !important;
    }
    .closeIcon {
        position: absolute;
        right: 16px;
    }
    .loginTitle {
        .responsive-property(font-size, 20px, null, 22px);
        color: @color-login-text-title;
        font-weight: 600;
    }
    .bottom {
        .helpLinks {
            cursor: pointer;
            font-size: 14px;
            letter-spacing: -0.37px;
            color: @global-blue;
        }
        .btns {
            border-radius: 6px;
        }
        .verifyPhone {
            color: @global-auth-text;
            .methods {
                .linkAction {
                    cursor: pointer;
                }
            }
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
</style>

