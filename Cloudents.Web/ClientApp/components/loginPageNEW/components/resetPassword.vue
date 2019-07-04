<template>
    <div class="resetPassword">
        <p v-language:inner="'loginRegister_resetpass_title'"/>
        <sb-input
                :errorMessage="errorMessages.password"
                :hint="passHint"
                :class="hintClass"
                v-model="password"
				placeholder="loginRegister_resetpass_input"
				:bottomError="true"
				:autofocus="true" 
				name="pass" type="password"/>

		<sb-input 
                :errorMessage="errorMessages.confirmPassword"
                v-model="passwordConfirm"
                class="mt-4"
				placeholder="loginRegister_resetpass_input_confirm"  
				:bottomError="true" 
				type="password" name="pass"
				:autofocus="true"/>

        <v-btn  @click="change"
                :disabled="!isPassValid"
                :loading="loading"
                color="#304FFE" large round 
                class="white--text">
                <span v-language:inner="'loginRegister_resetpass_btn'"></span>
                </v-btn>
    </div>
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
			passwordConfirm: '',
         	score: {
				default: 0,
				required: false
			},
		}
    },
	watch: {
		password: function (val){
			this.updatePassword(val)
		},
		passwordConfirm: function(val){
			this.updateConfirmPassword(val)
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
        },
        isPassValid(){
            return (this.password.length >= 8 && this.passwordConfirm.length >= 8)
        }
    },
    methods: {
        ...mapActions(['updatePassword','updateConfirmPassword','changePassword','updateStep']),
        change(){
            let id = this.$route.query['Id'] ? this.$route.query['Id'] : '';           
            let code = this.$route.query['code'] ? this.$route.query['code'] : '';
            this.changePassword({id,code})
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
    .resetPassword{
    text-align: center;
        p {
        font-size: 28px;
        line-height: 1.54;
        letter-spacing: -0.51px;
        color: #434c5f;
        margin-bottom: 62px;
        }
        .input-wrapper {
            input {
            border: none;
            border-radius: 4px;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.26);
            border: solid 1px rgba(55, 81, 255, 0.29);
            background-color: #ffffff;
            padding: 10px !important;
            }
        }
        button{
        margin: 30px 0 0;
        .responsive-property(width, 100%, null, 90%);
        font-size: 16px;
        font-weight: 600;
        letter-spacing: -0.42px;
        text-align: center;
        text-transform: none !important;
        }
    }
</style>

