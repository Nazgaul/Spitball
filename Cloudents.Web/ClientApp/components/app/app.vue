<template>
    <v-app>
        <app-header ref="header" v-if="$route.meta.showHeader"></app-header>
        <template v-if="hasVerticalList">
            <router-view name="verticalList"></router-view>
        </template>
        <div  class="loader" v-show="!$route.meta.isStatic&&loading">
            <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
        </div>
        <router-view v-show="!loading||$route.meta.isStatic"></router-view>
    </v-app>
</template>
<script>
    import AppHeader from '../header/header.vue'
    import {mapGetters} from 'vuex'
    export default {
        computed:{
          ...mapGetters(['loading']),
                    hasVerticalList() {
                        return this.$route.matched[0] &&
                            this.$route.matched[0].components &&
                            !!this.$route.matched[0].components.verticalList
                    }
        },
        components: { AppHeader }
    }
</script>
<style lang="less" src="./app.less"></style>

