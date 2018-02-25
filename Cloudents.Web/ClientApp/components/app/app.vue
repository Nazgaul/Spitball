<template>
    <v-app>
        <router-view name="header"></router-view>
        <v-content :class="{'loading':loading}">
            <div class="loader" v-show="loading">
                <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
            </div>
            <router-view ref="mainPage"></router-view>
        </v-content>
    </v-app>
</template>
<script>
    import smartBunner from 'smartbanner.js/dist/smartbanner'
    import smartBunnerCss from 'smartbanner.js/dist/smartbanner.css'
    import { mapGetters } from 'vuex'
    export default {
        computed: {
            ...mapGetters(['loading']),
        },
        updated: function () {
            this.$nextTick(function () {
                dataLayer.push({ 'event': 'optimize.activate' });
                // Code that will run only after the
                // entire view has been re-rendered
            })
        },
        mounted: function () {
            this.$nextTick(function () {
                dataLayer.push({ 'event': 'optimize.activate' });
                // Code that will run only after the
                // entire view has been rendered
            })
        }
    }
</script>
<style lang="less" src="./app.less"></style>

