<template>
    <!-- <div id="marketing"> -->
        <!-- <router-view :name="$route.name"></router-view> -->
    <div id="marketing" class="fullWidth">

        <marketingActions></marketingActions>
        <marketingBlogs></marketingBlogs>
        <tableCoupon></tableCoupon>

    </div>
</template>

<style lang="less">
@import '../../../styles/mixin.less';
#marketing {
    max-width: 944px;
    margin: 24px 34px;

    @media (max-width: @screen-xs) {
        margin: 24px 0;
    }
    .fullWidth {
        width: 100%;
    }
}
</style>