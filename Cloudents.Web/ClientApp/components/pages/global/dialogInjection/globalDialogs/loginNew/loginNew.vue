<template>
    <v-dialog :value="true" max-width="620px" content-class="loginDialog" persistent :fullscreen="$vuetify.breakpoint.xsOnly">
        <p class="loginTitle">{{$t('loginRegister_setemail_title')}}</p>
        <v-form class="setEmail pa-4" @submit.prevent="validate" ref="form">

            <v-text-field 
                v-model="email"
                class="widther input-fields" 
                color="#304FFE"
                outlined
                height="44" 
                dense
                :label="labels['email']"
                :error-messages="errorMessages.email"
                :rules="[rules.required, rules.email]"
                placeholder=" "
                type="email"
            >
            </v-text-field>

            <v-text-field 
                v-model="password"
                :class="[hintClass,'widther','input-fields','mb-3']"
                color="#304FFE"
                outlined
                height="44"
                dense
                :label="labels['password']"
                :error-messages="errorMessages.password"
                :rules="[rules.required, rules.minimumCharsPass]"
                placeholder=" "
                type="password"
                :hint="passHint"
            >
            </v-text-field>
            
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
                    <span>{{$t('loginRegister_setemail_btn')}}</span>
            </v-btn>

            <router-link :to="{name: 'forgotPassword'}" class="bottom text-center mt-4">{{$t('loginRegister_setpass_forgot')}}</router-link>
        </v-form>
    </v-dialog>
</template>

<script>
import { mapActions, mapMutations } from 'vuex';

import storeService from "../../../../../../services/store/storeService";
import loginRegister from "../../../../../../store/loginRegister";

import authMixin from '../../../../../mixins/authMixin'

import registrationService from '../../../../../../services/registrationService';

export default {
    mixins: [authMixin],
    data() {
        return {
            email: '',
            password: ''
        }
    },
    methods: {
        ...mapActions(['emailValidate']),
        ...mapMutations(['setErrorMessages']),
        validate(){
            registrationService.validateEmail(encodeURIComponent(this.email))
                .then(() => {
                    _analytics(['Login Email validation', 'email send']);
                    router.push({name: routeNames.LoginSetPassword});
                }, (error)=> {
                    commit('setErrorMessages',{email: error.response.data["Email"] ? error.response.data["Email"][0] : ''});
                });
        },

    },
    beforeDestroy() {
        storeService.unregisterModule(this.$store, "loginRegister");
    },
    created() {
        storeService.registerModule(this.$store, "loginRegister", loginRegister);
        this.$loadScript("https://unpkg.com/zxcvbn@4.4.2/dist/zxcvbn.js");
    },
};
</script>

<style lang="less">
@import '../../../../../../styles/mixin.less';
@import '../../../../../../styles/colors.less';
.loginDialog {
    background: #ffffff;
    @media (max-width: @screen-xs) {
        display: flex;
        flex-direction: column;
        align-items: center;
        width: auto;
    }
    .loginTitle {
        .responsive-property(font-size, 28px, null, 22px);
        .responsive-property(letter-spacing, -0.51px, null, -0.4px);
        line-height: 1.54;
        text-align: center;
        color: @color-login-text-title;
    }
//   .input-wrapper {
//     .responsive-property(margin-top, 62px, null, 16px);
//     input {
//       .login-inputs-style();
//       padding-left: 54px !important;
//     }
//     i {
//       position: absolute;
//       left: 12px;
//       top: 10px;
//       font-size: 31px;
//     }
//   }

    button {
        margin: 45px 0 0;
        @media (max-width: @screen-xs) {
            display: flex;
            flex-direction: column;
            align-items: center;
        }


    .responsive-property(width, 100%, null, 72%);
    font-size: 16px;
    font-weight: 600;
    letter-spacing: -0.42px;
    text-align: center;
    text-transform: none !important;
  }
}
</style>

