<template>
  <div class="authenticationPage">
    <router-link class="backButton" :to="{query: {dialog: 'exitRegisterDialog'}}">
      <close class="closeIcon" />
    </router-link>

    <div class="leftSection d-none d-sm-none d-md-none d-lg-flex" :class="{'reg_frymo': isFrymo}">
      <logo class="logo" />
      <p class="text text-center white--text">{{$t('loginRegister_main_txt')}}</p>
    </div>

    <div class="stepsSections">
      <div class="gap d-none d-sm-flex"></div>
      <div class="stepContainer" :class="{'maxWidthCourseUni': dynamicClass}">
        <router-view></router-view>
      </div>
    </div>
  </div>
</template>

<script>
import { mapGetters } from "vuex";

//STORE
import storeService from "../../../services/store/storeService";
import loginRegister from "../../../store/loginRegister";

const logo = () => import("../../app/logo/logo.vue");
const close = () => import("../../../font-icon/close.svg");

export default {
  components: { logo, close },
  data: () => ({
    from: ""
  }),
  computed: {
    ...mapGetters(["isFrymo"]),
    
    dynamicClass() {
      return this.$route.meta.dynamicClass
    }
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.from = from;
    });
  },
  beforeDestroy() {
    storeService.unregisterModule(this.$store, "loginRegister");
  },
  created() {
    storeService.registerModule(this.$store, "loginRegister", loginRegister);

    this.$nextTick(() => {
      this.$store.dispatch("updateToUrl", this.from.path);
    });
  }
};
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

      @media (max-width: @screen-sm) {
        width: 100%
      }
    }
    input {
      padding-top: 12px; // global for all inputs in authenticate pages
    }
    .stepContainer {
      width: 400px; // global width for all components
      margin: 0 auto;
      max-width: 100%;
      height: 100%;
      .maintitle {
        font-size: 26px;
        font-weight: 600;
        color: @global-purple;
        margin-bottom: 10px;
      }
      .subtitle {
        font-size: 16px;
        color: @global-purple;
        margin-bottom: 40px;
      }
      .gradesWrap {
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
      .v-select__selection--comma {
        line-height: normal; //v-select, text was off from input
      }
      .v-input__append-inner {
        margin-top: 12px !important; // center icon, cuz custom height
      }

      &.maxWidthCourseUni {
        width: 746px;
      }
    }
    .stepsSections{
      display: flex;
      flex-direction: column;
      margin: 0 auto;
      height: 100vh;
      max-width: 100%;
      .gap {
        height: 120px;
        flex-shrink: 1;
      }
      
      @media (max-width: @screen-xs) {
        padding: 40px 14px 14px;
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
