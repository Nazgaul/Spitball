<template>
    <form class="forgotPass" @submit.prevent="resetPass">
        <div class="top">
            <p v-t="'loginRegister_forgot_title'"></p>
            <span v-t="'loginRegister_forgot_subtitle'"></span>
        </div>

        <sb-input 
            v-model="email"
            class="widther"
            :errorMessage="errorMessages.email"
            :placeholder="$t('loginRegister_setpass_input_email')"
            icon="sbf-email" 
            :bottomError="true"
            :autofocus="true" 
            name="email" type="email"/>
        <v-btn  
            :loading="isEmailLoading"
            type="submit"
            large rounded 
            class="white--text btn-login">
                <span>{{$t('loginRegister_forgot_btn')}}</span>
        </v-btn>

        <button class="bottom" @click="rememberNow" v-t="'loginRegister_forgot_remember'"></button>
    </form>
</template>

<script>
import SbInput from "../../../question/helpers/sbInput/sbInput.vue";
import { mapActions, mapGetters,mapMutations } from 'vuex';

export default {
    components:{
        SbInput
    },
    computed: {
        ...mapGetters(['getEmail1','getErrorMessages','getGlobalLoading']),
        isEmailLoading(){
            return this.getGlobalLoading
        },
        errorMessages(){
            return this.getErrorMessages
        },
        email:{
            get(){
               return this.getEmail1
            },
            set(val){
                this.updateEmail(val)
            },
        }
    },
    methods: {
        ...mapActions(['updateEmail', 'resetPassword']),
        ...mapMutations(['setErrorMessages']),
        resetPass(){
            this.resetPassword()
        },
        rememberNow() {
            this.$router.push('/')
            this.$store.commit('setComponent', 'login')
        }
    },
    watch: {
        email: function(){
            this.setErrorMessages({})
        }
    },
    mounted() {
        if(this.$route.params.email) {
            this.updateEmail(this.$route.params.email)
        }
        this.setErrorMessages({})
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';
    .forgotPass{
        text-align: center;
        @media (max-width: @screen-xs) {
            display: flex;
            flex-direction: column;
            align-items: center;
        }
        .top{
                display: flex;
                flex-direction: column;
                align-items: center;
                margin-bottom: 30px;
            p{
                .responsive-property(font-size, 28px, null, 22px);
                .responsive-property(letter-spacing, -0.51px, null, -0.4px);
                margin: 0;
                line-height: 1.54;
                color: @color-login-text-title;
            }
            span {
                cursor: initial;
                .responsive-property(font-size, 16px, null, 14px);
                .responsive-property(letter-spacing, -0.42px, null, -0.37px);
                padding-top: 8px;
                color: @color-login-text-subtitle;
            }
        }
        .input-wrapper {
            input {
                .login-inputs-style();
                padding-left: 54px !important;
            }
            i {
                position: absolute;
                left: 12px;
                top: 10px;
                font-size: 31px;
            }
        }
        button{
            margin: 66px 0 48px;
            @media (max-width: @screen-xs) {
                margin: 45px 0 48px;
            }
            .responsive-property(width, 100%, null, 72%);
            font-size: 16px;
            font-weight: 600;
            letter-spacing: -0.42px;
            text-align: center;
            text-transform: none !important;
        }
        .bottom {
            cursor: pointer;
            font-size: 14px;
		    letter-spacing: -0.37px;
		    color: @global-blue;
        }
    }
</style>

