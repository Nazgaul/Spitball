<template>
    <div id="marketing">
        <promoteStteper class="mb-2 mb-sm-4" />
        <marketingBlogs />
        <!-- <tableCoupon /> -->
    </div>
</template>

<script>
const promoteStteper = () => import('../marketing/promote/promoteStteper.vue')
const marketingBlogs = () => import('../marketing/marketingBlogs/marketingBlogs.vue')
// const tableCoupon = () => import('../dashboardPage/myCoupons/tableCoupon/tableCoupon.vue');

export default {
    components: {
        promoteStteper,
        marketingBlogs,
        // tableCoupon
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
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