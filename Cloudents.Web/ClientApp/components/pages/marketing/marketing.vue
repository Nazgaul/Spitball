<template>
    <div id="marketing">

        <marketingActions></marketingActions>
        <spitballTips></spitballTips>
        <tableCoupon></tableCoupon>

    </div>
</template>

<script>
const marketingActions = () => import('./marketingActions/marketingActions.vue');
const spitballTips = () => import('./marketingBlogs/marketingBlogs.vue');
const tableCoupon = () => import('./tableCoupon/tableCoupon.vue');

export default {
    components: {
        marketingActions,
        spitballTips,
        tableCoupon,
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
#marketing {
    max-width: 944px;
    margin: 24px 34px;

    @media (max-width: @screen-xs) {
        margin: 24px 0;
    }
}
</style>