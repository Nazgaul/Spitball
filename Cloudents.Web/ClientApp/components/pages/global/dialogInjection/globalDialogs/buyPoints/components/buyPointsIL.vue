<template>
    <div class="buy-dialog-wrap">
        <div class="close-buy-dialog">
            <v-icon class="closeIcon" @click="''" color="#000" size="14" v-closeDialog>sbf-close</v-icon>
        </div>
        <div class="buy-tokens-wrap">
            <v-container px-4 pt-6 pb-0 class="buy-tokens-top-container">
                <v-layout>
                    <v-flex text-center xs12>
                        <span class="buy-tokens-title-text" v-t="'buyTokens_get_points'"></span>
                    </v-flex>
                </v-layout>
            </v-container>
            <v-container class="buy-tokens-bottom-container px-sm-6" :class="{'pt-0': $vuetify.breakpoint.xsOnly}">
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
                        <img class="img-warning" src="./img/warning.png" alt=""/>
                        <div class="txt-buy-tokens">
                            <p style="color:red;" v-t="'buyTokens_bottom_1'" />
                            <p v-t="'buyTokens_bottom_2'" />
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

<script src="./buyPoints.js"></script>

<style lang="less" src="./buyPoints.less"></style>
