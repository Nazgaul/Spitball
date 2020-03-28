<template>
  <div class="smsConfirmation text-center">

    <div class="top">
      	<p class="smsconfirm_title mb-6" v-t="'loginRegister_smsconfirm_title'"></p>
		<span>
			<div v-t="'loginRegister_smsconfirm_subtitle'"></div>
			<bdi>{{userPhone}}</bdi>
		</span>
    </div>

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

	<div class="bottom">
		<div class="cursor mb-sm-2 mb-4">
			<span  @click="phoneCall" v-t="'loginRegister_smsconfirm_call'"></span>
		</div>
		<div class="cursor">
			<span @click="$emit('goStep', 'setPhone2')" v-t="'loginRegister_smsconfirm_change'"></span>
		</div>
	</div>

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
		phoneCall(){
			let self = this
			registrationService.voiceConfirmation()
            	.then(() => {
					self.$store.dispatch('updateToasterParams',{
						toasterText: self.$t("login_call_code"),
						showToaster: true,
					});
				}, ex => {
					self.$appInsights.trackException({exception: new Error(ex)});
				});
		}
	}
};
</script>


<style lang="less">
@import '../../../../../../../styles/mixin.less';
@import '../../../../../../../styles/colors.less';

.smsConfirmation {
  	.top {
		.responsive-property(margin-bottom, 64px, null, 32px);
		.smsconfirm_title {
			.responsive-property(font-size, 28px, null, 22px);
			color: @color-login-text-title;
			margin-bottom: 8px;
		}
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
	.bottom {
		.cursor{
			color: @global-blue;
			span {
				cursor: pointer;
			}
		}
	}
}
</style>

