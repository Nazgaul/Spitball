<template>
    <v-app>
        <router-view name="header"></router-view>

        <v-content class="site-content" :class="{'loading':getIsLoading}">
            <div class="loader" v-show="getIsLoading">
                <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
            </div>
            <div v-show="showUniSelect" style="height: 100%;">
                <uni-select></uni-select>
            </div>
            <div style="height: 100%;" v-show="showMarketingMobile && getMobileFooterState">
                <marketing-box></marketing-box>
            </div>
            <div v-if="showLeadersMobile && getMobileFooterState">
                <leaders-board></leaders-board>
            </div>
            <v-tour
                    name="myTour"
                    :steps="tourObject.tourSteps"
                    :options="tourObject.toursOptions"
                    :callbacks="tourObject.tourCallbacks"
            ></v-tour>
            <router-view v-show="!showUniSelect && showFeed" ref="mainPage"></router-view>

            <!--<router-view v-show="!showUniSelect && showFeed && !getOnBoardState" ref="mainPage"></router-view>-->

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

            <sb-dialog
                    :showDialog="universitySelectPopup"
                    :popUpType="'universitySelectPopup'"
                    :onclosefn="closeUniPopDialog"
                    :activateOverlay="true"
                    :content-class="'pop-uniselect-container'"
            >
                <uni-Select-pop :showDialog="universitySelectPopup"
                                :popUpType="'universitySelectPopup'"></uni-Select-pop>
            </sb-dialog>


            <sb-dialog
                    :isPersistent="true"
                    :showDialog="newQuestionDialogSate"
                    :popUpType="'newQuestion'"
                    :content-class="'newQuestionDialog'"
            >
                <Add-Question></Add-Question>
                <!-- <New-Question></New-Question> -->
            </sb-dialog>
            <sb-dialog
                    :showDialog="newIsraeliUser"
                    :popUpType="'newIsraeliUserDialog'"
                    :content-class="`newIsraeliPop ${isRtl? 'rtl': ''}` "
            >
                <new-israeli-pop :closeDialog="closeNewIsraeli"></new-israeli-pop>
            </sb-dialog>

            <!--upload dilaog-->
            <!--<sb-dialog-->
                    <!--:showDialog="getDialogState"-->
                    <!--:transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition' "-->
                    <!--:popUpType="'uploadDialog'"-->
                    <!--:maxWidth="'966px'"-->
                    <!--:onclosefn="setUploadDialogState"-->
                    <!--:activateOverlay="isUploadAbsoluteMobile"-->
                    <!--:isPersistent="$vuetify.breakpoint.smAndUp"-->
                    <!--:content-class="isUploadAbsoluteMobile ? 'upload-dialog mobile-absolute' : 'upload-dialog'"-->
            <!--&gt;-->
                <!--<upload-files v-if="getDialogState"></upload-files>-->
            <!--</sb-dialog>-->
            <!--multiple upload dilaog-->
            <sb-dialog
                    :showDialog="getDialogState"
                    :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition' "
                    :popUpType="'uploadDialog'"
                    :maxWidth="'966px'"
                    :onclosefn="setUploadDialogState"
                    :activateOverlay="false"
                    :isPersistent="$vuetify.breakpoint.smAndUp"
                    :content-class="isUploadAbsoluteMobile ? 'upload-dialog' : 'upload-dialog'"
            >
                <upload-multiple-files v-if="getDialogState"></upload-multiple-files>
            </sb-dialog>

            <sb-dialog
                    :showDialog="getOnBoardState"
                    :popUpType="'onBoardGuide'"
                    :content-class=" $vuetify.breakpoint.smAndUp ?  'onboard-guide-container' : ''"
                    :maxWidth="'1280px'"
                    :isPersistent="$vuetify.breakpoint.smAndUp"
            >
                <board-guide></board-guide>
            </sb-dialog>

            <sb-dialog
                    :showDialog="showBuyTokensDialog"
                    :popUpType="'buyTokens'"
                    :content-class="'buy-tokens-popup'"
            >
                <buy-tokens></buy-tokens>
            </sb-dialog>

            <mobile-footer v-show="$vuetify.breakpoint.xsOnly && getMobileFooterState && !hideFooter"
                           :onStepChange="onFooterStepChange"></mobile-footer>
        </v-content>
        <v-snackbar absolute top :timeout="toasterTimeout" :value="getShowToaster">
            <div class="text-wrap" v-html="getToasterText"></div>
        </v-snackbar>
    </v-app>
</template>
<script>
    import { mapGetters, mapActions } from "vuex";
    import sbDialog from "../wrappers/sb-dialog/sb-dialog.vue";
    import loginToAnswer from "../question/helpers/loginToAnswer/login-answer.vue";
    import NewQuestion from "../question/newQuestion/newQuestion.vue";
    import AddQuestion from "../question/addQuestion/addQuestion.vue";
    import uploadFiles from "../results/helpers/uploadFiles/uploadFiles.vue";
    import uploadMultipleFiles from "../results/helpers/uploadMultipleFiles/uploadMultipleFiles.vue";

    import {
        GetDictionary,
        LanguageService
    } from "../../services/language/languageService";
    import tourService from "../../services/tourService";
    import uniSelectPop from "../helpers/uni-select/uniSelectPop.vue";
    import uniSelect from "../helpers/uni-select/uniSelect.vue";
    import newIsraeliPop from "../dialogs/israeli-pop/newIsraeliPop.vue";
    import reportItem from "../results/helpers/reportItem/reportItem.vue";
    import mobileFooter from "../footer/mobileFooter/mobileFooter.vue";
    import marketingBox from "../helpers/marketingBox/marketingBox.vue";
    import leadersBoard from "../helpers/leadersBoard/leadersBoard.vue";
    import boardGuide from "../helpers/onBoardGuide/onBoardGuide.vue";
    import buyTokens from "../dialogs/buyTokens/buyTokens.vue";


    export default {
        components: {
            AddQuestion,
            NewQuestion,
            sbDialog,
            loginToAnswer,
            uniSelectPop,
            uniSelect,
            uploadFiles,
            newIsraeliPop,
            reportItem,
            mobileFooter,
            marketingBox,
            leadersBoard,
            boardGuide,
            uploadMultipleFiles
            boardGuide,
            buyTokens
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
                    region: global.country.toLocaleLowerCase() === 'il' ? 'ilTours' : 'usTours',
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
                "showRegistrationBanner",
                "loginDialogState",
                "newQuestionDialogSate",
                "getShowSelectUniPopUpInterface",
                "getShowSelectUniInterface",
                "getDialogState",
                "getUploadFullMobile",
                "confirmationDialog",
                "getShowToaster",
                "getToasterText",
                "getMobileFooterState",
                "showMarketingBox",
                "showLeaderBoard",
                "showMobileFeed",
                "HomeworkHelp_isDataLoaded",
                "StudyDocuments_isDataLoaded",
                "getOnBoardState"
            ]),
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
            showUniSelect() {
                if (this.getShowSelectUniInterface) {
                    this.tourTempClose();
                }
                return this.getShowSelectUniInterface;
            },
            showMarketingMobile() {
                return this.$vuetify.breakpoint.smAndDown && this.showMarketingBox;
            },
            showLeadersMobile() {
                return this.$vuetify.breakpoint.smAndDown && this.showLeaderBoard;
            },
            isUploadAbsoluteMobile() {
                return this.$vuetify.breakpoint.smAndDown && this.getUploadFullMobile;
            },
            newIsraeliUser() {
                return false;
                // return !this.accountUser && global.country.toLowerCase() === "il" && !this.acceptIsraeli && (this.$route.path.indexOf("ask") > -1 || this.$route.path.indexOf("note") > -1);
            }
        },
        updated: function () {
            this.$nextTick(function () {
                dataLayer.push({event: "optimize.activate"});
                // Code that will run only after the
                // entire question-details has been re-rendered
            });
        },
        mounted: function () {
            this.$nextTick(function () {
                dataLayer.push({event: "optimize.activate"});
                // Code that will run only after the
                // entire question-details has been rendered
            });
        },
        watch: {
            getShowToaster: function (val) {
                if (val) {
                    var self = this;
                    setTimeout(function () {
                        self.updateToasterParams({
                            showToaster: false
                        });
                    }, this.toasterTimeout);
                }
            },
            HomeworkHelp_isDataLoaded: function (val) {
                let supressed = global.localStorage.getItem("sb_walkthrough_supressed");
                let self = this;
                if (val && !supressed && !!self.accountUser) {
                    setTimeout(() => {
                        if (self.$route.name === "ask" && !this.showUniSelect) {
                            if (self.$vuetify.breakpoint.xsOnly) {
                                self.tourObject.tourSteps = tourService[self.tourObject.region].HWSteps.mobile;
                                if (self.getIsFeedTabActive()) {
                                    self.$tours["myTour"].start();
                                }
                            } else {
                                self.tourObject.tourSteps = tourService[self.tourObject.region].HWSteps.desktop;
                                self.$tours["myTour"].start();
                            }
                        }
                    }, 3000)
                }
            },
            StudyDocuments_isDataLoaded: function (val) {
                let supressed = global.localStorage.getItem("sb_walkthrough_supressed");
                let self = this;
                if (val && !supressed && !!self.accountUser) {
                    setTimeout(() => {
                        if (self.$route.name === "note" && !this.showUniSelect) {
                            if (self.$vuetify.breakpoint.xsOnly) {
                                self.tourObject.tourSteps = tourService[self.tourObject.region].StudyDocumentsSteps.mobile;
                                if (self.getIsFeedTabActive()) {
                                    self.$tours["myTour"].start();
                                }
                            } else {
                                self.tourObject.tourSteps = tourService[self.tourObject.region].StudyDocumentsSteps.desktop;
                                self.$tours["myTour"].start();
                            }

                        }
                    }, 3000)
                }
            },
            $route: function () {
                this.tourTempClose();
                this.openOnboardGuide();
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

            ]),
            ...mapGetters(["getCookieAccepted", "getIsFeedTabActive"]),
            onFooterStepChange() {
                this.tourTempClose();
            },
            openOnboardGuide(){
                let isLogedIn = this.accountUser;
                let supressed = global.localStorage.getItem("sb-onboard-supressed");
                let validRoutesNames = ['ask', 'note'].indexOf(this.$route.name) > -1;
                if(isLogedIn && !supressed && validRoutesNames){
                  setTimeout(()=>{
                      this.updateOnBoardState(true);
                  },)

              }
            },
            tourClosed: function () {
                console.log("tourClosed");
                global.localStorage.setItem("sb_walkthrough_supressed", true);
            },
            tourTempClose: function () {
                this.$tours["myTour"].close();
            },
            removeCookiesPopup: function () {
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
            }
        },
        created() {
            this.openOnboardGuide();
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
            // setTimeout(()=>{
            //     this.showBuyTokensDialog = true;
            // }, 2000)
        }
    };
</script>
<style lang="less" src="./app.less"></style>

