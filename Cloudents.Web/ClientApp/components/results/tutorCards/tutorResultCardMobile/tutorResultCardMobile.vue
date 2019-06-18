<template>
  <router-link
    @click.native.prevent="tutorCardClicked"
    :to="{name: 'profile', params: {id: tutorData.userId,name:tutorData.name}}">
    
    <v-card
      class="tutor-card-wrap pa-12 cursor-pointer"
      :class="{'list-tutor-card elevation-0': isInTutorList}"
    >
      <div v-if="isInTutorList">
        <v-layout>
          <v-flex class="image-wrap d-flex" shrink>
            <div class="tutor-image-loader" v-if="!isLoaded">
              <v-progress-circular indeterminate v-bind:size="50"></v-progress-circular>
            </div>
            <img
              class="tutor-image"
              v-show="isLoaded"
              @error="onImageLoadError"
              @load="loaded"
              :src="userImageUrl"
              :alt="tutorData.name"
              >
          </v-flex>
          <v-flex>
            <v-layout align-start row wrap fill-height>
              <v-flex xs12 grow>
                <v-layout row justify-space-between align-baseline class="top-section">
                  <v-flex grow>
                    <span class="tutor-name" v-line-clamp:18="1">{{tutorData.name}}</span>
                  </v-flex>
                  <v-flex shrink>
                    <!--keep this inline style to fix price / hour spacing-->
                    <span
                      class="font-weight-bold pricing pr-1"
                      v-if="showStriked"
                      style="display: table;"
                    >
                      <span class="subheading font-weight-bold">₪{{discountedPrice}} </span>
                      <span class="font-weight-regular caption" v-language:inner>resultTutor_hour</span>
                    </span>

                    <span class="font-weight-bold pricing pr-1" v-else>
                      <!--keep this wraper to fix price / hour spacing-->
                      <div class="d-inline-flex align-baseline">
                        <span>
                        <span class="subheading font-weight-bold">₪{{tutorData.price}} </span>
                        <span class="pricing caption" v-language:inner>resultTutor_hour</span>

                        </span>
                      </div>
                    </span>
                    <v-flex shrink v-if="showStriked" class="strike-through">
                      <span class="striked-price">₪{{tutorData.price}}</span>
                      <span class="pricing striked-hour"> /
                        <span v-language:inner>resultTutor_hour</span>
                      </span>
                    </v-flex>
                  </v-flex>
                </v-layout>
              </v-flex>
              <v-flex class="bottom-section">
                <userRating
                  class="rating-holder"
                  :rating="tutorData.rating"
                  :starColor="'#ffca54'"
                  :rateNumColor="'#43425D'"
                  :size="isInTutorList ? '16' : '20'"
                  :rate-num-color="'#43425D'"
                ></userRating>

                <v-flex shrink class="tutor-courses text-truncate">
                  <span class="blue-text courses-text">{{tutorData.courses}}</span>
                </v-flex>
              </v-flex>
            </v-layout>
          </v-flex>
        </v-layout>
      </div>
<!-- the diff!!!!!!!!!!!!!!!! -->
      <div v-else style="display: flex;flex-direction: column;align-items: center;">
        <v-layout align-start style="width: 100%;">
          <v-flex class="image-wrap d-flex" shrink>
            <div class="tutor-image-loader" v-if="!isLoaded">
              <v-progress-circular indeterminate v-bind:size="50"></v-progress-circular>
            </div>
            <img class="tutor-image" v-show="isLoaded" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
          </v-flex>
          <v-flex>
            <v-layout align-start row wrap fill-height>
              <v-flex xs12 grow>
                <v-layout row justify-space-between align-baseline class="top-section">

                  <v-flex grow>
                    <span class="tutor-name" v-line-clamp:18="1">{{tutorData.name}}</span>
                    <userRating class="rating-holder mt-2" :rating="tutorData.rating" :showRateNumber="false" :size="isInTutorList ? '16' : '20'"/>
                  </v-flex>

                  <v-flex shrink>
                    <div v-if="showStriked" style="display:flex; flex-direction: column">
                      <span class="font-weight-bold pricing ">
                        <span class="font-weight-regular caption px-1 striked">₪{{tutorData.price}}</span>
                        <span class="title font-weight-bold" >₪{{discountedPrice}}</span>
                      </span>
                    </div >
                    <div v-else>
                    <span class="font-weight-bold pricing " >
                      <div class="d-inline-flex align-baseline">
                        <span class="title font-weight-bold" >₪{{tutorData.price}}</span>
                      </div>
                    </span>

                    </div>
                    <v-flex shrink>
                      <span class="hour" style="display: flex; justify-content: flex-end;">
                        <span v-language:inner>resultTutor_hour</span>
                      </span>
                    </v-flex>
                  </v-flex>
                </v-layout>
              </v-flex>
              <v-flex class="bottom-section">
                <p>{{tutorData.bio}}</p>
              </v-flex>
            </v-layout>
          </v-flex>
        </v-layout>
          <v-flex shrink class="tutor-courses text-truncate mt-3">
            <span class="blue-text courses-text">{{tutorData.courses}}</span>
          </v-flex>
          <div @click.prevent="openRequestDialog($event,tutorData)" v-if="!isInTutorList" class="my-3">
            <div class="btn-section"><commentSVG class="mr-2"/><span v-language:inner="'resultTutor_contact_me'"></span></div>
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
        // let size = this.isInTutorList ? [56, 64] : [76, 96];
        //enlarged due to striked price addition, (didn't fit);
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

    .strike-through {
      //text-decoration: line-through; //will not work cause of different font sizes
      position: relative;
      color: @colorBlackNew;
      display: table;
      .striked-price {
        font-size: 12px;
      }
      .striked-hour {
        font-size: 10px;
      }
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
    &.pa-12 {
      padding: 16px 12px;
    }
    &:first-child {
      border-radius: 4px 4px 0 0;
    }
    &:last-child {
      border-radius: 0 0 4px 4px;
    }
    .top-section {
    }
    .tutor-name {
      font-size: 13px;
      font-weight: 700;
      line-height: 18px;
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
  }
  // END styles for card rendered inside tutor list only

  &.pa-12 {
    padding: 12px;
  }
  .user-rating-val {
    font-weight: bold;
  }
  .tutor-name {
    color: @profileTextColor;
    line-height: 20px;
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
        width: 76px;
        height: 96px;
        display: flex;
        justify-content: center;
        align-items: center;
      }
    }
  }
  .tutor-image {
    border-radius: 4px;
    width: 96px;
  }
  .rating-number {
    font-weight: bold;
  }
  .bottom-section {
    justify-self: flex-end;
    margin-top: auto;
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
    }
  .strike-through {
    //text-decoration: line-through; //will not work cause of different font sizes
    font-size: 14px;
    position: relative;
    display: table;
    &:after {
      content: "";
      width: 100%;
      border-bottom: solid 1px @textColor;
      position: absolute;
      left: 0;
      top: 50%;
      z-index: 1;
    }
    .striked-hour {
      font-size: 12px;
    }
  }
  .small-text {
    font-size: 10px;
  }
  .tutor-courses {
    color: @colorBlue;
    max-width: 0;
    min-width: 100%;
    font-size: 14px;
    min-height: 19px; //keep it to prevent rating stars shift
  }
  .courses-text {
    line-height: 1;
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
