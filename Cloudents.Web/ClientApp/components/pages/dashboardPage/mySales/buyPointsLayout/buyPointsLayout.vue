<template>
   <div class="elevation-1">
      <v-layout wrap class="buyPointsLayout">
         <v-flex class="buyPointsLayout_img_container" >
            <img class="buyPointsLayout_img" src="./image/cardBuyPoints.jpg" alt="">
         </v-flex>
         <v-flex class="buyPointsLayout_action" text-center>
            <p class="buyPointsLayout_title" v-language:inner="'dashboardPage_my_sales_action_need'"/>
            <v-btn @click="openSblToken" class="buyPointsLayout_btn white--text" depressed color="#4c59ff">
               <span v-language:inner="'dashboardPage_my_sales_action_need_btn'"/>
            </v-btn>
         </v-flex>
      </v-layout>
   </div>
</template>

<script>
import analyticsService from '../../../../../services/analytics.service';
import { mapActions } from 'vuex';
export default {
   name:"buyPointsLayout",
   methods: {
      ...mapActions(['updateShowBuyDialog']),
      openSblToken(){
         analyticsService.sb_unitedEvent("BUY_POINTS", "ENTER");
         this.updateShowBuyDialog(true);
      },   
   },
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';
.buyPointsLayout{
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
   .buyPointsLayout_img_container{
      .flexSameSize();
      .buyPointsLayout_img{
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
   .buyPointsLayout_action{
      .flexSameSize();
      width: 100%;
      @media (max-width: @screen-md-plus) {
      margin-left: 4px;
         width: auto;

      }

      .buyPointsLayout_title{
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
      .buyPointsLayout_btn{
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