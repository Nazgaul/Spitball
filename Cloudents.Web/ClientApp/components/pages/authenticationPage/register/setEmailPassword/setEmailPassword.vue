<template>
  <section class="setEmailPassword text-center">
    <p class="setemailpass_title">{{$t('loginRegister_setemailpass_title')}}</p>
    <form @submit.prevent="submit" class="form">
      <div>
        <!-- autocomplete because chrome address -->
        <!-- placeholder for the legend -->
        <v-layout wrap class="widther">
          <v-flex xs12 sm6 class="mb-3 pr-sm-2">
            <v-text-field
              v-model="firstName"
              class="input-fields"
              color="#304FFE"
              outlined
              height="44"
              dense
              :label="labels['fname']"
              :error-messages="firstNameError"
              placeholder=" "
              autocomplete="nope"
              type="text">
            </v-text-field>
          </v-flex>

          <v-flex xs12 sm6 class="pl-sm-2 mb-3">
            <v-text-field
              v-model="lastName"
              class="input-fields"
              color="#304FFE"
              outlined
              height="44"
              dense
              :label="labels['lname']"
              :error-messages="lastNameError"
              placeholder=" "
              autocomplete="nope"
              type="text">
            </v-text-field>
          </v-flex>
        </v-layout>

        <v-text-field 
          v-model="email"
          class="widther input-fields" 
          color="#304FFE"
          outlined
          height="44" 
          dense
          :label="labels['email']"
          :error-messages="errorMessages.email"
          placeholder=" "
          type="email">
        </v-text-field>

        <v-radio-group v-model="gender" row class="radioActive mt-n1" dense :mandatory="true">
          <v-radio :label="labels['genderMale']" value="male" on-icon="sbf-radioOn" off-icon="sbf-radioOff"></v-radio>
          <v-radio :label="labels['genderFemale']" value="female" on-icon="sbf-radioOn" off-icon="sbf-radioOff"></v-radio>
        </v-radio-group>

        <v-text-field 
          v-model="password"
          :class="[hintClass,'widther','input-fields','mb-3']"
          color="#304FFE"
          outlined
          height="44"
          dense
          :label="labels['password']"
          :error-messages="errorMessages.password"
          placeholder=" "
          type="password"
          :hint="passHint">
        </v-text-field>

        <vue-recaptcha
          size="invisible"
          class="captcha"
          :sitekey="siteKey"
          ref="recaptcha"
          @verify="onVerify"
          @expired="onExpired()"
        />
      </div>

      <div>
        <v-btn 
          type="submit"
          :loading="isEmailLoading"
          depressed
          large
          rounded
          class="ctnBtn white--text btn-login">
            <span>{{$t('loginRegister_setemailpass_btn')}}</span>
        </v-btn>
      </div>

    </form>
  </section>
</template>

<script>
import { mapActions, mapGetters, mapMutations } from "vuex";
import { LanguageService } from "../../../../../services/language/languageService";

import VueRecaptcha from "vue-recaptcha";

export default {
  // name: "setEmailPassword",
  components: { VueRecaptcha },
  data() {
    return {
      gender: "male",
      password: "",
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

      labels: {
        fname: LanguageService.getValueByKey('loginRegister_setemailpass_first'),
        lname: LanguageService.getValueByKey('loginRegister_setemailpass_last'),
        email: LanguageService.getValueByKey('loginRegister_setemailpass_input_email'),
        genderMale: LanguageService.getValueByKey('loginRegister_setemailpass_male'),
        genderFemale: LanguageService.getValueByKey('loginRegister_setemailpass_female'),
        password: LanguageService.getValueByKey('loginRegister_setemailpass_input_pass'),
      }
    };
  },
  watch: {
    password: function(){
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
    gender() {
      this.updateGender(this.gender);
    }
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
    ...mapActions(["updateEmail","emailSigning",'updateName', 'updateGender']),
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
@import "../../../../../styles/mixin.less";
@import "../../../../../styles/colors.less";
.setEmailPassword {
  display: flex;
  flex-direction: column;
  height: 100%;
  .setemailpass_title {
    .responsive-property(font-size, 28px, null, 18px);
    .responsive-property(letter-spacing, -0.51px, null, -0.4px);
    .responsive-property(margin-bottom, 50px, null, 20px);
    font-weight: 600;
    text-align: center;
    color: #43425d;
     @media (max-width: @screen-xs) {
       margin-top: 20px;
    }
  }
  form {
    @media (max-width: @screen-xs) {
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      // height: calc(100vh - 130px);
    }
    .input-fields {
      width: 100%;
    }
    .captcha {
      @media (max-width: @screen-xs) {
        .grecaptcha-badge {
          bottom: 80px !important;
        }
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
      .responsive-property(width, 100%, null, @btnDialog);
      margin: 10px 0 0 0;
      font-size: 14px;
      font-weight: 600;
      letter-spacing: -0.42px;
      text-align: center;
      text-transform: none !important;
    }
  }
}
</style>
