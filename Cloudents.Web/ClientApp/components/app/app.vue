<template>
  <v-app>
    <v-tour
        name="myTour"
        :steps="tourObject.tourSteps"
        :options="tourObject.toursOptions"
        :callbacks="tourObject.tourCallbacks"
      ></v-tour>
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
                       :max-width="'640px'"
                       :content-class="'question-request-dialog'">
                <Add-Question></Add-Question>
            </sb-dialog>
            <sb-dialog :isPersistent="true"
                       :showDialog="getRequestTutorDialog"
                       :popUpType="'tutorRequestDialog'"
                       :max-width="'640px'"
                       :content-class="'tutor-request-dialog'">
                <tutor-request></tutor-request>
            </sb-dialog>
            <sb-dialog :showDialog="newIsraeliUser"
                       :popUpType="'newIsraeliUserDialog'"
                       :content-class="`newIsraeliPop ${isRtl? 'rtl': ''}` ">
                <new-israeli-pop :closeDialog="closeNewIsraeli"></new-israeli-pop>
            </sb-dialog>
            <sb-dialog :showDialog="getDialogState"
                       :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition' "
                       :popUpType="'uploadDialog'"
                       :maxWidth="'852'"
                       :onclosefn="setUploadDialogState"
                       :activateOverlay="false"
                       :isPersistent="$vuetify.breakpoint.smAndUp"
                       :content-class="'upload-dialog'">
                <upload-multiple-files v-if="getDialogState"></upload-multiple-files>
            </sb-dialog>

        <sb-dialog :showDialog="becomeTutorDialog"
                   :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition' "
                   :popUpType="'becomeTutorDialog'"
                   :maxWidth="'762'"
                   :onclosefn="setUploadDialogState"
                   :activateOverlay="false"
                   :isPersistent="$vuetify.breakpoint.smAndUp"
                   :content-class="'become-tutor'">
            <become-tutor v-if="becomeTutorDialog"></become-tutor>
        </sb-dialog>



            <sb-dialog :showDialog="getOnBoardState"
                       :popUpType="'onBoardGuide'"
                       :content-class=" $vuetify.breakpoint.smAndUp ?  'onboard-guide-container' : ''"
                       :maxWidth="'1280px'"
                       :isPersistent="$vuetify.breakpoint.smAndUp">
                <board-guide></board-guide>
            </sb-dialog>

            <sb-dialog :showDialog="newBallerDialog"
                       :popUpType="'newBallerDialog'"
                       :content-class="'new-baller'"
                       :maxWidth="'700px'"
                       :isPersistent="$vuetify.breakpoint.smAndUp">
                <new-baller></new-baller>
            </sb-dialog>

            <sb-dialog :showDialog="getShowBuyDialog"
                       :popUpType="'buyTokens'"
                       :content-class="'buy-tokens-popup'"
                       :onclosefn="closeSblToken">
                <buy-tokens></buy-tokens>
            </sb-dialog>

            <mobile-footer v-show="$vuetify.breakpoint.xsOnly && getMobileFooterState && !hideFooter"
                           :onStepChange="onFooterStepChange"></mobile-footer>
        </v-content>
        <v-snackbar absolute top :timeout="toasterTimeout" :class="getShowToasterType" :value="getShowToaster">
            <div class="text-wrap" v-html="getToasterText"></div>
        </v-snackbar>

        <v-snackbar absolute top :timeout="0" :value="getShowPayMeToaster">
            <div class="text-wrap">
              <a @click="enterPayme()" style="text-decoration: none;" v-language:inner>app_payme_toaster_text</a>
            </div>
            <div>
              <v-icon style="font-size:10px;color:rgb(255, 255, 255, .54);display: flex;" @click="closePayMe">sbf-close</v-icon>
            </div>
        </v-snackbar>
    </v-app>
</template>
<script>
  
import { mapGetters, mapActions } from "vuex";
import sbDialog from "../wrappers/sb-dialog/sb-dialog.vue";
import loginToAnswer from "../question/helpers/loginToAnswer/login-answer.vue";
// import AddQuestion from "../question/addQuestion/addQuestion.vue";
import AddQuestion from "../question/askQuestion/askQuestion.vue";
import uploadMultipleFiles from "../results/helpers/uploadMultipleFiles/uploadMultipleFiles.vue";
import newBaller from "../helpers/newBaller/newBaller.vue";
import {  GetDictionary,  LanguageService} from "../../services/language/languageService";
import tourService from "../../services/tourService";
import walletService from "../../services/walletService";
import uniSelectPop from "../helpers/uni-select-popup/uniSelectPop.vue";
// import uniSelect from "../helpers/uni-select-popup/uniSelect.vue";
import newIsraeliPop from "../dialogs/israeli-pop/newIsraeliPop.vue";
import reportItem from "../results/helpers/reportItem/reportItem.vue";
import mobileFooter from "../footer/mobileFooter/mobileFooter.vue";
import marketingBox from "../helpers/marketingBox/marketingBox.vue";
import leadersBoard from "../helpers/leadersBoard/leadersBoard.vue";
import boardGuide from "../helpers/onBoardGuide/onBoardGuide.vue";
import buyTokens from "../dialogs/buyTokens/buyTokens.vue";
import chatComponent from "../chat/chat.vue";
import becomeTutor from "../becomeTutor/becomeTutor.vue";
import tutorList from "../helpers/tutorList/tutorList.vue";
      import tutorRequest from '../tutorRequest/tutorRequest.vue';
export default {
  components: {
    AddQuestion,
    sbDialog,
    loginToAnswer,
    uniSelectPop,
    // uniSelect,
    chatComponent,
    newIsraeliPop,
    reportItem,
    mobileFooter,
    marketingBox,
    leadersBoard,
    boardGuide,
    uploadMultipleFiles,
    buyTokens,
    newBaller,
    becomeTutor,
        tutorList,
        tutorRequest
  },
  data() {
    return {
      acceptIsraeli: true,
      isRtl: global.isRtl,
      toasterTimeout: 5000,
      hideFooter: false,
      showOnBoardGuide: true,
      showBuyTokensDialog: false,

      tourObject: {
        region:
          global.country.toLocaleLowerCase() === "il" ? "ilTours" : "usTours",
        tourCallbacks: {
          onStop: this.tourClosed
        },
        toursOptions: tourService.toursOptions,
        tourSteps: []
      }
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
      "getToasterText",
      "getMobileFooterState",
      "showMarketingBox",
      "showLeaderBoard",
      "showMobileFeed",
      "HomeworkHelp_isDataLoaded",
      "StudyDocuments_isDataLoaded",
      "getOnBoardState",
      "getShowBuyDialog",
      "getShowPayMeToaster",
      "getCurrentStep",
      "newBallerDialog",
        "becomeTutorDialog",
         "getRequestTutorDialog"

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
      return this.getCookieAccepted();
    },
    universitySelectPopup() {
      return this.getShowSelectUniPopUpInterface;
    },
    showMarketingMobile() {
      return this.$vuetify.breakpoint.smAndDown && this.showMarketingBox;
    },
    showLeadersMobile() {
      return this.$vuetify.breakpoint.smAndDown && this.showLeaderBoard;
    },

    newIsraeliUser() {
      return false;
      // return !this.accountUser && global.country.toLowerCase() === "il" && !this.acceptIsraeli && (this.$route.path.indexOf("ask") > -1 || this.$route.path.indexOf("note") > -1);
    }
  },
  updated: function() {
    this.$nextTick(function() {
      if(!!dataLayer){
        dataLayer.push({ event: "optimize.activate" });
      }
      // Code that will run only after the
      // entire question-details has been re-rendered
    });
  },
  mounted: function() {
    this.$nextTick(function() {
      if(!!dataLayer){
        dataLayer.push({ event: "optimize.activate" });
      }
      
      // Code that will run only after the
      // entire question-details has been rendered
    });
  },
  watch: {
    getShowToaster: function(val) {
      if (val) {
        var self = this;
        setTimeout(function() {
          self.updateToasterParams({
            showToaster: false
          });
        }, this.toasterTimeout);
      }
    }
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
      "updateShowPayMeToaster",
      "updateCurrentStep",
      "changeSelectUniState",
      "updateRequestDialog"
    ]),
    ...mapGetters(["getCookieAccepted", "getIsFeedTabActive"]),
    enterPayme(){
      walletService.getPaymeLink().then(({data})=>{
        global.open(data.link, '_blank', 'height=520,width=440');
      })
    },
    onFooterStepChange() {
      this.tourTempClose();
    },
    closeSblToken() {
      this.updateShowBuyDialog(false);
    },
    closePayMe(){
      this.updateShowPayMeToaster(false);
    },
    tourClosed: function() {
      console.log("tourClosed");
      global.localStorage.setItem("sb_walkthrough_supressed", true);
    },
    tourTempClose: function() {
      this.$tours["myTour"].close();
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
    closeNewIsraeli() {
      //the set to the local storage happens in the component itself
      this.acceptIsraeli = true;
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
    if(!!this.accountUser && this.accountUser.needPayment){
      this.updateShowPayMeToaster(true);
    }
    if(!!this.$route.query && this.$route.query.requesttutor){
        console.log(this.$route.query.requesttutor);
        if(this.$route.query.requesttutor.toLowerCase() === 'open'){
            console.log(this.$route.query.requesttutor);
            setTimeout(() => {
                this.updateRequestDialog(true)
            }, 170);
        }
    }

    //this.openOnboardGuide();
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
    this.$nextTick(() => {
      setTimeout(() => {
        this.acceptIsraeli = !!global.localStorage.getItem("sb-newIsraei");
      }, 130);
    });

    global.addEventListener("resize", event => {
      if (global.isMobileAgent) {
        if (
          (document && document.activeElement.tagName == "INPUT") ||
          document.activeElement.tagName == "TEXTAREA"
        ) {
          this.hideFooter = true;
        } else {
          this.hideFooter = false;
        }
      }
    });
    let failedTranscationId = global.localStorage.getItem(
      "sb_transactionError"
    );
    if (failedTranscationId) {
      global.localStorage.removeItem("sb_transactionError");
      let transactionObjectError = {
        id: failedTranscationId
      };
      this.tryBuyTokens(transactionObjectError);
    }
  }
};
</script>
<style lang="less" src="./app.less"></style>
<!--<style lang="less" src="./main.less"></style>-->

