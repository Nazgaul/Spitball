<template>
    <div class="profilePage">
        <profileDialogs/>
        <div class="profilePage_main profile-page-container">
            
            <profileUserBox/>
            <calendarTab v-if="showProfileCalendar" class="mb-3" :globalFunctions="globalFunctions"/>
            <profileEarnMoney v-if="showEarnMoney" class="mb-3" :globalFunctions="globalFunctions"/>
            <profileBecomeTutor v-if="showBecomeTutor" class="mb-3" :globalFunctions="globalFunctions"/>
            <profileFindTutor v-if="showFindTutor" class="mb-3" :globalFunctions="globalFunctions"/>
            <profileItemsBox v-if="showItems" class="mb-3" :globalFunctions="globalFunctions"/>

            <!-- <v-layout wrap v-bind="xsColumn" align-start  justify-start>
                <v-flex sm12 :class="[isMyProfile && isTutorProfile ? '' : ''  ]">
                    <v-flex xs12 class="mt-3" :class="[$vuetify.breakpoint.xsOnly ? 'mb-2' : 'mb-4']">
                        <v-divider v-if="$vuetify.breakpoint.xsOnly" style="height:2px; color: rgba(163, 160, 251, 0.32);"></v-divider>
                        <v-tabs :dir="isRtl && $vuetify.breakpoint.xsOnly ? `ltr` : isRtl? 'rtl' : ''" class="tab-padding" hide-slider xs12>

                            <v-tab sel="uploaded_tab" @click="activeTab = 1" :id="`tab-${1}`" :href="'#tab-1'" :key="1">
                                <span class="text-capitalize body-1" v-language:inner="'profile_documents'"/>
                            </v-tab>

                            <v-tab sel="answer_tab" @click="activeTab = 2" :id="`tab-${2}`" :href="'#tab-2'" :key="2">
                                <span class="text-capitalize body-1" v-language:inner="'profile_Answers'"/>
                            </v-tab>

                            <v-tab sel="question_tab" @click="activeTab = 3" :id="`tab-${3}`" :href="'#tab-3'" :key="3">
                                <span class="text-capitalize body-1" v-language:inner="'profile_Questions'"/>
                            </v-tab>

                            <v-tab sel="purchased_tab" @click="activeTab = 4" :id="`tab-${4}`" :href="'#tab-4'" :key="4">
                                <span class="text-capitalize body-1" v-language:inner="'profile_purchased_documents'"/>
                            </v-tab>

                            <v-tab sel="calendar_tab" @click="openCalendar" :id="`tab-${5}`" :href="'#tab-5'" :key="5" v-if="showCalendar">
                                <span class="text-capitalize body-1" v-language:inner="'profile_calendar'"/>
                            </v-tab>


                        </v-tabs>
                        <v-divider style="height:2px; color: rgba(163, 160, 251, 0.32);"></v-divider>

                    </v-flex>
                    <v-flex class="web-content">

                        <div class="empty-state doc-empty-state"
                                v-if="activeTab === 1 && isMyProfile && !uploadedDocuments.length && !loadingContent">
                            <div class="text-block">
                                <p v-html="emptyStateData.text"></p>
                                <b>{{emptyStateData.boldText}}</b>
                            </div>
                            <div class="upload-btn-wrap">
                                <upload-document-btn></upload-document-btn>
                            </div>
                        </div>
                        <div class="empty-state"
                                v-else-if="activeTab === 2 && isMyProfile && !answerDocuments.length && !loadingContent">
                            <div class="text-block">
                                <p v-html="emptyStateData.text"></p>
                                <b>{{emptyStateData.boldText}}</b>
                            </div>
                            <router-link class="ask-question" :to="{name: emptyStateData.btnUrl}">
                                {{emptyStateData.btnText}}
                            </router-link>
                        </div>

                        <div class="empty-state"
                                v-if="activeTab === 3 && isMyProfile && !questionDocuments.length && !loadingContent">
                            <div class="text-block">
                                <p v-html="emptyStateData.text"></p>
                                <b>{{emptyStateData.boldText}}</b>
                            </div>
                            <a class="ask-question" @click="emptyStateData.btnUrl()">{{emptyStateData.btnText}}</a>
                        </div>
                        <scroll-list v-if="activeTab === 1" :scrollFunc="loadDocuments" :isLoading="documents.isLoading"
                                        :isComplete="documents.isComplete">
                            <div 
                                            v-for="(document ,index) in uploadedDocuments"
                                            :key="index" class="mb-3">
                                <result-note :item="document" class="pa-3 "></result-note>
                            </div>
                        </scroll-list>


                        <scroll-list v-if="activeTab === 2" :scrollFunc="loadAnswers" :isLoading="answers.isLoading"
                                        :isComplete="answers.isComplete">
                            <div 
                                            v-for="(answerData,index) in answerDocuments"
                                            :key="index" class="mb-3">
                                <question-card :cardData="answerData"></question-card>
                            </div>
                        </scroll-list>

                        <scroll-list v-if="activeTab === 3" :scrollFunc="loadQuestions" :isLoading="questions.isLoading"
                                        :isComplete="questions.isComplete">
                                            <div class="mb-3"  v-for="(questionData,index) in questionDocuments" :key="index">
                                <question-card :cardData="questionData"></question-card>
                                </div>

                        </scroll-list>

                        <scroll-list v-if="activeTab === 4 && isMyProfile" :scrollFunc="loadPurchasedDocuments"
                                        :isLoading="purchasedDocuments.isLoading"
                                        :isComplete="purchasedDocuments.isComplete">
                            <div 
                                            v-for="(document ,index) in purchasedsDocuments"
                                            :key="index" class="mb-3">
                                <result-note :item="document" class="pa-3 "></result-note>
                            </div>
                        </scroll-list>
                        <scroll-list v-if="activeTab === 5" :scrollFunc="(()=>{})"
                                        :isLoading="calendar.isLoading"
                                        :isComplete="calendar.isComplete">
                            <div class="mb-3">
                                <calendarTab></calendarTab>
                            </div>
                        </scroll-list>                            
                    </v-flex>
                </v-flex>
                <v-flex sm3 xs12 v-if="isMyProfile || isTutorProfile">
                    <v-spacer></v-spacer>
                </v-flex>
            </v-layout> -->
            <profileReviewsBox v-if="showReviewBox"/>
            <profileUserStickyMobile :globalFunctions="globalFunctions" v-if="$vuetify.breakpoint.mdAndDown"/>
        </div>
        
        <profileUserSticky :globalFunctions="globalFunctions" v-if="$vuetify.breakpoint.lgAndUp && !isTutorPending"/>






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
                <v-flex class="text-xs-center coupon-dialog-header" :class="{'mt-5': $vuetify.breakpoint.xsOnly}">
                    <span v-language:inner="'coupon_title'"></span>
                    <v-icon @click="closeCouponDialog" class="coupon-close" v-html="'sbf-close'" />
                </v-flex>
            </v-layout>
            <v-layout class="px-4" column>
                <v-flex class="mb-2">
                    <div class="coupon coupon__dialog" v-if="isTutor && !isMyProfile">
                      <div class="text-xs-right ">
                        <div class="coupon__dialog--flex">
                          <input type="text" @keyup.enter="applyCoupon" v-model="coupon" :placeholder="couponPlaceholder" class="profile-coupon_input">
                          <button class="profile-coupon_btn white--text" :disabled="disableApplyBtn" @click="applyCoupon" v-language:inner="'coupon_apply_btn'"></button>
                        </div>
                        <div class="profile-coupon_error" v-language:inner="'coupon_apply_error'" v-if="getCouponError"></div>
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
@import '../../styles/mixin.less';
.profilePage{
  position: relative;
    display: flex;
    margin: 24px 70px 26px 34px;

    @media (max-width: @screen-md) {
        margin: 20px;
        justify-content: center;
    }
    @media (max-width: @screen-xs) {
        margin: 0;
        margin-bottom: 40px;
        display: block;
    }
    .profilePage_main {
        max-width: 720px;
        width: 100%;
        margin-right: 33px;
        @media (max-width: @screen-sm) {
            margin-right: 0;
            max-width: auto;
        }
        &.profile-page-container {
  &.content-center{
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
  .question-container{
    margin: unset;
  }
  .back-button{
    transform: none /*rtl:rotate(180deg)*/ ;
    position: absolute;
    top: 32px;
    left: 10px;
    z-index: 99;
    outline: none;
  }
  .limit-width{
    max-width: 500px;
  }
  .bio-wrap{
    align-items: flex-start;
    @media (max-width: @screen-xs) {
     align-items: unset;
    }
  }
  .limited-760 {
    max-width: 760px;
  }

  .tab-padding {
    .v-tabs__bar {
      background-color: transparent;
      @media (max-width: @screen-xs) {
        height: 42px;
        font-size: 18px;
      }
    }
    .v-tabs__container {
      height: 42px;
      .v-tabs__item {
        font-size: 14px;
        font-weight: 300;
        color: @textColor;
        text-transform: capitalize;
        &.v-tabs__item--active {
          font-weight: bold;
          color: @color-blue-new !important;
        }
      }
      .v-tabs__slider.blue {
        border-bottom: 2px solid @color-blue-new;
      }
    }
  }
  .web-content {
    // max-width: 760px;
    .question-card-wrapper {
      display: block;
      margin-bottom: 16px;
    }
  }
  .empty-state {
    &:not(.doc-empty-state) {
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      border-radius: 4px;
      background-color: @color-white;
      box-shadow: 0 0 2px 0 rgba(0, 0, 0, 0.5);
      margin-bottom: 16px;
      .responsive-property(height, 228px, null, 248px);
      .text-block {
        flex-grow: 1;
        display: flex;
        flex-direction: column;
        justify-content: center;
        font-weight: 300;
        margin-bottom: 0;
        .responsive-property(padding-top, 4px, null, 0);
        padding-left: 20px;
        padding-right: 20px;
        p, b {
          line-height: 0.9;
          letter-spacing: -0.3px;
          color: @color-grey;
          .responsive-property(font-size, 32px, null, 30px);
          font-size: 32px;
          .responsive-property(line-height, 1, null, 1.2);
          margin-bottom: 0;
        }
        b {
          font-weight: bold;
        }
      }
      a {
        display: inline-block;
        line-height: 40px;
        min-width: 208px;
        max-width: 208px;
        background: @color-dark-blue;
        color: @color-white;
        font-size: 20px;
        font-weight: bold;
        letter-spacing: -0.5px;
        border-radius: 4px;
        margin-bottom: 24px;
      }
    }

  }
  .doc-empty-state {
    display: flex;
    flex-direction: column;
    align-items: center;
    text-align: center;
    border-radius: 4px;
    background-color: @color-white;
    box-shadow: 0 0 2px 0 rgba(0, 0, 0, 0.5);
    margin-bottom: 16px;
    .responsive-property(height, 228px, null, 248px);
    .responsive-property(padding-bottom, 0, null, 16px);
    .upload-btn:not(.rounded-floating-button) {
      //TODO create a new button componet for upload doc, without floating, and unused styles
      margin-right: 0 !important;
    }
    .text-block {
      flex-grow: 1;
      display: flex;
      flex-direction: column;
      justify-content: center;
      font-weight: 300;
      margin-bottom: 0;
      .responsive-property(padding-top, 4px, null, 0);
      padding-left: 20px;
      padding-right: 20px;
      p, b {
        line-height: 0.9;
        letter-spacing: -0.3px;
        color: @color-grey;
        .responsive-property(font-size, 32px, null, 30px);
        font-size: 32px;
        .responsive-property(line-height, 1, null, 1.2);
        margin-bottom: 0;
      }
      b {
        font-weight: bold;
      }
    }
  }

}

































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