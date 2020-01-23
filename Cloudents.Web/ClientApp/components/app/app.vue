<template>
  <v-app>
      <router-view name="banner"></router-view>
      <router-view name="header"></router-view>
      <router-view v-if="showSideMenu" name="sideMenu"></router-view>

      <v-content class="site-content" :class="{'loading':getIsLoading}">
        <chat></chat>

        <router-view name="verticals"></router-view>
        <router-view class="main-container"></router-view>
      
        <div class="s-cookie-container" :class="{'s-cookie-hide': cookiesShow}">
          <span v-language:inner>app_cookie_toaster_text</span> &nbsp;
          <span class="cookie-approve">
            <button
              @click="removeCookiesPopup()"
              style="outline:none;"
              v-language:inner
            >app_cookie_toaster_action</button>
          </span>
        </div>

        <sb-dialog
          :showDialog="loginDialogState"
          :popUpType="'loginPop'"
          :content-class="'login-popup'"
          :max-width="'550px'"
        >
          <login-to-answer v-if="loginDialogState"></login-to-answer>
        </sb-dialog>

        <sb-dialog
          :showDialog="universitySelectPopup"
          :popUpType="'universitySelectPopup'"
          :onclosefn="closeUniPopDialog"
          :activateOverlay="true"
          :content-class="'pop-uniselect-container'"
        >
          <uni-Select-pop v-if="universitySelectPopup" :showDialog="universitySelectPopup" :popUpType="'universitySelectPopup'"></uni-Select-pop>
        </sb-dialog>

        <sb-dialog
          :isPersistent="true"
          :showDialog="newQuestionDialogSate"
          :popUpType="'newQuestion'"
          :max-width="'510px'"
          :content-class="'question-request-dialog'"
        >
          <Add-Question v-if="newQuestionDialogSate"></Add-Question>
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
          :showDialog="getDialogState"
          :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition' "
          :popUpType="'uploadDialog'"
          :maxWidth="'716'"
          :onclosefn="setUploadDialogState"
          :activateOverlay="false"
          :isPersistent="$vuetify.breakpoint.smAndUp"
          :content-class="'upload-dialog'"
        >
          <upload-multiple-files v-if="getDialogState"></upload-multiple-files>
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

        <sb-dialog
          :showDialog="becomeTutorDialog"
          :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition' "
          :popUpType="'becomeTutorDialog'"
          :maxWidth="'840'"
          :maxHeight="'588'"
          :onclosefn="setUploadDialogState"
          :activateOverlay="false"
          :isPersistent="$vuetify.breakpoint.smAndUp"
          :content-class="'become-tutor'"
        >
          <become-tutor v-if="becomeTutorDialog"></become-tutor>
        </sb-dialog>

        <sb-dialog
          :showDialog="getShowBuyDialog"
          :popUpType="'buyTokens'"
          :content-class="!isFrymo ? 'buy-tokens-popup' : 'buy-tokens-frymo-popup'"
          :onclosefn="closeSblToken"
          maxWidth="840px"
        >
          <buy-tokens v-if="!isFrymo && getShowBuyDialog" popUpType="buyTokens"></buy-tokens>
          <buy-token-frymo v-if="isFrymo && getShowBuyDialog" popUpType="buyTokensFrymo"></buy-token-frymo>
        </sb-dialog>

        <sb-dialog
          :isPersistent="true"
          :showDialog="getShowPaymeDialog"
          :popUpType="'payme'"
          :content-class="'payme-popup'"
          maxWidth="840px"
        >
          <payment-dialog v-if="getShowPaymeDialog" />
        </sb-dialog>

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

const sbDialog = () => import("../wrappers/sb-dialog/sb-dialog.vue");
const loginToAnswer = () => import("../question/helpers/loginToAnswer/login-answer.vue");
const AddQuestion = () => import("../question/askQuestion/askQuestion.vue");
const uploadMultipleFiles = () => import("../uploadFilesDialog/uploadMultipleFiles.vue");
const walletService = () => import("../../services/walletService");
const mobileFooter = () => import("../pages/layouts/mobileFooter/mobileFooter.vue");
const buyTokens = () => import("../dialogs/buyTokens/buyTokens.vue");
const buyTokenFrymo = () => import("../dialogs/buyTokenFrymo/buyTokenFrymo.vue");
const chat = () => import("../chat/chat.vue");
const becomeTutor = () => import("../becomeTutor/becomeTutor.vue");
const tutorRequest = () => import("../tutorRequestNEW/tutorRequest.vue");
const paymentDialog = () => import("../studyroom/tutorHelpers/paymentDIalog/paymentDIalog.vue");
const referralDialog = () => import("../question/helpers/referralDialog/referral-dialog.vue");

export default {
  components: {
    referralDialog,
    AddQuestion,
    sbDialog,
    loginToAnswer,
    chat,
    mobileFooter,
    uploadMultipleFiles,
    buyTokens,
    buyTokenFrymo,
    becomeTutor,
    tutorRequest,
    paymentDialog
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
      "getIsLoading",
      "accountUser",
      "loginDialogState",
      "newQuestionDialogSate",
      "getShowSelectUniPopUpInterface",
      "getDialogState",
      "getShowToaster",
      "getShowToasterType",
      "getToasterTimeout",
      "getToasterText",
      "getMobileFooterState",
      "showLeaderBoard",
      // "showMobileFeed",
      "getShowBuyDialog",
      "becomeTutorDialog",
      "getRequestTutorDialog",
      "getShowPaymeDialog",
      "isFrymo",
      "getShowSchoolBlock"
    ]),
    showSideMenu() {
      if (this.$vuetify.breakpoint.xsOnly) {
        return this.getShowSchoolBlock;
      } else {
        return true;
      }
    },
    isMobile() {
      return this.$vuetify.breakpoint.smAndDown;
    },
    // showFeed() {
    //   if (this.$vuetify.breakpoint.smAndDown && this.getMobileFooterState) {
    //     return this.showMobileFeed;
    //   } else {
    //     return true;
    //   }
    // },
    cookiesShow() {
      if (global.country === "IL") return true;
      if (!this.accountUser) {
        return this.getCookieAccepted();
      } else {
        return true;
      }
    },
    universitySelectPopup() {
      return this.getShowSelectUniPopUpInterface;
    },
    showMobileFooter() {
      return this.$vuetify.breakpoint.xsOnly && this.getMobileFooterState && !this.hideFooter && this.$route.name !== 'tutorLandingPage';
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
    getShowPaymeDialog: function(val) {
      if (val) {
        setTimeout(function() {
          document.querySelector(".payme-popup").parentNode.style.zIndex = 999;
        }, 1000);
      }
    },
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
      if (this.loginDialogState) {
        this.updateLoginDialogState(false);
      }
      this.$nextTick(() => {
        this.fireOptimizeActivate();
      });
    }
  },
  methods: {
    ...mapActions([
      "updateReferralDialog",
      "updateToasterParams",
      "updateLoginDialogState",
      "updateNewQuestionDialogState",
      "changeSelectPopUpUniState",
      "updateDialogState",
      "setCookieAccepted",
      "updateShowBuyDialog",
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
    closeSblToken() {
      this.updateShowBuyDialog(false);
    },
    removeCookiesPopup: function() {
      this.setCookieAccepted();
    },
    closeUniPopDialog() {
      this.changeSelectPopUpUniState(false);
    },
    setUploadDialogState() {
      this.updateDialogState(false);
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

    this.$root.$on("closePopUp", name => {
      if (name === "suggestions") {
        this.showDialogSuggestQuestion = false;
      } else if (name === "newQuestionDialog") {
        this.updateNewQuestionDialogState(false);
      } else {
        this.updateLoginDialogState(false);
      }
    });

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

