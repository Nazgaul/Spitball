<template>
  <router-link class="tutor-result-card-mobile justify-space-between" @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId,name:tutorData.name}}">
      <div class="card-mobile-header">
          <userAvatarNew
            :userName="tutorData.name"
            :userImageUrl="tutorData.image"
            class="me-2"
            :userId="tutorData.userId"
            :width="102"
            :height="116"
            :fontSize="24"
            :borderRadius="4"
            :tile="true"
          />
          <div class="card-mobile-header-content">
              <div>
                <h3 class="text-truncate font-weight-bold card-mobile-tutor-name mb-2">{{tutorData.name}}</h3>

                <template>
                    <div class="user-rate align-center" v-if="tutorData.reviews > 0">
                      <user-rating :rating="tutorData.rating" :showRateNumber="false" :size="'18'" class="flex-grow-0 me-2" />
                      <span class="reviews">{{$tc('resultTutor_review_one',tutorData.reviews)}}</span> 
                    </div>
                    <div class="user-rate align-center" v-else>
                      <star class="me-1 icon-star" />
                      <span class="reviews">{{$tc('resultTutor_review_one',tutorData.reviews)}}</span>
                    </div>
                </template>
              </div>
          </div>
      </div>

      <div class="card-mobile-center">{{tutorData.bio}}</div>

      <div class="courses text-truncate" v-if="subjects">
          <div class="courses-title font-weight-bold" v-t="'resultTutor_study-area'"></div>
          <div class="text-truncate">{{subjects}}</div>
      </div> 
      <div class="courses text-truncate" v-else>
          <div class="courses-title font-weight-bold" v-t="'resultTutor_courses'"></div>
          <div class="text-truncate">{{courses}}</div>
      </div> 


      <div class="card-mobile-footer">
          <v-btn class="btn-chat white--text text-truncate my-0" depressed rounded block color="#4452fc" @click.prevent.stop="sendMessage(tutorData)">
                <iconChat class="chat_icon_btn" />
                <div class="text-truncate text_icon_btn">{{$t('resultTutor_send_button',[showFirstName])}}</div>
          </v-btn>
      </div>

  </router-link>
</template>

<script>
import { mapActions, mapGetters } from "vuex";

import chatService from '../../../../services/chatService';
import analyticsService from "../../../../services/analytics.service";
import * as routeNames from '../../../../routes/routeNames.js'
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";

import iconChat from '../icon-chat.svg';
import star from '../stars-copy.svg';

export default {
  name: "tutorCard",
  components: {
    userRating,
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
    ...mapActions(["updateRequestDialog", 'updateCurrTutor', 'setTutorRequestAnalyticsOpenedFrom', 'setActiveConversationObj']),

    tutorCardClicked() {
      if(this.fromLandingPage){
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_landing_page");
      }else{
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_page");
      }
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
      } else if(user.isTutor && user.userId == this.accountUser.id) { // this is my profile

      } else {
          analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:${this.accountUser.id}`);
          let conversationObj = {
              userId: user.userId,
              image: user.image,
              name: user.name,
              conversationId: chatService.createConversationId([user.userId, this.accountUser.id]),
          }
          let isNewConversation = !(this.$store.getters.getIsActiveConversationTutor(conversationObj.conversationId))
          if(isNewConversation){
            let tutorInfo = {
              id: user.userId,
              name: user.name,
              image: user.image,
            }
            this.$store.commit('ACTIVE_CONVERSATION_TUTOR',{tutorInfo,conversationId:conversationObj.conversationId})
          }

          let currentConversationObj = chatService.createActiveConversationObj(conversationObj)
          this.setActiveConversationObj(currentConversationObj);
          this.$router.push({name:routeNames.MessageCenter,params:{id:currentConversationObj.conversationId}})
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
    subjects() {
      if (this.tutorData.subjects) {
        return this.tutorData.subjects.join(', ');
      }
      return '';
    },
    showFirstName() {
      let maxChar = 5;
      let name = this.tutorData.name.split(' ')[0];
      if(name.length > maxChar) {
        return this.$t('resultTutor_message_me');
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
          display: flex;
          flex-direction: column;
          justify-content: space-between;
          .card-mobile-tutor-name {
            font-size: 14px;
          }
        }
        .user-rate {
            display: inline-flex;
            // margin-top: 6px;
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
      //   .price {
      //     display: flex;
      //     align-items: flex-end;
      //     flex: .5;
      //     // margin: 4px 0 1px 0;
      //     .price_oneline {
      //       display: flex;
      //       align-items: baseline;
      //       color: #5158af;

      //       &--count {
      //         font-size: 20px;
      //       }
      //     }
      //     .striked {
      //           margin: 0 0 0 auto;
      //           max-width: max-content;
      //           color: #a0a4be;
      //           font-size: 14px;
      //           text-decoration: line-through;
      //       }
      //  }
      //  .applyCoupon {
      //     color: #4c59ff;
      //     font-weight: 600;
      //     font-size: 12px;
      //     // margin-top: 6px;
      //   }
    }
    .card-mobile-center {
      margin: 10px 0;
      .giveMeEllipsis(2,20px);
    }
    .card-mobile-footer {
        display: inherit;
        .btn-chat {
          position: unset;
          font-weight: 600;
          &:before{
            position:unset;
          }
          .v-btn__content{
            .chat_icon_btn{
              align-self: flex-end;
            }
            :last-child {
              margin-bottom: 2px;
            }
          }
          text-transform: inherit;
          border-radius: 7.5px;
          .text_icon_btn {
            padding-left: 10px;
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