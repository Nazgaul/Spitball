<template>
    <div class="buy-dialog-wrap">
        <div class="close-buy-dialog">
            <v-icon class="closeIcon" @click="''" color="#000" size="14" v-closeDialog>{{$vuetify.icons.values.close}}</v-icon>
        </div>
        <div class="buy-tokens-wrap">
            <v-container px-4 pt-6 pb-0 class="buy-tokens-top-container">
                <v-layout>
                    <v-flex text-center xs12>
                        <span class="buy-tokens-title-text" v-t="'buyTokens_get_points'"></span>
                    </v-flex>
                </v-layout>
            </v-container>
            <v-container class="buy-tokens-bottom-container px-6 pt-4 pt-sm-0">
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
                            <span v-t="'buyTokens_points'"></span>
                        </div>
                        <div>{{products.currency}}{{products.basic.price}}</div>
                        <div class="buy-tokens-text-small">
                            <span>{{products.currency}}</span>
                            <span>{{basicConversionRate}}</span>&nbsp;
                            <span v-t="'buyTokens_per_point'"></span>
                        </div>
                        <div>
                            <button class="buy-tokens-choose-button" v-t="'buyTokens_choose'"></button>
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
                        <div class="buy-tokens-text-absolute" v-t="'buyTokens_most_popular'"></div>
                        <div class="buy-tokens-center-price-title">
                            <span class="buy-tokens-points-num">{{products.inter.pts}}</span>&nbsp;
                            <span v-t="'buyTokens_points'"></span>
                        </div>
                        <div>{{products.currency}}{{products.inter.price}}</div>
                        <div class="buy-tokens-text-small">
                            <span>{{products.currency}}</span>
                            <span>{{interConversionRate}}</span>&nbsp;
                            <span v-t="'buyTokens_per_point'"></span>
                        </div>
                        <div>
                            <button class="buy-tokens-choose-button middle-box" v-t="'buyTokens_choose'"></button>
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
                            <span v-t="'buyTokens_points'"></span>
                        </div>
                        <div>{{products.currency}}{{products.pro.price}}</div>
                        <div class="buy-tokens-text-small">
                            <span>{{products.currency}}</span>
                            <span>{{proConversionRate}}</span>&nbsp;
                            <span v-t="'buyTokens_per_point'"></span>
                        </div>
                        <div>
                            <button class="buy-tokens-choose-button" v-t="'buyTokens_choose'"></button>
                        </div>
                    </v-flex>
                </v-layout>
                <v-layout justify-center class="buy-tokens-price-container2">
                    <v-flex class="buy-tokens-details-container">
                        <img class="img-warning" src="./img/warning.png" alt="" />
                        <div class="txt-buy-tokens">
                            <p class="red--text" v-t="'buyTokens_bottom_1'"></p>
                            <p v-t="'buyTokens_bottom_2'"></p>
                        </div>
                    </v-flex>
                </v-layout>
                <v-layout class="buymebtn">
                    <v-flex text-center>
                        <v-btn class="buyme-button white--text" :loading="isLoading" depressed color="#4452fc" @click="buyPoints">
                            <span class="me-2 d-flex"><v-icon size="16">sbf-lock-icon</v-icon></span>
                            <span class="font-weight-bold" v-t="{path: 'buyTokens_secure_payment', args: [products[selectedProduct].pts]}"></span>
                        </v-btn>
                    </v-flex>
                </v-layout>
            </v-container>
        </div>
    </div>
</template>

<script>
export default {
  name:'buyPointsUS',
  data() {
    return {
      isLoading: false,
      selectedProduct: 'inter',
      transactionId: 500,
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
    buyPoints() {
        this.isLoading = true
        this.$store.dispatch('buyPointsUS', {Points: this.transactionId})
    }
  },
  mounted() {
    this.$loadScript("https://js.stripe.com/v3/?advancedFraudSignals=false")
  }
};
</script>

<style lang="less" src="./buyPoints.less"></style>
