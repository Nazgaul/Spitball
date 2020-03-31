<template>
    <div class="buy-dialog-wrap">
        <div class="close-buy-dialog">
            <v-icon class="closeIcon" @click="''" color="#000" size="14" v-closeDialog>sbf-close</v-icon>
        </div>
        <div class="buy-tokens-wrap">
            <v-container px-4 pt-6 pb-0 class="buy-tokens-top-container">
                <v-layout>
                    <v-flex text-center xs12>
                        <span class="buy-tokens-title-text" v-language:inner="'buyTokens_get_points'"></span>
                    </v-flex>
                </v-layout>
            </v-container>
            <v-container class="buy-tokens-bottom-container px-6" :class="{'pt-4': $vuetify.breakpoint.xsOnly}">
                <v-layout pt-6 justify-center class="buy-tokens-price-container">
                    <v-flex
                            class="buy-tokens-details-container"
                            text-center
                            column
                            xs4
                            :class="{'item-selected': selectedProduct === 'basic'}"
                            @click="selectProduct('basic')"
                    >
                        <div class="buy-tokens-center-price-title">
                            <span class="buy-tokens-points-num">{{products.basic.pts}}</span>&nbsp;
                            <span v-language:inner="'buyTokens_points'"></span>
                        </div>
                        <div>{{products.currency}}{{products.basic.price}}</div>
                        <div class="buy-tokens-text-small">
                            <span>{{products.currency}}</span>
                            <span>{{basicConversionRate}}</span>&nbsp;
                            <span v-language:inner="'buyTokens_per_point'"></span>
                        </div>
                        <div>
                            <button class="buy-tokens-choose-button" v-language:inner="'buyTokens_choose'"></button>
                        </div>
                    </v-flex>
                    <v-flex
                            class="buy-tokens-details-container middle-box"
                            text-center
                            column
                            xs4
                            :class="{'item-selected': selectedProduct === 'inter'}"
                            @click="selectProduct('inter')"
                    >
                        <div class="buy-tokens-text-absolute" v-language:inner="'buyTokens_most_popular'"></div>
                        <div class="buy-tokens-center-price-title">
                            <span class="buy-tokens-points-num">{{products.inter.pts}}</span>&nbsp;
                            <span v-language:inner="'buyTokens_points'"></span>
                        </div>
                        <div>{{products.currency}}{{products.inter.price}}</div>
                        <div class="buy-tokens-text-small">
                            <span>{{products.currency}}</span>
                            <span>{{interConversionRate}}</span>&nbsp;
                            <span v-language:inner="'buyTokens_per_point'"></span>
                        </div>
                        <div>
                            <button class="buy-tokens-choose-button middle-box" v-language:inner="'buyTokens_choose'"></button>
                        </div>
                    </v-flex>
                    <v-flex
                            class="buy-tokens-details-container"
                            text-center
                            column
                            xs4
                            :class="{'item-selected': selectedProduct === 'pro'}"
                            @click="selectProduct('pro')"
                    >
                        <div class="buy-tokens-center-price-title">
                            <span class="buy-tokens-points-num">{{$n(products.pro.pts)}}</span>&nbsp;
                            <span v-language:inner="'buyTokens_points'"></span>
                        </div>
                        <div>{{products.currency}}{{products.pro.price}}</div>
                        <div class="buy-tokens-text-small">
                            <span>{{products.currency}}</span>
                            <span>{{proConversionRate}}</span>&nbsp;
                            <span v-language:inner="'buyTokens_per_point'"></span>
                        </div>
                        <div>
                            <button class="buy-tokens-choose-button" v-language:inner="'buyTokens_choose'"></button>
                        </div>
                    </v-flex>
                </v-layout>
                 <v-layout justify-center class="buy-tokens-price-container2">
                    <v-flex class="buy-tokens-details-container">
                        <img class="img-warning" src="./img/warning.png" alt=""/>
                        <div class="txt-buy-tokens">
                            <p style="color:red;" v-language:inner="'buyTokens_bottom_1'" />
                            <p v-language:inner="'buyTokens_bottom_2'" />
                        </div>
                            </v-flex>
                 </v-layout>

                <v-layout class="buymebtn">
                    <v-flex text-center>
                        <v-progress-circular v-show="isLoading" class="mb-4" size="80" width="2" indeterminate color="info"></v-progress-circular>
                        <div v-show="!isLoading" id="paypal-button-container" style="width:400px; margin: 0 auto;"></div>
                    </v-flex>
                </v-layout>

            </v-container>
        </div>
    </div>
</template>
<script>
import analyticsService from '../../../../../../../services/analytics.service';

export default {
  name:'buyPointsUS',
  data() {
    return {
      isLoading: false,
      selectedProduct: 'inter',
      transactionId: 750,
      products:{
        currency: '$',
        basic:{
            id:1,
            pts:100,
            price:1.5,
            currency: 'USD'
        },
        inter:{
            id:2,
            pts:500,
            price:6,
            currency: 'USD'
        },
        pro:{
            id:3,
            pts:1000,
            price:10,
            currency: 'USD'
        }
      },
    };
  },
  computed:{
    basicConversionRate(){
        return this.products.basic.price / this.products.basic.pts;
    },
    interConversionRate(){
        return this.products.inter.price / this.products.inter.pts;
    },
    proConversionRate(){
        return (this.products.pro.price / this.products.pro.pts).toFixed(2);
    }
  },
  methods: {
    selectProduct(val) {
      if (this.selectedProduct !== val) {
        this.selectedProduct = val;
        this.transactionId = this.products[val].pts;
      }
    },
  },
  mounted() {
    let self = this;
    let paypalUrl = `https://www.paypal.com/sdk/js?client-id=${window.paypalClientId}&intent=authorize`;
    this.$loadScript(paypalUrl)
        .then(() => {
            window.paypal
            .Buttons({
                 style: {
                    //layout:  'horizontal',
                   // color:   'blue',
                    shape:   'pill',
                   // tagline : false
                },
                createOrder: function(data, actions) {
                    analyticsService.sb_unitedEvent("BUY_POINTS", "PRODUCT_SELECTED", self.transactionId);
                    return actions.order.create({
                        purchase_units: [
                            {
                                reference_id: "points_" + self.products[self.selectedProduct].id,
                                amount: {
                                    value: self.products[self.selectedProduct].price,
                                    currency: self.products[self.selectedProduct].currency
                                }
                            },
                        ]
                    });
                },
                onApprove: function(data) {
                    self.isLoading = true;
                    //actions.order.authorize().then((authorization) => {
                        // var authorizationID = authorization.purchase_units[0]
                        //         .payments.authorizations[0].id;
                        //action.authorization.capture(authorizationID)
                        self.$closeDialog();
                        self.$store.dispatch('updatePaypalBuyTokens',data.orderID);
                        self.$store.dispatch('updateToasterParams', {
                        toasterText: self.$t("buyTokens_success_transaction"),
                        showToaster: true,
                        toasterTimeout: 5000
                    //});
                });
                    
                    //TODO happy go lucky - update the balance of the user
                }
            })
            .render('#paypal-button-container');
        });
  },
};
</script>


<style lang="less" src="./buyPoints.less"></style>
