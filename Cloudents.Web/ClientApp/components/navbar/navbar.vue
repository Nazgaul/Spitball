<template>
    <aside>
        <search-verticals></search-verticals>
    </aside>
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
