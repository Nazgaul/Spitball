<template>
    <v-navigation-drawer light app clipped persistent>
        <search-verticals class="blue"></search-verticals>
    </v-navigation-drawer>
    <!--<aside>
        <search-verticals class="blue"></search-verticals>
    </aside>-->
</template>
<style src="./navbar.less" lang="less"></style>
<script>
    import SearchVerticals from './../general/link-verticals.vue'
    export default {
        components: {
            'search-verticals': SearchVerticals
        }
    }
</script>
