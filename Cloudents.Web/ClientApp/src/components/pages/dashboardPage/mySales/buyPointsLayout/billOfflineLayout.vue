<template>
   <div class="elevation-1">
      <v-layout wrap class="billOfflineLayout">
         <v-flex class="billOfflineLayout_img_container" >
            <img class="billOfflineLayout_img" src="./image/cardBuyPoints.jpg" alt="">
         </v-flex>
         <v-flex class="billOfflineLayout_action" text-center>
            <p class="billOfflineLayout_title">{{$t('dashboardPage_my_sales_action_need_tutor')}}</p>
            <v-btn @click="openBillStudents()" class="billOfflineLayout_btn white--text" depressed color="#4c59ff">
               <span>{{$t('dashboardPage_my_sales_action_need_btn_tutor')}}</span>
            </v-btn>
         </v-flex>
      </v-layout>
   </div>
</template>

<script>
export default {
   name:"billOfflineLayout",
   methods: {
      openBillStudents(){
         this.$store.commit('setComponent', 'teacherBillOfflineDialog')
      }   
   },
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';
.billOfflineLayout{
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
   .billOfflineLayout_img_container{
      .flexSameSize();
      .billOfflineLayout_img{
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
   .billOfflineLayout_action{
      .flexSameSize();
      width: 100%;
      @media (max-width: @screen-md-plus) {
      margin-left: 4px;
         width: auto;

      }

      .billOfflineLayout_title{
         font-size: 16px;
         font-weight: 600;
         color: #43425d;
         margin: 0;
         padding: 10px 0 0;
         @media (max-width: @screen-md-plus) {
             font-size: 14px;
          }
         @media (max-width: @screen-xs) {
            padding: 0;
         }
      }
      .billOfflineLayout_btn{
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