<template>
  <v-form class="smsConfirmation" @submit.prevent="verifyPhone" ref="form">

    <div class="top">
      	<p class="smsconfirm_title" v-t="'loginRegister_smsconfirm_title'"></p>
		<span>
			<div v-t="'loginRegister_smsconfirm_subtitle'"></div>
			<bdi>{{userPhone}}</bdi>
		</span>
    </div>

	<v-text-field
		v-model="smsCode"
		class="code widther"
		color="#304FFE"
		height="44"
		outlined
		dense
		prepend-inner-icon="sbf-keyCode"
		:error-messages="errorMessages.code"
		:label="$t('loginRegister_smsconfirm_input')"
		placeholder=" "
	></v-text-field>

	<div class="bottom">
		<v-btn 	
			:loading="getGlobalLoading"
			type="submit"
			large
			rounded
			block
			color="primary"
			class="white--text btn-login">
				<span v-t="'loginRegister_smsconfirm_btn'"></span>
		</v-btn>

		<div class="actions">
			<div class="mb-sm-2 mb-4" @click="phoneCall" v-t="'loginRegister_smsconfirm_call'"></div>
			<div @click="numberChange" v-t="'loginRegister_smsconfirm_change'"></div>
		</div>
	</div>

  </v-form>
</template>

<script>
import { mapGetters, mapMutations } from 'vuex';

import registrationService from '../../../../../../services/registrationService2.js'

import analyticsService from '../../../../../../services/analytics.service.js';

export default {
	data() {
		return {
			smsCode: '',
		}
	},
	watch: {
        smsCode: function(){
            this.setErrorMessages({})
		}
    },
	computed: {
		...mapGetters(['getLocalCode','getPhone','getGlobalLoading','getErrorMessages']),
		errorMessages(){
			return this.getErrorMessages
		},
		userPhone(){
			if (this.getLocalCode) {
				//todo this can be simplify due the use of bdi
				return `${this.getPhone} (${this.getLocalCode}+)`
				// return global.isRtl? `${this.getPhone} (${this.getLocalCode}+)` : `(+${this.getLocalCode}) ${this.getPhone}`
			}
			//this can happen when getting the info from the server
			return this.getPhone;
		}
	},
	methods: {
		...mapMutations(['setErrorMessages']),

		verifyPhone(){
			let self = this
			let smsCode = {number: this.smsCode}
			registrationService.smsCodeVerification(smsCode)
				.then(userId => {
					self.$store.commit('setRegisterDialog', false)
					analyticsService.sb_unitedEvent('Registration', 'Phone Verified');
					if(!!userId){
						analyticsService.sb_unitedEvent('Registration', 'User Id', userId.data.id);
					}
					self.$store.commit('changeLoginStatus', true)
					self.$store.dispatch('userStatus');
				}, ex => {
					self.$appInsights.trackException({exception: new Error(ex)});
					self.setErrorMessages({code: "Invalid code"});
				});
		},
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
		},
		numberChange(){
			this.$emit('goStep', 'setPhone2');
		}
	}
};
</script>


<style lang="less">
@import '../../../../../../styles/mixin.less';
@import '../../../../../../styles/colors.less';

.smsConfirmation {
	display: flex;
	flex-direction: column;
    height: inherit;
    background: #fff;
	.phoneGapFooterFix();
  	.top {
		margin-top: 48px;
	  	display: flex;
		flex-direction: column;
		align-items: center;
		.responsive-property(margin-bottom, 64px, null, 32px);
		.smsconfirm_title {
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
		flex-grow: 0;
		.v-input__icon--prepend-inner {
            i {
				color: #4a4a4a;
				font-size: 16px;
				margin-top: 10px;
			}
        }
	 }
	.bottom {
		@media (max-width: @screen-xs) {
			display: flex;
			flex-direction: column;
			height: 100%;
			align-items: center;
			justify-content: space-between;
		}
		.btn-login{
			.responsive-property(width, 100%, null, @btnDialog);
			margin: 20px 0 48px;
			font-size: 14px;
			font-weight: 600;
			letter-spacing: -0.42px;
			text-align: center;
			// text-transform: none !important;
			@media (max-width: @screen-xs) {
				margin: 0;
				order: 1;
			}
		}
		.actions{
			cursor: pointer;
			font-size: 14px;
			letter-spacing: -0.37px;
			text-align: center;
			color: @global-blue;
		}
	}
}
</style>

