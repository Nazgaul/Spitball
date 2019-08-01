<template>
    <router-link class="tutor-result-card-desktop pa-3 mb-3 row" @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}">

        <v-flex row class="user-details">
            <img :class="[isUserImage ? '' : 'tutor-no-img']" class="mr-3 user-image" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
            <div class="main-card justify-space-between">
                <h3 class="title font-weight-bold tutor-name text-truncate" v-html="$Ph('resultTutor_private_tutor', tutorData.name)"></h3>
                <h4 class="mb-1 text-truncate" :class="{'university-hidden': !university}">{{university}}</h4>
                <div class="user-bio mb-4 overflow-hidden" :class="{'user-bio-hidden': !tutorData.bio}" v-html="ellipsizeTextBox(tutorData.bio)"></div>
                <div class="study-area mb-2" :class="{'study-area-hidden': !isSubjects}">
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
            <div class=" price font-weight-bold mb-1">
              <template>
                  <span v-if="showStriked" class="headline font-weight-bold">&#8362;{{discountedPrice}}</span>
                  <span class="headline font-weight-bold" v-else>&#8362;{{tutorData.price}}</span>
              </template>
              <span class="caption">
                /<span v-language:inner="'resultTutor_hour'"></span>
                <div v-if="!showStriked" class="price-default-height"></div>
              </span>
              <div class="striked" v-if="showStriked"> &#8362;{{tutorData.price}}</div>
            </div>

            <template>
              <div class="user-rank mt-3 mb-2 align-center" v-if="isReviews">
                <user-rating :rating="tutorData.rating" :showRateNumber="false" />
                <div class="reviews" v-html="$Ph(`resultTutor_reviews_many`, reviewsPlaceHolder(tutorData.reviews))"></div>
              </div>
              <div v-else class="user-rank mt-3 mb-2 align-center">
                <star/>
                <span class="no-reviews font-weight-bold caption" v-language:inner="'resultTutor_no_reviews'"></span>
              </div>
            </template>
            
            <div class="classes-hours align-center mb-4 mt-1">
              <clock />
              <span class="font-weight-bold caption ml-2" v-if="tutorData.lessons > 0">{{tutorData.lessons}}</span>
              
              <template>
                <span class="font-weight-bold caption no-classes" v-language:inner="'resultTutor_no_hours_completed'" v-if="tutorData.lessons === 0"></span>
                <span class="font-weight-bold caption no-classes" v-language:inner="'resultTutor_hours_completed'" v-else></span>    
              </template>
            </div>                

            <div class="send-btn">
                <v-btn class="btn-chat white--text text-truncate" round block color="#4452fc" @click.prevent="sendMessage(tutorData)">
                  <iconChat class="chat-icon-btn" />
                  <div class="font-weight-bold text-truncate" v-html="$Ph('resultTutor_send_button', showFirstName)" ></div>
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
import star from '../stars-copy.svg';
export default {
  name: "tutorResultCard",
  components: {
    userRating,
    clock,
    star,
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
    ...mapActions(["updateRequestDialog",'updateCurrTutor', 'setTutorRequestAnalyticsOpenedFrom','openChatInterface','setActiveConversationObj']),


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
      let maxChars = 176;
      let showBlock = text.length > maxChars;
      let newText = showBlock ? text.slice(0, maxChars) + '...' : text;
      let hideText = showBlock ? `<span style="display:none">${text.slice(maxChars)}</span>` : '';
      let readMore = showBlock ? `<span class="read-more" style="${showBlock ? 'display: inline-block;position:absolute' : ''}">${LanguageService.getValueByKey('resultTutor_read_more')}</span>` : '';
      return `${newText} ${readMore} ${hideText}`;
    }
  },
  computed: {
    ...mapGetters(['accountUser']),

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
        return "../../../images/placeholder-profile.png";
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
    },
    showFirstName() {
      return this.tutorData.name.split(' ')[0];
    },
    isReviews() {
      return this.tutorData.reviews > 0 ? true : false;
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

@purple: #43425d;

  .tutor-result-card-desktop {
    .heightMinMax(214px);
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
        .university-hidden {
          visibility: hidden;
        }
        .user-bio {
          display: inline-block;
          word-wrap: break-word;
          .heightMinMax(62px);
          line-height: 1.5em;
          .read-more {
            color: #4452fc;
          }
          &.user-bio-hidden {
            visibility: hidden;
          }
        }
        .tutor-name {
          overflow: unset !important;
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
        margin-left: 3px;
        display: flex;
        .no-classes {
          margin-left: 8px;
        }
      }
      .user-rank {
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
      }
      .send-btn {
        width: 100%;
        min-width: 100%;
        max-width: 0;
        .btn-chat {
          position: relative;
          margin: 0 auto;
          text-transform: inherit;
          .v-btn__content {
            .chat-icon-btn{
              position: absolute;
              top: 0;
              left: -10px;
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
