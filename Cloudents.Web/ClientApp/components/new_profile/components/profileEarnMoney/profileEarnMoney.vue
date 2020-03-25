<template>
   <div class="profileEarnMoney">
      <div class="profileEarnMoney_wrapper">
         <div class="profileEarnMoney_title" v-t="'profile_earn_title'"/>
         <div class="profileEarnMoney_desc" v-t="'profile_earn_desc'"/>
         <v-btn class="profileEarnMoney_action white--text" depressed rounded color="#4452fc" v-openDialog="uploadDialog">
            <div class="profileEarnMoney_action_txt" v-t="'profile_earn_btn'"/>
         </v-btn>
      </div>
   </div>
</template>

<script>
import * as dialogNames from '../../../pages/global/dialogInjection/dialogNames.js'
export default {
   name:'profileEarnMoney',
   data() {
      return {
         uploadDialog: dialogNames.Upload
      }
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.profileEarnMoney{
   width: 100%;
   border-radius: 8px;
   box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
   background-color: #ffffff;
   color: #43425d;
   text-align: center;
   padding: 14px;
   padding-bottom: 18px;
   @media (max-width: @screen-xs) {
      border-radius: 0px;
      box-shadow: none;
   }

   .profileEarnMoney_wrapper{
      max-width: 490px;
      margin: 0 auto;
      .profileEarnMoney_title{
         font-weight: bold;
         font-size: 21px;
         padding-bottom: 8px;
      }
      .profileEarnMoney_desc{
         font-size: 16px;
         padding-bottom: 18px;
      }
      .profileEarnMoney_action{
         text-transform: capitalize;
         max-width: 250px;
         width: 100%;
         height: 38px;
         margin: 0;
         .profileEarnMoney_action_txt{
            font-size: 16px;
            font-weight: 600;
         }
      }
   }
  
  

}
</style>