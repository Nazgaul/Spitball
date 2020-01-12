<template>
  <div class="profileUserStickyMobile" v-if="!!getProfile && isTutor">
     <template v-if="getProfile.user.isTutor">
      <div class="profileUserStickyMobile_user">
         <div class="profileUserStickyMobile_user_img" v-if="!$vuetify.breakpoint.xsOnly">
            <userAvatar  
                  size="40" 
                  :userImageUrl="getProfile.user.image" 
                  :user-name="getProfile.user.name"/>
         </div>
         <div class="profileUserStickyMobile_user_info">
               <h6 class="profileUserStickyMobile_user_info_name text-truncate" v-if="!$vuetify.breakpoint.xsOnly">
                  <span v-language:inner="'profile_tutor'"/>
                  {{getProfile.user.name}}
               </h6>
               <div class="profileUserStickyMobile_user_info_pricing">
                  <v-flex class="profileUserStickyMobile_pricing_price">
                     <span class="profileUserStickyMobile_pricing_price_number">{{isDiscount && tutorPrice !== 0  ? tutorDiscountPrice : tutorPrice | currencyFormat(getProfile.user.tutorData.currency)}}</span>/<span class="profileUserStickyMobile_pricing_price_hour" v-language:inner="'profile_points_hour'"/>
                  </v-flex>
                  <!-- <v-flex class="profileUserStickyMobile_pricing_discount" v-if="tutorDiscountPrice">
                     {{tutorPrice ? tutorPrice : tutorDiscountPrice | currencyFormat(getProfile.user.tutorData.currency)}}
                  </v-flex> -->
                  <v-flex class="profileUserStickyMobile_pricing_discount" v-if="isDiscount">
                     {{tutorPrice ? tutorPrice : tutorDiscountPrice | currencyFormat(getProfile.user.tutorData.currency)}}
                  </v-flex>
               </div>
               <button :class="{'isMyProfileCoupon':isMyProfile}" class="profileUserStickyMobile_coupon" @click="globalFunctions.openCoupon" v-language:inner="'coupon_apply_coupon'"/>
         </div>
      </div>
      <div class="profileUserStickyMobile_actions ml-1">
            <v-btn :disabled="isMyProfile" class="profileUserStickyMobile_btn mr-2 white--text" :class="{'isMyProfile':isMyProfile}" depressed round color="#4452fc" @click="globalFunctions.sendMessage">
               <chatIcon class="profileUserStickyMobile_btn_icon" :class="[{'mr-2':$vuetify.breakpoint.mdAndUp}]"/>
               <div v-if="$vuetify.breakpoint.mdAndUp" class="profileUserStickyMobile_btn_txt" v-language:inner="'profile_send_message'"/>
            </v-btn>
            <v-btn :disabled="isMyProfile" v-if="getProfile.user.calendarShared" @click="globalFunctions.openCalendar" :class="{'isMyProfile':isMyProfile}" class="profileUserStickyMobile_btn profileUserStickyMobile_btn_book white--text" depressed round :color="$vuetify.breakpoint.xsOnly? '#4452fc':'white'">
               <calendarIcon  class="profileUserStickyMobile_btn_icon" :class="[{'mr-3':$vuetify.breakpoint.mdAndUp}]"/>
               <div v-if="$vuetify.breakpoint.mdAndUp" class="profileUserStickyMobile_btn_txt" v-language:inner="'profile_book_session_mobile'"/>
            </v-btn>
      </div>

     </template>
     <template v-if="!getProfile.user.isTutor">
         <v-btn class="profileUserSticky_btn profileUserSticky_btn_find white--text" depressed rounded color="#4452fc" @click="isMyProfile? globalFunctions.openBecomeTutor() : globalFunctions.goTutorList()">
            <div class="profileUserSticky_btn_txt" v-language:inner="isMyProfile? 'profile_become_tutor_btn':'profile_find_tutors'"/>
         </v-btn>
     </template>
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import userAvatar from '../../../helpers/UserAvatar/userAvatar.vue';
import chatIcon from './images/chatIcon_mobile.svg';
import calendarIcon from './images/calendarIcon_mobile.svg';

export default {
   components:{
      userAvatar,calendarIcon,chatIcon
   },
   props:{
      globalFunctions:{
         type: Object,
         required:true
      }
   },
   computed: {
      ...mapGetters(['getProfile','accountUser']),
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
      isMyProfile(){
         return !!this.getProfile && !!this.accountUser && this.accountUser.id == this.getProfile.user.id
      },
      isTutor(){
         return !!this.getProfile && this.getProfile.user.isTutor
      },
   },

}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
.profileUserStickyMobile{
    background: white;
    width: 100%;
    box-shadow: 0 0 10px 1px rgba(0, 0, 0, 0.22);
    height: 98px;
    color: #4d4b69;
    left: 0;
    right: 0;
    padding: 8px 0;
    z-index: 4;
    margin-top: 20px;
    position: sticky;
    bottom: 0;
    display: flex;
    justify-content: center;
    align-items: center;
    padding-right: 14px;
    @media (max-width: @screen-sm) {
       padding-right: 0;
    }
    @media (max-width: @screen-xs) {
       position: fixed;
       justify-content: space-evenly;
       align-items: end;
       padding: 2px 0;
       height: 70px;
       
    }
    .profileUserStickyMobile_user{
       display: flex;
      @media (max-width: @screen-xs) {
         text-align: center;
      }
      .profileUserStickyMobile_user_img{
         margin-right: 10px;
      }
      .profileUserStickyMobile_user_info{
         .profileUserStickyMobile_user_info_name{
            font-size: 16px;
            font-weight: bold;
            line-height: 1.2;
         }
         .profileUserStickyMobile_user_info_pricing{
            display: flex;
            justify-content: space-between;
            align-items: baseline;
            .profileUserStickyMobile_pricing_price{
               .profileUserStickyMobile_pricing_price_number{
                  font-size: 18px;
                  font-weight: bold;
                  line-height: 1.9;
                  @media (max-width: @screen-xs) {
                     font-size: 22px;
                     line-height: 1.3;
                  }
               }
               .profileUserStickyMobile_pricing_price_hour{
                  font-size: 12px;
                  font-weight: 600;
               }
            }
            .profileUserStickyMobile_pricing_discount{
               font-size: 16px;
               color: #b2b5c9;
               text-decoration: line-through;
               @media (max-width: @screen-xs) {
                  padding-left: 16px;
               }
            }
         }
         .profileUserStickyMobile_coupon{
            outline: none;
            font-size: 14px;
            font-weight: 600;
            color: #4c59ff;
            text-align: end;
            @media (max-width: @screen-xs) {
               font-size: 12px;
            }
            &.isMyProfileCoupon{
               color: #c5c8cf;
               cursor: initial;
            }
         }
      }

    }
    .profileUserStickyMobile_actions{
      display: flex;
      align-items: flex-end;
      height: 100%;
      padding-bottom: 8px;
      @media (max-width: @screen-xs) {
         padding-bottom: 0;
         padding-top: 8px;
         height: auto;
      }
      .profileUserStickyMobile_btn{
            max-width: 186px;
            height: 40px;
            border-radius: 26px;
            margin: 0;
            @media (max-width: @screen-xs) {
               height: 50px;
               width: 50px;
               min-width: 50px;
            }
            .v-btn__content{
               justify-content: flex-start;
               text-transform: none;
               @media (max-width: @screen-sm) {
                  justify-content: center;
               }
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
            .profileUserStickyMobile_btn_icon{
               line-height: 0;
            }
            .profileUserStickyMobile_btn_txt{
               font-size: 14px;
               font-weight: 600;
            }
            &.profileUserStickyMobile_btn_book{
               @media (max-width: @screen-sm) {
                  width: auto;
               }
               @media (max-width: @screen-xs) {
                  margin-left: 10%;
                  height: 50px;
                  width: 50px;
                  min-width: 50px;
                  color: initial !important;
                  border: none !important;
                  svg{
                  path{
                     fill: white;
                  }
               }
               }
               width: 100%;
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
            &.profileUserStickyMobile_btn_find{
               .v-btn__content{
                  justify-content: center;
               }
            }
         }
    }
}
</style>