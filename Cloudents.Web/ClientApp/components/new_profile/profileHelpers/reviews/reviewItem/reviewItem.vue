<template>
    <v-container class="rating-item-container">
        <v-layout align-center justify-space-between>
            <div class="image-rating-wrap">
                <div>
                    <img class="user-picture" style="border-radius: 50%;"
                         src="
                         n-avatar-rounded-icon-vector-illustration-black-sign-on-isolated-background.jpg?ver=6" alt="">
                </div>
                <div class="user-rating ml-2 pt-3">
                    <userRating
                            :rating="reviewData.rate"
                            :starColor="starRateColor"
                            :readonly="true"
                            :rateNumColor="rateNumColor"
                            :size="'18'"></userRating>
                    <div class="name-rank-wrap">
                        <span class="user-review-name mr-2 ">{{reviewData.name}}</span>
                        <userRank :score="reviewData.score"></userRank>
                    </div>
                </div>
            </div>
            <div class="review-date">
                <span>{{reviewData.created | dateFromISO}}</span>
            </div>
        </v-layout>
        <v-layout class="left-padding">
            <div class="mt-3">
                          <span class="review-text">
                    {{reviewData.reviewText}}
                </span>
            </div>
        </v-layout>
    </v-container>
</template>

<script>
    import userRating from '../../profileBio/bioParts/userRating.vue';
    import userRank from '../../../../helpers/UserRank/UserRank.vue'
    export default {
        name: "reviewItem",
        components: {userRating, userRank},
        data() {
            return {
                // rating: 3.5,
                starRateColor: '#000000',
                rateNumColor: '#000000',

            }
        },
        props: {
            reviewData: {
            },
        },
    }
</script>

<style lang="less">
    @import '../../../../../styles/mixin.less';
    .rating-item-container{
        .left-padding{
            padding-left: 48px;
        }
        .image-rating-wrap{
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .user-picture{
            max-width: 40px;
        }
        .user-rating{
            .sbf{
                color: @textColor;
            }
        }
        .user-name-badge{
            padding-left: 48px;
        }
        .user-review-name{
            font-size: 12px;
            font-weight: bold;
            color: @profileTextColor;
        }
        .name-rank-wrap{
            display: flex;
            flex-direction: row;
            align-items: baseline;
            @media(max-width: @screen-xs){
                margin-top: 12px;
            }
        }
        .review-text{
            font-size: 16px;
            line-height: 1.4;
            color: @profileTextColor;
            @media(max-width: @screen-xs){
                font-size: 14px;
            }
        }
        .review-date{
            font-size: 11px;
            font-weight: bold;
            color: @profileTextColor;
        }

    }


</style>