<template>
    <v-app>
        <router-view name="header"></router-view>
        <v-content class="site-content" :class="{'loading':getIsLoading}">
            <div class="loader" v-show="getIsLoading">
                <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
            </div>
            <signup-banner v-if="!accountUser && showRegistrationBanner"></signup-banner>
            <router-view ref="mainPage"></router-view>
        </v-content>
    </v-app>
</template>
<script>
    import signupBanner from './../helpers/signup-banner/signup-banner.vue'
    import { mapGetters } from 'vuex'
    export default {
        components:{signupBanner},
        computed: {
            ...mapGetters(["getIsLoading", "accountUser","showRegistrationBanner"]),
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
        }
    }
</script>
<style lang="less" src="./app.less"></style>

