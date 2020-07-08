<template>
    <v-form class="loginForm pa-4" @submit.prevent="submit" ref="form">
        <div class="top">
            <div class="closeIcon" v-if="!isStudyRoomRoute">
                <v-icon size="12" color="#aaa" @click="closeRegister">sbf-close</v-icon>
            </div>

            <div class="loginTitle text-center mb-8"  v-t="'loginRegister_setemail_title'"></div>

            <v-btn
                @click="gmailRegister"
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
            <component
                :is="component"
                ref="childComponent"
                :email="email"
                :errors="errors"
                @updateEmail="updateEmail"
            >
            </component>
        </div>

        <div class="bottom text-center mt-6">
            <span class="helpLinks" @click="linksAction" v-t="'loginRegister_setpass_forgot'"></span>
            <v-btn
                type="submit"
                depressed
                height="40"
                :loading="btnLoading && !googleLoading"
                block
                class="btns white--text mt-6"
                color="#4452fc"
            >
                <span v-t="'loginRegister_setemail_btn'"></span>
            </v-btn>
            
            <div class="getStartedBottom mt-2">
                <div class="text-center mt-2">
                    <span class="needAccount" v-t="'loginRegister_getstarted_signup_text'"></span>
                    <span class="link" v-t="'loginRegister_getstarted_signup_link'" @click="$emit('goTo', 'register')"></span>
                </div>
            </div>
        </div>
    </v-form>
</template>

<script>
import authMixin from '../authMixin'
import { LoginForgotPassword } from '../../../../../../../routes/routeNames'

const loginDetails = () => import('./loginDetails.vue')
import gIcon from '../images/g-icon.svg'

export default {
    mixins: [authMixin],
    components: {
        loginDetails,
        gIcon
    },
    data() {
        return {
            email: '',
            component: 'loginDetails',
        }
    },
    methods: {
        updateEmail(email) {
            this.email = email
        },
        closeRegister() {
            this.$store.commit('setComponent', '')
            this.$store.commit('setRequestTutor')
        },
        submit() {
            let formValidate = this.$refs.form.validate()
            if(formValidate) {
                this.login()
            }
        },
        linksAction() {
            this.errors.email = ''
            this.errors.password = ''
            this.$router.push({name: LoginForgotPassword, params: { email: this.email }})
            this.$store.commit('setComponent', '')
        }
    }
}
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

