<template>
    <v-dialog :value="true" max-width="620px" content-class="loginDialog" persistent :fullscreen="$vuetify.breakpoint.xsOnly">

        <div class="text-right pa-4 pb-0">
            <v-icon size="14" color="grey" @click="$store.commit('setLoginDialog', false)">sbf-close</v-icon>
        </div>

        <component :is="component" :email="email"></component>
    </v-dialog>
</template>

<script>
import { mapMutations } from 'vuex';

import storeService from "../../../../../../services/store/storeService";
import loginRegister from "../../../../../../store/loginRegister";

import authMixin from '../../../../../mixins/authMixin'

const login = () => import('./login.vue')
const forgotPass = () => import('./forgotPassword.vue')

export default {
    mixins: [authMixin],
    components: {
        login,
        forgotPass
    },
    data() {
        return {
            email: '',
            component: 'login'
        }
    },
    methods: {
        ...mapMutations(['setErrorMessages']),
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
        // margin: 45px 0 0;
        // @media (max-width: @screen-xs) {
        //     display: flex;
        //     flex-direction: column;
        //     align-items: center;
        // }


    // .responsive-property(width, 100%, null, 72%);
    // font-size: 16px;
    // font-weight: 600;
    // letter-spacing: -0.42px;
    // text-align: center;
    // text-transform: none !important;
  }
}
</style>

