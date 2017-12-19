<template>
    <v-app>
        <!--<app-header ref="header" v-if="$route.meta.showHeader" :showMoreOptions="showMoreOptions" :showSingleLine="$route.meta.showHeaderSingleLine">-->
           <!--<template  v-if="isMobileApp">-->
               <!--<router-view name="mobileHeaderFirstLine" :slot="`${$route.name}Mobile`"></router-view>-->
                <!--<router-view :name="`${$route.name}SecondLineMobile`" :slot="`${$route.name}SecondLineMobile`"></router-view>-->
           <!--</template>-->
        <!--</app-header>-->
        <router-view :name="`header${isMobileApp}`"></router-view>
        <router-view :name="`verticalList${isMobileApp}`" :class="`${$route.name}${isMobileApp}`"></router-view>
        <router-view ref="personalize" name="personalize"></router-view>
        <v-content>
            <div class="loader" v-show="!$route.meta.isStatic&&loading">
                <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
            </div>
            <router-view ref="mainPage" v-show="!loading||$route.meta.isStatic"></router-view>
        </v-content>
    </v-app>
</template>
<script>
    import AppHeader from '../header/header.vue'
    import { mapGetters } from 'vuex'
    export default {
        computed: {
            ...mapGetters(['loading']),
            showMoreOptions(){return !(this.$vuetify.breakpoint.xsOnly&&this.$route.name.includes("Details"))},
            isMobileApp(){
                return (this.$vuetify.breakpoint.xsOnly)?'Mobile':"";
            }
        },
        components: { AppHeader }
    }
</script>
<style lang="less" src="./app.less"></style>

