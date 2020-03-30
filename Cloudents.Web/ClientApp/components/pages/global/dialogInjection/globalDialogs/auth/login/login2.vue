<template>
    <v-form class="loginForm pa-4" @submit.prevent="submit" ref="form">
        <div class="top">
            <div class="closeIcon">
                <v-icon size="14" color="" @click="$store.commit('setToaster', '')">sbf-close</v-icon>
            </div>

            <template v-if="isLoginDetails">
                <div class="loginTitle text-center mb-8"  v-t="'loginRegister_setemail_title'"></div>

                <v-btn
                    @click="gmailRegister"
                    :loading="googleLoading"
                    class="btns google white--text"
                    sel="gmail"
                    block
                    height="40"
                    color="#da6156"
                    depressed
                >
                    <img width="40" src="../../../../../authenticationPage/images/G icon@2x.png" />
                    <span class="googleBtnText" v-t="'loginRegister_getstarted_btn_google_signin'"></span>
                </v-btn>
            </template>


            <component
                :is="component"
                ref="childComponent"
                class="mt-6"
                :email="email"
                :errors="errors"
                @updateEmail="updateEmail"
            >
            </component>
        </div>

        <div class="bottom text-center mt-6">
            <span class="helpLinks" @click="linksAction" v-t="remmberForgotLink"></span>
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

            
            <div class="getStartedBottom mt-2">    
                <div class="termsWrap text-center">
                    <div class="my-3">
                        <span v-t="'loginRegister_getstarted_terms_i_agree'"></span>
                        <a class="link" :href="termsLink" v-t="'loginRegister_getstarted_terms_terms'"></a>
                        <span class="" v-t="'loginRegister_getstarted_terms_and'"></span>
                        <a class="link" :href="policyLink" v-t="'loginRegister_getstarted_terms_privacy'"></a>
                    </div>
                </div>
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
// const forgotPassword = () => import('./forgotPassword.vue')

export default {
    mixins: [authMixin],
    components: {
        loginDetails,
        // forgotPassword
    },
    data() {
        return {
            email: '',
            component: 'loginDetails',
            errors: {
                gmail: '',
                email: '',
                password: ''
            }
        }
    },
    computed: {
        isLoginDetails() {
            return this.component === 'loginDetails'
        },
        btnResource() {
            return this.isLoginDetails ? 'loginRegister_setemail_btn' : 'loginRegister_forgot_btn'
        },
        remmberForgotLink() {
            return this.isLoginDetails ? 'loginRegister_setpass_forgot' : 'loginRegister_forgot_remember'
        }
    },
    methods: {
        submit() {
            let formValidate = this.$refs.form.validate()

            if(formValidate) {
                switch(this.component) {
                    case 'loginDetails':
                        this.login()
                        break;
                    // case 'forgotPassword':
                    //     this.forgotPassword()
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

                    global.country = data.country; // TODO: should we need this? @idan

                    analyticsService.sb_unitedEvent('Login', 'Start');
                    commit('setToaster', '')
                    dispatch('updateLoginStatus', true)
                    
                    if(self.$route.path === '/') {
                        this.$router.push({name: this.routeNames.LoginRedirect})
                    }
                }).catch(error => {      
                    let { response: { data } } = error

                    self.errors.email = data["Password"] ? error.response.data["Password"][0] : '' //TODO:
                    self.errors.password = data["Password"] ? error.response.data["Password"][0] : '' //TODO:

                    // if(data.Locked) {
                    //     self.errors.password = self.$t('loginRegister_error_locked_user')
                    // }
                    // if(data.Wrong) {
                    //     self.errors.email = self.$t('loginRegister_error_wrong_password')
                    //     self.errors.password = self.$t('loginRegister_error_wrong_password')
                    // }
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
        updateEmail(email) {
            this.email = email
        },
        linksAction() {
            this.errors.email = ''
            this.errors.password = ''
            
            if(this.isLoginDetails) {
                // this.component = 'forgotPassword'
                this.$router.push({name: 'forgotPassword', params: { email: this.email }})
                this.$store.commit('setToaster', '')
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
    height: 100%;
    position: relative;
    @media (max-width: @screen-xs) {
        display: flex;
        flex-direction: column;
        justify-content: space-between;
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
    .googleBtnText {
        margin-bottom: 2px;
    }
    .bottom {
        .helpLinks {
            cursor: pointer;
            font-size: 14px;
            letter-spacing: -0.37px;
            color: @global-blue;
        }
        .termsWrap {
            color: @global-purple;
            font-size: 10px;
            border-bottom: 1px solid #dddddd;

            .link {
                color: @global-auth-text;
                text-decoration: underline;
            }
        }
        .btns {
            border-radius: 6px;
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

