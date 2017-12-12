<template>
    <v-app>
        <app-header ref="header" v-if="$route.meta.showHeader"></app-header>
        <router-view name="verticalList"></router-view>
        <router-view ref="personalize" name="personalize"></router-view>
        <v-content>
            <div class="loader" v-show="!$route.meta.isStatic&&loading">
                <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
            </div>
            <router-view v-show="!loading||$route.meta.isStatic"></router-view>
        </v-content>
    </v-app>
</template>
<script>
    import AppHeader from '../header/header.vue'
    import { mapGetters } from 'vuex'
    export default {
        computed: {
            ...mapGetters(['loading'])
        },
        components: { AppHeader }
    }
</script>
<style lang="less" src="./app.less"></style>

