<template>
    <form class="forgotPass" @submit.prevent="resetPass">
        <div class="top">
            <p v-language:inner="'loginRegister_forgot_title'"/>
            <span v-language:inner="'loginRegister_forgot_subtitle'"/>
        </div>

        <sb-input 
                v-model="email"
                class="widther"
                :errorMessage="errorMessages.email"
				placeholder="loginRegister_setpass_input_email"
				icon="sbf-email" 
				:bottomError="true"
				:autofocus="true" 
				name="email" type="email"/>
        <v-btn  :loading="isEmailLoading"
                type="submit"
                large rounded 
                class="white--text btn-login">
                <span v-language:inner="'loginRegister_forgot_btn'"></span>
                </v-btn>
        <span class="bottom" @click="goLogin" v-language:inner="'loginRegister_forgot_remember'"/>
    </form>
</template>

<script>
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import { mapActions, mapGetters,mapMutations } from 'vuex';

export default {
    name: 'forgotPass',
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
        ...mapActions(['updateStep','updateEmail','resetPassword']),
        ...mapMutations(['setErrorMessages']),
        goLogin(){
            this.updateStep('setPassword')
        },
        resetPass(){
            this.resetPassword()
        }
    },
    watch: {
    email: function(val){
        this.setErrorMessages({})
    }
	}
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
@import '../../../styles/colors.less';
    .forgotPass{
              @media (max-width: @screen-xs) {
        display: flex;
        flex-direction: column;
        align-items: center;
      }
        text-align: center;
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
            span{
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
            left: 16px;
            top: 17px;
            font-size: 18px;
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
        .bottom{
            cursor: pointer;
            font-size: 14px;
		    letter-spacing: -0.37px;
		    color: @global-blue;
        }
    }
</style>

