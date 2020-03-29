<template>
    <v-dialog :value="true" max-width="620px" content-class="loginDialog" persistent :fullscreen="$vuetify.breakpoint.xsOnly">

        <v-form class="loginForm pa-4" @submit.prevent="submit" ref="form">

            <div class="top">
                <div class="text-right">
                    <v-icon size="14" color="" @click="$store.commit('setLoginDialog', false)">sbf-close</v-icon>
                </div>

                <component
                    ref="childComponent"
                    :is="component"
                    :email="email"
                    :errors="errors"
                    @updateEmail="updateEmail"
                >
                </component>

                <v-btn
                    v-if="isLoginDetails"
                    @click="gmailRegister"
                    :loading="googleLoading"
                    class="white--text"
                    sel="gmail"
                    color="#304FFE"
                    height="40"
                    rounded
                    depressed
                >
                    <img width="40" src="../../../../../authenticationPage/images/G icon@2x.png" />
                    <span v-t="'loginRegister_getstarted_btn_google_signin'"></span>
                </v-btn>
            </div>

            <div class="bottom text-center mt-6">
                <span class="helpLinks" @click="linksAction" v-t="remmberForgotLink"></span>
                <v-btn
                    type="submit"
                    depressed
                    large
                    :loading="btnLoading && !googleLoading"
                    block
                    rounded
                    class="white--text mt-6"
                    color="#304FFE"
                >
                    <span v-t="btnResource"></span>
                </v-btn>
            </div>

        </v-form>
    </v-dialog>
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
                    commit('setLoginDialog', false)
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
                this.$store.commit('setLoginDialog', false)
                return
            }
            this.component = 'loginDetails'
        }
    }
};
</script>

<style lang="less">
@import '../../../../../../../styles/colors.less';
.loginDialog {
    background: #ffffff;
    .loginForm {
        height: 100%;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        .helpLinks {
            cursor: pointer;
            font-size: 14px;
            letter-spacing: -0.37px;
            color: @global-blue;
        }
    }
}
</style>

