<template>
    <router-link class="tutor-result-card-desktop pa-4" @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}">

        <v-flex class="user-details">
            <!-- <user-avatar-rect 
              :userName="tutorData.name" 
              :userImageUrl="tutorData.image" 
              class="user-avatar-rect" 
              :userId="tutorData.userId"
              :width="148" 
              :height="182"
              :borderRadius="4"
            /> -->
            <userAvatarNew 
              :userName="tutorData.name" 
              :userImageUrl="tutorData.image" 
              class="user-avatar-rect" 
              :userId="tutorData.userId"
              :width="148" 
              :height="182"
              :borderRadius="4"
              :fontSize="24"
              :tile="true"
            />
            <div class="main-card">
                <h3 class="font-weight-bold text-truncate mb-8">{{tutorData.name}}</h3>
                <div class="user-bio-wrapper mb-4">
                  <div class="user-bio">{{tutorData.bio}}</div>
                </div>
                <div class="study-area mb-2 text-truncate" :class="{'study-area-hidden': !isSubjects}">
                  <span class="me-1 font-weight-bold" v-t="'resultTutor_study-area'"></span>
                  <span>{{subjects}}</span>
                </div>
                <div class="courses text-truncate" v-if="isCourses">
                  <span class="me-2 font-weight-bold" v-t="'resultTutor_courses'"></span>
                  <span>{{courses}}</span> 
                </div>
            </div>
        </v-flex>

        <v-divider vertical class="mx-4"></v-divider>

        <div class="user-rates">
            <div class="my-auto">
              <div class="user-rank align-center mb-5">
                <v-rating 
                  v-model="tutorData.rating"
                  color="#ffca54"
                  half-increments
                  background-color="#ffca54"
                  :length="isReviews ? 5 : 1"
                  :size="18" readonly
                />
                <span :class="{'reviews': isReviews,'no-reviews font-weight-bold': !isReviews}">{{$tc('resultTutor_review_one',tutorData.reviews)}}</span>
                
              </div>
              
              <div class="classes-hours align-center">
                <clock />
                <span class="font-weight-bold no-classes">{{ $tc('resultTutor_hour_completed', tutorData.lessons) }}</span>
              </div>
            </div>               

            <div class="send-btn">
                <v-btn class="btn-chat white--text" depressed rounded block color="#4452fc" @click.prevent="sendMessage(tutorData)">
                  <iconChat class="chat-icon-btn" v-if="fromLandingPage" />
                  <div>{{$t('resultTutor_send_button',[showFirstName])}}</div>
                </v-btn>
            </div>
        </div>

    </router-link>
</template>

<script>
import { mapActions, mapGetters } from "vuex";

import analyticsService from "../../../../services/analytics.service";
import chatService from '../../../../services/chatService';
import * as routeNames from '../../../../routes/routeNames.js'

import iconChat from '../icon-chat.svg';
import clock from './clock.svg';

export default {
  name: "tutorResultCard",
  components: {
    clock,
    iconChat,
  },
  props: {
    tutorData: {},
    fromLandingPage: {
      type: Boolean,
      default: false
    }
  },
  methods: {
    ...mapActions(['updateRequestDialog', 'updateCurrTutor', 'setTutorRequestAnalyticsOpenedFrom', 'setActiveConversationObj']),

    tutorCardClicked() {
      if(this.fromLandingPage){
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_landing_page");
      }else{
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_page");
      }
    },
    // reviewsPlaceHolder(reviews) {
    //   return reviews === 0 ? reviews.toString() : reviews
    // },
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
        return `${this.tutorData.courses.join(', ')}`;
      }
      return '';
    },
    isSubjects() {
      return !!(this.tutorData && this.tutorData.subjects.length > 0);
    },
    isCourses() {
      return !!(this.tutorData && this.tutorData.courses.length > 0);
    },
    subjects() {
      return this.tutorData.subjects.join(', ');
    },
    showFirstName() {
      let maxChar = 5;
      let name = this.tutorData.name.split(' ')[0];
      if(name.length > maxChar) {
        return this.$t('resultTutor_message_me');
      }
      return name;
    },
    isReviews() {
      return this.tutorData.reviews > 0;
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
        .user-bio-wrapper {
          min-height: 60px;
          .user-bio {
            .giveMeEllipsis(3, 20px);
          }
        }
        .study-area-hidden {
          visibility: hidden;
        }
        .courses {
          // margin-top: 2px;
          padding-top: 2px;
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
        color: #a0a4be;
        font-size: 14px;
        text-decoration: line-through;
        &.no-discount {
          min-height: 19px;
        }
      }
      // .price {
      //   width: 100%;
      //   display: flex;
      //   justify-content: space-between;
      //   margin-bottom: -20px;
        // .applyCoupon {
        //   color: #5a61ba;
        //   font-weight: 600;
        //   font-size: 12px;
        //   margin-top: 6px;
        // }
        // .user-rates-top {
        //   align-items: baseline;
        //   .tutor-card-currency {
        //     font-size: 16px;
        //     color:#5158af;
        //   }
        //   .tutor-card-price {
        //     font-size: 18px;
        //     color:#5158af;
        //   }
        //   .tutor-card-price-divider {
        //     font-size: 12px;
        //     color:#5158af;
        //   }
        //   .menu-area {
        //       margin-top: -12px;
        //       width: 21px;
        //     .v-btn__content {
        //       i {
        //         font-size: 16px;
        //         color: rgba(0, 0, 0, 0.25);
        //       }
        //     }
        //   }
        // }
      //   .price-default-height {
      //     .heightMinMax(16px);
      //   }
      // }
      .classes-hours {
        margin-left: 3px;
        display: flex;
        align-items: end;
        font-size: 12px;
        
        span {
          margin-left: 6px;
        }
        // .classes-hours_lesson{
        //   font-size: 12px;
        // }
        // &_lesson {
        //   margin-left: 6px;
        // }
        // .no-classes {
        //   font-size: 12px;
        //   margin-left: 6px;
        // }
      }
      .user-rank {
        display: flex;
        // padding-top: 35px;
        .reviews {
          font-size: 12px;
          color: #4452fc;
        }
        .no-reviews {
          margin-left: 5px;
          font-size: 12px;
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
          position: unset;
          margin: 0 auto;
          text-transform: initial;
          &:before{
            position:unset;
          }
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
