<template>
  <v-app>
    <!-- <v-tour
        name="myTour"
        :steps="tourObject.tourSteps"
        :options="tourObject.toursOptions"
        :callbacks="tourObject.tourCallbacks"
      ></v-tour> -->
    <router-view name="header"></router-view>
    <router-view v-if="showSsideMenu" name="sideMenu"></router-view>
    <v-content class="site-content" :class="{'loading':getIsLoading}">
      <div class="loader" v-show="getIsLoading">
        <v-progress-circular indeterminate v-bind:size="50" color="amber"/>
      </div>
      <chatComponent v-if="isMobile"/>
      <tutorList v-if="showLeadersMobile && getMobileFooterState"/>
      <router-view name="verticals"/>
      <router-view class="main-container" v-show="showFeed" ref="mainPage"/>
      <chatComponent v-if="!isMobile"/>
      <div class="s-cookie-container" :class="{'s-cookie-hide': cookiesShow}">
        <span v-language:inner="'app_cookie_toaster_text'"/>&nbsp;
        <span class="cookie-approve">
          <button @click="removeCookiesPopup()" style="outline:none;" v-language:inner="'app_cookie_toaster_action'"/>
        </span>
      </div>
      <dialogsContainer/>
      <mobile-footer v-if="$vuetify.breakpoint.xsOnly && getMobileFooterState && !hideFooter"/>
    </v-content>
    <v-snackbar absolute top :timeout="getToasterTimeout" :class="getShowToasterType" :value="getShowToaster">
        <div class="text-wrap" v-html="getToasterText"></div>
    </v-snackbar>
  </v-app>
</template>
<script>
  
import { mapGetters, mapActions } from "vuex";

import {LanguageService} from "../../services/language/languageService";
import walletService from "../../services/walletService";
import reportItem from "../results/helpers/reportItem/reportItem.vue";
import mobileFooter from '../pages/layouts/mobileFooter/mobileFooter.vue';
import marketingBox from "../helpers/marketingBox/marketingBox.vue";
import chatComponent from "../chat/chat.vue";
import tutorList from "../helpers/tutorList/tutorList.vue";
import dialogsContainer from '../pages/global/dialogs/dialogsContainer.vue'

export default {
  components: {
    dialogsContainer,
    chatComponent,
    reportItem,
    mobileFooter,
    marketingBox,
    tutorList,
  },
  data() {
    return {
      hideFooter: false,
      toasterTimeoutObj: null,
    };
  },
  computed: {
    ...mapGetters([
      "getIsLoading",
      "accountUser",
      "getShowToaster",
      "getShowToasterType",
      "getToasterTimeout",
      "getToasterText",
      "getMobileFooterState",
      "showLeaderBoard",
      "showMobileFeed",
      "getShowSchoolBlock"
    ]),
    showSsideMenu(){
      if(this.$vuetify.breakpoint.xsOnly){
        return this.getShowSchoolBlock;
      }else{
        return true;
      }
    },
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
      "updateDialogState",
      "setCookieAccepted",
      "updateRequestDialog",
      "openChatInterface",
      "setTutorRequestAnalyticsOpenedFrom",
      "fireOptimizeActivate"
    ]),
    ...mapGetters(["getCookieAccepted"]),
    removeCookiesPopup: function() {
      this.setCookieAccepted();
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