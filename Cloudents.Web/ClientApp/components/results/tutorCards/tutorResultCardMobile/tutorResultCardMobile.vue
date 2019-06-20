<template>
  <router-link @click.native.prevent="tutorCardClicked"
               :to="{name: 'profile', params: {id: tutorData.userId,name:tutorData.name}}">
    
    <v-card class="tutor-card-wrap pa-12" :class="{'list-tutor-card elevation-0': isInTutorList}">
      <div class="tutor-card-flex">
        <v-layout class="tutor-card-flex-width" >
          <v-flex class="image-wrap d-flex" shrink>
            <div class="tutor-image-loader" v-if="!isLoaded">
              <v-progress-circular indeterminate v-bind:size="50"></v-progress-circular>
            </div>
            <img class="tutor-image" v-show="isLoaded" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
          </v-flex>
          <v-flex>
            <v-layout align-start row wrap fill-height>
              <v-flex xs12>
                <v-layout row justify-space-between align-baseline>

                  <v-flex grow>
                    <span class="tutor-name">{{tutorData.name}}</span>
                    <userRating class="rating-holder mt-2" :rating="tutorData.rating" :showRateNumber="false" :size="isInTutorList ? '16' : '20'"/>
                  </v-flex>

                  <v-flex shrink>
                    <div v-if="showStriked" class="pricing">
                        <span class="caption px-1 striked">₪{{tutorData.price}}</span>
                        <span class="title font-weight-bold">₪{{discountedPrice}}</span>
                    </div>
                    <div v-else class="title font-weight-bold pricing">₪{{tutorData.price}}</div>
                    <v-flex shrink class="hour" v-language:inner="'resultTutor_hour'"/>
                  </v-flex>
                </v-layout>
              </v-flex>
              <v-flex class="bottom-section">{{tutorData.bio}}</v-flex>
            </v-layout>
          </v-flex>
        </v-layout>

          <v-flex shrink class="tutor-courses text-truncate mt-3 blue-text">{{tutorData.courses}}</v-flex>
          <div @click.prevent="openRequestDialog($event,tutorData)" v-if="!isInTutorList" class="my-3 btn-section">
              <commentSVG class="mr-2"/><span v-language:inner="'resultTutor_contact_me'"/>
          </div>
      </div>
    </v-card>
  </router-link>
</template>

<script>
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import utilitiesService from "../../../../services/utilities/utilitiesService";
import analyticsService from "../../../../services/analytics.service";
import { mapActions, mapGetters } from "vuex";
import commentSVG from './commentSVG.svg'

export default {
  name: "tutorCard",
  components: {
    userRating,
    commentSVG
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
      ev.stopImmediatePropagation()
      this.updateCurrTutor(tutorData.name)
      this.updateRequestDialog(true);
    },
    onImageLoadError(event) {
      event.target.src = "./images/placeholder-profile.png";
    }
  },

  computed: {
    userImageUrl() {
      if (this.tutorData.image) {
        let size = [76, 96];
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
        ? discountedAmount
        : this.minimumPrice;
    }
  }
};
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
.tutor-card-wrap {
  border-radius: 4px;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.13);
  min-width: 304px;
  margin-bottom: 8px;
  // START styles for card rendered inside tutor list only
  &.list-tutor-card {
    margin-bottom: 4px;
    border-radius: 0;
      &.pa-12 {
        padding: 16px 12px;
      }
      &:first-child {
        border-radius: 4px 4px 0 0;
      }
      &:last-child {
        border-radius: 0 0 4px 4px;
      }
      .tutor-name {
        .giveMeEllipsis(1,1.5);
        font-size: 13px;
        font-weight: 700;
        max-width: 120px;
      }
      .rating-holder {
        margin-bottom: 0;
      }
      .tutor-courses {
        font-size: 12px;
      }
      .tutor-image {
        width: 76px;
        height: 96px;
      }
      .bottom-section {
        .giveMeEllipsis(2,1.7);
      }
  }
  // END styles for card rendered inside tutor list only
  .tutor-card-flex{
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  .tutor-card-flex-width{
    width: 100%;
  }
  &.pa-12 {
    padding: 12px;
  }
  .tutor-name {
    .giveMeEllipsis(1,1.5);
    color: @profileTextColor;
    word-break: break-all;
    font-size: 16px;
    font-weight: bold;
  }
  .flex {
    &.image-wrap {
      margin-right: 12px;
      display: flex;
      justify-content: center;
      align-items: center;
      color: #5d62fd;
      .tutor-image-loader {
        width: 96px;
        display: flex;
        justify-content: center;
        align-items: center;
      }
    }
  }
  .tutor-image {
    border-radius: 4px;
    width: 96px;
    height: 120px
  }
  .bottom-section {
    justify-self: flex-end;
    margin-top: auto;
    .giveMeEllipsis(3,1.7);
  }
  .rating-holder {
    margin-bottom: 8px;
  }
  .pricing {
    font-family: @fontOpenSans;
    color: @profileTextColor;
    font-size: 18px;
    line-height: 16px;
  }
  .striked{
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
  .hour{
    font-size: 12px;
    color: #43425d;
    display: flex; justify-content: flex-end
  }
  .tutor-courses {
    color: @colorBlue;
    max-width: 0;
    min-width: 100%;
    font-size: 14px;
    min-height: 19px; //keep it to prevent rating stars shift
  }
  .btn-section{
    border-radius: 4px;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
    background-color: #facb57;
    padding: 11px 64px;
    font-size: 14px;
    font-weight: bold; 
    display: flex;
    align-items: center;
  }
}
</style>
