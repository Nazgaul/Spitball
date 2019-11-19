<template>
  <section class="setEmailPassword">
    <p v-language:inner="'loginRegister_setemailpass_title'"/>
    <form @submit.prevent="submit">
      <sb-input
        class="widther"
        v-model="email"
        placeholder="loginRegister_setemailpass_input_email"
        icon="sbf-email"
        :bottomError="true"
        :autofocus="true"
        :errorMessage="errorMessages.email"
        name="email"
        type="email"
      />

      <sb-input
        v-model="password"
        :class="['mt-4', hintClass,'widther']"
        :hint="passHint"
        placeholder="loginRegister_setemailpass_input_pass"
        :errorMessage="errorMessages.password"
        :bottomError="true"
        type="password"
        name="pass"
        :autofocus="false"
      />

      <sb-input
        class="mt-4 widther"
        :errorMessage="errorMessages.confirmPassword"
        :bottomError="true"
        v-model="confirmPassword"
        placeholder="loginRegister_setemailpass_input_passconfirm"
        name="confirm"
        type="password"
        :autofocus="false"
      />

      <vue-recaptcha
        size="invisible"
        class="mt-4 captcha"
        :sitekey="siteKey"
        ref="recaptcha"
        @verify="onVerify"
        @expired="onExpired()"
      />

      <v-btn type="submit" :loading="isEmailLoading" 
             large rounded class="ctnBtn white--text btn-login">

        <span v-language:inner="'loginRegister_setemailpass_btn'"></span>
      </v-btn>
    </form>
  </section>
</template>

<script>
import { mapActions, mapGetters, mapMutations } from "vuex";
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
    };
  },
  watch: {
    password: function(val){
        this.setErrorMessages({})
    },
    confirmPassword: function(val){
        this.setErrorMessages({})
    }
  },
  computed: {
    ...mapGetters(["getEmail1","getGlobalLoading","getErrorMessages","getPassScoreObj"]),
    passHint() {
      if (this.password.length > 0) {
        let passScoreObj = this.getPassScoreObj;
        this.score = global.zxcvbn(this.password).score;
        return `${passScoreObj[this.score].name}`;
      }
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
    }
  },
  methods: {
    ...mapActions(["updateEmail","emailSigning"]),
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
      this.emailSigning(paramObj).then(response => {},err => {
        this.$refs.recaptcha.reset()
        });
    },
    submit(){
      this.$refs.recaptcha.execute()
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
    .responsive-property(margin-bottom, 64px, null, 34px);
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
      margin: 48px 0 0 0;
      font-size: 16px;
      font-weight: 600;
      letter-spacing: -0.42px;
      text-align: center;
      text-transform: none !important;
    }
  }
}
</style>
