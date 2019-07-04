<template>
  	<div class="setEmail">
		<p v-language:inner="'loginRegister_setemail_title'"/>

		<sb-input
      v-model="email"
			placeholder="loginRegister_setemail_input"
      :errorMessage="errorMessages.email"
			icon="sbf-email"
			:bottomError="true"
			:autofocus="true"
			name="email"
			type="email"/>
      
		<v-btn
      :disabled="!isEmail"
      @click="validate"
      :loading="isEmailLoading"
			color="#304FFE"
			large
			round
			class="white--text">
                <span v-language:inner="'loginRegister_setemail_btn'"></span>
                </v-btn>
  	</div>
</template>

<script>
import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import { mapGetters, mapActions } from 'vuex';

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
			get(){},
			set(val){
				this.updateEmail(val)
      }
    },
    isEmail(){
      return (this.getEmail1)
    }
  },
  methods: {
    ...mapActions(['emailValidate','updateEmail']),
    validate(){
      this.emailValidate()
    }
  },
};
</script>

<style lang="less">
@import '../../../styles/mixin.less';
.setEmail {
  p {
    font-size: 28px;
    line-height: 1.54;
    letter-spacing: -0.51px;
    text-align: center;
    color: #434c5f;
    margin-bottom: 8px;
  }
  .input-wrapper {
    margin: 62px 0 0px;
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

  button {
    margin: 66px 0 48px;
    .responsive-property(width, 100%, null, 90%);
    font-size: 16px;
    font-weight: 600;
    letter-spacing: -0.42px;
    text-align: center;
    text-transform: none !important;
  }
}
</style>

