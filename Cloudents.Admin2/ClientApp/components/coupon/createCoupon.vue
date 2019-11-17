<template>
  <v-form @submit.prevent="createCoupon" class="createCoupon">
    <v-layout row wrap>
      <v-flex xs12 class="createCoupon_row">
        <v-text-field v-model="code" label="Coupon code"></v-text-field>
      </v-flex>
      <v-flex xs6 class="createCoupon_row">
        <v-select
          v-model="couponType"
          :items="items"
          label="Discount type"
          @change="changeCouponDiscountType"
        ></v-select>
      </v-flex>
      <v-flex xs6 class="createCoupon_row">
        <v-text-field v-model="value" label="Value"></v-text-field>
      </v-flex>
      <!-- <v-flex xs6 class="createCoupon_row">
                <v-text-field
                    v-model="amount"
                    label="Amount of users"
                ></v-text-field>
            </v-flex>
            <v-flex xs6 class="createCoupon_row">
                <v-text-field
                    v-model="amountPerUser"
                    label="Amount coupon per user"
                ></v-text-field>
      </v-flex>-->
      <!-- <v-flex xs12 class="createCoupon_row">
                <v-menu
                    ref="menu"
                    v-model="menu"
                    :close-on-content-click="false"
                    :nudge-right="40"
                    :return-value.sync="expiration"
                    lazy
                    transition="scale-transition"
                    offset-y
                    full-width
                    min-width="290px">
                    <template slot="activator">
                    <v-text-field readonly v-model="expiration" label="Expired date"></v-text-field>
                    </template>
                    <v-date-picker v-model="expiration" no-title scrollable>
                    <v-spacer></v-spacer>
                    <v-btn flat color="primary" @click="menu = false">Cancel</v-btn>
                    <v-btn flat color="primary" @click="$refs.menu.save(expiration)">OK</v-btn>
                    </v-date-picker>
                </v-menu>
      </v-flex>-->
      <v-flex xs12>
        <v-text-field v-model="tutorId" label="Tutor Id"></v-text-field>
      </v-flex>
      <v-btn @click="createCoupon" flat class="couponBtn primary">Submit</v-btn>
    </v-layout>
  </v-form>
</template>

<script>
import couponService from "./couponService";

export default {
  name: "createCoupon",
  data() {
    return {
      items: ["flat", "percentage"],
      menu: false,
      date: null,
      couponType: "flat",
      value: null,
      code: null,
      expiration: null,
      tutorId: null
      // amount: null,
      // amountPerUser: null,
    };
  },
  methods: {
    createCoupon() {
      let couponObj = {
        couponType: this.couponType,
        code: this.code,
        amount: null,
        usePerUser: 1,
        value: this.value,
        expiration: this.expiration,
        tutorId:this.tutorId
      };
      couponService
        .createNewCoupon(couponObj)
        .then(res => {
          this.$toaster.success(`Created new coupon`);
        })
        .catch(res => {
          let ex = res.response.data;
          ex = ex || `Error occurred, try again later`;
          this.$toaster.error(ex);
        });
    },
    changeCouponDiscountType(item) {
      this.couponType = item;
    }
  }
};
</script>

<style lang="less">
.createCoupon {
  .couponBtn {
    margin: 0 0 0 auto;
  }
}
</style>