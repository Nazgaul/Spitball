<template>
  <div class="tutor-card-wrap-desk cursor-pointer">
    <router-link event="" @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId,name:tutorData.name}}">
      <v-layout>
        <div class="section-tutor-info">
          <v-layout>
            <v-flex class="image-wrap mr-3" shrink>
              <div v-if="!isLoaded">
                <v-progress-circular indeterminate v-bind:size="50"></v-progress-circular>
              </div>
              <img class="tutor-image" v-show="isLoaded" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
            </v-flex>
            <v-flex>
              <v-layout align-start justify-space-between column fill-height>
                <v-flex shrink class="pb-3">
                  <span class="tutor-name font-weight-bold" v-line-clamp:18="1">{{tutorData.name}}</span>
                </v-flex>
                <v-flex grow>
                  <span class="tutor-about subheading" v-line-clamp:22="2">{{tutorData.bio}}</span>
                </v-flex>
                <v-flex shrink class="tutor-courses">
                  <span class="blue-text subheading" v-line-clamp:18="1">{{tutorData.courses}}</span>
                </v-flex>
              </v-layout>
            </v-flex>
          </v-layout>
        </div>
        <v-layout
          row
          wrap
          align-start
          justify-start
          grow
          class="price-review-column section-tutor-price-review ml-1"
        >
          <v-flex xs12 grow>
            <v-flex xs12 shrink v-if="showStriked" class="strike-through">
              <span class="pricing striked-price">₪{{tutorData.price}}</span>
              <span class="pricing caption striked-price">
                <span v-language:inner="'resultTutor_hour'"></span>
              </span>
            </v-flex>
            <v-flex xs12 shrink>
              <!--keep this wraper to fix price / hour spacing-->
              <div class="d-inline-flex align-baseline">
                        <span class="font-weight-bold headline pricing" v-if="showStriked">₪{{discountedPrice}}</span>
                <span class="font-weight-bold headline pricing" v-else>₪{{tutorData.price}}</span>
                <span class="pricing caption">
                  <span v-language:inner="'resultTutor_hour'"></span>
                </span>
              </div>
            </v-flex>
            <v-flex xs12 shrink class="pt-2">
              <userRating
                v-if="tutorData.reviews > 0"
                :rating="tutorData.rating"
                :starColor="'#ffca54'"
                :rateNumColor="'#43425D'"
                :size="'24'"
                :rate-num-color="'#43425D'"
              ></userRating>
            </v-flex>
            <v-flex xs12 class="pt-1" shrink>
              <span class="blue-text body-2" v-show="tutorData.reviews > 0">
                {{tutorData.reviews}}
                <span v-show="tutorData.reviews > 1" v-language:inner="'resultTutor_reviews_many'"></span>
                <span v-show="tutorData.reviews === 1" v-language:inner="'resultTutor_review_one'"></span>
              </span>
              <span class="body-2" v-show="!tutorData.reviews" v-language:inner="'resultTutor_no_reviews'"></span>
            </v-flex>
          </v-flex>

          <v-flex xs12 class="d-flex btn-bottom-holder text-xs-center">
            <v-btn
              style="max-width: 80%; margin: 0 auto;"
              round
              class="blue-btn rounded elevation-0 ma-0"
              block
            >
              <span @click.prevent="openRequestDialog" class="font-weight-bold text-capitalize" v-language:inner="buttonText"></span>
            </v-btn>
          </v-flex>
        </v-layout>
      </v-layout>
    </router-link>
  </div>
</template>


<script>
import userRank from "../../../helpers/UserRank/UserRank.vue";
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import utilitiesService from "../../../../services/utilities/utilitiesService";
import analyticsService from "../../../../services/analytics.service";
import tutorRequest from "../../../tutorRequest/tutorRequest.vue";
import sbDialog from "../../../wrappers/sb-dialog/sb-dialog.vue";
import { mapActions } from "vuex";

export default {
  name: "tutorResultCard",
  components: {
    userRank,
    userRating,
    userAvatar,
    tutorRequest,
    sbDialog
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
        }
        this.$router.push({
          name: "profile",
          params: { id: this.tutorData.userId, name: this.tutorData.name }
        });
      
    },
    openRequestDialog(ev) {
      ev.stopImmediatePropagation()
      this.updateCurrTutor(this.tutorData)
      this.updateRequestDialog(true);
    },
    onImageLoadError(event) {
      event.target.src = "./images/placeholder-profile.png";
    }
  },
  computed: {
    userImageUrl() {
      if (this.tutorData.image) {
        return utilitiesService.proccessImageURL(
          this.tutorData.image,
          166,
          186
        );
      } else {
        return "./images/placeholder-profile.png";
      }
    },
    showStriked() {
                let price = this.tutorData.price;
                return price > this.minimumPrice;
            },
            discountedPrice(){
                let price = this.tutorData.price;
                let discountedAmount = price - this.discountAmount;
                return discountedAmount >  this.minimumPrice ? discountedAmount : this.minimumPrice;
    },
    buttonText() {
      return this.fromLandingPage
        ? "resultTutor_contact_me"
        : "resultTutor_btn_view";
    }
  }
};
</script>


<style lang="less" src="./tutorResultCard.less">
</style>
