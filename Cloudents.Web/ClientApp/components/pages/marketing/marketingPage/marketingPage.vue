
<template>
    <div>
        <marketingActions></marketingActions>
        <marketingBlogs></marketingBlogs>
        <tableCoupon></tableCoupon>
    </div>
</template>

<script>
const marketingActions = () => import('../marketingActions/marketingActions.vue');
const marketingBlogs = () => import('../marketingBlogs/marketingBlogs.vue');
const tableCoupon = () => import('../tableCoupon/tableCoupon.vue');

export default {
    name: 'marketing',
    components: {
        marketingActions,
        marketingBlogs,
        tableCoupon,
    },
}
</script>