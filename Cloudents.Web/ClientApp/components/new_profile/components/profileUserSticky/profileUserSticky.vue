<template>
   <div :class="['profileUserSticky',{'profileUserSticky_bannerActive':getBannerStatus}]" v-if="!!getProfile">
      <template v-if="isTutor">
         <transition name="fade">
            <div v-if="showScrollHeader" class="profileUserSticky_scrollHeader">
               <div class="profileUserSticky_scrollHeader_img">
                  <userAvatar  
                           size="40" 
                           :userImageUrl="getProfile.user.image" 
                           :user-name="getProfile.user.name"/>
               </div>
               <div class="profileUserSticky_scrollHeader_user text-truncate">
                  <h6 class="profileUserSticky_scrollHeader_name text-truncate">
                     <span v-language:inner="'profile_tutor'"/>
                     {{getProfile.user.name}}
                  </h6>
                  <div class="profileUserSticky_scrollHeader_rating mt-1">
                     <userRating class="scrollHeader_rating" :showRateNumber="false" :rating="getProfile.user.tutorData.rate" :size="'18'" />
                     <span class="scrollHeader_rating_span ml-1" v-text="$Ph(getProfile.user.tutorData.reviewCount === 1 ? 'resultTutor_review_one' : `resultTutor_reviews_many`, reviewsPlaceHolder(getProfile.user.tutorData.reviewCount))"/>
                  </div>
               </div>
            </div>
         </transition>
         <div class="profileUserSticky_pricing">
            <v-flex class="profileUserSticky_pricing_discount" v-if="isDiscount">
               {{tutorPrice ? tutorPrice : tutorDiscountPrice | currencyFormat(getProfile.user.tutorData.currency)}}
            </v-flex>
            <v-flex class="profileUserSticky_pricing_price">
               <span class="profileUserSticky_pricing_price_number">{{isDiscount && tutorPrice !== 0  ? tutorDiscountPrice : tutorPrice | currencyFormat(getProfile.user.tutorData.currency)}}</span>/<span class="profileUserSticky_pricing_price_hour" v-language:inner="'profile_points_hour'"/>
            </v-flex>
         </div>
         <div class="profileUserSticky_btns">
            <v-btn sel="send" :disabled="isMyProfile" class="profileUserSticky_btn white--text" :class="{'isMyProfile':isMyProfile}" depressed rounded color="#4452fc" @click="globalFunctions.sendMessage">
               <v-flex xs2 mr-1>
                  <chatIcon class="profileUserSticky_btn_icon"/>
               </v-flex>
               <v-flex xs8>
                  <div class="profileUserSticky_btn_txt" v-language:inner="'profile_send_message'"/>
               </v-flex>
               <v-flex xs1>
                  
               </v-flex>
            </v-btn>
            <v-btn sel="calendar" :disabled="isMyProfile" v-if="getProfile.user.calendarShared" @click="globalFunctions.openCalendar" :class="{'isMyProfile':isMyProfile}" class="profileUserSticky_btn profileUserSticky_btn_book white--text" depressed rounded color="white">
               <v-flex xs2 mr-1>
                  <calendarIcon class="profileUserSticky_btn_icon"/>
               </v-flex>
               <v-flex xs8>
                  <div class="profileUserSticky_btn_txt" v-language:inner="'profile_book_session'"/>
               </v-flex>
               <v-flex xs1>
                  
               </v-flex>
            </v-btn>
         </div>
         <!-- <div class="profileUserSticky_respone">
            <span v-language:inner="'profile_response'"/>
            <span class="font-weight-bold" v-text="$Ph(1 > 0 ? 'profile_response_hours' : 'profile_response_hour',2)"/>
         </div> -->
         <div class="profileUserSticky_whyUs">
            <div class="profileUserSticky_whyUs_row mb-2">
               <shield class="profileUserSticky_whyUs_row_icon"></shield>
               <span class="profileUserSticky_whyUs_row_text" v-language:inner="'documentPage_money_back'"></span>
            </div>
            <div class="profileUserSticky_whyUs_row mb-2">
               <secure class="profileUserSticky_whyUs_row_icon"></secure>
               <span class="profileUserSticky_whyUs_row_text" v-language:inner="'documentPage_secure_payment'"></span>
            </div>
            <div class="profileUserSticky_whyUs_row mb-2">
               <exams class="profileUserSticky_whyUs_row_icon"></exams>
               <span class="profileUserSticky_whyUs_row_text" v-language:inner="'documentPage_prepared_exams'"></span>
            </div>
         </div>
         <button sel="coupon" :class="{'isMyProfileCoupon':isMyProfile}" class="profileUserSticky_coupon" @click="globalFunctions.openCoupon" v-language:inner="'coupon_apply_coupon'"/>
      </template>
      <template v-else>         
         <div v-if="isMyProfile" class="profileUserSticky_title_become" v-language:inner="'profile_become_title'"/>
         <div v-if="!isMyProfile" class="profileUserSticky_title" v-language:inner="'profile_why_learn'"/>
         <div class="profileUserSticky_whyUs why_learn_user" v-if="!isMyProfile">
            <div class="profileUserSticky_whyUs_row why_learn_user_row">
               <shield class="profileUserSticky_whyUs_row_icon"></shield>
               <span class="profileUserSticky_whyUs_row_text" v-language:inner="'documentPage_money_back'"></span>
            </div>
            <div class="profileUserSticky_whyUs_row why_learn_user_row">
               <secure class="profileUserSticky_whyUs_row_icon"></secure>
               <span class="profileUserSticky_whyUs_row_text" v-language:inner="'documentPage_secure_payment'"></span>
            </div>
            <div class="profileUserSticky_whyUs_row why_learn_user_row">
               <exams class="profileUserSticky_whyUs_row_icon"></exams>
               <span class="profileUserSticky_whyUs_row_text" v-language:inner="'documentPage_prepared_exams'"></span>
            </div>
         </div>
         <div class="profileUserSticky_title_become_text" v-else v-language:inner="'profile_become_txt'"/>
         <div class="profileUserSticky_btns why_learn_user_btn mt-3">
            <v-btn class="profileUserSticky_btn profileUserSticky_btn_find white--text" depressed rounded color="#4452fc" @click="isMyProfile? globalFunctions.openBecomeTutor() : globalFunctions.goTutorList()">
               <div class="profileUserSticky_btn_txt" v-language:inner="isMyProfile? 'profile_become_tutor_btn':'profile_find_tutors'"/>
            </v-btn>
         </div>
      </template>
   </div>
</template>


<script>
import shield from './images/sheild.svg';
import exams from './images/exams.svg';
import secure from './images/secure.svg';
import chatIcon from './images/chatIcon.svg';
import calendarIcon from './images/calendarIcon.svg';

import { mapGetters, mapActions } from 'vuex';

import userRating from '../../profileHelpers/profileBio/bioParts/userRating.vue';
import userAvatar from '../../../helpers/UserAvatar/userAvatar.vue';

export default {
   components:{
      shield,
      exams,
      secure,
      chatIcon,
      calendarIcon,
      userRating,
      userAvatar
   },
   props:{
      globalFunctions:{
         type: Object,
         required:true
      }
   },
   data() {
      return {
         showScrollHeader: false,
         coupon: '',
      }
   },
   computed: {
      ...mapGetters(['getBannerStatus','getProfile','accountUser','getCouponDialog','getCouponError']),
      isTutor(){
         return !!this.getProfile && this.getProfile.user.isTutor
      },
      isMyProfile(){
         return !!this.getProfile && !!this.accountUser && this.accountUser.id == this.getProfile.user.id
      },
      isDiscount() {
         return !!this.getProfile && (this.getProfile.user.tutorData.discountPrice || this.getProfile.user.tutorData.discountPrice === 0)
      },
      tutorPrice() {
         if (
         this.getProfile &&
         this.getProfile.user &&
         this.getProfile.user.tutorData
         ) {
         return this.getProfile.user.tutorData.price;
         }
         return 0;
      },
      tutorDiscountPrice() {
         return !!this.getProfile && this.getProfile.user.tutorData.discountPrice ? this.getProfile.user.tutorData.discountPrice : null;
      },
   },
   methods: {
      ...mapActions([
         'updateLoginDialogState',
         'updateTutorDialog']),
      reviewsPlaceHolder(reviews) {
         return reviews === 0 ? reviews.toString() : reviews;
      },


      openCalendar(){
         if(global.isAuth) {
            if(this.isMyProfile) {
               return
            }
         }else{
            this.updateLoginDialogState(true);
         }
      },
      openBecomeTutor(){
         this.updateTutorDialog(true)
      },
      goTutorList(){
         this.$router.push({name:'tutorLandingPage'})
      },
      handleScroll(){
         let scrollHeight = window.pageYOffset;
         if(scrollHeight > 300){
            this.showScrollHeader = true;
         }else{
            this.showScrollHeader = false;
         }

      }
   },
   beforeDestroy(){
      window.removeEventListener('scroll', this.handleScroll);
   },
   created() {
      window.addEventListener('scroll', this.handleScroll);
   },

}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.profileUserSticky{
   padding: 12px;
   position: sticky;
   top: 80px;
   min-width: 292px;
   width: 292px;
   height: max-content;
   border-radius: 8px;
   box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
   background-color: #ffffff;
   text-align: center;
   font-size: 14px;
   color: #43425d;
   .fade-enter-active, .fade-leave-active {
  transition: opacity .5s;
}
.fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
  opacity: 0;
}
   &.profileUserSticky_bannerActive{
      top: 150px;
   }
   .profileUserSticky_scrollHeader{
      display: flex;
      text-align: initial;
      align-items: center;
      margin-bottom: 28px;
      padding-left: 2px;
      .profileUserSticky_scrollHeader_img{
         margin-right: 12px;
      }
      .profileUserSticky_scrollHeader_name{
         font-size: 16px;
         font-weight: bold;
         line-height: 1.79;
      }
      .profileUserSticky_scrollHeader_rating{
         display: inline-flex;
         align-items: center;
         .scrollHeader_rating{
            flex: 0 0 auto;
         }
         .scrollHeader_rating_span{
            font-size: 12px;
         }
      }
   }
   .profileUserSticky_title_become{
      font-size: 20px;
      font-weight: bold;
      line-height: 1.1;
   }
   .profileUserSticky_title_become_text{
      font-size: 14px;
      line-height: 1.75;
      margin-top: 10px;
      text-align: initial;
   }
   .profileUserSticky_title{
      font-size: 18px;
      font-weight: 600;
   }
   .profileUserSticky_pricing{
      margin-bottom: 16px;
      display: flex;
      align-items: baseline;
      padding-top: 6px;
      .profileUserSticky_pricing_price{
         text-align: right;
         .profileUserSticky_pricing_price_hour{
            font-size: 16px;
            font-weight: 600;
         }
         .profileUserSticky_pricing_price_currency{
            font-size: 18px;
            font-weight: bold;
         }
         .profileUserSticky_pricing_price_number{
            font-size: 22px;
            font-weight: bold;
         }
      }
      .profileUserSticky_pricing_discount{
         text-align: left;
         font-size: 20px;
         color: #b2b5c9;
         text-decoration: line-through;
         padding-left: 6px;
      }
   }
   .profileUserSticky_coupon{
      margin-top: 18px;
      outline: none;
      font-weight: 600;
      color: #4c59ff;
      &.isMyProfileCoupon{
         color: #c5c8cf;
         cursor: initial;
      }
   }
   .profileUserSticky_respone{
      margin-top: 10px;
   }
   .profileUserSticky_btns{
      &.why_learn_user_btn{
         margin-top: 34px !important;
      }
      .profileUserSticky_btn{
         margin: 0;
         width: 100%;
         height: 44px;
         border-radius: 26px;
         .v-btn__content{
            // justify-content: flex-start;
            justify-content: start;
            // text-align: initial;
         }
         &.isMyProfile{
            color: white !important;
            border: none !important;
            svg{
               path{
                  fill: white;
               }
            }
         }
         .profileUserSticky_btn_icon{
            line-height: 0;
         }
         .profileUserSticky_btn_txt{
            font-size: 16px;
            font-weight: 600;
            text-transform: initial;
         }
         &.profileUserSticky_btn_book{
            margin-top: 14px;
            color: #4c59ff !important;
            border: solid 1.5px #4c59ff !important;
            &.isMyProfile{
               color: white !important;
               border: none !important;
               svg{
                  path{
                     fill: white;
                  }
               }
            }
         }
         &.profileUserSticky_btn_find{
            .v-btn__content{
               justify-content: center;
            }
         }
      }
   }
   .profileUserSticky_whyUs{
      padding-left: 36px;
      margin-top: 30px;
      font-size: 12px;
      &.why_learn_user{
         margin-top: 30px;
         .why_learn_user_row{
            margin-bottom: 8px;
         }
      }
      .profileUserSticky_whyUs_row{
         display: flex;
         .profileUserSticky_whyUs_row_icon{
            fill: #43425d !important;
            margin-right: 8px;
         }
         .profileUserSticky_whyUs_row_text{
            line-height: 1.62;
         }
      }
   }
}

</style>