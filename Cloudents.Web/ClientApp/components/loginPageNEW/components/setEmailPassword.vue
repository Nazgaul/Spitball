<template>
	<section class="setEmailPassword">
		<p v-language:inner="'loginRegister_setemailpass_title'"/>
		<form @submit.prevent="register">
			<sb-input 
				class="widther"
				v-model="email"
				placeholder="loginRegister_setemailpass_input_email"
				icon="sbf-email" 
				:bottomError="true"
				:autofocus="true" 
				:errorMessage="errorMessages.email"
				name="email" type="email"/>

			<sb-input 
				v-model="password"
				:class="['mt-4', hintClass,'widther']"
				:hint="passHint"
				placeholder="loginRegister_setemailpass_input_pass"  
				:errorMessage="errorMessages.password"
				:bottomError="true" 
				type="password" name="pass"
				:autofocus="true"/>

			<sb-input   
				class="mt-4 widther" 
				:errorMessage="errorMessages.confirmPassword"
				:bottomError="true" 
				v-model="passwordConfirm"
				placeholder="loginRegister_setemailpass_input_passconfirm" 
				name="confirm" type="password"
				:autofocus="true"/>

			<vue-recaptcha  class="mt-4 captcha"
							:sitekey="siteKey"
							ref="recaptcha"
							@verify="onVerify"
							@expired="onExpired()"/>

			<v-btn  type="submit"
					:disabled="!isFormValid" 
					:loading="isEmailLoading" 
					color="#304FFE" large round 
					class="ctnBtn white--text">
					<span v-language:inner="'loginRegister_setemailpass_btn'"></span>
			</v-btn>
		</form>
	</section>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import VueRecaptcha from 'vue-recaptcha';

export default {
	name:'setEmailPassword',
	components:{
		SbInput,
		VueRecaptcha
	},
	data() {
		return {
			password: '',
			passwordConfirm: '',
         	score: {
				default: 0,
				required: false
			},
		}
	},
	computed: {
		...mapGetters(['getEmail1','getSiteKey','getGlobalLoading',
						'getErrorMessages','getPassScoreObj','getIsFormValid',
						'getResetRecaptcha']),
		resetRecaptcha(){
			return this.getResetRecaptcha
		},
		isFormValid(){
			return this.getIsFormValid
		},
		passHint(){
			if(this.password.length > 0){
			let passScoreObj = this.getPassScoreObj
				this.score = global.zxcvbn(this.password).score;
				return `${passScoreObj[this.score].name}`
			}
		},
		errorMessages(){
			return this.getErrorMessages
		},
		siteKey(){ 
			return this.getSiteKey
		},
		isEmailLoading(){
			return this.getGlobalLoading
		},
		email:{
			get(){
				return this.getEmail1
			},
			set(val){
				this.updateEmail(val)
			}
		},
		hintClass(){
			let passScoreObj = this.getPassScoreObj
			if(this.passHint){
				return passScoreObj[this.score].className;
			} 
		},
	},
	watch: {
		password: function (val){
			this.updatePassword(val)
		},
		passwordConfirm: function(val){
			this.updateConfirmPassword(val)
		},
		resetRecaptcha: function(val){
			this.$refs.recaptcha.reset();
		}
	},
	methods: {
		...mapActions(['updateEmail','updatePassword','updateConfirmPassword','emailSigning','updateRecaptcha']),
		onVerify(response) {
			this.updateRecaptcha(response)
		},
		onExpired() {
			this.updateRecaptcha('')
		},
		register(){
			this.emailSigning()
		},
	},
	created() {
		this.$loadScript("https://unpkg.com/zxcvbn@4.4.2/dist/zxcvbn.js");
		let captchaLangCode = global.lang === 'he' ? 'iw' : 'en';
		this.$loadScript(`https://www.google.com/recaptcha/api.js?onload=vueRecaptchaApiLoaded&render=explicit&hl=${captchaLangCode}`);
	},
}
</script>

<style lang='less'>
@import '../../../styles/mixin.less';

	.setEmailPassword{
		@media (max-width: @screen-xs) {
			display: flex;
			flex-direction: column;
			align-content: flex-start
    	}
	p{
	.responsive-property(font-size, 28px, null, 22px);
    .responsive-property(letter-spacing, -0.51px, null, -0.4px);
	text-align: center;
	color: #434c5f;
    .responsive-property(margin-bottom, 64px, null, 34px);

	margin-bottom: 64px;
	}
	form{
				@media (max-width: @screen-xs) {
    display: flex;
    flex-direction: column;
    align-items: center;
    	}


			.widther{
				@media (max-width: @screen-xs) {
					width: 92%
    			}
			}
			.captcha{
				@media (max-width: @screen-xs) {
					width: 92%
    			}
			}
		.input-wrapper{
			input[type='password']{
				padding: 10px !important;
			}
			input {
				border: none;
				border-radius: 4px;
				box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.26);
				border: solid 1px rgba(55, 81, 255, 0.29);
				background-color: #ffffff;
				padding: 10px !important;
					padding-left: 54px !important;
				@media (max-width: @screen-xs) {
					padding-left: 45px !important;
					
    			}
            	.responsive-property(font-size, 16px, null, 14px);

			}
			i {
				position: absolute;
            	.responsive-property(left, 16px, null, 13px);	

				left: 16px;
				top: 17px;
            	.responsive-property(font-size, 16px, null, 14px);	
				font-size: 16px;
			}
		}
		.ctnBtn{
            .responsive-property(width, 100%, null, 90%);
			margin: 48px 0 0 0;
			font-size: 16px;
			font-weight: 600;
			letter-spacing: -0.42px;
			text-align: center;
			text-transform: none !important;
		}
	}
	}

</style>
