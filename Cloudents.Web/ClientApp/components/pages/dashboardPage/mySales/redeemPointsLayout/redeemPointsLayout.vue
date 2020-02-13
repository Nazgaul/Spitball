<template>
   <div class="elevation-1">
      <v-layout wrap class="redeemPointsLayout">
         <v-flex class="redeemPointsLayout_img_container" >
            <img class="redeemPointsLayout_img" :src="redeemImg" alt="">
         </v-flex>
         <v-flex class="redeemPointsLayout_action" text-center>
            <p class="redeemPointsLayout_title" v-text="$Ph('dashboardPage_my_sales_action_redeem',userPts)"/>
            <v-btn @click="redeem(cost)" :disabled="!isAvailable" :loading="isLoading" class="redeemPointsLayout_btn white--text" depressed color="#4c59ff">
               <span v-text="$Ph('dashboardPage_my_sales_action_redeem_btn',cost.toLocaleString())"/>
            </v-btn>
         </v-flex>
      </v-layout>
   </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import walletService from '../../../../../services/walletService';
import { LanguageService } from '../../../../../services/language/languageService';
import paymentService from '../../../../../services/payment/paymentService.js'

export default {
   name:"redeemPointsLayout",
   data() {
      return {
         isLoading:false,
         cost:1000,
      }
   },
   computed: {
      ...mapGetters(['accountUser']),
      redeemImg(){
         return paymentService.getRedeemImg();
      },
      userPts(){
         return Math.round(this.accountUser.balance).toLocaleString()
      },
      isAvailable(){
         return this.accountUser.balance >= this.cost
      }
   },
   methods: {
      ...mapActions(['updateToasterParams','updateBalancesItems']),
      redeem(amount){
         this.isLoading = true;
         walletService.redeem(amount)
            .then(() => {
               this.updateToasterParams({
                     toasterText: LanguageService.getValueByKey('cashoutcard_Cashed'),
                     showToaster: true,
               });
               this.updateBalancesItems();
               this.isLoading = false;
            },
               error => {
                  console.error('error getting transactions:', error)
               });

      }
   },
   created(){
      this.updateBalancesItems()
   },
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';
.redeemPointsLayout{
   max-width: 234px;
   background-color: #ffffff;
   padding: 10px; 
   justify-content: center;
    @media (max-width: @screen-md-plus) {
      max-width: unset;
      max-height: 88px;
      padding: 6px;
      margin: 0 auto;
    }
   @media (max-width: @screen-xs) {
      max-height: 76px;
   }
   .redeemPointsLayout_img_container{
      .flexSameSize();
      .redeemPointsLayout_img{
         width: 100%;
         object-fit: cover;
         height: 90px;
         margin-right: 4px;
         @media (max-width: @screen-md-plus) {
            max-width: 180px;
            height: unset;
         }
         @media (max-width: @screen-xs) {
            max-width: 152px;
            height: 64px;
         }
      }
   }
   .redeemPointsLayout_action{
      .flexSameSize();
      width: 100%;
      @media (max-width: @screen-md-plus) {
      margin-left: 4px;
         width: auto;
      }

      .redeemPointsLayout_title{
         font-size: 16px;
         font-weight: 600;
         color: #43425d;
         margin: 0;
         padding: 0;
         padding-top: 10px;
          @media (max-width: @screen-md-plus) {
             font-size: 14px;
          }
         @media (max-width: @screen-xs) {
            padding: 0;
         }
      }
      .redeemPointsLayout_btn{
         border-radius: 8px;
         margin-top: 10px;
         font-size: 14px;
         font-weight: 600;
         text-transform: none;
         width: 100%;
         @media (max-width: @screen-md-plus) {
            margin-top: 8px;
            min-width: 150px;
            max-width: 150px;
         }
         @media (max-width: @screen-xs) {
            font-size: 12px;
            min-width: 134px;
            max-width: 134px;
         }
      }
   }
}
</style>