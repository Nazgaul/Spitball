<template>
  <div class="smsConfirmation">
    <div class="top">
      <p v-language:inner="'loginRegister_smsconfirm_title'"/>
	  <span>
		  <span v-language:inner="'loginRegister_smsconfirm_subtitle'"/>
		  <span>{{userPhone}}</span>
	  </span>
    </div>
	<sb-input 
			icon="sbf-key"
			v-model="code"
			class="code"
			:bottomError="true" 
			:errorMessage="errorMessages.code"
			placeholder="loginRegister_smsconfirm_input" 
			name="phone" :type="'number'"
			v-language:placeholder/>

    <v-btn color="#304FFE"
			:loading="isLoading"
			:disabled="smsCode.length < 5" 
			@click="verifyPhone"
			large round 
			class="white--text">
                <span v-language:inner="'loginRegister_smsconfirm_btn'"></span>
                </v-btn>

    <div class="bottom">
      <span @click="phoneCall" class="top" v-language:inner="'loginRegister_smsconfirm_call'"/>
      <span @click="numberChange" v-language:inner="'loginRegister_smsconfirm_change'"/>
    </div>
  </div>
</template>

<script>
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import { mapGetters, mapActions } from 'vuex';

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
			return `(+${this.getLocalCode}) ${this.getPhone}`
		},
		isLoading(){
			return this.getGlobalLoading
		},
		code: {
			get(val){
				return this.smsCode
			},
			set(val){
				this.smsCode = val
				this.updateSmsCode(val)
			}
		}
	},
	methods: {
		...mapActions(['updateSmsCode','smsCodeVerify','callWithCode','changeNumber']),
		verifyPhone(){
			this.smsCodeVerify()
		},
		phoneCall(){
			this.callWithCode()
		},
		numberChange(){
			this.changeNumber()
		}
	}
};
</script>


<style lang="less">
@import '../../../styles/mixin.less';

.smsConfirmation {
  .top {
	  	display: flex;
		flex-direction: column;
		align-items: center;
		margin-bottom: 64px;
		p {
			font-size: 28px;
			line-height: 1.54;
			letter-spacing: -0.51px;
			text-align: center;
			color: #434c5f;
			margin-bottom: 8px;
		}
		span{
			font-size: 16px;
			letter-spacing: -0.42px;
			text-align: center;
			color: #888b8e;
		}
  }
    .code{
        input {
        position: relative;
        border-radius: 4px;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.26);
        border: solid 1px rgba(55, 81, 255, 0.29);
        background-color: #ffffff;
        padding: 10px !important;
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
        .responsive-property(width, 100%, null, 90%);
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
		color: #4452fc;
		.top{
			margin-bottom: 12px;
		}
		span{
			cursor: pointer;
		}
	 }
}
</style>

