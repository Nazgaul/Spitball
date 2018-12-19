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
            <router-view v-show="!showUniSelect" ref="mainPage"></router-view>
            <div class="s-cookie-container" :class="{'s-cookie-hide': cookiesShow}">
                <span v-language:inner>app_cookie_toaster_text</span> &nbsp;
                <span class="cookie-approve"><button @click="removeCookiesPopup()" style="outline:none;"
                                                     v-language:inner>app_cookie_toaster_action</button></span>
            </div>
            <sb-dialog :showDialog="loginDialogState" :popUpType="'loginPop'" :content-class="'login-popup'">
                <login-to-answer></login-to-answer>
            </sb-dialog>
            <sb-dialog :showDialog="universitySelectPopup" :popUpType="'universitySelectPopup'"
                       :onclosefn="closeUniPopDialog" :activateOverlay="true"
                       :content-class="'pop-uniselect-container'">
                <uni-Select-pop :showDialog="universitySelectPopup"
                                :popUpType="'universitySelectPopup'"></uni-Select-pop>
            </sb-dialog>
            <sb-dialog :isPersistent="true" :showDialog="newQuestionDialogSate" :popUpType="'newQuestion'"
                       :content-class="'newQuestionDialog'">
                <new-question></new-question>
            </sb-dialog>
            <sb-dialog :showDialog="newIsraeliUser" :popUpType="'newIsraeliUserDialog'"
                       :content-class="`newIsraeliPop ${isRtl? 'rtl': ''}` ">
                <new-israeli-pop :closeDialog="closeNewIsraeli"></new-israeli-pop>
            </sb-dialog>

            <!--upload dilaog-->
            <sb-dialog :showDialog="getDialogState"
                       :transitionAnimation="$vuetify.breakpoint.smAndUp ? 'slide-y-transition' : 'slide-y-reverse-transition' "
                       :popUpType="'uploadDialog'"
                       :fullWidth="true"
                       :onclosefn="setUploadDialogState"
                       :activateOverlay="isUploadAbsoluteMobile"
                       :isPersistent="$vuetify.breakpoint.smAndUp"
                       :content-class="isUploadAbsoluteMobile ? 'upload-dialog mobile-absolute' : 'upload-dialog'">
                <upload-files v-if="getDialogState"></upload-files>
            </sb-dialog>
        </v-content>
        <v-snackbar absolute top :timeout="toasterTimeout" :value="getShowToaster">
            <div class="text-wrap" v-html="getToasterText"></div>
        </v-snackbar>
    </v-app>
</template>
<script>
    import { mapGetters, mapActions } from 'vuex';
    import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';
    import loginToAnswer from '../question/helpers/loginToAnswer/login-answer.vue'
    import NewQuestion from "../question/newQuestion/newQuestion.vue";
    import uploadFiles from "../results/helpers/uploadFiles/uploadFiles.vue"
    import { GetDictionary } from '../../services/language/languageService';
    import uniSelectPop from '../helpers/uni-select/uniSelectPop.vue';
    import uniSelect from '../helpers/uni-select/uniSelect.vue';
    import newIsraeliPop from '../dialogs/israeli-pop/newIsraeliPop.vue';
    import reportItem from '../results/helpers/reportItem/reportItem.vue'
    export default {
        components: {
            NewQuestion,
            sbDialog,
            loginToAnswer,
            uniSelectPop,
            uniSelect,
            uploadFiles,
            newIsraeliPop,
            reportItem
        },
        data() {
            return {
                acceptIsraeli: true,
                isRtl: global.isRtl,
                toasterTimeout: 5000,
            }
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
                "getToasterText"
            ]),
            cookiesShow() {
                return this.getCookieAccepted()
            },
            universitySelectPopup() {
                return this.getShowSelectUniPopUpInterface;
            },
            showUniSelect() {
                return this.getShowSelectUniInterface;
            },
            isUploadAbsoluteMobile() {
                return this.$vuetify.breakpoint.smAndDown && this.getUploadFullMobile
            },
            newIsraeliUser() {
                return false
                // return !this.accountUser && global.country.toLowerCase() === "il" && !this.acceptIsraeli && (this.$route.path.indexOf("ask") > -1 || this.$route.path.indexOf("note") > -1);
            }
        },
        updated: function () {
            this.$nextTick(function () {
                dataLayer.push({'event': 'optimize.activate'});
                // Code that will run only after the
                // entire question-details has been re-rendered
            })
        },
        mounted: function () {
            this.$nextTick(function () {
                dataLayer.push({'event': 'optimize.activate'});
                // Code that will run only after the
                // entire question-details has been rendered
            })
        },
        watch:{
            getShowToaster: function (val) {
                if (val) {
                    var self = this;
                    setTimeout(function () {
                        self.updateToasterParams({
                            showToaster: false
                        })
                    }, this.toasterTimeout)
                }
            },
        },
        methods: {
            ...mapActions(['updateToasterParams', 'updateLoginDialogState', 'updateNewQuestionDialogState', 'changeSelectPopUpUniState', 'updateDialogState', 'setCookieAccepted']),
            ...mapGetters(['getCookieAccepted']),
            removeCookiesPopup: function () {
                this.setCookieAccepted();
            },
            closeUniPopDialog() {
                this.changeSelectPopUpUniState(false);
            },

            setUploadDialogState() {
                this.updateDialogState(false)
            },
            closeNewIsraeli() {
                //the set to the local storage happens in the component itself
                this.acceptIsraeli = true;
            }

        },
        created() {
            this.$root.$on('closePopUp', (name) => {
                if (name === 'suggestions') {
                    this.showDialogSuggestQuestion = false
                } else if (name === 'newQuestionDialog') {
                    this.updateNewQuestionDialogState(false)
                } else {
                    this.updateLoginDialogState(false)
                }
            });

            this.acceptedCookies = this.getCookieAccepted();
            this.$nextTick(() => {
                setTimeout(() => {
                    this.acceptIsraeli = !!global.localStorage.getItem("sb-newIsraei");
                }, 130)
            })

        }
    }
</script>
<style lang="less" src="./app.less"></style>

