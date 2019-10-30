<template>
    <router-link class="tutor-result-card-desktop pa-3 row" @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}">

        <v-flex row class="user-details">
            <user-avatar-rect 
              :userName="tutorData.name" 
              :userImageUrl="tutorData.image" 
              class="mr-3 user-avatar-rect" 
              :userId="tutorData.userId" 
              :width="148" 
              :height="182" />
            <div class="main-card justify-space-between">
                <h3 class="font-weight-bold tutor-name text-truncate" v-html="$Ph('resultTutor_private_tutor', tutorData.name)"></h3>
                <h4 class="mb-1 text-truncate" :class="{'university-hidden': !university}">{{university}}</h4>
                <div class="user-bio-wrapper">
                  <div class="user-bio mb-4">{{tutorData.bio}}</div>
                  <!-- <div class="read-more" v-show="isOverflow" v-language:inner="'resultTutor_read_more'"></div> -->
                </div>
                <div class="study-area mb-2" :class="{'study-area-hidden': !isSubjects}">
                  <span class="mr-1" v-language:inner="'resultTutor_study-area'"></span>
                  <span class="text-truncate">{{subjects}}</span>
                </div>
                <div class="courses" v-if="isCourses">
                  <span class="mr-2" v-language:inner="'resultTutor_courses'"></span>
                  <span class="text-truncate">{{courses}}</span> 
                </div>
            </div>
        </v-flex>

        <v-divider vertical class="mx-3"></v-divider>

        <div class="user-rates">
            <div class="price font-weight-bold mb-1">
              <div class="user-rates-top">
                <div class="striked" v-if="tutorData.discountPrice">{{tutorData.price}}</div>
                <template>
                    <!-- <span class="tutor-card-currency">&#8362;</span> -->
                    <span v-if="tutorData.discountPrice" class="tutor-card-price font-weight-bold">{{tutorData.discountPrice}}</span>
                    <span class="tutor-card-price font-weight-bold" v-else>{{tutorData.price}}</span>
                </template>
                <span class="caption">
                  <span class="tutor-card-price-divider font-weight-bold">/</span>
                  <span class="tutor-card-price-divider font-weight-bold" v-language:inner="'resultTutor_hour'"></span>
                  <div v-if="!showStriked" class="price-default-height"></div>
                </span>
                <!-- <v-menu class="menu-area" lazy bottom left content-class="card-user-actions">
                    <v-btn :depressed="true" @click.prevent slot="activator" icon>
                    <v-icon>sbf-3-dot</v-icon>
                    </v-btn>
                    <v-list>
                    <v-list-tile v-show="item.isVisible" class="report-list-item" :disabled="item.isDisabled" v-for="(item, i) in actions" :key="i">
                        <v-list-tile-title style="cursor:pointer;" @click="item.action()">{{ item.title }}</v-list-tile-title>
                    </v-list-tile>
                    </v-list>
                </v-menu> -->
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
            
            <div class="classes-hours align-center mb-4 mt-1">
              <clock />
              <span class="font-weight-bold caption ml-2" v-if="tutorData.lessons > 0">{{tutorData.lessons}}</span>
              
              <template>
                <span class="font-weight-bold caption no-classes" v-language:inner="'resultTutor_no_hours_completed'" v-if="tutorData.lessons === 0"></span>
                <span class="font-weight-bold caption no-classes" v-language:inner="tutorData.lessons === 1 ? 'resultTutor_hour_completed' : 'resultTutor_hours_completed' " v-else></span>    
              </template>
            </div>                

            <div class="send-btn">
                <v-btn class="btn-chat white--text" depressed round block color="#4452fc" @click.prevent="sendMessage(tutorData)">
                  <iconChat class="chat-icon-btn" v-if="fromLandingPage" />
                  <div class="" v-html="$Ph('resultTutor_send_button', showFirstName)" ></div>
                </v-btn>
            </div>
        </div>

    </router-link>
</template>

<script>
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import analyticsService from "../../../../services/analytics.service";
import chatService from '../../../../services/chatService';
import { mapActions, mapGetters } from "vuex";
import { LanguageService } from "../../../../services/language/languageService.js";
import clock from './clock.svg';
import iconChat from '../tutorResultCardOther/icon-chat.svg';
import star from '../stars-copy.svg';
import userAvatarRect from '../../../helpers/UserAvatar/UserAvatarRect.vue';

export default {
  name: "tutorResultCard",
  components: {
    userRating,
    clock,
    star,
    iconChat,
    userAvatarRect
  },
  data() {
    return {
      minimumPrice: 55,
      discountAmount: 70,
      isMounted: false,
      actions: [
        {
          title: LanguageService.getValueByKey("questionCard_Report"),
          action: this.reportItem,
          isDisabled: !this.isDisabled,
          isVisible: true,
          icon: 'sbf-flag'
        }
      ],
    };
  },
  props: {
    tutorData: {},
    fromLandingPage: {
      type: Boolean,
      default: false
    }
  },
  methods: {
    ...mapActions(["updateRequestDialog",'updateCurrTutor', 'setTutorRequestAnalyticsOpenedFrom','openChatInterface','setActiveConversationObj']),

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
    ...mapGetters(['accountUser', 'getActivateTutorDiscounts']),

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
    showStriked() {
      if(!this.getActivateTutorDiscounts) return false;
      let price = this.tutorData.price;
      return price > this.minimumPrice;
    },
    discountedPrice() {
      let price = this.tutorData.price;
      let discountedAmount = price - this.discountAmount;
      return discountedAmount >  this.minimumPrice ? discountedAmount.toFixed(0) : this.minimumPrice.toFixed(0);
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
    isOverflow() {
      if(this.isMounted){
        let currentDiv = this.$el.querySelector('.user-bio')
        return currentDiv.scrollHeight > currentDiv.clientHeight || currentDiv.scrollWidth > currentDiv.clientWidth;
      }
      return false;
    },
    isDisabled() {
      // let isOwner = this.cardOwner;
      // let account = this.accountUser;
      // if (isOwner || !account ) {
      //     return true;
      // }
      return false;
    },
    cardOwner() {
      // let userAccount = this.accountUser;
      // if (userAccount && this.cardData.userId) {
      //     return userAccount.id === this.cardData.userId; // will work once API call will also return userId
      // }
      return false;
    },
  },
  mounted(){
    this.isMounted = true;
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

  @purple: #43425d;

  .tutor-result-card-desktop {
    .heightMinMax(214px);
    border-radius: 8px;
    background: #fff;
    width: 100%;
    display: flex;
    h3, h4, .user-bio, .courses, .price, .classes-hours, .study-area {
      color: @purple;
    }
    .user-details {
      width: 0;
      flex-grow: 5;
      display: flex;
      .main-card {
        min-width: 0;
        h3{
          // line-height: 1.1 !important;
        }
        display: flex;  
        flex-direction: column;
        .university-hidden {
          visibility: hidden;
        }
        .user-bio-wrapper {
          position: relative;
          .user-bio {
            .giveMeEllipsis(3, 20px);
          }
        }
        .study-area-hidden {
          visibility: hidden;
        }
        .courses {
          display: flex;
          white-space: nowrap;
        }
      }
    }
    div:nth-child(2) {
      flex-basis: auto;
    }
    .user-rates {
      display: flex;
      flex-direction: column;
      align-items: baseline;
      justify-content: space-between;
      flex: 2;
      .striked {
        max-width: max-content;
        position: relative;
        color: @colorBlackNew;
        &:after {
            content: "";
            width: 100%;
            border-bottom: solid 1px @colorBlackNew;
            position: absolute;
            left: 0;
            top: 50%;
            z-index: 1;
        }
      }
      .price {
        width: 100%;
        display: flex;
        justify-content: flex-end;
        margin-top: -8px;
        // padding-right: 26px;
        .user-rates-top {
          display: flex;
          align-items: baseline;
          margin-right: 10px;
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
        .no-classes {
          margin-left: 8px;
        }
      }
      .user-rank {
        margin-top: -12px;
        display: inline-flex;
        i{
          font-size: 20px !important;
        }
        .reviews {
          color: #4452fc;
        }
        .no-reviews {
          margin-left: 5px;
          color: #43425d;
        }
        .user-rank-star {
          width: 20px;
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
          text-transform: inherit;
          .v-btn__content {
            .chat-icon-btn{
              position: absolute;
              top: 0;
              left: 0px;
            }
            svg {
              width: 40px;
            }
          }
          .chat-icon {
            margin: 0 auto 0 0;
          }
        }
      }
    }
  }
</style>
