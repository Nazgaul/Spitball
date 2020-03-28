<template>
  <v-app>
    <router-view name="banner"></router-view>
    <router-view name="header"></router-view>
    <router-view name="sideMenu" v-if="isDrawer"></router-view>
    <v-content :class="['site-content',{'hidden-sideMenu': drawerPlaceholder}]">
        <chat v-if="visible"/>
        <router-view class="main-container"></router-view>
      
        <div class="s-cookie-container" v-if="!cookiesShow">
          <span v-language:inner>app_cookie_toaster_text</span> &nbsp;
          <span class="cookie-approve">
            <button
              @click="removeCookiesPopup()"
              style="outline:none;"
              v-language:inner
            >app_cookie_toaster_action</button>
          </span>
        </div>

        <dialogInjection class="dialogInjection" />
        <toasterInjection class="toasterInjection" />

        <sb-dialog
          :isPersistent="true"
          :showDialog="newQuestionDialogSate"
          :popUpType="'newQuestion'"
          :max-width="'510px'"
          :content-class="'question-request-dialog'"
        >
          <AddQuestion v-if="newQuestionDialogSate"></AddQuestion>
        </sb-dialog>

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

        <auth />

      <mobile-footer v-if="showMobileFooter" />
    </v-content>
    
    <v-snackbar
      absolute
      top
      :timeout="getToasterTimeout"
      :class="getShowToasterType"
      :value="getShowToaster"
    >
      <div class="text-wrap" v-html="getToasterText"></div>
    </v-snackbar>

    <router-view name="footer"></router-view>
  </v-app>
</template>

<script>
import { mapGetters, mapActions } from "vuex";
import { LanguageService } from "../../services/language/languageService";

const dialogInjection = () => import('../pages/global/dialogInjection/dialogInjection.vue');
const toasterInjection = () => import('../pages/global/toasterInjection/toasterInjection.vue');

const sbDialog = () => import("../wrappers/sb-dialog/sb-dialog.vue");
const AddQuestion = () => import("../question/askQuestion/askQuestion.vue");
const walletService = () => import("../../services/walletService");
const mobileFooter = () => import("../pages/layouts/mobileFooter/mobileFooter.vue");
const chat = () => import("../chat/chat.vue");
const tutorRequest = () => import("../tutorRequestNEW/tutorRequest.vue");
const referralDialog = () => import("../question/helpers/referralDialog/referral-dialog.vue");
const auth = () => import('../pages/global/dialogInjection/globalDialogs/auth/auth.vue');
export default {
  components: {
    referralDialog,
    AddQuestion,
    sbDialog,
    chat,
    mobileFooter,
    tutorRequest,
    dialogInjection,
    toasterInjection,
    auth,
  },
  data() {
    return {
      hideFooter: false,
      toasterTimeoutObj: null
    };
  },
  computed: {
    ...mapGetters([
      "getReferralDialog",
      "accountUser",
      "newQuestionDialogSate",
      "getShowToaster",
      "getShowToasterType",
      "getToasterTimeout",
      "getToasterText",
      "getMobileFooterState",
      "getRequestTutorDialog",
      "getIsChatVisible",
      'getUserLoggedInStatus',
      'getIsTeacher',
      'getLoginDialog',
      'getRegisterDialog'
    ]),

    isDrawer() {
      let isLogged = this.getUserLoggedInStatus
      let isTeacher = this.getIsTeacher
      let isMobile = this.$vuetify.breakpoint.xsOnly
      return isLogged && isTeacher && !isMobile
    },
    drawerPlaceholder() {
      // need to think of better way to check if placeholder
      let isRoutes = [
      'feed',
      'document',
      'question',
      'profile',
      'myFollowers',
      'mySales',
      'myContent',
      'myPurchases',
      'myStudyRooms',
      'myCalendar',
      'addCourse',
      'editCourse'].some(route => this.$route.name === route)
      return isRoutes
    },
    isMobile() {
      return this.$vuetify.breakpoint.smAndDown;
    },
    cookiesShow() {
      if (global.country === "IL") return true;
      if (!this.accountUser) {
        return this.getCookieAccepted();
      } else {
        return true;
      }
    },
    showMobileFooter() {
      return this.$vuetify.breakpoint.xsOnly && this.getMobileFooterState && !this.hideFooter && this.$route.name !== 'tutorLandingPage';
    },
    visible() {
      if (this.accountUser === null) {
        return false;
      } else {
        return this.getIsChatVisible;
      }
    },
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
    visible: function(val) {
      if (!this.isMobile) {
        return;
      }
      if (val) {
        document.body.classList.add("noscroll");
      } else {
        document.body.classList.remove("noscroll");
      }
    }
  },
  methods: {
    ...mapActions([
      "updateReferralDialog",
      "updateToasterParams",
      "setCookieAccepted",
      "updateRequestDialog",
      "openChatInterface",
      "setTutorRequestAnalyticsOpenedFrom",
      "fireOptimizeActivate",
      "updateBannerStatus"
    ]),
    ...mapGetters(["getCookieAccepted"]),

    closeReferralDialog() {
      this.updateReferralDialog(false);
    },
    removeCookiesPopup: function() {
      this.setCookieAccepted();
    },
    tryBuyTokens(transactionObjectError) {
      walletService.buyTokens(transactionObjectError).then(
        () => {
          this.updateToasterParams({
            toasterText: LanguageService.getValueByKey("buyToken_success"),
            showToaster: true
          });
        },
        error => {
          global.localStorage.setItem(
            "sb_transactionError",
            transactionObjectError.points
          );
          console.log(error);
        }
      );
    }
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

    if (this.$vuetify.breakpoint.xsOnly) {
      if (!!this.$route.query && this.$route.query.chat) {
        if (this.$route.query.chat.toLowerCase() === "expand") {
          setTimeout(() => {
            this.openChatInterface(true);
          }, 170);
        }
      }
    }
    this.acceptedCookies = this.getCookieAccepted();
    if (global.isMobileAgent) {
      global.addEventListener("resize", () => {
        if (
          (document && document.activeElement.tagName == "INPUT") ||
          document.activeElement.tagName == "TEXTAREA"
        ) {
          this.hideFooter = true;
        } else {
          this.hideFooter = false;
        }
      });
    }
    global.addEventListener("error", event => {
      event.stopImmediatePropagation();
      event.stopPropagation();
      event.preventDefault();
    });

    let failedTranscationId = global.localStorage.getItem(
      "sb_transactionError"
    );
    if (failedTranscationId) {
      global.localStorage.removeItem("sb_transactionError");
      let transactionObjectError = {
        points: failedTranscationId
      };
      this.tryBuyTokens(transactionObjectError);
    }
  }
};
</script>
<style lang="less" src="./app.less"></style>
<style lang="less" src="./main.less"></style>

