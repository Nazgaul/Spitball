<template>
    <aside>
        <label class="upper">search type</label>
        <search-verticals></search-verticals>
    </aside>
</template>
<style src="./navbar.less" lang="less"></style>
<script>
    import SearchVerticals from './../../general/link-verticals.vue';
    export default {
        props: ["isOpen","section"],
        components: {
            'search-verticals': SearchVerticals
        },
        methods: {
            changeSection: function (val) {
                console.log(val);
                this.$emit('update:section',val)
            } 
        }
    }
</script>
