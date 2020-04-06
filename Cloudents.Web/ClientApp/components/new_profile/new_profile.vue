<template>
  <div class="profilePage">
    <cover></cover>
    <profileDialogs />
    <div class="profilePage_main profile-page-container">
      <profileUserBox :globalFunctions="globalFunctions" />
      <shareContent
        :link="shareContentParams.link"
        :twitter="shareContentParams.twitter"
        :whatsApp="shareContentParams.whatsApp"
        :email="shareContentParams.email"
        class="mb-2 mb-sm-3 shareContentProfile"
        v-if="getProfile"
      />
      <calendarTab v-if="showProfileCalendar" class="mb-6 mx-auto calendarSection" :globalFunctions="globalFunctions" />
      <profileBecomeTutor v-if="showBecomeTutor" class="mb-3 d-lg-none" />
      <profileFindTutor v-if="showFindTutor" class="mb-3 d-lg-none" />
      <profileItemsBox v-if="showItems" class="mt-12" />
      <profileEarnMoney class="mt-0 mt-sm-5" v-if="showEarnMoney" />
      <profileItemsEmpty class="mt-0 mt-sm-5 mb-2 mb-sm-4" v-show="showItemsEmpty" />
      <profileReviewsBox v-if="showReviewBox" class="mt-10" />
      <profileUserStickyMobile
        :globalFunctions="globalFunctions"
        v-if="$vuetify.breakpoint.mdAndDown"
      />
    </div>
    <!-- SIDE -->
    <!-- <div :class="['profile-sticky',{'profileUserSticky_bannerActive':getBannerParams}]">
          <profileUserSticky class="mb-2" :globalFunctions="globalFunctions" v-if="$vuetify.breakpoint.lgAndUp && !isTutorPending"/>
          <shareContent 
              :link="shareContentParams.link"
              :twitter="shareContentParams.twitter"
              :whatsApp="shareContentParams.whatsApp"
              :email="shareContentParams.email" v-if="getProfile && $vuetify.breakpoint.lgAndUp"/>
    </div>-->
    <sb-dialog
      :onclosefn="closeCouponDialog"
      :activateOverlay="false"
      :showDialog="isShowCouponDialog"
      :maxWidth="'410px'"
      :popUpType="'coupon'"
      :content-class="'coupon-dialog'"
      :isPersistent="true"
    >
      <v-card class="pb-4 coupon-dialog-card" :class="{'d-block': $vuetify.breakpoint.xsOnly}">
        <v-layout class="header py-6">
          <v-flex
            class="text-xs-center coupon-dialog-header"
            :class="{'mt-5': $vuetify.breakpoint.xsOnly}"
          >
            <span v-language:inner="'coupon_title'"></span>
            <v-icon @click="closeCouponDialog" class="coupon-close" v-html="'sbf-close'" />
          </v-flex>
        </v-layout>
        <v-layout class="px-4" column>
          <v-flex class="mb-2">
            <div class="coupon coupon__dialog" v-if="isTutor && !isMyProfile">
              <div class="text-xs-right">
                <div class="coupon__dialog--flex">
                  <input
                    type="text"
                    @keyup.enter="applyCoupon"
                    v-model="coupon"
                    :placeholder="couponPlaceholder"
                    class="profile-coupon_input"
                  />
                  <button
                    class="profile-coupon_btn white--text"
                    :disabled="disableApplyBtn"
                    @click="applyCoupon"
                    v-language:inner="'coupon_apply_btn'"
                  ></button>
                </div>
                <div
                  class="profile-coupon_error"
                  v-language:inner="'coupon_apply_error'"
                  v-if="getCouponError"
                ></div>
              </div>
            </div>
          </v-flex>
        </v-layout>
      </v-card>
    </sb-dialog>
  </div>
</template>

<script src="./new_profile.js"></script>

<style lang="less">
@import "../../styles/mixin.less";
.profilePage {
  position: relative;
  // display: flex;
  // margin: 24px 0;
  //  justify-content: center;

  margin-bottom: 30px;
 
  // margin: 24px 70px 26px 34px;

  @media (max-width: @screen-md) {
    // margin: 20px;
    justify-content: center;
  }
  @media (max-width: @screen-xs) {
    margin: 0;
    margin-bottom: 40px;
    display: block;
  }
  .profile-sticky {
    position: sticky;
    height: fit-content;
    top: 80px;
    &.profileUserSticky_bannerActive {
      top: 150px;
    }
  }
  .profilePage_main {
    max-width: 1920px;
    width: 100%;
    padding-top: 180px;
    //margin-right: 33px;
    //         @media (max-width: @screen-sm) {
    //           //  margin-right: 0;
    // //            max-width: auto;
    //         }
    &.profile-page-container {
      &.content-center {
        margin: 0 auto;
      }
      @media (max-width: @screen-md-plus) {
        margin-left: 0;
      }
      @media (max-width: @screen-xs) {
        margin-left: 0;
        padding: 0;
        margin-bottom: 60px;
      }
      .question-container {
        margin: unset;
      }
      .back-button {
        transform: none /*rtl:rotate(180deg)*/;
        position: absolute;
        top: 32px;
        left: 10px;
        z-index: 99;
        outline: none;
      }
      .limit-width {
        max-width: 500px;
      }
      .bio-wrap {
        align-items: flex-start;
        @media (max-width: @screen-xs) {
          align-items: unset;
        }
      }
      .limited-760 {
        max-width: 760px;
      }
    }

    .shareContentProfile {
      max-width: 292px;
      margin: -10px auto 0;
      //position: absolute;
      //right: 0;
      //left: 0;
      z-index: 1;
    }
  }
  .calendarSection {
    max-width: 800px;
  }
}
.coupon-dialog {
  border-radius: 6px;
  background: white;
  display: flex;
  flex-direction: column;
  .coupon-dialog-header {
    text-align: center;
    font-weight: 600;
    font-size: 18px;
    color: #43425d;
    .coupon-close {
      position: absolute;
      right: 10px;
      top: 10px;
      font-size: 12px;
      fill: #adadba;
      cursor: pointer;
    }
  }
  .coupon {
    display: flex;
    width: 100%;
    justify-content: center;
    &__dialog {
      &--flex {
        display: flex;
      }
      .profile-coupon_input {
        outline: none;
        border-radius: 6px 0 0 6px;
        width: 200px;
        border: 1px solid #bbb;
        padding: 10px 2px 11px 8px;
        margin-right: -5px;
      }
      .profile-coupon_btn {
        border-radius: 0 6px 6px 0;
        background-color: #4c59ff;
        font-size: 12px;
        padding: 8px 20px;
        font-weight: 600;
        outline: none;
      }
      .profile-coupon_error {
        width: 236px;
        margin-top: 4px;
        text-align: left;
        font-size: 11px;
        color: #ff5252;
      }
    }
  }
}
</style>