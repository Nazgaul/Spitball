<template>
    <form class="setPassword" @submit.prevent="login">
		<p v-language:inner="'loginRegister_setemail_title'"/>
        <sb-input 
                class="widther"
				v-model="email"
				placeholder="loginRegister_setpass_input_email"
				icon="sbf-email" 
				:bottomError="true"
				:autofocus="false" 
                :errorMessage="errorMessages.email"
				name="email" type="email"/>

		<sb-input 
                class="mt-4 widther"
                icon="sbf-key"
				v-model="password"
				placeholder="loginRegister_setpass_input_pass"  
				:bottomError="true" 
				type="password" name="pass"
				:autofocus="true"/>
        <v-btn  
                type="submit"
                :loading="isEmailLoading"
                large rounded 
                class="white--text btn-login">
                <span v-language:inner="'loginRegister_setpass_btn'"></span>
                </v-btn>

        <router-link to="" class="bottom" @click.native="goForgotPassword" v-language:inner="'loginRegister_setpass_forgot'"/>
    </form>    
</template>

<script>
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import { mapGetters, mapActions, mapMutations } from 'vuex';

export default {
    name: 'setPassword',
    components:{
        SbInput
    },
    data() {
        return {
            password: '',
        }
    },
    watch: {
        password: function(val){
            this.setErrorMessages({})
        },
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
                this.setErrorMessages({})
            }
        }
    },
    methods: {
        ...mapActions(['updateEmail','logIn','updateStep']),
        ...mapMutations(['setErrorMessages']),
        login(){
            this.logIn(this.password)
        },
        goForgotPassword(){
            this.updateStep('forgotPass')
        }
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
@import '../../../styles/colors.less';

    .setPassword{
      @media (max-width: @screen-xs) {
        display: flex;
        flex-direction: column;
        align-items: center;
      }
        text-align: center;
        p {
        .responsive-property(font-size, 28px, null, 22px);
        .responsive-property(letter-spacing, -0.51px, null, -0.4px);
        line-height: 1.54;
        color: @color-login-text-title;
        }
        .input-wrapper {
        .responsive-property(margin-top, 62px, null, 16px);
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

