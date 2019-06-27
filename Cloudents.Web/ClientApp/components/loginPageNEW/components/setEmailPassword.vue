<template>
    <section class="setEmailPassword">
        <p>Set your email and password</p>
        <form>
					<sb-input 	icon="sbf-email" :bottomError="true"
									v-model="email"
									placeholder="login_placeholder_email" name="email" type="email"
									:autofocus="true" v-language:placeholder required/>

					<sb-input   :bottomError="true" class="mt-4" v-model="password"
									placeholder="login_placeholder_choose_password"  name="pass"
									:type="'password'"
									:autofocus="true"  v-language:placeholder></sb-input>

					<sb-input   class="mt-4" :bottomError="true" v-model="passwordConfirm"
									placeholder="login_placeholder_confirm_password" name="confirm" type="password"
									:autofocus="true" v-language:placeholder></sb-input>
					<vue-recaptcha class="mt-4"
										:sitekey="siteKey"
										ref="recaptcha"
										@verify="onVerify"
										@expired="onExpired()">
					</vue-recaptcha>
										
					<v-btn :loading="isEmailLoading" @click="register" color="#304FFE" large round class="ctnBtn white--text">Continue</v-btn>
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
	 computed: {
		 ...mapGetters(['getEmail','getSiteKey','getEmailLoading']),
		 siteKey(){ 
			 return this.getSiteKey
		 },
		 isEmailLoading(){
			 return this.getEmailLoading
		 },
		email:{
			get(){
				return this.getEmail
			},
			set(val){
				this.updateEmail(val)
			}
		},
		password:{
			get(){},
			set(val){
				this.updatePassword(val)
			}
		},
		passwordConfirm:{
			get(){},
			set(val){
				this.updateConfirmPassword(val)
			}
		},
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
		}
	 },
	 created() {
		let captchaLangCode = global.lang === 'he' ? 'iw' : 'en';
      this.$loadScript(`https://www.google.com/recaptcha/api.js?onload=vueRecaptchaApiLoaded&render=explicit&hl=${captchaLangCode}`);
	 },
}
</script>

<style lang='less'>
    .setEmailPassword{
		p{
		font-size: 28px;
		line-height: 1.54;
		letter-spacing: -0.51px;
		text-align: center;
		color: #434c5f;
		margin-bottom: 64px;
		}
		form{
			.blabla{
				width: 100px;
				height: 150px;
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
				}
				i {
					position: absolute;
					left: 16px;
					top: 17px;
					font-size: 18px;
				}
			}
			.ctnBtn{
				width: 400px;
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
