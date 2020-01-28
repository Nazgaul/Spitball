<template>
   <div class="authenticationPage">
        <button class="back-button">
            <!-- <v-icon right @click="updateDialog(true)">sbf-close</v-icon> -->
            <router-link :to="{query: {dialog: 'login'}}">
              <v-icon right>sbf-close</v-icon>
            </router-link>
        </button>

        <div class="leftSection d-none d-sm-none d-md-none d-lg-flex" :class="{'reg_frymo': isFrymo}">
            <logo class="logo"/>
            <p class="text" v-language:inner="'loginRegister_main_txt'"></p>
        </div>

        <div class="stepsSections">
            <div class="stepContainer">
                <router-view></router-view>
                <router-view name="registerButtons"></router-view>
            </div>
        </div>

        <v-dialog v-model="showDialog" max-width="600px" :fullscreen="isMobile" content-class="registration-dialog">
            <v-card>
                <button class="close-btn" @click="updateDialog(false)">
                    <v-icon>sbf-close</v-icon>
                </button>
                <v-card-text class="limited-width py-8 px-7">
                    <h1 style="font-size: 22px;" v-language:inner="'login_are_you_sure_you_want_to_exit'"/>
                    <p><span class="pre-line" v-language:inner="'login_exiting_information1'"/><br /></p>

                    <v-btn v-if="isMobile" height="48" class="continue-registr" @click="updateDialog(false)">
                        <span v-language:inner="'login_continue_registration'"/>
                    </v-btn>
                    <button class="continue-btn" @click="exit" v-language:inner>login_Exit</button>
                </v-card-text>
            </v-card>
        </v-dialog>

   </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex'
const logo = () => import('../../app/logo/logo.vue');


// reset password
// const forgotPass = () => import('../../loginPageNEW/components/forgotPass.vue');
// const resetPassword = () => import('../../loginPageNEW/components/resetPassword.vue');

//STORE
import storeService from '../../../services/store/storeService';
import loginRegister from '../../../store/loginRegister';

export default {
  data() {
    return {
        showDialog: false,
        from: ''
    }
  },
   components:{
      logo,
    },
    computed: {
        ...mapGetters(['getCurrentLoginStep', 'isFrymo']),
        currentStep(){
            return this.getCurrentLoginStep
        },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        }
    },
    methods: {
        ...mapActions(['goBackStep','updateStep','updateToUrl','exit','finishRegister']),
            goExit() {
            this.exit()
        },
        updateDialog(val){
            if(this.currentStep === 'congrats'){
                this.finishRegister()
            } else{
                this.showDialog = val
            }
        }
    },
    beforeDestroy(){
        storeService.unregisterModule(this.$store, 'loginRegister');
    },
    beforeRouteEnter (to, from, next) {
        next((vm) => {
            vm.from = from;
        });
    },
    created() {
        storeService.registerModule(this.$store, 'loginRegister', loginRegister);
        global.onpopstate = () => {
            this.goBackStep()
        }; 
        let path = this.$route.path.toLowerCase();

        this.$nextTick(() => {
            this.updateToUrl({path: this.from.fullPath});
        })       
        
        if (!!this.$route.query.returnUrl) {
            this.updateToUrl({ path: `${this.$route.query.returnUrl}`, query: { term: '' } })
        }
        if (path === '/resetpassword'){
            this.updateStep('resetPassword')
        }
        if (this.$route.query && this.$route.query.step) {
            if(this.$route.query.step === 'EnterPhone'){
                this.updateStep('setPhone')
            }else if (this.$route.query.step === 'VerifyPhone'){
                this.updateStep('VerifyPhone')
            }
        } 
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
@import '../../../styles/colors.less';

.authenticationPage{
    display: flex; 
    background-color: #ffffff;
    flex-direction: row;
    flex-wrap: wrap;
    justify-content: space-between;
    height: 100%;
   .leftSection{
      width: 508px;
      height: 100vh;
      background-image: url('./images/side-image.jpg');
      background-repeat: no-repeat;
      background-size: auto;
      background-position: center;
      padding: 20px 24px 18px;
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      align-items: center;
      .logo{
         width: 174px;
         height: 40px;
         text-shadow: 0 2px 21px #ffffff;
         fill: #43425d;
      }
      .text{
         font-size: 26px;
         font-weight: 600;
         text-align: center;
         color: #ffffff;
      }

      &.reg_frymo {
         background-image: url('./images/group-3_frymo.png') !important;
         @media (max-width: @screen-sm) {
         background-image: url('./images/group-3@3x_frymo.png') !important;
         }
      }
      @media (max-width: @screen-sm) {
         width: 100%
      }
   }
    input {
      padding-top: 12px; // global for all inputs in in authenticate pages
    }
    .stepContainer{
      height: 100%;
    }
    .stepsSections{
      margin: 120px auto auto;

      @media (max-width: @screen-xs) {
          width: 100%;
          height: 100%;
          margin: 0 auto;
          padding: 14px;
      }
      button{
          &.btn-login{
              background-color: @color-login-btn !important;
          }
      }
   }
   .back-button {
    outline: none;
    z-index: 20;
    position: absolute;
    top: 40px;
    right: 40px;
    
    @media (max-width: @screen-sm) {
      top: 20px;
      right: 20px;
    }
    .sbf-close {
      font-size: 18px;
      color: #adadba;
      @media (max-width: @screen-sm) {
        font-size: 14px;
      }
    }
  }
  .registration-dialog {
    z-index: 100;
    .pre-line{
      white-space: pre-line;
    }
    text-align: center;
    color: @color-white;
    .v-card {
      display: flex;
      align-items: center;
      justify-content: flex-start;
      min-height: 392px;
      @media (max-width: @screen-xs) {
        min-height: 100%;
        background: @color-main-purple;
        color: @color-white;
      }
  
      .close-btn {
        position: absolute;
        top: 0;
        right: 0;
        padding: 16px;
        i {
          font-size: 16px;
          color: @color-main-purple;
          @media (max-width: @screen-xs) {
            // color: @color-white;
          }
        }
      }
    }
    h1 {
      font-size: 22px;
      font-weight: bold;
      text-align: center;
      color: @color-main-purple;
      @media (max-width: @screen-xs) {
        // color: @color-white;
        font-size: 32px;
        font-weight: 200;
        text-align: left;
        line-height: 1.1;
        width: 200px;
      }
      b {
        font-weight: 600;
      }
    }
    p {
      font-size: 18px;
      letter-spacing: -0.3px;
      text-align: center;
    //   color: @color-grey;
      margin-top: 14px;
      margin-bottom: 18px;
      line-height: 1.5;
      @media (max-width: @screen-xs) {
        font-size: 24px;
        text-align: left;
        color: #aaaaff;
      }
  
    }
    .continue-registr {
      width: 100%;
      height: 48px;
    //   color: @color-main-purple;
      text-transform: none;
      font-size: 18px;
      font-weight: bold;
      margin: 32px 0 0 0;
      @media (max-width: @screen-xs) {
        margin: 32px 0 0 0;
      }
  
    }
    .continue-btn {
      box-shadow: none;
      font-size: 18px;
      font-weight: bold;
      line-height: 1.39;
      color: #ffffff;
      width: 100%;
      margin-top: 28px;
    }
  }
}

.registration-dialog {
    .pre-line{
      white-space: pre-line;
    }
    text-align: center;
    color: @color-white;
    .v-card {
      display: flex;
      align-items: center;
      justify-content: flex-start;
      min-height: 392px;
      @media (max-width: @screen-xs) {
        min-height: 100%;
        background: @color-main-purple;
        color: @color-white;
      }

      .close-btn {
        outline: none;
        position: absolute;
        top: 0;
        right: 0;
        padding: 16px;
        i {
          font-size: 16px;
          color: @color-main-purple;
          @media (max-width: @screen-xs) {
            color: @color-white;
          }
        }
      }
    }
    h1 {
      font-size: 26px;
      font-weight: bold;
      text-align: center;
      color: @color-main-purple;
      @media (max-width: @screen-xs) {
        color: @color-white;
        font-size: 32px;
        font-weight: 200;
        text-align: left;
        line-height: 1.1;
        width: 200px;
      }
      b {
        font-weight: 600;
      }
    }
    p {
      font-size: 18px;
      letter-spacing: -0.3px;
      text-align: center;
      color: @color-grey;
      margin-top: 14px;
      margin-bottom: 18px;
      line-height: 1.5;
      @media (max-width: @screen-xs) {
        font-size: 24px;
        text-align: left;
        color: #aaaaff;
      }

    }
    .continue-registr {
      width: 100%;
      height: 48px;
      color: @color-main-purple;
      text-transform: none;
      font-size: 18px;
      font-weight: bold;
      margin: 32px 0 0 0;
      @media (max-width: @screen-xs) {
        margin: 32px 0 0 0;
      }

    }
    .continue-btn {
      background: @color-main-purple;
      box-shadow: none;
      font-size: 18px;
      font-weight: bold;
      line-height: 1.39;
      color: #ffffff;
      width: 100%;
      height: @registrationFieldHeight;
      &:hover {
        background: lighten(@color-main-purple, 10%);
      }
      margin-top: 28px;
    }
    .limited-width {
      max-width: 400px;
      width: 100%;
      margin: 0 auto;
      padding: 32px 32px;
      .custom {
        padding: 40px 0 47px;
        height: 393px;
        @media (max-width: @screen-xs) {
          padding: 52px 16px 10px;
          height: 363px;
        }
        p {
          margin-top: 0;
          @media (max-width: @screen-xs) {
            margin: 0 16px;
          }
        }
      }
    }
}
</style>
