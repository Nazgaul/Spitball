<template>
   <div class="profileFindTutor">
      <div class="profileFindTutor_wrapper">
         <div class="profileFindTutor_title" v-language:inner="'profile_become_title'"/>
         <div class="profileFindTutor_desc" v-language:inner="'profile_become_txt'"/>
         <v-btn class="profileFindTutor_action white--text" depressed rounded color="#4452fc" @click="globalFunctions.openBecomeTutor()">
            <div class="profileFindTutor_action_txt" v-language:inner="'profile_become_tutor_btn'"/>
         </v-btn>
      </div>
   </div>
</template>

<script>
export default {
   name:'profileFindTutor',
   props:{
      globalFunctions:{
         type: Object,
         required:true
      }
   },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.profileFindTutor{
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

   .profileFindTutor_wrapper{
      max-width: 490px;
      margin: 0 auto;
      .profileFindTutor_title{
         font-weight: bold;
         font-size: 21px;
         padding-bottom: 8px;
      }
      .profileFindTutor_desc{
         font-size: 16px;
         padding-bottom: 18px;
      }
      .profileFindTutor_action{
         text-transform: capitalize;
         max-width: 250px;
         width: 100%;
         height: 38px;
         margin: 0;
         .profileFindTutor_action_txt{
            font-size: 16px;
            font-weight: 600;
         }
      }
   }
  
  

}
</style>