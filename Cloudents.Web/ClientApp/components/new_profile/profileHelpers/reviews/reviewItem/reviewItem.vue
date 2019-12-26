<template>
    <div class="rating-item-container pt-4 pb-2">
        <v-layout align-center justify-space-between>
            <div class="image-rating-wrap">
                <div>
                    <user-avatar size="32"
                                 :userImageUrl="reviewData.image"
                                 :userId="reviewData.id"
                                 :user-name="reviewData.name" />
                </div>
                <div class="user-rating ml-2 pt-4">
                    <userRating :rating="reviewData.rate"
                                :starColor="starRateColor"
                                :readonly="true"
                                :rateNumColor="rateNumColor"
                                :size="'16'"></userRating>
                    <div class="name-rank-wrap mt-2">
                        <span class="user-review-name mr-2 ">{{reviewData.name}}</span>
                    </div>
                </div>
            </div>
            <div class="review-date">
                <span>{{reviewData.created | dateFromISO}}</span>
            </div>
        </v-layout>
        <v-layout class="left-padding">
            <div class="mt-4">
                <span class="review-text">
                    {{reviewData.reviewText}}
                </span>
            </div>
        </v-layout>
    </div>
</template>

<script>
    import userRating from '../../profileBio/bioParts/userRating.vue';
    import userAvatar from '../../../../helpers/UserAvatar/UserAvatar.vue';
    export default {
        name: "reviewItem",
        components: { userRating, userAvatar },
        data() {
            return {
                // rating: 3.5,
                starRateColor: '#43425d',
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

    .rating-item-container {
        .left-padding

    {
        padding-left: 38px;
    }

    .image-rating-wrap {
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .user-picture {
        max-width: 40px;
    }

    .user-rating {
        .sbf

    {
        color: @textColor;
    }

    }

    .user-name-badge {
        padding-left: 48px;
    }

    .user-review-name {
        font-size: 12px;
        font-weight: bold;
        color: @global-purple;
    }

    .name-rank-wrap {
        display: flex;
        flex-direction: row;
        align-items: baseline;

        @media(max-width: @screen-xs) {
            margin-top: 12px;
        }
    }

    .review-text {
        font-size: 14px;
        line-height: 1.4;
        color: @global-purple;

        @media(max-width: @screen-xs) {
            font-size: 14px;
        }
    }

    .review-date {
        font-size: 12px;
        font-weight: bold;
        color: @global-purple;
    }
    }
</style>