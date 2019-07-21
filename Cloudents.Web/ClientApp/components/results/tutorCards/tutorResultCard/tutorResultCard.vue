<template>
    <router-link @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}">
        <v-layout class="tutor-result-card-desktop pa-3 mb-3" row>
            <v-flex row class="mb-2 user-details">
                <img :class="[isUserImage ? '' : 'tutor-no-img']" class="mr-3 user-image" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
                <div class="main-card">
                    <h3 class="subheading font-weight-bold tutor-name text-truncate mb-1">{{tutorData.name}}</h3>
                    <h4 class="mb-4 text-truncate">אוניברסיטה בן גוריון</h4> <!-- university name needed -->
                    <div class="user-bio mb-5">{{tutorData.bio}}</div>
                    <div class="courses text-truncate">
                      <span>קורסים:</span> <!-- v-language:inner="'tutorCard-courses'" -->
                      <span>{{courses}}</span> 
                    </div>
                </div>
            </v-flex>
            <v-divider vertical class="mx-3"></v-divider>
            <div class="user-rates">
                <div class="title price font-weight-bold">
                  <span class="headline font-weight-bold">&#8362;{{tutorData.price}}</span>
                  <span class="caption">
                    <span>/</span>
                    <span>לשעה</span>
                  </span>
                </div>
                <div class="striked"> &#8362;{{discountedPrice}}</div>
                <div class="user-rank my-3 align-center">
                  <user-rating :rating="tutorData.rating" :showRateNumber="false" />
                  <div>reviews {{tutorData.reviews}}</div> <!-- v-language:inner="'tutorCard-reviews'" -->
                </div>
                <div class="classes-hours align-center mb-4">
                    <clock />
                    <span class="ml-2 font-weight-bold caption">50 שעות שיעור הושלמו</span> <!-- v-language:inner="'tutorCard-hours-completed'" -->
                </div>
                <v-btn class="btn-chat white--text" round block color="#4452fc" @click.prevent="">
                  <iconChat class="chat-icon"/>
                  <div>שלחו לעידן הודעה</div>
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
    }
  },
  computed: {
    ...mapGetters(['accountUser']),
    courses(){
      if (this.tutorData.courses) {
        return `${LanguageService.getValueByKey("resultTutor_teaching")} ${this.tutorData.courses}`
      }
      return '';
    },
    tutorReviews() {
      return this.tutorData.reviewsCount || this.tutorData.reviews;
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
      // console.log(this.tutorData.price)
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
          max-height: 3.6em;
          min-height: 3.6em;
          line-height: 1.2em;
          text-align: justify;
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
      height: 182px;
    }
    .courses {
      display: flex;
      white-space: nowrap;
      // margin-top: 88px;
    }
    .user-rates {
      flex: 1;
      .striked {
        max-width: fit-content;
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
      .user-rank, .classes-hours {
        display: flex;
      }
      .user-rank {
        i{
          font-size: 20px;
        }
      }
      .btn-chat {
        .chat-icon {
          position: absolute;
          left: 0;
        }
      }
    }
  }
</style>
<!--<style lang="less" src="./tutorResultCard.less"></style>-->
