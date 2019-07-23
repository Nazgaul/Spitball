<template>
    <router-link @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}">
        <v-layout class="tutor-result-card-desktop pa-3 mb-3" row>
            <v-flex row class="user-details">
                <img :class="[isUserImage ? '' : 'tutor-no-img']" class="mr-3 user-image" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
                <div class="main-card">
                    <h3 class="title font-weight-bold tutor-name text-truncate mb-1">{{tutorData.name}}</h3>
                    <h4 class="mb-4 text-truncate">אוניברסיטה בן גוריון</h4> <!-- university name needed -->
                    <div class="user-bio mb-5">{{tutorData.bio}}</div>
                    <div class="study-area mb-2" v-if="false">
                      <span class="font-weight-bold mr-2" v-language:inner="'resultTutor_study-area'"></span>
                      <span class="text-truncate"></span> 
                    </div>
                    <div class="courses">
                      <span class="font-weight-bold mr-2" v-language:inner="'resultTutor_courses'"></span>
                      <span class="text-truncate">{{courses}}</span> 
                    </div>
                </div>
            </v-flex>
            <v-divider vertical class="mx-3"></v-divider>
            <div class="user-rates">
                <div class="title price font-weight-bold">
                  <span class="headline font-weight-bold">&#8362;{{tutorData.price}}</span>
                  <span class="caption">
                    <span v-language:inner="'resultTutor_hour'"></span>
                  </span>
                </div>
                <div class="striked"> &#8362;{{discountedPrice}}</div>
                <div class="user-rank mt-3 mb-2 align-center">
                  <user-rating :rating="tutorData.rating" :showRateNumber="false" />
                  <div v-html="$Ph(`resultTutor_reviews_many`, reviewsPlaceHolder(tutorData.reviewsCount || tutorData.reviews))"></div>
                </div>
                <div class="classes-hours align-center mb-4 mt-2">
                    <clock />
                    <span class="ml-2 font-weight-bold caption" v-html="$Ph(`resultTutor_hours_completed`, '32')"></span>
                </div>
                <v-btn class="btn-chat white--text text-truncate" round block color="#4452fc" @click.stop="">
                  <iconChat class="chat-icon mr-2" />
                  <div class="font-weight-bold text-truncate" v-html="$Ph('resultTutor_send_button', tutorData.name)"></div>
                </v-btn>
            </div>
        </v-layout>
    </router-link>
</template>

<script>
import userRank from "../../../helpers/UserRank/UserRank.vue";
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import utilitiesService from "../../../../services/utilities/utilitiesService";
import analyticsService from "../../../../services/analytics.service";
// import tutorRequest from "../../../tutorRequest/tutorRequest.vue";
// import sbDialog from "../../../wrappers/sb-dialog/sb-dialog.vue";
import { mapActions, mapGetters } from "vuex";
import { LanguageService } from "../../../../services/language/languageService.js";
import clock from './clock.svg';
import iconChat from '../tutorResultCardOther/icon-chat.svg';

export default {
  name: "tutorResultCard",
  components: {
    userRank,
    userRating,
    userAvatar,
    // tutorRequest,
    // sbDialog,
    clock,
    iconChat
  },
  data() {
    return {
      isLoaded: false,
      handleDialogProfile: false,
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
    ...mapActions(["updateRequestDialog",'updateCurrTutor']),
    loaded() {
      this.isLoaded = true;
    },
    tutorCardClicked(e) {
        if(this.fromLandingPage){
            analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_landing_page");
            
          
        }else{
            analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_page");
            
          };
        
        this.$router.push({
            name: "profile",
            params: { id: this.tutorData.userId, name: this.tutorData.name }});
    },
    openRequestDialog(ev) {
      let userId = !!this.accountUser ? this.accountUser.id : 'GUEST';
      if(this.fromLandingPage){
           analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_landing_page', `userId:${userId}`);
      }else{
           analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_tutor_page', `userId:${userId}`);
      };
      ev.stopImmediatePropagation()
      this.updateCurrTutor(this.tutorData)
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
      return discountedAmount >  this.minimumPrice ? discountedAmount.toFixed(2) : this.minimumPrice.toFixed(2);
    },
    buttonText() {
      return "resultTutor_contact_me";
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
    h3, h4, .user-bio, .courses, .price, .classes-hours {
      color: @purple;
    }
    .user-details {
      display: flex;
      flex: 3;
      .main-card {
        min-width: 400px;
        max-width: 400px;
        .user-bio {
          position: relative;
          display: inline-block;
          word-wrap: break-word;
          overflow: hidden;
          max-height: 0;
          min-height: 48px;
          line-height: 1.2em;
          text-align: justify;
        }
        .study-area {
          color: @purple;
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
      flex: 1;
      min-width: fit-content;
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
      .user-rank, {
        display: inline-flex;
      }
      .classes-hours {
        display: flex;
      }
      .user-rank {
        i{
          font-size: 20px !important;
        }
      }
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
</style>
<!--<style lang="less" src="./tutorResultCard.less"></style>-->
