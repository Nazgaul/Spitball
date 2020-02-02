<template>
    <div class="authenticationPage">
        <router-link class="backButton" :to="{query: {dialog: 'login'}}">
            <close class="closeIcon" />
        </router-link>

        <div class="leftSection d-none d-sm-none d-md-none d-lg-flex" :class="{'reg_frymo': isFrymo}">
            <logo class="logo" />
            <p class="text text-center white--text" v-language:inner="'loginRegister_main_txt'"></p>
        </div>

        <div class="stepsSections">
            <div class="stepContainer">
                <router-view></router-view>
            </div>
        </div>
    </div>
</template>

<script>
import { mapGetters } from 'vuex';

//STORE
import storeService from '../../../services/store/storeService';
import loginRegister from '../../../store/loginRegister';

const logo = () => import('../../app/logo/logo.vue');
const close = () => import('../../../font-icon/close.svg');

export default {
  components:{ logo, close },
  data: () => ({
    from: ''
  }),
  computed: {
    ...mapGetters(['isFrymo']),
  },
  beforeRouteEnter (to, from, next) {
    next((vm) => {
        vm.from = from;
    });
  },
  beforeDestroy(){
    storeService.unregisterModule(this.$store, 'loginRegister');
  },
  created() {
    storeService.registerModule(this.$store, 'loginRegister', loginRegister);

    this.$nextTick(() => {
      this.$store.dispatch('updateToUrl', this.from.path);
    })
    
  }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
@import '../../../styles/colors.less';

.authenticationPage{
    display: flex; 
    background-color: #ffffff !important; //main-container class overide color in general page
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
      padding-top: 12px; // global for all inputs in authenticate pages
    }
    .stepContainer {
      width: 400px; // global width for all components
      max-width: 400px;
      margin: 0 auto;
      height: 100%;
      @media (max-width: @screen-xs) {
        width: 100%;
        max-width: 100%;
      }
      .maintitle {
        font-size: 26px;
        color: @global-purple;
        margin-bottom: 10px;
      }
      .subtitle {
        font-size: 16px;
        color: @global-purple;
        margin-bottom: 40px;
      }
      .gradesWrap {
        width: 250px;
        i {
          font-size: 8px;
          color: @global-purple;
        }
      }
      #registerButtons {
        .actions {
          .btn {
              min-width: 140px;
              @media(max-width: @screen-xs) {
                  min-width: 120px;
              }
          }
          .register_btn_back {
              margin-right: 10px;
          }
        }
      }
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
   .backButton {
    outline: none;
    position: absolute;
    top: 40px;
    right: 40px;
    @media (max-width: @screen-sm) {
      top: 20px;
      right: 20px;
    }
    .closeIcon {
      font-size: 18px;
      fill: #adadba;
      @media (max-width: @screen-sm) {
        font-size: 14px;
      }
    }
  }
}
</style>
