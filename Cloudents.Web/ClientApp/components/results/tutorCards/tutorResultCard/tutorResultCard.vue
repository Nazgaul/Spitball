<template>
    <router-link class="tutor-result-card-desktop pa-3 mb-3 row" @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}">

        <v-flex row class="user-details">
            <img :class="[isUserImage ? '' : 'tutor-no-img']" class="mr-3 user-image" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
            <div class="main-card justify-space-between">
                <h3 class="title font-weight-bold tutor-name text-truncate mb-1" v-html="$Ph('resultTutor_private_tutor', tutorData.name)"></h3>
                <h4 class="mb-4 text-truncate" v-if="isUniversity">{{university}}</h4>
                <div class="user-bio mb-5 overflow-hidden" v-html="ellipsizeTextBox(tutorData.bio)"></div>
                <div class="study-area mb-2" v-if="isSubjects">
                  <span class="font-weight-bold mr-2" v-language:inner="'resultTutor_study-area'"></span>
                  <span class="text-truncate">{{subjects}}</span>
                </div>
                <div class="courses" v-if="isCourses">
                  <span class="font-weight-bold mr-2" v-language:inner="'resultTutor_courses'"></span>
                  <span class="text-truncate">{{courses}}</span> 
                </div>
            </div>
        </v-flex>

        <v-divider vertical class="mx-3"></v-divider>

        <div class="user-rates">
            <div class="title price font-weight-bold mb-1">
              <template>
                  <span v-if="showStriked" class="title font-weight-bold">&#8362;{{discountedPrice}}</span>
                  <span class="title font-weight-bold" v-else>&#8362;{{tutorData.price}}</span>
              </template>
              <span class="caption">
                /<span v-language:inner="'resultTutor_hour'"></span>
                <div v-if="!showStriked" class="price-default-height"></div>
              </span>
            </div>
            <div class="striked" v-if="showStriked"> &#8362;{{tutorData.price}}</div>
            <div class="user-rank mt-3 mb-2 align-center">
              <user-rating :rating="tutorData.rating" :showRateNumber="false" />
              <div class="reviews" v-html="$Ph(`resultTutor_reviews_many`, reviewsPlaceHolder(tutorData.reviews))"></div>
            </div>
            <div class="classes-hours align-center mb-4 mt-1">
                <clock />
                <span class="ml-2 font-weight-bold caption">{{tutorData.lessons}}</span>
                <span class="ml-2 font-weight-bold caption" v-language:inner="'resultTutor_hours_completed'"></span>
            </div>
            <div class="send-btn">
                <v-btn class="btn-chat white--text text-truncate" round block color="#4452fc" @click.prevent="sendMessage(tutorData)">
                  <iconChat class="chat-icon mr-2" />
                  <div class="font-weight-bold text-truncate" v-html="$Ph('resultTutor_send_button', tutorData.name)" ></div>
                </v-btn>
            </div>
        </div>

    </router-link>
</template>

<script>
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import utilitiesService from "../../../../services/utilities/utilitiesService";
import analyticsService from "../../../../services/analytics.service";
import chatService from '../../../../services/chatService';
import { mapActions, mapGetters } from "vuex";
import { LanguageService } from "../../../../services/language/languageService.js";
import clock from './clock.svg';
import iconChat from '../tutorResultCardOther/icon-chat.svg';

export default {
  name: "tutorResultCard",
  components: {
    userRating,
    clock,
    iconChat
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
    fromLandingPage: {
      type: Boolean,
      default: false
    }
  },
  methods: {
    ...mapActions(["updateRequestDialog",'updateCurrTutor', 'openChatInterface','setActiveConversationObj']),

    loaded() {
      this.isLoaded = true;
    },
    tutorCardClicked(e) {
      if(this.fromLandingPage){
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_landing_page");
      }else{
          analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_page");
      };
    },
    onImageLoadError(event) {
      event.target.src = "./images/placeholder-profile.png";
    },
    reviewsPlaceHolder(reviews) {
      return reviews === 0 ? reviews.toString() : reviews
    },
    sendMessage(user) {
      if (this.accountUser == null) {
          analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:GUEST`);
          this.updateCurrTutor(user);
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
      let maxChars = 176;
      let showBlock = text.length > maxChars;
      let newText = showBlock ? text.slice(0, maxChars) + '...' : text;
      let hideText = showBlock ? `<span style="display:none">${text.slice(maxChars)}</span>` : '';
      let readMore = showBlock ? `<span class="read-more" style="${showBlock ? 'display: inline-block' : ''}">${LanguageService.getValueByKey('resultTutor_read_more')}</span>` : '';
      return `${newText} ${readMore} ${hideText}`;
    }
  },
  computed: {
    ...mapGetters(['accountUser']),

    courses() {
      if (this.tutorData.courses) {
        return `${LanguageService.getValueByKey("resultTutor_teaching")} ${this.tutorData.courses}`
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
    isSubjects() {
      return this.isTutorData && this.tutorData.subjects.length > 0 ? true : false;
    },
    isCourses() {
      return this.isTutorData && this.tutorData.courses.length > 0 ? true : false;
    },
    userImageUrl() {
      if (this.tutorData.image) {
        let size = [148, 182];
        return utilitiesService.proccessImageURL(
          this.tutorData.image,
          ...size,
          "crop"
        );
      } else {
        return "./images/placeholder-profile.png";
      }
    },
    showStriked() {
      let price = this.tutorData.price;
      return price > this.minimumPrice;
    },
    discountedPrice() {
      let price = this.tutorData.price;
      let discountedAmount = price - this.discountAmount;
      return discountedAmount >  this.minimumPrice ? discountedAmount : this.minimumPrice;
    },
    university() {
      return this.tutorData.university;
    },
    subjects() {
      return this.tutorData.subjects.toString();
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

@purple: #43425d;

  .tutor-result-card-desktop {
    border-radius: 4px;
    background: #fff;
    width: 100%;
    display: flex;
    h3, h4, .user-bio, .courses, .price, .classes-hours, .study-area {
      color: @purple;
    }
    .user-details {
      display: flex;
      .widthMinMax(600px);
      .main-card {
        .widthMinMax(400px);
        display: flex;  
        flex-direction: column;
        h4 {
          .heightMinMax(14px);
        }
        .user-bio {
          display: inline-block;
          word-wrap: break-word;
          .heightMinMax(48px);
          line-height: 1.2em;
          text-align: justify;
          .read-more {
            color: #4452fc;
          }
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
    .user-image {
      border-radius: 4px;
    }
    .tutor-no-img {
      width: 142px;
      height: auto;
    }
    
    .user-rates {
      display: flex;
      flex-direction: column;
      align-items: baseline;
      justify-content: space-between;
      flex: 1;
      min-width: inherit;
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
        .price-default-height {
          .heightMinMax(16px);
        }
      }
      .classes-hours {
        display: flex;
      }
      .user-rank {
        display: inline-flex;
        i{
          font-size: 20px !important;
        }
        .reviews {
          color: #4452fc;
        }
      }
      .send-btn {
        width: 100%;
        .btn-chat {
          max-width: 220px;
          min-width: 100%;
          text-transform: lowercase;
          .chat-icon {
            margin: 0 auto 0 0;
          }
          div {
            margin: 0 auto;
          }
        }
      }
    }
  }
</style>
