<template>
    <div id="couponPage">
    </div>
</template>

<script>

export default {
    components: {
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
#couponPage {
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