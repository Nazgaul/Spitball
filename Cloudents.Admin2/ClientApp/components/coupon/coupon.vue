<template>
    <v-container grid-list-md class="coupon">
        <createCoupon />
    </v-container>
</template>

<script>
import createCoupon from './createCoupon.vue';

export default {
    name: 'coupon',
    components: {
        createCoupon
    }
}
</script>

<style lang="less">
    .coupon {
        max-width: 960px !important;
    }
</style>