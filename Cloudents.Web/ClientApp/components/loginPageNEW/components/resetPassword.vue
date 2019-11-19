<template>
    <form class="resetPassword" @submit.prevent="change">
        <p v-language:inner="'loginRegister_resetpass_title'"/>
        <sb-input
                :errorMessage="errorMessages.password"
                :hint="passHint"
                :class="[hintClass,'widther']"
                v-model="password"
				placeholder="loginRegister_resetpass_input"
				:bottomError="true"
				:autofocus="true" 
				name="pass" type="password"/>

		<sb-input 
                :errorMessage="errorMessages.confirmPassword"
                v-model="confirmPassword"
                class="mt-4 widther"
				placeholder="loginRegister_resetpass_input_confirm"  
				:bottomError="true" 
				type="password" name="pass"
				:autofocus="false"/>

        <v-btn  type="submit"
                :loading="loading"
                large rounded 
                class="white--text btn-login">
                <span v-language:inner="'loginRegister_resetpass_btn'"></span>
                </v-btn>
    </form>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import SbInput from "../../question/helpers/sbInput/sbInput.vue";

export default {
    name: 'resetPassword',
    components:{
        SbInput
    },
	data() {
		return {
			password: '',
			confirmPassword: '',
         	score: {
				default: 0,
				required: false
			},
		}
    },
    computed: {
        ...mapGetters(['getGlobalLoading','getPassScoreObj','getErrorMessages']),
        loading(){
            return this.getGlobalLoading
        },
        errorMessages(){
			return this.getErrorMessages
		},
        passHint(){
			if(this.password.length > 0){
			let passScoreObj = this.getPassScoreObj
				this.score = global.zxcvbn(this.password).score;
				return `${passScoreObj[this.score].name}`
			}
        },
        hintClass(){
			let passScoreObj = this.getPassScoreObj
			if(this.passHint){
				return passScoreObj[this.score].className;
			} 
        }
    },
    methods: {
        ...mapActions(['changePassword','updateStep']),
        change(){
            let paramsObj = {
                id: this.$route.query['Id'] ? this.$route.query['Id'] : '',
                code: this.$route.query['code'] ? this.$route.query['code'] : '',
                password: this.password,
                confirmPassword: this.confirmPassword
            }
            this.changePassword(paramsObj)
        }
    },
    created(){
        if(!this.$route.query['code'] || !this.$route.query['Id']){
            this.updateStep('getStarted')
        }
        this.$loadScript("https://unpkg.com/zxcvbn@4.4.2/dist/zxcvbn.js");
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
@import '../../../styles/colors.less';
    .resetPassword{
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
        .responsive-property(margin-bottom, 62px, null, 32px);

        }
        .input-wrapper {
            input {
            .login-inputs-style();
            }
        }
        button{
        margin: 30px 0 0;
        @media (max-width: @screen-xs) {
            margin: 50px 0 0;
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

