<template>
  <router-link class="tutor-result-card-mobile justify-space-between" @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId,name:tutorData.name}}">
      <div class="card-mobile-header">
          <user-avatar-rect
            :userName="tutorData.name"
            :userImageUrl="tutorData.image"
            class="mr-2"
            :userId="tutorData.userId"
            :width="102"
            :height="116"
            :fontSize="24"
            :borderRadius="4"
          />
          <div class="card-mobile-header-content">
              <h3 class="text-truncate body-2 font-weight-bold" v-html="$Ph('resultTutor_private_tutor', tutorData.name)"></h3>

              <template>
                <h4 class="text-truncate university mt-1 font-weight-light" v-if="tutorData.university">{{tutorData.university}}</h4>
              </template>

              <template>
                  <div class="user-rate align-center" v-if="tutorData.reviews > 0">
                    <user-rating :rating="tutorData.rating" :showRateNumber="false" :size="'18'" class="mr-2" />
                    <span class="reviews" v-html="$Ph(tutorData.reviews === 1 ? 'resultTutor_review_one' : `resultTutor_reviews_many`, reviewsPlaceHolder(tutorData.reviews))"></span>
                  </div>
                  <div class="user-rate align-center" v-else>
                    <star class="mr-1 icon-star" />
                    <span class="reviews" v-html="$Ph(`resultTutor_collecting_review`, reviewsPlaceHolder(tutorData.reviews))"></span>
                  </div>
              </template>

              <div class="price align-center">
                  <div class="price_oneline">
                      <template>
                          <span v-if="isDiscount" class="price_oneline--count font-weight-bold">{{tutorData.discountPrice | currencyFormat(tutorData.currency)}}</span>
                          <span v-else class="price_oneline--count font-weight-bold">{{tutorData.price | currencyFormat(tutorData.currency)}}</span>
                          <span>/</span>
                      </template>
                      <span class="caption" v-language:inner="'resultTutor_hour'"></span>
                  </div>
                  <div class="striked ml-3" v-if="isDiscount">{{tutorData.price | currencyFormat(tutorData.currency)}}</div>
              </div>

              <router-link class="applyCoupon" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name},  query: {coupon: true}}" v-language:inner="'resultTutor_apply_coupon'"></router-link>
              
              <!-- DO NOT REMOVE THIS WAITING SHIRAN -->
              <!-- <div class="courses text-truncate">
                  <div class="" v-language:inner="'resultTutor_courses'"></div>
                  <div class="text-truncate">{{courses}}</div>
              </div>  -->

              <!-- <template>
                <h4 class="text-truncate mb-1 university font-weight-light" v-if="isUniversity" v-html="$Ph('resultNote_university',[tutorData.university])"/>
                <h4 class="text-truncate mb-1 university" v-else></h4>
              </template>  -->

          </div>
      </div>

      <div class="card-mobile-center">{{tutorData.bio}}</div>

      <!-- DO NOT REMOVE THIS WAITING SHIRAN -->
      <div class="courses text-truncate">
          <div class="courses-title" v-language:inner="'resultTutor_courses'"></div>
          <div class="text-truncate">{{courses}}</div>
      </div> 

      <div class="card-mobile-footer">
          <v-btn class="btn-chat white--text text-truncate my-0" depressed round block color="#4452fc" @click.prevent.stop="sendMessage(tutorData)">
                <iconChat class="chat-icon-btn" />
                <div class="text-truncate" v-html="$Ph('resultTutor_send_button', showFirstName)"></div>
          </v-btn>
      </div>

  </router-link>
</template>

<script>
import { mapActions, mapGetters } from "vuex";

import { LanguageService } from "../../../../services/language/languageService.js";
import chatService from '../../../../services/chatService';
import analyticsService from "../../../../services/analytics.service";

import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import userAvatarRect from '../../../helpers/UserAvatar/UserAvatarRect.vue';

import iconChat from '../tutorResultCardOther/icon-chat.svg';
import star from '../stars-copy.svg';

export default {
  name: "tutorCard",
  components: {
    userRating,
    userAvatarRect,
    iconChat,
    star
  },
  props: {
    tutorData: {},
    fromLandingPage: {
      type: Boolean,
      default: false
    }
  },
  methods: {
    ...mapActions(["updateRequestDialog", 'updateCurrTutor', 'setTutorRequestAnalyticsOpenedFrom', 'openChatInterface', 'setActiveConversationObj']),

    tutorCardClicked() {
      if(this.fromLandingPage){
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_landing_page");
      }else{
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_page");
      }
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
        return `${this.tutorData.courses.join(', ')}`
      }
      return '';
    },
    isUniversity() {
      return (this.tutorData && this.tutorData.university) ? true : false;
    },
    showFirstName() {
      let maxChar = 5;
      let name = this.tutorData.name.split(' ')[0];
      if(name.length > maxChar) {
        return LanguageService.getValueByKey('resultTutor_message_me');
      }
      return name;
    },
    isDiscount() {
      return this.tutorData.discountPrice !== undefined;
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
@purple: #43425d;

.tutor-result-card-mobile {
    padding: 12px;
    border-radius: 4px;
    background: #fff;
    display: flex;
    flex-direction: column;
    h3, h4, .courses, .card-mobile-center, .price {
        color: @purple;
    }
    .card-mobile-header {
        display: flex;
        .card-mobile-header-content {
          min-width: 0;
        }
        .user-rate {
            display: inline-flex;
            margin-top: 6px;
            .reviews {
                font-size: 12px;
                letter-spacing: normal;
                color: #43425d;
            }
            .icon-star {
              width: 18px;
            }
        }
        .courses {
            font-size: 12px;
            div {
              display: inline;
            }
        }
        .university {
          font-size: 12px;

          &.no-field {
            .heightMinMax(16px);
          }
        }
        .price {
          display: flex;
          align-items: flex-end;
          flex: .5;
          margin: 4px 0 1px 0;
          .price_oneline {
            display: flex;
            align-items: flex-end;
            color: #5158af;

            &--count {
              font-size: 20px;
            }
          }
          .striked {
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
          }
       }
       .applyCoupon {
          color: #4c59ff;
          font-weight: 600;
          font-size: 12px;
          margin-top: 6px;
        }
    }
    .card-mobile-center {
      margin: 10px 0;
      .giveMeEllipsis(2,20px);
    }
    .card-mobile-footer {
        display: inherit;
        .btn-chat {
          font-weight: 600;
          .v-btn__content{
            .chat-icon-btn{
              // position: absolute;
              // top: 0;
              // left: 0;
              align-self: flex-end;
            }
            :last-child {
              margin-bottom: 2px;
            }
          }
          position: relative;
          text-transform: inherit;
          border-radius: 7.5px;
          div {
            div {
              padding-left: 10px;
            }
          }
        }
    }
    .courses {
      display: flex;
      font-size: 12px;
      margin-bottom: 10px;
      &-title {
        margin-right: 4px;
      }
    }
}
</style>