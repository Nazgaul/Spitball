<template>
    <div id="marketing" class="fullWidth">

        <marketingActions></marketingActions>
        <marketingBlogs></marketingBlogs>
        <tableCoupon></tableCoupon>

    </div>
</template>

<script>
const marketingActions = () => import('./marketingActions/marketingActions.vue');
const marketingBlogs = () => import('./marketingBlogs/marketingBlogs.vue');
const tableCoupon = () => import('./tableCoupon/tableCoupon.vue');

export default {
    components: {
        marketingActions,
        marketingBlogs,
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
        margin: 8px 0;
    }
    .fullWidth {
        width: 100%;
    }
}
</style>