<template>
  <div class="buy-dialog-wrap">
    <div class="close-buy-dialog"><v-icon @click="closeModal">sbf-close</v-icon></div>
    <v-container pa-3 pt-4 pb-4 class="buy-tokens-top-container">
      <v-layout>
        <v-flex text-xs-center xs12>
          <span class="buy-tokens-title-text">Get Points</span> 
        </v-flex>
      </v-layout>
      <v-layout pt-4 ml-2 mr-2>
        <v-flex text-xs-center column xs4 pl-4 pr-4>
          <div class="buy-tokens-icon"><v-icon>sbf-answer-icon</v-icon></div>
          <div class="buy-tokens-bold-text mt-3">Answer</div>
          <div class="buy-tokens-normal-text mt-1">Earn points by answering student quesitons</div>
        </v-flex>
        <v-flex text-xs-center column xs4 pl-4 pr-4>
          <div class="buy-tokens-icon"><v-icon>sbf-upload-icon</v-icon></div>
          <div class="buy-tokens-bold-text mt-3">Upload</div>
          <div class="buy-tokens-normal-text mt-1">Upload Study Documents. Earn from every download</div>
        </v-flex>
        <v-flex text-xs-center column xs4 pl-4 pr-4>
          <div class="buy-tokens-icon"><v-icon>sbf-invite-icon</v-icon></div>
          <div class="buy-tokens-bold-text mt-3">Invite</div>
          <div class="buy-tokens-normal-text mt-1">Earn points by answering student quesitons</div>
        </v-flex>
      </v-layout>
      </v-container>
      <v-container class="buy-tokens-bottom-container">
        <v-layout>
          <v-flex text-xs-center>
            <span class="buy-tokens-title-text">Need points now?</span>
          </v-flex>
        </v-layout>
        <v-layout pt-4 justify-center class="buy-tokens-price-container">
          <v-flex class="buy-tokens-details-container" text-xs-center column xs4 :class="{'item-selected': selectedProduct === 'basic'}" @click="selectProduct('basic')">
            <div><span class="buy-tokens-points-num">100</span> pts</div>
            <div>$1.5</div>
            <div class="buy-tokens-text-small">$0.015 /point</div>
            <div><button class="buy-tokens-choose-button">Choose</button></div>
          </v-flex>
          <v-flex class="buy-tokens-details-container middle-box" text-xs-center column xs4 :class="{'item-selected': selectedProduct === 'inter'}" @click="selectProduct('inter')">
            <div class="buy-tokens-text-absolute">MOST POPULAR</div>
            <div><span class="buy-tokens-points-num">500</span> pts</div>
            <div>$5</div>
            <div class="buy-tokens-text-small">$0.01 /point</div>
            <div><button class="buy-tokens-choose-button middle-box">Choose</button></div>
          </v-flex>
          <v-flex class="buy-tokens-details-container" text-xs-center column xs4 :class="{'item-selected': selectedProduct === 'pro'}" @click="selectProduct('pro')">
            <div><span class="buy-tokens-points-num">1000</span> pts</div>
            <div>$9.5</div>
            <div class="buy-tokens-text-small">$0.0095 /point</div>
            <div><button class="buy-tokens-choose-button">Choose</button></div>
          </v-flex>
        </v-layout>
        <v-layout pt-4>
          <v-flex text-xs-center mt-3>
            <div class="paypal-button" id="paypal-button">
              <span><v-icon style="font-size: 20px;">sbf-lock-icon</v-icon></span>
              <span>Secure Payment with</span>
              <img src="./img/paypal-icon.png" style="vertical-align: middle;"/>
            </div>
          </v-flex>
        </v-layout>
      </v-container>
  </div>
</template>

<script src="./buyTokens.js"></script>

<style lang="less" src="./buyTokens.less"></style>
