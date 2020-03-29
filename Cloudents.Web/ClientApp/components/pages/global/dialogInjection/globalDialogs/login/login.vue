<template>
    <v-dialog :value="true" persistent max-width="550px" :fullscreen="$vuetify.breakpoint.xsOnly">
        <v-card class="login-popup">
            <div class="dialog-wrapp">
                <button class="close-btn text-md-right" v-closeDialog>
                    <v-icon>sbf-close</v-icon>
                </button>
                <div class="ml-1 wrap-text">
                    <h2 class="text-md-center" v-language:inner="'profileData_generalProfile_modal_heading'"></h2>
                    <div class="wrapper-body-text">
                        <p class="dialog-body-text" v-language:inner="'profileData_generalProfile_modal_text'"></p>
                    </div>
                </div>
                <div class="btn-container">
                    <a  class="login-btns" @click="goToSignIn()" v-language:inner>loginAnswer_login</a>
                    <a  class="login-btns" @click="$store.commit('setRegisterDialog', true)" v-closeDialog v-language:inner>loginAnswer_Sign_Up</a>
                </div>
            </div>
        </v-card>
    </v-dialog>
</template>

<script>
export default {
    methods: {
        goToRegister(){
            this.$router.push({name: 'register', query:{returnUrl : this.$route.path}});
        },
        goToSignIn(){
            this.$router.push({name: 'login', query:{returnUrl : this.$route.path}});
        }
    },
}
</script>

<style lang="less">
@import "../../../../../../styles/mixin.less";

div.login-popup {
  border-radius: 6px;
  font-size: 16px;
  .close-btn {
    position: absolute;
    right: 16px;
    top: 6px;
    outline: none;

    .v-icon {
      font-size: 12px;
    }
  }
  .dialog-wrapp {
    height: 100%;
    padding: 20px;
    position: relative;
    @media (max-width: @screen-xs) {
      display: flex;
      flex-direction: column;
      // justify-content: space-between;
    }
      .wrap-text{
        margin-bottom: 26px;
        text-align: center;
        h2{
          font-size: 32px;
          font-weight: 600;
          font-style: normal;
          font-stretch: normal;
          line-height: 42px;
          letter-spacing: -0.3px;
          color: #43425d;
          white-space: pre-wrap;
          @media(max-width: @screen-xs){
            font-size: 26px;
            font-weight: 600;
            letter-spacing: -0.5px;
          }
        }
        .wrapper-body-text{
          max-width: 445px;
          margin: 0 auto;
        }
        .dialog-body-text{
          white-space: pre-line;
          margin-top: 20px;
          font-size: 18px;
          font-weight: 600;
          line-height: 1.78;
          letter-spacing: -0.3px;
          color: #43425d;
          text-align: center;
          @media(max-width: @screen-xs){
            font-size: 16px;
          }
        }
        @media(max-width: @screen-xs){
          margin-top: 10%;
          margin-bottom: 16px;
          padding: 0;

        }
      }
      .btn-container{
        display: flex;
        justify-content: center;
        .login-btns {
          font-weight: 600;
          height: 42px;
          border-radius: 26.5px;
          border: solid 1px #4c59ff;
          font-size: 14px;
          outline: none;
          display: flex;
          align-items: center;
          justify-content: center;
          color: #4c59ff;
          letter-spacing: -0.5px;
          padding: 0 20px;
          min-width: 180px;

          &:first-child {
            background: #4c59ff;
            color: @color-white;
            margin-right: 24px;
            .transition (all .2s ease-in-out);
            // &:hover {
            //   background-color: lighten(@color-main-purple, 10%);
            // }
          }
          &:last-child{
            .transition (all .2s ease-in-out);
            // &:hover {
            //   background-color: darken(@color-white, 10%);
            // }
          }
          @media (max-width: @screen-xs) {
            min-width: 140px;
          }
        }
      }
  }
}

</style>
