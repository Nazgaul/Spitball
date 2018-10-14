<template>
    <v-app>
        <router-view name="header"></router-view>
        <v-content class="site-content" :class="{'loading':getIsLoading}">
            <div class="loader" v-show="getIsLoading">
                <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
            </div>
            <router-view ref="mainPage"></router-view>
            <div class="s-cookie-container" :class="{'s-cookie-hide': cookiesShow}">
              <span style="text-align:left;" v-language:inner>app_cookie_toaster_text</span> &nbsp;
              <span class="cookie-approve"><button @click="removeCookiesPopup()" style="outline:none;" v-language:inner>app_cookie_toaster_action</button></span>
            </div>
            <sb-dialog :showDialog="loginDialogState" :popUpType="'loginPop'" :content-class="'login-popup'">
                <login-to-answer></login-to-answer>
            </sb-dialog>
            <sb-dialog :showDialog="newQuestionDialogSate" :popUpType="'newQuestion'" :content-class="'newQuestionDialog'">
                <new-question></new-question>
            </sb-dialog>

        </v-content>
    </v-app>
</template>
<script>
    import { mapGetters, mapActions } from 'vuex';
    import sbDialog from '../wrappers/sb-dialog/sb-dialog.vue';
    import loginToAnswer from '../question/helpers/loginToAnswer/login-answer.vue'
    import NewQuestion from "../question/newQuestion/newQuestion.vue";
    import {GetDictionary} from '../../services/language/languageService'
    export default {
        components: {
            NewQuestion,
            sbDialog,
            loginToAnswer
        },
        data(){
            return {
                acceptedCookies: false,
            }
        },
        computed: {
            ...mapGetters(["getIsLoading", "accountUser","showRegistrationBanner", "loginDialogState", "newQuestionDialogSate"]),
            cookiesShow(){
                return this.acceptedCookies
            }
        },
        updated: function () {
            this.$nextTick(function () {
                dataLayer.push({ 'event': 'optimize.activate' });
                // Code that will run only after the
                // entire question-details has been re-rendered
            })
        },
        mounted: function () {
            this.$nextTick(function () {
                dataLayer.push({ 'event': 'optimize.activate' });
                // Code that will run only after the
                // entire question-details has been rendered
            })
        },
        methods: {
            ...mapActions([ 'updateLoginDialogState', 'updateNewQuestionDialogState']),
            removeCookiesPopup: function(){
                global.localStorage.setItem("sb-acceptedCookies", true);
                this.acceptedCookies = true;
            },
        },
        created() {
            this.$root.$on('closePopUp', (name) => {
                if (name === 'suggestions') {
                    this.showDialogSuggestQuestion = false
                } else if(name === 'newQuestionDialog'){
                    this.updateNewQuestionDialogState(false)
                }else{
                    this.updateLoginDialogState(false)
                }
            });
           this.acceptedCookies = (global.localStorage.getItem("sb-acceptedCookies") === 'true');

        }
    }
</script>
<style lang="less" src="./app.less"></style>

