<template>
    <v-app>
        <router-view name="header"></router-view>
        <router-view ref="personalize" name="personalize"></router-view>
        <v-content :class="{'loading':loading}">
            <div class="loader" v-show="loading">
                <v-progress-circular indeterminate v-bind:size="50" color="amber"></v-progress-circular>
            </div>
            <router-view ref="mainPage"></router-view>
        </v-content>
    </v-app>
</template>
<script>
    import { mapGetters } from 'vuex'
    export default {
        computed: {
            ...mapGetters(['loading']),
        }
    }
</script>
<style lang="less" src="./app.less"></style>

