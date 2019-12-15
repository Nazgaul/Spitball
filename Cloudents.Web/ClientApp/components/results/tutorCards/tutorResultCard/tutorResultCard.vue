<template>
    <router-link class="tutor-result-card-desktop pa-4" @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}">

        <v-flex class="user-details">
            <user-avatar-rect 
              :userName="tutorData.name" 
              :userImageUrl="tutorData.image" 
              class="user-avatar-rect" 
              :userId="tutorData.userId"
              :width="148" 
              :height="182"
              :borderRadius="4"
            />
            <div class="main-card">
                <h3 class="font-weight-bold text-truncate body-1 mb-1" v-html="$Ph('resultTutor_private_tutor', tutorData.name)"></h3>
                <h4 class="mb-3 body-2 font-weight-bold text-truncate" :class="{'university-hidden': !university}">{{university}}</h4>
                <div class="user-bio-wrapper mb-3">
                  <div class="user-bio body-2">{{tutorData.bio}}</div>
                </div>
                <div class="study-area text-truncate" :class="{'study-area-hidden': !isSubjects}">
                  <span class="mr-1 body-2 font-weight-bold" v-language:inner="'resultTutor_study-area'"></span>
                  <span class="body-2">{{subjects}}</span>
                </div>
                <div class="courses text-truncate" v-if="isCourses">
                  <span class="mr-2 body-2 font-weight-bold" v-language:inner="'resultTutor_courses'"></span>
                  <span class="body-2">{{courses}}</span> 
                </div>
            </div>
        </v-flex>

        <v-divider vertical class="mx-3"></v-divider>

        <div class="user-rates">
            <div class="price">
              <router-link class="applyCoupon" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name},  query: {coupon: true}}" v-language:inner="'resultTutor_apply_coupon'"></router-link>
              <div class="user-rates-top">
                <template>
                    <span v-if="isDiscount" class="tutor-card-price font-weight-bold">{{tutorData.discountPrice | currencyFormat(tutorData.currency)}}</span>
                    <span class="tutor-card-price font-weight-bold" v-else>{{tutorData.price | currencyFormat(tutorData.currency)}}</span>
                </template>
                <span class="caption">
                  <span class="tutor-card-price-divider font-weight-bold">/</span>
                  <span class="tutor-card-price-divider font-weight-bold" v-language:inner="'resultTutor_hour'"></span>
                </span>
                <div class="striked mr-1" v-if="isDiscount">{{tutorData.price | currencyFormat(tutorData.currency)}}</div>
                <div class="striked no-discount" v-else></div>
              </div>
            </div>

            <template>
              <div class="user-rank align-center" v-if="isReviews">
                <user-rating :rating="tutorData.rating" :showRateNumber="false"/>
                <div class="reviews" v-html="$Ph(tutorData.reviews === 1 ? 'resultTutor_review_one' : `resultTutor_reviews_many`, reviewsPlaceHolder(tutorData.reviews))"></div>
              </div>
              <div v-else class="user-rank align-center">
                <star class="user-rank-star"/>
                <span class="no-reviews font-weight-bold caption" v-language:inner="'resultTutor_no_reviews'"></span>
              </div>
            </template>
            
            <div class="classes-hours body-2 align-center">
              <clock />
              <span class="font-weight-bold caption classes-hours_lesson" v-if="tutorData.lessons > 0">{{tutorData.lessons}}</span>
              
              <template>
                <span class="font-weight-bold caption no-classes" v-language:inner="'resultTutor_no_hours_completed'" v-if="tutorData.lessons === 0"></span>
                <span class="font-weight-bold caption no-classes" v-language:inner="tutorData.lessons === 1 ? 'resultTutor_hour_completed' : 'resultTutor_hours_completed' " v-else></span>    
              </template>
            </div>                

            <div class="send-btn">
                <v-btn class="btn-chat white--text" depressed rounded block color="#4452fc" @click.prevent="sendMessage(tutorData)">
                  <iconChat class="chat-icon-btn" v-if="fromLandingPage" />
                  <div class="" v-html="$Ph('resultTutor_send_button', showFirstName)" ></div>
                </v-btn>
            </div>
        </div>

    </router-link>
</template>

<script>
import { mapActions, mapGetters } from "vuex";

import analyticsService from "../../../../services/analytics.service";
import chatService from '../../../../services/chatService';
import { LanguageService } from "../../../../services/language/languageService.js";

import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import userAvatarRect from '../../../helpers/UserAvatar/UserAvatarRect.vue';

import iconChat from '../tutorResultCardOther/icon-chat.svg';
import clock from './clock.svg';
import star from '../stars-copy.svg';

export default {
  name: "tutorResultCard",
  components: {
    userRating,
    clock,
    star,
    iconChat,
    userAvatarRect
  },
  props: {
    tutorData: {},
    fromLandingPage: {
      type: Boolean,
      default: false
    }
  },
  methods: {
    ...mapActions(['updateRequestDialog', 'updateCurrTutor', 'setTutorRequestAnalyticsOpenedFrom', 'openChatInterface', 'setActiveConversationObj']),

    tutorCardClicked(e) {
      if(this.fromLandingPage){
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_landing_page");
      }else{
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_page");
      };
    },
    reviewsPlaceHolder(reviews) {
      return reviews === 0 ? reviews.toString() : reviews
    },
    sendMessage(user) {
      if (this.accountUser == null) {
          analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:GUEST`);
          this.updateCurrTutor(user);
          this.setTutorRequestAnalyticsOpenedFrom({
            component: 'tutorCard',
            path: this.$route.path
          });
          this.updateRequestDialog(true);
      } else {
          analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:${this.accountUser.id}`);
          let conversationObj = {
              userId: user.userId,
              image: user.image,
              name: user.name,
              conversationId: chatService.createConversationId([user.userId, this.accountUser.id]),
          }
          let currentConversationObj = chatService.createActiveConversationObj(conversationObj)
          this.setActiveConversationObj(currentConversationObj);
          let isMobile = this.$vuetify.breakpoint.smAndDown;
          this.openChatInterface();                    
      }
    }
  },
  computed: {
    ...mapGetters(['accountUser']),

    courses() {
      if (this.tutorData.courses) {
        return `${this.tutorData.courses.join(', ')}`;
      }
      return '';
    },
    isSubjects() {
      return this.tutorData && this.tutorData.subjects.length > 0 ? true : false;
    },
    isCourses() {
      return this.tutorData && this.tutorData.courses.length > 0 ? true : false;
    },
    university() {
      return this.tutorData.university;
    },
    subjects() {
      return this.tutorData.subjects.join(', ');
    },
    showFirstName() {
      let maxChar = 5;
      let name = this.tutorData.name.split(' ')[0];
      if(name.length > maxChar) {
        return LanguageService.getValueByKey('resultTutor_message_me');
      }
      return name;
    },
    isReviews() {
      return this.tutorData.reviews > 0 ? true : false;
    },
    isDiscount() {
      return this.tutorData.discountPrice !== undefined;
    }
  },
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

  @purple: #43425d;

  .tutor-result-card-desktop {
    // .heightMinMax(214px);
    border-radius: 8px;
    background: #fff;
    width: 100%;
    display: flex;
    h3, h4, .user-bio, .courses, .price, .classes-hours, .study-area {
      color: @purple;
    }
    .user-avatar-rect {
      margin-right: 12px;
    }
    .user-details {
      width: 0;
      flex-grow: 4.5;
      display: flex;
      .main-card {
        min-width: 0;
        .university-hidden {
          visibility: hidden;
          min-height: 16px;
        }
        .user-bio-wrapper {
          position: relative;
          min-height: 60px;
          .user-bio {
            .giveMeEllipsis(3, 20px);
          }
        }
        .study-area-hidden {
          visibility: hidden;
        }
        .courses {
          margin-top: 2px;
        }
      }
    }
    div:nth-child(2) {
      flex-basis: auto;
    }
    .user-rates {
      // display: grid;
      // grid-gap: 28px;
      display: flex;
      flex-direction: column;
      align-items: baseline;
      justify-content: space-between;
      flex: 2;
      .striked {
        margin: 0 0 0 auto;
        max-width: max-content;
        position: relative;
        color: #a0a4be;
        &:after {
            content: "";
            width: 120%;
            border-bottom: solid 1px #a0a4be;
            position: absolute;
            left: -2px;
            top: 50%;
            z-index: 1;
        }
        &.no-discount {
          min-height: 19px;
        }
      }
      .price {
        width: 100%;
        display: flex;
        justify-content: space-between;
        margin-bottom: -20px;
        .applyCoupon {
          color: #5a61ba;
          font-weight: 600;
          font-size: 12px;
          margin-top: 6px;
        }
        .user-rates-top {
          align-items: baseline;
          .tutor-card-currency {
            font-size: 16px;
            color:#5158af;
          }
          .tutor-card-price {
            font-size: 18px;
            color:#5158af;
          }
          .tutor-card-price-divider {
            font-size: 12px;
            color:#5158af;
          }
          .menu-area {
              margin-top: -12px;
              width: 21px;
            .v-btn__content {
              i {
                font-size: 16px;
                color: rgba(0, 0, 0, 0.25);
              }
            }
          }
        }
        .price-default-height {
          .heightMinMax(16px);
        }
      }
      .classes-hours {
        margin-left: 3px;
        display: flex;
        align-items: end;
        &_lesson {
          margin-left: 6px;
        }
        .no-classes {
          margin-left: 6px;
        }
      }
      .user-rank {
        display: -webkit-box;
        margin-bottom: -20px;
        i{
          font-size: 18px !important;
        }
        .reviews {
          font-size: 12px;
          color: #4452fc;
          font-size: 14px;
        }
        .no-reviews {
          margin-left: 5px;
          color: #43425d;
        }
        .user-rank-star {
          width: 18px;
          vertical-align: sub;
        }
      }
      .send-btn {
        width: 100%;
        min-width: 100%;
        max-width: 0;
        .btn-chat {
          font-weight: 600;
          position: relative;
          margin: 0 auto;
          text-transform: initial;
          .chat-icon-btn {
            text-transform: inherit;
            position: absolute;
            left: 0;
          }
        }
      }
    }
  }
</style>
