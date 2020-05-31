<template>
    <div id="couponPage">
        <tableCoupon />
    </div>
</template>

<script>
import tableCoupon from './tableCoupon/tableCoupon.vue'

export default {
    components: {
        tableCoupon
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
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