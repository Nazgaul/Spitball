<template>
    <v-navigation-drawer light app clipped permanent class="aside" width="216">
        <search-verticals></search-verticals>
    </v-navigation-drawer>
</template>
<!--<style src="./navbar.less" lang="less"></style>-->
<script>
    import SearchVerticals from './../general/link-verticals.vue'
    export default {
        components: {
            'search-verticals': SearchVerticals
        }
    }
</script>

<style scoped>
    /*need to override theme*/ 
    .navigation-drawer.aside {
        background: #3293f6;
    }
</style>
