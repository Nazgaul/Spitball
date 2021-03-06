﻿<template>
  <v-app>
    <!-- <component :is="layout"></component> -->
    <router-view name="banner"></router-view>
    <router-view v-if="showHeader" name="header"></router-view>
    <router-view name="sideMenu" v-if="isDrawer"></router-view>
   <router-view v-if="isCourseDrawer" name="courseDrawer"></router-view>
    <v-main :class="[{'site-content': $route.path !== '/' && $route.name !== profileRoute}, {'hidden-sideMenu': drawerPlaceholder}]">
        <router-view class="main-container"></router-view>

        <sb-dialog
          :isPersistent="true"
          :showDialog="getRequestTutorDialog"
          :popUpType="'tutorRequestDialog'"
          :max-width="'510px'"
          :content-class="'tutor-request-dialog'"
        >
          <tutor-request v-if="getRequestTutorDialog"></tutor-request>
        </sb-dialog>

        <sb-dialog
          v-if="!!accountUser"
          :showDialog="getReferralDialog"
          :popUpType="'referralPop'"
          :onclosefn="closeReferralDialog"
          :content-class="'login-popup'"
        >
          <referral-dialog v-if="getReferralDialog"
            :isTransparent="true"
            :onclosefn="closeReferralDialog"
            :showDialog="getReferralDialog"
            :popUpType="'referralPop'"
          ></referral-dialog>
        </sb-dialog>

      <mobile-footer v-if="showMobileFooter" />
    </v-main>
    
    <v-snackbar
      top
      :timeout="getToasterTimeout"
      :class="getShowToasterType"
      :value="getShowToaster"
    >
      <div class="text-wrap" v-html="getToasterText"></div>
    </v-snackbar>
    <slot name="appInjections"></slot>
    <router-view name="footer"></router-view>
  </v-app>
</template>

<script>
import { mapGetters, mapActions } from "vuex";
import * as routeNames from '../../routes/routeNames.js';

const sbDialog = () => import("../wrappers/sb-dialog/sb-dialog.vue");
// const walletService = () => import("../../services/walletService");
const mobileFooter = () => import("../pages/layouts/mobileFooter/mobileFooter.vue");
const tutorRequest = () => import("../tutorRequestNEW/tutorRequest.vue");
const referralDialog = () => import("../question/helpers/referralDialog/referral-dialog.vue");

export default {
  components: {
    referralDialog,
    sbDialog,
    mobileFooter,
    tutorRequest,
  },
  data() {
    return {
      profileRoute: routeNames.Profile,
      hideFooter: false,
      toasterTimeoutObj: null
    };
  },
  computed: {
    ...mapGetters([
      "getReferralDialog",
      "accountUser",
      "getShowToaster",
      "getShowToasterType",
      "getToasterTimeout",
      "getToasterText",
      "getMobileFooterState",
      "getRequestTutorDialog",
      'getUserLoggedInStatus',
      'getIsTeacher',
      'getLoginDialog',
      'getRegisterDialog'
    ]),
    isCourseDrawer(){
      return this.$store.getters.getIsCourseTutor && !this.$vuetify.breakpoint.smAndDown && this.$route.params?.edit;   
    },
    isDrawer() {
      let isLogged = this.getUserLoggedInStatus
      let isTeacher = this.getIsTeacher
      // let isMobile = this.$vuetify.breakpoint.xsOnly
      return isLogged && isTeacher
    },
    drawerPlaceholder() {
      // need to think of better way to check if placeholder
      let isRoutes = [
      'question',
      'myFollowers',
      'mySales',
      'myCourses',
      'myPurchases',
      'myStudyRooms',
      'myCalendar'].some(route => this.$route.name === route)
      return isRoutes && !this.getIsTeacher
    },
    isMobile() {
      return this.$vuetify.breakpoint.smAndDown;
    },
    // cookiesShow() {
    //   if (global.country === "IL") return true;
    //   if (!this.accountUser) {
    //     return this.getCookieAccepted();
    //   } else {
    //     return true;
    //   }
    // },
    showMobileFooter() {
      let isMobileChatConversation = this.$route.name === routeNames.MessageCenter && this.$route.params?.id
      return this.$vuetify.breakpoint.xsOnly && this.getMobileFooterState && !this.hideFooter && this.$route.name !== 'tutorLandingPage' && !isMobileChatConversation;
    },
    showHeader(){
      if(this.$route.name == routeNames.MessageCenter){
        return !this.$vuetify.breakpoint.xsOnly 
      }else{
        return true;
      }
    }
  },
  updated: function() {
    this.$nextTick(function() {
      if (!!global.dataLayer) {
        this.fireOptimizeActivate();
      }
      // Code that will run only after the
      // entire question-details has been re-rendered
    });
  },
  mounted: function() {
    if (!!global.dataLayer) {
      this.$nextTick(function() {
        this.fireOptimizeActivate();
      });
    }
  },
  watch: {
    getShowToaster: function(val) {
      let self = this;
      if (val) {
        this.toasterTimeoutObj = setTimeout(() => {
          if (val) {
            self.updateToasterParams({
              showToaster: false
            });
          }
        }, this.getToasterTimeout);
      } else {
        global.clearTimeout(this.toasterTimeoutObj);
        self.updateToasterParams({
          showToaster: false
        });
      }
    },
    getToasterTimeout: function() {
      let self = this;
      global.clearTimeout(this.toasterTimeoutObj);
      this.toasterTimeoutObj = setTimeout(() => {
        self.updateToasterParams({
          showToaster: false
        });
      }, this.getToasterTimeout);
    },
    $route() {
      this.$nextTick(() => {
        this.fireOptimizeActivate();
      });
    },
  },
  methods: {
    ...mapActions([
      "updateReferralDialog",
      "updateToasterParams",
      "setCookieAccepted",
      "updateRequestDialog",
      "setTutorRequestAnalyticsOpenedFrom",
      "fireOptimizeActivate",
      "updateBannerStatus"
    ]),
//    ...mapGetters(["getCookieAccepted"]),

    closeReferralDialog() {
      this.updateReferralDialog(false);
    },
    // removeCookiesPopup: function() {
    //   this.setCookieAccepted();
    // },
    
  },
  created() {
    if (!!this.$route.query && this.$route.query.requesttutor) {
      if (this.$route.query.requesttutor.toLowerCase() === "open") {
        setTimeout(() => {
          this.setTutorRequestAnalyticsOpenedFrom({
            component: "query",
            path: this.$route.path
          });
          this.updateRequestDialog(true);
        }, 170);
      }
    }
    this.updateBannerStatus(true);

    //this.acceptedCookies = this.getCookieAccepted();
    if (global.isMobileAgent) {
      global.addEventListener("resize", () => {
        this.hideFooter = (document && document.activeElement.tagName == "INPUT") ||
            document.activeElement.tagName == "TEXTAREA";
      });
    }
    global.addEventListener("error", event => {
      event.stopImmediatePropagation();
      event.stopPropagation();
      event.preventDefault();
    });
  }
};
</script>
<style lang="less" src="./app.less"></style>
<style lang="less" src="./main.less"></style>

