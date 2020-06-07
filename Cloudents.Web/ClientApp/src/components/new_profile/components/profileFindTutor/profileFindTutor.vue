<template>
   <div class="profileFindTutor">
      <div class="profileFindTutor_wrapper">
         <div class="profileFindTutor_title" v-language:inner="'profile_why_learn'"/>

         <div class="whyUs-bottom">
            <div class="cont">
               <sheild class="cont-icon"/>
               <span class="cont-text" v-language:inner="'documentPage_money_back'"></span>
            </div>
            <div class="cont">
               <secure class="cont-icon"/>
               <span class="cont-text" v-language:inner="'documentPage_secure_payment'"></span>
            </div>
            <div class="cont">
               <exams class="cont-icon"/>
               <span class="cont-text" v-language:inner="'documentPage_prepared_exams'"></span>
            </div>
         </div>
         <v-btn class="profileFindTutor_action white--text mt-4" depressed rounded color="#4452fc" @click="goTutorList()">
            <div class="profileFindTutor_action_txt" v-language:inner="'profile_find_tutors'"/>
         </v-btn>
      </div>
   </div>
</template>

<script>
import sheild from './images/sheild.svg';
import exams from './images/exams.svg';
import secure from './images/secure.svg';
import * as routeNames from '../../../../routes/routeNames.js';

export default {
   name:'profileFindTutor',
   components:{sheild,exams,secure},
   methods: {
      goTutorList(){
         this.$router.push({name: routeNames.TutorList})
      },
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
      // max-width: 490px;
      margin: 0 auto;
      .profileFindTutor_title{
         font-weight: bold;
         font-size: 21px;
         padding-bottom: 14px;
      }
      .whyUs-bottom{
        color: #43425d;
        display: flex;
        justify-content: space-between;
        text-align: center;
        @media (max-width: @screen-xs) {
            flex-direction: column;
            max-width: 236px;
            margin: 0 auto;
        }

        .cont{
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            align-items: center;
            @media (max-width: @screen-xs) {
                padding-bottom: 14px;
                justify-content: normal;
                &:last-child{
                    padding-bottom: 0;
                }
            }
            .cont-icon{
                fill: red !important;
                margin-right: 8px;
                
                @media (max-width: @screen-md) {
                    margin-bottom: 4px;
                }
                @media (max-width: @screen-xs) {
                    margin-bottom: 0;
                }
            }
            .cont-text{
                font-size: 14px;
                font-weight: normal;
                font-stretch: normal;
                font-style: normal;
                line-height: 1.4;
                letter-spacing: normal;
                @media (max-width: @screen-md) {
                    margin-bottom: 4px;
                }
                @media (max-width: @screen-xs) {
                    margin-bottom: 0;
                }
            }
        }
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