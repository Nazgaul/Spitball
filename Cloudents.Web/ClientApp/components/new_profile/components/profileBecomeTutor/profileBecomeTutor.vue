<template>
   <div class="profileBecomeTutor">
      <div class="profileBecomeTutor_wrapper">
         <div class="profileBecomeTutor_title" v-language:inner="'profile_become_title'"/>
         <div class="profileBecomeTutor_desc" v-language:inner="'profile_become_txt'"/>
         <v-btn class="profileBecomeTutor_action white--text" depressed rounded color="#4452fc" v-openDialog="becomeTutorDialog">
            <div class="profileBecomeTutor_action_txt" v-language:inner="'profile_become_tutor_btn'"/>
         </v-btn>
      </div>
   </div>
</template>

<script>
import * as dialogNames from '../../../pages/global/dialogInjection/dialogNames.js'

export default {
   name:'profileBecomeTutor',
   data() {
      return {
         becomeTutorDialog: dialogNames.BecomeTutor,
      }
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.profileBecomeTutor{
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

   .profileBecomeTutor_wrapper{
      max-width: 490px;
      margin: 0 auto;
      .profileBecomeTutor_title{
         font-weight: bold;
         font-size: 21px;
         padding-bottom: 8px;
      }
      .profileBecomeTutor_desc{
         font-size: 16px;
         padding-bottom: 18px;
      }
      .profileBecomeTutor_action{
         text-transform: capitalize;
         max-width: 250px;
         width: 100%;
         height: 38px;
         margin: 0;
         .profileBecomeTutor_action_txt{
            font-size: 16px;
            font-weight: 600;
         }
      }
   }
  
  

}
</style>