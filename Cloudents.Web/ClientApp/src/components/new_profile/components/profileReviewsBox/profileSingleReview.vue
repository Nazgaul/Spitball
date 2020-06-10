<template>
   <div class="profileSingleReview">
      <v-divider></v-divider>
      <div class="profileSingleReview_review">
         <div class="profileSingleReview_img">
            <userAvatar  
               :size="isMobile? '34' : '42'" 
               :userId="review.id"
               :userImageUrl="review.image" 
               :user-name="review.name"/>
         </div>
         <div class="profileSingleReview_content">
            <div class="profileSingleReview_content_info">
               <div class="profileSingleReview_content_info_user">
                  <div class="review_content_info_user_name">{{review.name}}</div>
                  <div class="review_content_info_user_stars_row">
                     <userRating class="review_content_info_user_stars" :showRateNumber="false" :rating="review.rate" :size="'18'" />
                     <span class="review_content_info_user_stars_span">{{review.rate}}</span>
                  </div>
               </div>
               <div class="profileSingleReview_content_info_date">
                  <span>{{$d(new Date(review.date), 'short')}}</span>
               </div>
            </div>
            <div class="profileSingleReview_content_txt">
               {{review.reviewText}}
            </div>
         </div>

      </div>
   </div>
</template>

<script>
import userRating from '../../profileHelpers/profileBio/bioParts/userRating.vue';
import { mapGetters } from 'vuex';

export default {
   name:'profileSingleReview',
   components:{userRating},
   props:{
      review:{
         type: Object,
         required: true,
      }
   },
   computed: {
      ...mapGetters(['getProfile']),
      isMobile(){
        return this.$vuetify.breakpoint.xsOnly;
      },
   }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

.profileSingleReview{
  .profileSingleReview_review{
     margin: 22px 0;
     display: flex;
     @media (max-width: @screen-xs) {
        margin: 14px 0;
     }
     .profileSingleReview_img{
        .flexSameSize();
        margin: 0 14px 0 4px;
         @media (max-width: @screen-xs) {
            margin: 0 8px 0 14px;
         }

     }
      .profileSingleReview_content{
         width: 100%;
         .profileSingleReview_content_info{
            display: flex;
            justify-content: space-between;
            align-items: baseline;
            .profileSingleReview_content_info_user{
                  .review_content_info_user_name{
                     font-size: 14px;
                     font-weight: bold;
                     margin-bottom: 6px;
                     margin-left: 4px;
                     @media (max-width: @screen-xs) {
                        margin-left: 2px;
                        margin-bottom: 2px;
                        line-height: 1.2;
                     }

                  }
                  .review_content_info_user_stars_row{
                     display: inline-flex;
                     align-items: center;
                     .review_content_info_user_stars{
                        flex: 0 0 auto;
                        &.rating-container{
                           .v-rating{
                              .v-icon{
                                 padding-right: 3px;
                                 &:last-child{
                                    padding-right: 0;
                                 }
                              }
                           }
                        }
                     }
                     .review_content_info_user_stars_span{
                        color: #ffca54;
                        font-size: 14px;
                        font-weight: 600;
                        @media (max-width: @screen-xs) {
                           margin-left: 4px;
                        }
                     }
                  }
            }
            .profileSingleReview_content_info_date{
               font-size: 12px;
               padding-right: 10px;
               @media (max-width: @screen-xs) {
                  padding-right: 16px;
                  align-self: flex-end;
                  padding-bottom: 4px;
               }

            }
         }
         .profileSingleReview_content_txt{
            font-size: 14px;
            line-height: 1.57;
            padding-top: 6px;
            padding-left: 2px;
            @media (max-width: @screen-xs) {
               font-size: 12px;
               padding-right: 14px;
               line-height: 1.4;
            }
         }
      }
  }
}
</style>