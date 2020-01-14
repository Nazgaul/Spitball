<template>
  <section class="setEmailPassword">
    <p v-language:inner="'loginRegister_setemailpass_title'"/>
    <form @submit.prevent="submit">
      <v-layout wrap justify-space-between class="widther">
        <v-flex xs6 class="pr-2 mb-2">
          <v-text-field class="input-fields" outlined height="50px" dense
                        v-model="firstName"
                        label="First Name" 
                        :error-messages="firstNameError"
                        placeholder=" "
                        type="text"
                        />
        </v-flex>
        <v-flex xs6 class="pl-2">
          <v-text-field class="input-fields" outlined height="50px" dense
                        v-model="lastName"
                        label="Last Name" 
                        :error-messages="lastNameError"
                        placeholder=" "
                        type="text"
                        />
        </v-flex>
      </v-layout>
      <v-text-field class="widther input-fields mb-2" outlined height="50" dense
              v-model="email"
              label="Email" 
              :error-messages="errorMessages.email"
              placeholder=" "
              type="email"
              />
      <v-text-field outlined height="50" dense
              v-model="password"
              label="Password" 
              :error-messages="errorMessages.password"
              placeholder=" "
              type="password"
              :class="[hintClass,'widther','input-fields','mb-2']"
              :hint="passHint"
              />
      <v-text-field outlined height="50" dense
              v-model="confirmPassword"
              label="Confirm Password" 
              :error-messages="errorMessages.confirmPassword"
              placeholder=" "
              type="password"
              class="widther input-fields"
              />

      <vue-recaptcha
        size="invisible"
        class="captcha"
        :sitekey="siteKey"
        ref="recaptcha"
        @verify="onVerify"
        @expired="onExpired()"
      />

      <v-btn type="submit" :loading="isEmailLoading" depressed
             large rounded class="ctnBtn white--text btn-login">

        <span v-language:inner="'loginRegister_setemailpass_btn'"></span>
      </v-btn>
    </form>
  </section>
</template>

<script>
import { mapActions, mapGetters, mapMutations } from "vuex";
import { LanguageService} from "../../../services/language/languageService";

import SbInput from "../../question/helpers/sbInput/sbInput.vue";
import VueRecaptcha from "vue-recaptcha";

export default {
  name: "setEmailPassword",
  components: {
    SbInput,
    VueRecaptcha
  },
  data() {
    return {
      password: "",
      confirmPassword: "",
      score: {
        default: 0,
        required: false
      },
      recaptcha: "",
      siteKey: '6LfyBqwUAAAAAM-inDEzhgI2Cjf2OKH0IZbWPbQA',
      firstName:'',
      lastName:'',
      firstNameError:'',
      lastNameError:'',
    };
  },
  watch: {
    password: function(){
        this.setErrorMessages({})
    },
    confirmPassword: function(){
        this.setErrorMessages({})
    },
    firstName: function(){
      this.firstNameError ='';
      let fullNameObj = {
        firstName: this.firstName,
        lastName: this.lastName
      }
      this.updateName(fullNameObj)
    },
    lastName: function(){
      this.lastNameError ='';
      let fullNameObj = {
        firstName: this.firstName,
        lastName: this.lastName
      }
      this.updateName(fullNameObj)
    },
    
  },
  computed: {
    ...mapGetters(["getEmail1","getGlobalLoading","getErrorMessages","getPassScoreObj"]),
    passHint() {
      if (this.password.length > 0) {
        let passScoreObj = this.getPassScoreObj;
        this.changeScore()
        return `${passScoreObj[this.score].name}`;
      }
      return null
    },
    errorMessages() {
      return this.getErrorMessages;
    },
    isEmailLoading() {
      return this.getGlobalLoading;
    },
    email: {
      get() {
        return this.getEmail1;
      },
      set(val) {
        this.updateEmail(val);
        this.setErrorMessages({})
      }
    },
    hintClass() {
      let passScoreObj = this.getPassScoreObj;
      if (this.passHint) {
        return passScoreObj[this.score].className;
      }
      return null
    }
  },
  methods: {
    ...mapActions(["updateEmail","emailSigning",'updateName']),
    ...mapMutations(['setErrorMessages']),
    onVerify(response) {
      this.recaptcha = response
      this.register()
    },
    onExpired() {
      this.recaptcha = ''
      this.$refs.recaptcha.reset();
    },
    register() {
      let paramObj = {
        password: this.password,
        confirmPassword: this.confirmPassword,
        recaptcha: this.recaptcha
      }
      this.emailSigning(paramObj).then(() => {},() => {
        this.$refs.recaptcha.reset()
        });
    },
    submit(){
      if(this.firstName.length > 1 && this.lastName.length > 1){
        this.$refs.recaptcha.execute()
      } else{
        if(this.firstName.length < 2 ){
          this.firstNameError = `${LanguageService.getValueByKey("formErrors_min_chars")} ${2}`
        }
        if(this.lastName.length < 2 ){
          this.lastNameError = `${LanguageService.getValueByKey("formErrors_min_chars")} ${2}`
        }
      }
    },
    changeScore() {
      this.score = global.zxcvbn(this.password).score;
    }
  },
  created() {
    this.$loadScript("https://unpkg.com/zxcvbn@4.4.2/dist/zxcvbn.js");
    let captchaLangCode = global.lang === "he" ? "iw" : "en";
    this.$loadScript(`https://www.google.com/recaptcha/api.js?onload=vueRecaptchaApiLoaded&render=explicit&hl=${captchaLangCode}`);
  }
};
</script>

<style lang='less'>
@import "../../../styles/mixin.less";
@import "../../../styles/colors.less";
.setEmailPassword {
  @media (max-width: @screen-xs) {
    display: flex;
    flex-direction: column;
    align-content: flex-start;
  }
  p {
    .responsive-property(font-size, 28px, null, 22px);
    .responsive-property(letter-spacing, -0.51px, null, -0.4px);
    .responsive-property(margin-bottom, 50px, null, 34px);
    text-align: center;
    color: @color-login-text-title;
  }
  form {
    @media (max-width: @screen-xs) {
      display: flex;
      flex-direction: column;
      align-items: center;
    }
    .captcha {
      @media (max-width: @screen-xs) {
        // width: 92%;
      }
    }
    .input-wrapper {
      input[type="password"] {
        padding: 10px !important;
      }
      input[type="text"] {
        padding: 10px !important;
      }
      input {
        .login-inputs-style();
        padding-left: 54px !important;
        @media (max-width: @screen-xs) {
          padding-left: 45px !important;
        }
        .responsive-property(font-size, 16px, null, 14px);
      }
      i {
		.responsive-property(left, 16px, null, 13px);
		.responsive-property(font-size, 16px, null, 14px);
        position: absolute;
        left: 16px;
        top: 17px;
        font-size: 16px;
      }
    }
    .ctnBtn {
      .responsive-property(width, 100%, null, 72%);
      margin: 10px 0 0 0;
      font-size: 16px;
      font-weight: 600;
      letter-spacing: -0.42px;
      text-align: center;
      text-transform: none !important;
    }
  }
}
</style>
