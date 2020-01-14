<template>
  <form class="smsConfirmation" @submit.prevent="verifyPhone">
    <div class="top">
      <p v-language:inner="'loginRegister_smsconfirm_title'"/>
	  <span>
		  <span v-language:inner="'loginRegister_smsconfirm_subtitle'"/>
		  <span> {{userPhone}}</span>
	  </span>
    </div>
	<sb-input 
			icon="sbf-key"
			v-model="smsCode"
			class="code widther"
			:bottomError="true" 
			:autofocus="true"
			:errorMessage="errorMessages.code"
			placeholder="loginRegister_smsconfirm_input" 
			name="phone" :type="'number'"
			v-language:placeholder/>

    <v-btn 	:loading="isLoading"
        	type="submit"
			large rounded 
			class="white--text btn-login">
                <span v-language:inner="'loginRegister_smsconfirm_btn'"></span>
                </v-btn>

    <div class="bottom">
      <span @click="phoneCall" class="top" v-language:inner="'loginRegister_smsconfirm_call'"/>
      <span @click="numberChange" v-language:inner="'loginRegister_smsconfirm_change'"/>
    </div>
  </form>
</template>

<script>
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import { mapGetters, mapActions, mapMutations } from 'vuex';

export default {
	name: 'VerifyPhone',
	data() {
		return {
			smsCode: '',
		}
	},
	components :{
		SbInput
	},
	computed: {
		...mapGetters(['getLocalCode','getPhone','getGlobalLoading','getErrorMessages']),
		errorMessages(){
			return this.getErrorMessages
		},
		userPhone(){
			return global.isRtl? `${this.getPhone} (${this.getLocalCode}+)` : `(+${this.getLocalCode}) ${this.getPhone}`
		},
		isLoading(){
			return this.getGlobalLoading
		},
	},
	methods: {
		...mapActions(['smsCodeVerify','callWithCode','changeNumber']),
		...mapMutations(['setErrorMessages']),
		verifyPhone(){
			this.smsCodeVerify(this.smsCode)
		},
		phoneCall(){
			this.callWithCode()
		},
		numberChange(){
			this.changeNumber()
		}
	},
	  watch: {
        smsCode: function(){
            this.setErrorMessages({})
		}
	}
};
</script>


<style lang="less">
@import '../../../styles/mixin.less';
@import '../../../styles/colors.less';

.smsConfirmation {
	@media (max-width: @screen-xs) {
        display: flex;
        flex-direction: column;
        align-items: center;
	}
  .top {
	  	display: flex;
		flex-direction: column;
		align-items: center;
		.responsive-property(margin-bottom, 64px, null, 32px);
		p {
			.responsive-property(font-size, 28px, null, 22px);
			.responsive-property(letter-spacing, -0.51px, null, -0.4px);
			line-height: 1.54;
			text-align: center;
			color: @color-login-text-title;
			margin-bottom: 8px;
		}
		span{
			.responsive-property(font-size, 16px, null, 14px);
			.responsive-property(letter-spacing, -0.42px, null, -0.37px);
			text-align: center;
			color: @color-login-text-subtitle;
		}
  }
    .code{
		input[type=number]::-webkit-inner-spin-button, 
        input[type=number]::-webkit-outer-spin-button { 
        -webkit-appearance: none; 
        margin: 0; 
        }
        input {
        position: relative;
        .login-inputs-style();
        padding-left: 40px !important;
            ~ i {
                position: absolute;
                top: 14px;
                left: 12px;
            }
        }
	 }
	button{
		margin: 66px 0 48px;
	@media (max-width: @screen-xs) {
		margin: 48px 0 48px;


	}
		.responsive-property(width, 100%, null, 72%);
		font-size: 16px;
		font-weight: 600;
		letter-spacing: -0.42px;
		text-align: center;
		text-transform: none !important;
	 }
	.bottom{
		font-size: 14px;
		letter-spacing: -0.37px;
		text-align: center;
		color: @global-blue;
		.top{
			margin-bottom: 12px;
		}
		span{
			cursor: pointer;
		}
	 }
}
</style>

