<template>
  	<form class="setEmail" @submit.prevent="validate">
		<p v-language:inner="'loginRegister_setemail_title'"/>

		<sb-input
      class="widther"
      v-model="email"
			placeholder="loginRegister_setemail_input"
      :errorMessage="errorMessages.email"
			icon="sbf-email"
			bottomError
			:autofocus="true"
			name="email"
			type="email"/>
      
		<v-btn
      :loading="isEmailLoading"
      type="submit"
			large
			rounded
			class="white--text btn-login">
                <span v-language:inner="'loginRegister_setemail_btn'"></span>
                </v-btn>
  	</form>
</template>

<script>
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import { mapGetters, mapActions,mapMutations } from 'vuex';

export default {
  name: "setEmail",
  components: {
    SbInput
  },
  computed: {
    ...mapGetters(['getGlobalLoading','getErrorMessages','getEmail1']),
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
    ...mapActions(['emailValidate','updateEmail']),
    ...mapMutations(['setErrorMessages']),
    validate(){
      this.emailValidate()
    }
  },
};
</script>

<style lang="less">
@import '../../../styles/mixin.less';
@import '../../../styles/colors.less';
.setEmail {

      @media (max-width: @screen-xs) {
        display: flex;
        flex-direction: column;
        align-items: center;
      }
  p {
    .responsive-property(font-size, 28px, null, 22px);
    .responsive-property(letter-spacing, -0.51px, null, -0.4px);
    line-height: 1.54;
    text-align: center;
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

  button {
    margin: 45px 0 0;
      @media (max-width: @screen-xs) {
        display: flex;
        flex-direction: column;
        align-items: center;
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

