<template>
  <v-app>
    <!-- <v-tour
        name="myTour"
        :steps="tourObject.tourSteps"
        :options="tourObject.toursOptions"
        :callbacks="tourObject.tourCallbacks"
      ></v-tour> -->
    <router-view name="header"></router-view>
    <router-view name="schoolBlock"></router-view>
    <v-content class="site-content" :class="{'loading':getIsLoading}">
      <div class="loader" v-show="getIsLoading">
        <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
      </div>
        <chat-component v-if="isMobile"></chat-component>
      <div v-if="showLeadersMobile && getMobileFooterState">
          <tutor-list></tutor-list>
      </div>

      <router-view name="verticals"></router-view>
      <router-view class="main-container" v-show="showFeed" ref="mainPage"></router-view>
      <chat-component v-if="!isMobile"></chat-component>
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
        <login-to-answer></login-to-answer>
      </sb-dialog>
            <sb-dialog :showDialog="universitySelectPopup"
                       :popUpType="'universitySelectPopup'"
                       :onclosefn="closeUniPopDialog"
                       :activateOverlay="true"
                       :content-class="'pop-uniselect-container'">
                <uni-Select-pop :showDialog="universitySelectPopup" :popUpType="'universitySelectPopup'"></uni-Select-pop>
            </sb-dialog>

            <sb-dialog :isPersistent="true"
                       :showDialog="newQuestionDialogSate"
                       :popUpType="'newQuestion'"
                       :max-width="'510px'"
                       :content-class="'question-request-dialog'">
                <Add-Question></Add-Question>
            </sb-dialog>
              <sb-dialog :isPersistent="true"
                       :showDialog="getRequestTutorDialog"
                       :popUpType="'tutorRequestDialog'"
                       :max-width="'510px'"
                       :content-class="'tutor-request-dialog'">
                <tutor-request></tutor-request>
            </sb-dialog>
            <sb-dialog :showDialog="getDialogState"
                       :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition' "
                       :popUpType="'uploadDialog'"
                       :maxWidth="'716'"
                       :onclosefn="setUploadDialogState"
                       :activateOverlay="false"
                       :isPersistent="$vuetify.breakpoint.smAndUp"
                       :content-class="'upload-dialog'">
                <upload-multiple-files v-if="getDialogState"></upload-multiple-files>
            </sb-dialog>

        <sb-dialog :showDialog="becomeTutorDialog"
                   :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition' "
                   :popUpType="'becomeTutorDialog'"
                   :maxWidth="'840'"
                   :maxHeight="'588'"
                   :onclosefn="setUploadDialogState"
                   :activateOverlay="false"
                   :isPersistent="$vuetify.breakpoint.smAndUp"
                   :content-class="'become-tutor'">
            <become-tutor v-if="becomeTutorDialog"></become-tutor>
        </sb-dialog>
        
        <sb-dialog :showDialog="getShowBuyDialog"
                    :popUpType="'buyTokens'"
                    :content-class="!isFrymo ? 'buy-tokens-popup' : 'buy-tokens-frymo-popup'"
                    :onclosefn="closeSblToken"
                    maxWidth='840px'>
            <buy-tokens v-if="!isFrymo" popUpType="buyTokens"></buy-tokens>
            <buy-token-frymo v-else popUpType="buyTokensFrymo"></buy-token-frymo>
        </sb-dialog>

        <sb-dialog
          :isPersistent="true"
          :showDialog="getShowPaymeDialog"
          :popUpType="'payme'"
          :content-class="'payme-popup'"
          maxWidth='840px'>
            <payment-dialog />
        </sb-dialog>

        <mobile-footer v-show="$vuetify.breakpoint.xsOnly && getMobileFooterState && !hideFooter"
                           :onStepChange="onFooterStepChange"></mobile-footer>
        </v-content>
        <v-snackbar absolute top :timeout="getToasterTimeout" :class="getShowToasterType" :value="getShowToaster">
            <div class="text-wrap" v-html="getToasterText"></div>
        </v-snackbar>
    </v-app>
</template>
<script>
  
import { mapGetters, mapActions } from "vuex";
import sbDialog from "../wrappers/sb-dialog/sb-dialog.vue";
import loginToAnswer from "../question/helpers/loginToAnswer/login-answer.vue";
import AddQuestion from "../question/askQuestion/askQuestion.vue";
import uploadMultipleFiles from '../uploadFilesDialog/uploadMultipleFiles.vue';
import {  GetDictionary,  LanguageService} from "../../services/language/languageService";
import walletService from "../../services/walletService";
import reportItem from "../results/helpers/reportItem/reportItem.vue";
import mobileFooter from "../footer/mobileFooter/mobileFooter.vue";
import marketingBox from "../helpers/marketingBox/marketingBox.vue";
import buyTokens from "../dialogs/buyTokens/buyTokens.vue";
import buyTokenFrymo from "../dialogs/buyTokenFrymo/buyTokenFrymo.vue";
import chatComponent from "../chat/chat.vue";
import becomeTutor from "../becomeTutor/becomeTutor.vue";
import tutorList from "../helpers/tutorList/tutorList.vue";
import tutorRequest from '../tutorRequestNEW/tutorRequest.vue';
import paymentDialog from '../tutor/tutorHelpers/paymentDIalog/paymentDIalog.vue';

export default {
  components: {
    AddQuestion,
    sbDialog,
    loginToAnswer,
    chatComponent,
    reportItem,
    mobileFooter,
    marketingBox,
    uploadMultipleFiles,
    buyTokens,
    buyTokenFrymo,
    becomeTutor,
    tutorList,
    tutorRequest,
    paymentDialog
  },
  data() {
    return {
      isRtl: global.isRtl,
      hideFooter: false,
      showBuyTokensDialog: false,
      toasterTimeoutObj: null,
    };
  },
  computed: {
    ...mapGetters([
      "getIsLoading",
      "accountUser",
      "loginDialogState",
      "newQuestionDialogSate",
      "getShowSelectUniPopUpInterface",
      "getDialogState",
      "confirmationDialog",
      "getShowToaster",
      "getShowToasterType",
      "getToasterTimeout",
      "getToasterText",
      "getMobileFooterState",
      "showMarketingBox",
      "showLeaderBoard",
      "showMobileFeed",
      "getShowBuyDialog",
      "getCurrentStep",
      "becomeTutorDialog",
      "getRequestTutorDialog",
      "getShowPaymeDialog",
      "isFrymo",

    ]),
    isMobile(){
      return this.$vuetify.breakpoint.smAndDown;
    },
    showFeed() {
      if (this.$vuetify.breakpoint.smAndDown && this.getMobileFooterState) {
        return this.showMobileFeed;
      } else {
        return true;
      }
    },
    cookiesShow() {
      if(!this.accountUser){
        return this.getCookieAccepted();
      }else{
        return true;
      }
    },
    universitySelectPopup() {
      return this.getShowSelectUniPopUpInterface;
    },
    showMarketingMobile() {
      return this.$vuetify.breakpoint.smAndDown && this.showMarketingBox;
    },
    showLeadersMobile() {
      return this.$vuetify.breakpoint.smAndDown && this.showLeaderBoard;
    }
  },
  updated: function() {
    this.$nextTick(function() {
      if(!!global.dataLayer){
       this.fireOptimizeActivate();
      }
      // Code that will run only after the
      // entire question-details has been re-rendered
    });
  },
  mounted: function() {
    if(!!global.dataLayer){
      this.$nextTick(function() {
         this.fireOptimizeActivate();
      });
    }
  },
  watch: {
    getShowToaster: function(val) {
      let self = this;
      if(val){
          this.toasterTimeoutObj = setTimeout(()=>{
          if (val) {
            self.updateToasterParams({
            showToaster: false
            });
          }
        }, this.getToasterTimeout)
      }else{
        global.clearTimeout(this.toasterTimeoutObj);
        self.updateToasterParams({
          showToaster: false
        });
      }
    },
    getToasterTimeout:function(){
      let self = this;
      global.clearTimeout(this.toasterTimeoutObj);
      this.toasterTimeoutObj = setTimeout(()=>{
          self.updateToasterParams({
          showToaster: false
        });
      }, this.getToasterTimeout);
    },
    '$route'(){
      this.$nextTick(()=>{
        this.fireOptimizeActivate()
      })
    },
  },
  methods: {
    ...mapActions([
      "updateToasterParams",
      "updateLoginDialogState",
      "updateNewQuestionDialogState",
      "changeSelectPopUpUniState",
      "updateDialogState",
      "setCookieAccepted",
      "updateOnBoardState",
      "updateShowBuyDialog",
      "updateCurrentStep",
      "changeSelectUniState",
      "updateRequestDialog",
      "openChatInterface",
      "setTutorRequestAnalyticsOpenedFrom",
      "fireOptimizeActivate"
    ]),
    ...mapGetters(["getCookieAccepted", "getIsFeedTabActive"]),
    enterPayme(){
      walletService.getPaymeLink().then(({data})=>{
        global.open(data.link, '_blank', 'height=520,width=440');
      })
    },
    onFooterStepChange() {
      console.log('footer changed');
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
          global.localStorage.setItem("sb_transactionError", transactionId);
          console.log(error);
        }
      );
    }
  },
  created() {
    if(!!this.$route.query && this.$route.query.requesttutor){
        if(this.$route.query.requesttutor.toLowerCase() === 'open'){
            setTimeout(() => {
              this.setTutorRequestAnalyticsOpenedFrom({
                component: 'query',
                path: this.$route.path
              });
                this.updateRequestDialog(true)
            }, 170);
        }
    }
      if(this.$vuetify.breakpoint.xsOnly){
          if(!!this.$route.query && this.$route.query.chat){
              if(this.$route.query.chat.toLowerCase() === 'expand'){
                  setTimeout(() => {
                      this.openChatInterface(true)
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
      global.addEventListener("resize", event => {
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
    global.addEventListener('error', (event)=>{
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

