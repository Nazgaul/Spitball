<template>
   <div class="profileReviewsBox" v-if="!!getProfile && getProfileReviews !== null">
      <div class="profileReviewsBox_state">
         <div class="profileReviewsBox_state_title" v-language:inner="'reviewBox_title'"/>
         <div class="profileReviewsBox_state_container">
            <div class="profileReviewsBox_state_score">
               <div class="profileReviewsBox_state_score_title">
                  {{getProfile.user.tutorData.rate.toFixed(1)}}
               </div>
               <div class="profileReviewsBox_state_score_rating">
                  <userRating class="state_score_rating" :showRateNumber="false" :rating="getProfile.user.tutorData.rate" :size="'20'" />
                  <span class="state_score_rating_span pl-1" >{{$tc('resultTutor_review_one',getProfile.user.tutorData.reviewCount)}}</span>
                
               </div>
            </div>
            <div class="profileReviewsBox_state_stats">
               <div class="profileReviewsBox_state_stats_lines" v-if="!$vuetify.breakpoint.xsOnly">
                  <v-progress-linear v-for="(rate, index) in rates" :key="index" class="mr-3" color="#ffca54" height="13" :value="rate.rate * 20"/>
               </div>
               <div class="profileReviewsBox_state_stats_stars">
                  <div class="profileReviewsBox_state_stats_stars_row" v-for="(rate, index) in rates" :key="index">
                     <userRating class="state_stats_stars" :showRateNumber="false" :rating="rate.rate" :size="'18'" />
                     <span class="state_stats_span">{{rate.users}}</span>
                  </div>
               </div>
            </div>
         </div>
      </div>
      <profileSingleReview v-for="(review, index) in reviews" :review="review" :key="index"/>
      <div class="profileReviewsBox_more" v-if="getProfileReviews.reviews.length > 2">
         <button sel="more_reviews" @click="isExpand = !isExpand">
            <span v-language:inner="isExpand?'reviewBox_see_less':'reviewBox_see_more'"/>
         </button>
      </div>
   </div> 
</template>

<script>
import userRating from '../../profileHelpers/profileBio/bioParts/userRating.vue';
import profileSingleReview from './profileSingleReview.vue';


import { mapGetters } from 'vuex';

export default {
   name:'profileReviewsBox',
   components:{
      userRating,
      profileSingleReview,
   },
   data() {
      return {
         isExpand: false,
         minimalReviewsCount: 2,
      }
   },
   computed: {
      ...mapGetters(['getProfile','getProfileReviews']),
      reviews(){
         if(this.getProfileReviews == null) {return}
         let reviewsList = this.getProfileReviews.reviews;
         if(!this.isExpand){
            return reviewsList.slice(0,this.minimalReviewsCount)
         }
         return reviewsList
      },
      rates(){
         return this.sortRate(this.getProfileReviews.rates);
      },
   },
   methods: {
      // reviewsPlaceHolder(reviews) {
      //    return reviews === 0 ? reviews.toString() : reviews;
      // },
      sortRate(ratesArray){
         return ratesArray.sort((a,b)=> {
            return b.rate - a.rate
         });
      }
   },

}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

.profileReviewsBox{
  width: 100%;
  max-width: 800px;
  margin: 0 auto;
//   height: 512px;
  border-radius: 8px;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  background-color: #ffffff;
  padding: 14px;
  padding-top: 12px;
  padding-bottom: 12px;
  color: #43425d;
  @media (max-width: @screen-xs) {
     border-radius: 0;
     padding: 12px 0;
  }
  .profileReviewsBox_state{
      .profileReviewsBox_state_title{
         font-size: 18px;
         font-weight: 600;
         padding-bottom: 12px;
         text-transform: capitalize;
         @media (max-width: @screen-xs) {
            font-size: 16px;
            font-weight: bold;
            padding-left: 16px;
            padding-bottom: 24px;
         }
      }
      .profileReviewsBox_state_container{
         padding: 0 10px;
         padding-bottom: 24px;
         display: flex;
         justify-content: space-between;
               @media (max-width: @screen-xs) {
                  padding-bottom: 26px;
               }

         .profileReviewsBox_state_score{
            .profileReviewsBox_state_score_title{
               padding-left: 10px;
               font-size: 70px;
               @media (max-width: @screen-xs) {
                  padding-left: 6px;
                  font-size: 52px;
                  padding-bottom: 6px; 
               }
            }
            .profileReviewsBox_state_score_rating{
               display: inline-flex;
               align-items: center;
               @media (max-width: @screen-xs) {
                  flex-wrap: wrap;
                  padding-left: 6px;
               }
                  .state_score_rating{
                     flex: 0 0 auto;
                     &.rating-container{
                        .v-rating{
                           .v-icon{
                              padding-right: 4px;
                              &.sbf-star-rating-half{
                                 padding-right: 4px/*rtl:4px*/;
                                 padding-left: 0px/*rtl:0px*/;
                                 height: auto;
                              }
                              &:last-child{
                                 padding-right: 0;
                              }
                           }
                        }
                     }
                  }
                  .state_score_rating_span{
                     font-size: 14px;
                     @media (max-width: @screen-xss) {
                        padding-top: 6px;
                     }
                  }
            }
         }
         .profileReviewsBox_state_stats{
               width: 50%;
               display: flex;
               align-items: center;
               @media (max-width: @screen-xs) {
                  width: auto;
               }
               .profileReviewsBox_state_stats_lines{
                  width: 100%;
                  margin-right: 16px;
                  .v-progress-linear{
                     margin: 10px 0;
                     .v-progress-linear__background{
                        display: none;
                     }
                     .v-progress-linear__buffer{
                        background-color: #bdc0d1 !important;
                        border-color: #bdc0d1 !important;
                        opacity: .3;
                     }
                     .v-progress-linear__determinate{
                        position: relative;
                        z-index: 2;
                     }
                  }
               }
               .profileReviewsBox_state_stats_stars{
                  @media (max-width: @screen-xs) {
                     padding: 14px 0 0;
                  }
                  .flexSameSize();
                  width: 114px;
                  height: 100%;
                  padding: 8px 0;
                  display: flex;
                  flex-direction: column;
                  justify-content: space-between;
                  .profileReviewsBox_state_stats_stars_row{
                     display: flex;
                     align-items: center;
                     .state_stats_stars{
                         flex: 0 0 auto;
                         &.rating-container{
                           .v-rating{
                              i{
                                 &.sbf-star-rating-empty{
                                    color: #bdc0d1 !important;
                                    caret-color: #bdc0d1 !important;
                                 }
                              }
                           }
                        }
                     }
                     .state_stats_span{
                        font-size: 12px;
                        color: #95979d;
                        @media (max-width: @screen-xs) {
                           padding-left: 10px
                        }
                     }
                  }

               }
         

         }
      }
  }
   .profileReviewsBox_more{
      text-align: center;
      font-size: 14px;
      font-weight: 600;
      color: #43425d;
      padding-top: 2px;
      button{
         outline: none;
      }
   }
}
</style>