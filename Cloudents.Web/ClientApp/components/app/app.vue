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
        </v-content>
    </v-app>
</template>
<script>
    import { mapGetters, mapActions } from 'vuex'
    export default {
        data(){
            return {
                acceptedCookies: false
            }
        },
        computed: {
            ...mapGetters(["getIsLoading", "accountUser","showRegistrationBanner"]),
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
            removeCookiesPopup: function(){
                global.localStorage.setItem("sb-acceptedCookies", true);
                this.acceptedCookies = true;
            },
        },
        created(){
           this.acceptedCookies = (global.localStorage.getItem("sb-acceptedCookies") === 'true');
        }
    }
</script>
<style lang="less" src="./app.less"></style>

