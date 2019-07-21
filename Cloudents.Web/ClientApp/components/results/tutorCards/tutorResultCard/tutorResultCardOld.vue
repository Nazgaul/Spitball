<template>
  <router-link
    class="tutor-card-wrap-desk cursor-pointer"
    event
    @click.native.prevent="tutorCardClicked"
    :to="{name: 'profile', params: {id: tutorData.userId,name:tutorData.name}}"
  >
    <v-layout>
      <div class="section-tutor-info">
        <v-layout>
          <v-flex class="image-wrap mr-3" shrink>
            <div v-if="!isLoaded">
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
          <v-flex class="rightSide">
            <v-layout align-start justify-space-between column fill-height>
              <div class="pb-3 tutor-name font-weight-bold" >
                {{tutorData.name}}
              </div>
              <v-flex grow class=" subheading">
                <div class="tutor-about">
               {{tutorData.bio}}
               </div>
              </v-flex>
              <div v-bind:title="courses" class="flex tutor-courses blue-text subheading shrink" >
                {{courses}}
              </div>
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
            <span class="pricing striked-price">₪{{tutorData.price.toFixed(2)}}</span>
            <span class="pricing caption striked-price" v-language:inner="'resultTutor_hour'"/>
          </v-flex>
          <v-flex xs12 shrink>
            <div class="d-inline-flex align-baseline">
              <span class="font-weight-bold headline pricing" v-if="showStriked">₪{{discountedPrice}}</span>
              <span class="font-weight-bold headline pricing" v-else>₪{{tutorData.price}}</span>
              <span class="pricing caption" v-language:inner="'resultTutor_hour'"/>
            </div>
          </v-flex>
          <v-flex xs12 shrink class="pt-2">
            <userRating
              v-if="tutorData.reviews > 0"
              :rating="tutorData.rating"
              :starColor="'#ffca54'"
              :rateNumColor="'#43425D'"
              :size="'24'"
              :rate-num-color="'#43425D'"/>
          </v-flex>
          <v-flex xs12 class="pt-1" shrink>
            <span class="blue-text body-2" v-show="tutorData.reviews > 0"> {{tutorData.reviews}}
              <span v-show="tutorData.reviews > 1" v-language:inner="'resultTutor_reviews_many'"/>
              <span v-show="tutorData.reviews === 1" v-language:inner="'resultTutor_review_one'"/>
            </span>
            <span class="body-2" v-show="!tutorData.reviews" v-language:inner="'resultTutor_no_reviews'"/>
          </v-flex>
        </v-flex>

        <v-flex xs12 class="d-flex btn-bottom-holder text-xs-center">
          <v-btn @click.prevent="openRequestDialog" 
                  round block 
                  class="blue-btn elevation-0 ma-0 font-weight-bold text-capitalize" 
                  v-language:inner="buttonText">
          </v-btn>
        </v-flex>
      </v-layout>
    </v-layout>
  </router-link>
</template>


<script>
import userRank from "../../../helpers/UserRank/UserRank.vue";
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import userAvatar from "../../../helpers/UserAvatar/UserAvatar.vue";
import utilitiesService from "../../../../services/utilities/utilitiesService";
import analyticsService from "../../../../services/analytics.service";
import tutorRequest from "../../../tutorRequest/tutorRequest.vue";
import sbDialog from "../../../wrappers/sb-dialog/sb-dialog.vue";
import { mapActions, mapGetters } from "vuex";
import { LanguageService } from "../../../../services/language/languageService.js";


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
      debugger;
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
      //  let query = this.$route.query.term
      //  if(query) {
        if (this.tutorData.courses) {
          return `${LanguageService.getValueByKey("resultTutor_teaching")} ${this.tutorData.courses}`
        }
        return '';
      // } else {
        //return `${this.tutorData.courses}`
      //}
    },
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
                return discountedAmount >  this.minimumPrice ? discountedAmount.toFixed(2) : this.minimumPrice.toFixed(2);
    },
    buttonText() {
      return "resultTutor_contact_me";
    }
  }
};
</script>


<style lang="less" src="./tutorResultCard.less">
</style>
