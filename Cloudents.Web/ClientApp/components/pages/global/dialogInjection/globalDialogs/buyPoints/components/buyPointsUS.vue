<template>
    <div class="buy-dialog-wrap">
       im US!
        <div class="buy-tokens-overlay" :class="{'visible': showOverlay}"></div>
        <div class="close-buy-dialog">
            <v-icon v-closeDialog>sbf-close</v-icon>
        </div>
        <div class="buy-tokens-wrap">
            <v-container pa-4 pt-6 pb-6 class="buy-tokens-top-container">
                <v-layout>
                    <v-flex text-center xs12>
                        <span class="buy-tokens-title-text" v-language:inner="'buyTokens_get_points'"></span>
                    </v-flex>
                </v-layout>
                <v-layout pt-6 ml-2 mr-2 :class="{'column': $vuetify.breakpoint.xsOnly}">
                    <v-flex class="flex-buy-tokens" text-center column xs4 :class="$vuetify.breakpoint.xsOnly ? 'pl-12 pr-12' : 'pl-6 pr-6'">
                        <div class="buy-tokens-icon">
                            <v-icon>sbf-answer-icon</v-icon>
                        </div>
                        <div class="buy-tokens-bold-text mt-4" v-language:inner="'buyTokens_answer'"></div>
                        <div class="buy-tokens-normal-text mt-1" v-language:inner="'buyTokens_earn_answer'"></div>
                        <div class="line-buy-tokens"></div>
                    </v-flex>
                    <v-flex class="flex-buy-tokens" text-center column xs4 pl-4 pr-4
                            :class="$vuetify.breakpoint.xsOnly ? 'mt-12 pl-12 pr-12' : 'pl-6 pr-6'">
                        <div class="buy-tokens-icon">
                            <v-icon>sbf-upload-icon</v-icon>
                        </div>
                        <div class="buy-tokens-bold-text mt-4" v-language:inner="'buyTokens_upload'"></div>
                        <div class="buy-tokens-normal-text mt-1" v-language:inner="'buyTokens_earn_upload'"></div>
                        <div class="line-buy-tokens"></div>

                    </v-flex>
                    <v-flex class="flex-buy-tokens" text-center column xs4 pl-4 pr-4
                            :class="$vuetify.breakpoint.xsOnly ? 'mt-12 pl-12 pr-12' : 'pl-6 pr-6'">
                        <div class="buy-tokens-icon">
                            <v-icon>sbf-invite-icon</v-icon>
                        </div>
                        <div class="buy-tokens-bold-text mt-4" v-language:inner="'buyTokens_invite'"></div>
                        <div class="buy-tokens-normal-text mt-1" v-language:inner="'buyTokens_earn_invite'"></div>
                        <div class="line-buy-tokens"></div>

                    </v-flex>
                </v-layout>
            </v-container>
            <v-container class="buy-tokens-bottom-container px-6" :class="{'pt-4': $vuetify.breakpoint.xsOnly}">
                <v-layout>
                    <v-flex text-center>
                        <span class="buy-tokens-title-text" v-language:inner="'buyTokens_need_points'"></span>
                    </v-flex>
                </v-layout>
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
                        <v-btn class="buyme-button white--text" depressed color="#4452fc" id="buyme-button" @click="openPaymeDialog">
                        <span class="mr-2 d-flex"><v-icon size="16">sbf-lock-icon</v-icon></span>
                        <span class="font-weight-bold" v-html="$Ph('buyTokens_secure_payment', products[selectedProduct].pts)"></span>
                        </v-btn>
                    </v-flex>
                </v-layout>

            </v-container>
        </div>
    </div>
</template>
<script>
import {mapGetters, mapActions} from 'vuex';
import analyticsService from '../../../../../../../services/analytics.service';

export default {
  name:'buyPointsUS',
  data() {
    return {
      selectedProduct: 'inter',
      showOverlay: false,
      transactionId: 750,
      products:{
        currency: '₪',
        basic:{
            pts: 250,
            price: 10,
            currency: 'ILS'
        },
        inter:{
            pts: 750,
            price: 30,
            currency: 'ILS'
        },
        pro:{
            pts: 1500,
            price: 60,
            currency: 'ILS'
        }
      },
      user: this.accountUser()
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
    ...mapGetters(['accountUser']),
    ...mapActions(['updateToasterParams', 'buyToken']),

    selectProduct(val) {
      if (this.selectedProduct !== val) {
        this.selectedProduct = val;
        this.transactionId = this.products[val].pts;
      }
    },

    openPaymeDialog() {
      let transactionId = this.transactionId;
      analyticsService.sb_unitedEvent("BUY_POINTS", "PRODUCT_SELECTED", transactionId);
        this.buyToken({points : transactionId});
        this.$closeDialog()
    }
  }
};
</script>


<style lang="less" src="./buyPoints.less"></style>
