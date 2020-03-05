<template>
    <div class="buy-dialog-wrap">
        <div class="close-buy-dialog">
            <v-icon class="closeIcon" color="#000" size="14" v-closeDialog>sbf-close</v-icon>
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
                            <span class="buy-tokens-points-num">{{products.pro.pts | commasFilter}}</span>&nbsp;
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
                        <v-skeleton-loader class="mb-4" v-show="isLoading" width="100%" height="44" type="button"></v-skeleton-loader>
                        <div v-show="!isLoading" id="paypal-button-container"></div>
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
      isLoading:false,
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
    let paypalUrl = `https://www.paypal.com/sdk/js?client-id=${window.paypalClientId}&commit=false`;
    this.$loadScript(paypalUrl)
        .then(() => {
            window.paypal
            .Buttons({
                createOrder: function(data, actions) {
                    self.isLoading = true;
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
                    self.$store.dispatch('updatePaypalBuyTokens',data.orderID).then(()=>{
                        self.isLoading = false
                    })
                }
            })
            .render('#paypal-button-container');
        });
  },
};
</script>


<style lang="less" src="./buyPoints.less"></style>
