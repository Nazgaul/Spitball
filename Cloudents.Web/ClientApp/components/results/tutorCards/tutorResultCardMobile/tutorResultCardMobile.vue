<template>
  <router-link @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId,name:tutorData.name}}">
    <v-layout class="tutor-result-card-mobile pa-2 ma-2 pr-4 column">
        <div class="card-mobile-header mb-3">
            <img :class="[isUserImage ? '' : 'tutor-no-img']" class="mr-3 user-image" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
            <div>
                <h3 class="text-truncate mb-2 subheading font-weight-bold">{{tutorData.name}}</h3>
                <div class="user-rate align-center mb-2">
                    <user-rating :rating="tutorData.rating" :showRateNumber="false" class="mr-2" />
                    <span class="reviews" v-html="$Ph(`resultTutor_reviews_many`, reviewsPlaceHolder(tutorData.reviewsCount || tutorData.reviews))"></span>
                </div>
                <h4 class="text-truncate mb-1 font-weight-light">אוניברסיטה בן גוריון</h4> <!-- university name needed -->
                <div class="courses text-truncate">
                    <span class="font-weight-bold mr-2" v-language:inner="'resultTutor_courses'"></span>
                    <span class="text-truncate">{{courses}}</span> 
                </div>
            </div>
        </div>
        <div class="card-mobile-center mb-4 subheading">
            {{tutorData.bio}}
        </div>
        <div class="card-mobile-footer">
            <v-btn class="btn-chat white--text text-truncate" round block color="#4452fc" @click.stop="">
                  <iconChat class="chat-icon" />
                  <div class="font-weight-bold text-truncate" v-html="$Ph('resultTutor_send_button', tutorData.name)"></div>
            </v-btn>
            <div class="price ml-4 align-center" :class="{'mt-3': !showStriked}">
                <div class="striked" v-if="showStriked"> &#8362;{{tutorData.price}}</div>
                <span v-if="showStriked">
                    <span class="title font-weight-bold">&#8362;{{discountedPrice}}</span>
                </span>
                <span v-else>
                    <span class="title font-weight-bold">&#8362;{{tutorData.price}}</span>
                </span>
                <span class="caption">
                  <span>/</span>
                  <span v-language:inner="'resultTutor_hour'"></span>
                </span>
            </div>
        </div>
    </v-layout>
  </router-link>
</template>

<script>
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import { LanguageService } from "../../../../services/language/languageService.js";

import utilitiesService from "../../../../services/utilities/utilitiesService";
import analyticsService from "../../../../services/analytics.service";
import { mapActions, mapGetters } from "vuex";
import commentSVG from './commentSVG.svg';
import iconChat from '../tutorResultCardOther/icon-chat.svg';

export default {
  name: "tutorCard",
  components: {
    userRating,
    commentSVG,
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
    ...mapActions(["updateRequestDialog",'updateCurrTutor']),
    loaded() {
      this.isLoaded = true;
    },
    tutorCardClicked() {
        if(this.fromLandingPage){
            analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_landing_page");
        }else{
            analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_page");
        }
        this.$router.push({
          name: "profile",
          params: { id: this.tutorData.userId, name: this.tutorData.name }
        });
    },
    openRequestDialog(ev ,tutorData) {
      let userId = !!this.accountUser ? this.accountUser.id : 'GUEST';
      if(this.fromLandingPage){
           analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_landing_page', `userId:${userId}`);
      }else{
           analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_tutor_page', `userId:${userId}`);
      };
      ev.stopImmediatePropagation()
      this.updateCurrTutor(tutorData)
      this.updateRequestDialog(true);
    },
    onImageLoadError(event) {
      event.target.src = "./images/placeholder-profile.png";
    },
    reviewsPlaceHolder(reviews) {
      return reviews === 0 ? reviews.toString() : reviews
    }
  },
  computed: {
    ...mapGetters(['accountUser']),
    courses(){
      let query = this.$route.query.term
      if(query) {
        return `${LanguageService.getValueByKey("resultTutor_teaching")}${query}`
      } else {
        return `${this.tutorData.courses}`
      }
    },
    userImageUrl() {
      if (this.tutorData.image) {
        let size = [67, 87];
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
      return discountedAmount > this.minimumPrice
        ? discountedAmount.toFixed(0)
        : this.minimumPrice;
    },
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
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
@purple: #43425d;

.tutor-result-card-mobile {
    border-radius: 4px;
    background: #fff;

    h3, h4, .courses, .card-mobile-center, .price {
        color: @purple;
    }

    .card-mobile-header {
        display: flex;
        .user-image, .tutor-no-img {
            border-radius: 4px;
        }
        .tutor-no-img {
            width: 67px;
            height: 87px;
        }
        .user-rate {
            display: inline-flex;
            i {
                font-size: 16px !important;
            }
            .reviews {
              color: #4452fc;
            }
        }
        .courses {
          max-width: 200px;
          min-width: auto;
        }
    }

    .card-mobile-center {
      .giveEllipsisUpdated(14px, 1.35, 2, 90px);
    }

    .card-mobile-footer {
        display: inherit;
        .btn-chat {
          border-radius: 7.5px;
          .chat-icon {
            margin: 0 auto 0 0;
          }
          div {
            margin: 0 auto 0 0;
          }
        }
        .price {
          .striked {
              max-width: max-content;
              position: relative;
              color: @colorBlackNew;
              font-size: 14px;
              font-weight: 100;
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
          .main-price {
            font-size: 22px;
          }
       }
    }
}
</style>