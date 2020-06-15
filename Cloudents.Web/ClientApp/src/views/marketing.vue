<template>
    <div id="marketing">
        <promoteStteper class="mb-2 mb-sm-4" />
        <marketingBlogs />
    </div>
</template>

<script>
const promoteStteper = () => import('../components/marketing/promote/promoteStteper.vue')
const marketingBlogs = () => import('../components/marketing/marketingBlogs/marketingBlogs.vue')

export default {
    components: {
        promoteStteper,
        marketingBlogs,
    }
}
</script>

<style lang="less">
@import '../styles/mixin.less';
#marketing {
    max-width: 944px;
    margin: 24px 34px;

    @media (max-width: @screen-xs) {
        margin: 8px 0;
    }
    .fullWidth {
        width: 100%;
    }
}
</style>