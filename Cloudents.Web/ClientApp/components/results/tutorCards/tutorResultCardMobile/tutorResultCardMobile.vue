<template>
  <router-link class="tutor-result-card-mobile pa-2 ma-2 pr-4 justify-space-between" @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId,name:tutorData.name}}">
      <div class="card-mobile-header mb-4">
        <div v-if="!isLoaded" class="mr-2 user-image tutor-card-loader">
              <v-progress-circular indeterminate v-bind:size="50"></v-progress-circular>
            </div>
          <img v-show="isLoaded" class="mr-2 user-image" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
          <div>
              <h3 class="text-truncate subheading font-weight-bold mb-2" v-html="$Ph('resultTutor_private_tutor', tutorData.name)"></h3>
              
              <template>
                  <div class="user-rate align-center" v-if="tutorData.reviews > 0">
                    <user-rating :rating="tutorData.rating" :showRateNumber="false" :size="'18'" class="mr-2" />
                    <span class="reviews" v-html="$Ph(`resultTutor_reviews_many`, reviewsPlaceHolder(tutorData.reviews))"></span>
                  </div>
                  <div class="user-rate align-center" v-else>
                    <star class="mr-2" />
                    <span class="reviews" v-html="$Ph(`resultTutor_no_reviews`, reviewsPlaceHolder(tutorData.reviews))"></span>
                  </div>
              </template>

              <template>
                  <h4 class="text-truncate mb-1 font-weight-light university" v-if="isUniversity">{{university}}</h4>
                  <h4 class="text-truncate mb-1 font-weight-light university" v-else></h4>
              </template>
              
              <div class="courses text-truncate">
                  <span class="font-weight-bold mr-1" v-language:inner="'resultTutor_courses'"></span>
                  <span class="text-truncate">{{courses}}</span>
              </div>
          </div>
      </div>

      <div class="card-mobile-center" v-html="ellipsizeTextBox(tutorData.bio)">{{tutorData.bio}}</div>

      <div class="card-mobile-footer mt-2">
          <v-btn class="btn-chat white--text text-truncate my-0" round block color="#4452fc" @click.prevent.stop="sendMessage(tutorData)">
                <iconChat class="chat-icon-btn"/>
                <div class="font-weight-bold text-truncate" v-html="$Ph('resultTutor_send_button', showFirstName)"></div>
          </v-btn>
          <div class="price ml-3 align-center" >
              <div class="striked" v-if="showStriked"> &#8362;{{tutorData.price}}</div>
              <template>
                <span v-if="showStriked" class="title font-weight-bold">&#8362;{{discountedPrice}}</span>
                <span v-else class="title font-weight-bold">&#8362;{{tutorData.price}}</span>
              </template>
              <span class="caption" v-language:inner="'resultTutor_hour'"></span>
          </div>
      </div>

  </router-link>
</template>

<script>
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import { LanguageService } from "../../../../services/language/languageService.js";
import chatService from '../../../../services/chatService';
import utilitiesService from "../../../../services/utilities/utilitiesService";
import analyticsService from "../../../../services/analytics.service";
import { mapActions, mapGetters } from "vuex";
import commentSVG from './commentSVG.svg';
import iconChat from '../tutorResultCardOther/icon-chat.svg';
import star from '../stars-copy.svg';

export default {
  name: "tutorCard",
  components: {
    userRating,
    commentSVG,
    iconChat,
    star
  },
  data() {
    return {
      isLoaded: false,
      minimumPrice: 55,
      discountAmount: 70
    };
  },
  props: {
    tutorData: {},
    isInTutorList: {
      type: Boolean,
      default: false
    },
    fromLandingPage: {
      type: Boolean,
      default: false
    }
  },
  methods: {
    ...mapActions(["updateRequestDialog",'updateCurrTutor', 'setTutorRequestAnalyticsOpenedFrom','openChatInterface','setActiveConversationObj']),


    loaded() {
      this.isLoaded = true;
    },
    tutorCardClicked() {
      if(this.fromLandingPage){
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_landing_page");
      }else{
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_page");
      }
    },
    onImageLoadError(event) {
      event.target.src = "../../../images/placeholder-profile.png";
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
    },
    ellipsizeTextBox(text) {
      let maxChars = 110;
      let showBlock = text.length > maxChars;
      let newText = showBlock ? text.slice(0, maxChars) + '...' : text;
      let hideText = showBlock ? `<span style="display:none">${text.slice(maxChars)}</span>` : '';
      return `${newText} ${hideText}`;
    }
  },
  computed: {
    ...mapGetters(['accountUser', 'getActivateTutorDiscounts']),
    userImageUrl() {
      if (this.tutorData.image) {
        let size = [67, 87];
        return utilitiesService.proccessImageURL(
          this.tutorData.image,
          ...size,
          "crop"
        );
      } else {
        return "../../../images/placeholder-profile.png";
      }
    },
    showStriked() {
      if(!this.getActivateTutorDiscounts) return false;
      let price = this.tutorData.price;
      return price > this.minimumPrice;
    },
    discountedPrice() {
      let price = this.tutorData.price;
      let discountedAmount = price - this.discountAmount;
      return discountedAmount > this.minimumPrice
        ? discountedAmount.toFixed(0)
        : this.minimumPrice;
    },
    courses() {
      if (this.tutorData.courses) {
        return `${this.tutorData.courses}`
      }
      return '';
    },
    isTutorData() {
      return this.tutorData ? true : false;
    },
    isUserImage() {
      return this.isTutorData && this.tutorData.image ? true : false;
    },
    isUniversity() {
      return (this.tutorData && this.tutorData.university) ? true : false;
    },
    university() {
      return this.tutorData.university;
    },
    showFirstName() {
      let maxChar = 5;
      let name = this.tutorData.name.split(' ')[0];
      if(name.length > maxChar) {
        return LanguageService.getValueByKey('resultTutor_message_me');
      }
      return name;
    },
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
@purple: #43425d;

.tutor-result-card-mobile {
    border-radius: 4px;
    background: #fff;
    display: flex;
    flex-direction: column;
    h3, h4, .courses, .card-mobile-center, .price {
        color: @purple;
    }
    .card-mobile-header {
        display: flex;
        .tutor-card-loader{
          display: flex;
          justify-content: center;
          align-items: center;
        }
        .user-image{
            border-radius: 4px;
            width: 67px;
            height: 95px;
        }
        .user-rate {
            display: inline-flex;
            .reviews {
                font-size: 12px;
                font-weight: bold;
                letter-spacing: normal;
                color: #43425d;
            }
        }
        .courses {
          .widthMinMax(200px);
        }
        .university {
          line-height: 30px;
          .heightMinMax(23px);
        }
    }
    .card-mobile-center {
      margin-bottom: 20px;
      .giveEllipsisUpdated(14px, 1.38, 2, 90px);
      // .heightMinMax(34px);
      .read-more {
        position: absolute;
        bottom: 68px;
        color: #4452fc;
      }
    }

    .card-mobile-footer {
        display: inherit;
        .btn-chat {
          .v-btn__content{
            .chat-icon-btn{
              position: absolute;
              top: 0;
              left: 10px;
            }
          }
          position: relative;
          .widthMinMax(220px);
          text-transform: inherit;
          border-radius: 7.5px;
          div {
            div {
              padding-left: 10px;
            }
          }
        }
        .price {
          align-self: flex-end;
          display: grid;
          .striked {
              max-width: max-content;
              position: relative;
              color: @colorBlackNew;
              font-size: 14px;
              font-weight: normal;
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
       }
    }
}
</style>