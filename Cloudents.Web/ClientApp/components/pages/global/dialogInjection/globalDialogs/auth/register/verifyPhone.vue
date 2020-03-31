<template>
  <div class="smsConfirmation text-center">

    <!-- <div class="top"> -->
	<div class="mainTitle mb-8" v-t="'loginRegister_verifyPhone_main_title'"></div>
	<i18n path="loginRegister_verifyPhone_subtitle" tag="div" class="subTitle mb-9">
		<bdi>{{userPhone}}</bdi>
	</i18n>
      	<!-- <p class="smsconfirm_title mb-6" v-t="'loginRegister_smsconfirm_title'"></p> -->
		<!-- <span>
			<div v-t="'loginRegister_smsconfirm_subtitle'"></div>
			<bdi>{{userPhone}}</bdi>
		</span> -->
    <!-- </div> -->

	<v-text-field
		v-model="smsCode"
		class="code widther"
		color="#304FFE"
		:rules="[rules.required]"
		:error-messages="errors.code"
		:label="$t('loginRegister_smsconfirm_input')"
		height="44"
		outlined
		dense
		prepend-inner-icon="sbf-keyCode"
		placeholder=" "
	>
	</v-text-field>

	<span class="resendCode" @click="resendCode" v-t="'loginRegister_smsconfirm_resend_code'"></span>

	<!-- <div class="bottom">
		<div class="cursor mb-sm-2 mb-4">
			<span  @click="phoneCall" v-t="'loginRegister_smsconfirm_call'"></span>
		</div>
		<div class="cursor">
			<span @click="$emit('goStep', 'setPhone2')" v-t="'loginRegister_smsconfirm_change'"></span>
		</div>
	</div> -->

  </div>
</template>

<script>
import registrationService from '../../../../../../../services/registrationService2';

import { validationRules } from '../../../../../../../services/utilities/formValidationRules2';

export default {
	props: {
		errors: {
			type: Object
		},
		phone: {
			type: String,
			required: true,
			default: ''
		},
		code: {
			type: String,
			required: true,
			default: ''
		}
	},
	data() {
		return {
			smsCode: '',
			rules: {
                required: (value) => validationRules.required(value)
            },
		}
	},
	watch: {
        smsCode(){
			if(this.errors.code) {
				this.errors.code = ''
			}
		}
    },
	computed: {
		userPhone(){
			if (this.code) {
				return `${this.phone} (${this.code}+)`
			}
			return this.phone;
		},
	},
	methods: {
		resendCode() {
			let self = this
			registrationService.resendCode()
            	.then(() => {
					self.$store.dispatch('updateToasterParams',{
                        toasterText: self.$t("login_verification_code_sent_to_phone"),
                        showToaster: true,
                    });
				}).catch(error => {
					self.$appInsights.trackException({exception: new Error(error)});
				})
		}
	}
};
</script>


<style lang="less">
@import '../../../../../../../styles/mixin.less';
@import '../../../../../../../styles/colors.less';

.smsConfirmation {
  	.top {
		.smsconfirm_title {
			.responsive-property(font-size, 28px, null, 22px);
			color: @color-login-text-title;
			margin-bottom: 8px;
		}
		.mainTitle
		.subTitle
		span{
			.responsive-property(font-size, 16px, null, 14px);
			color: @color-login-text-subtitle;
		}
  	}
    .code{
		i {
			color: #4a4a4a;
			font-size: 16px;
			margin-top: 10px;
		}
	 }
	.resendCode {
		cursor: pointer;
		color: @global-blue;
		font-weight: 600;
	}
	// .bottom {
	// 	.cursor{
	// 		color: @global-blue;
	// 		span {
	// 			cursor: pointer;
	// 		}
	// 	}
	// }
}
</style>

