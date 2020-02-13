<template>
    <v-container grid-list-md class="coupon">
        <createCoupon />
        <couponTable />
    </v-container>
</template>

<script>
import createCoupon from './createCoupon.vue';
import couponTable from './couponTable.vue';

export default {
    name: 'coupon',
    components: {
        createCoupon,
        couponTable
    }
}
</script>

<style lang="less">

</style>