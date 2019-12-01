<template>
    <div class="testimonialCard">
        <img class="card-t" src="./image/cardT.png" alt="">
        <div class="testimonialCard-top">
            <img class="quote-card" src="./image/quote.png" alt="">
            <p class="testimonialCard-txt">{{item.text}}</p>
            <h1 class="t-name text-truncate">{{item.userName}}</h1>
        </div>
        <div class="tutorInfoT">
            <UserAvatar class="tutorImg-t" :size="'54'" 
                :user-name="item.tutorName" 
                :user-id="item.tutorId" 
                :userImageUrl="item.tutorImage"/>
            <div class="tutorName-t text-truncate">{{item.tutorName}}</div>
            <template>
                <div class="user-rate-testimonialCard mt-1">
                    <userRating :rating="item.tutorCount? item.tutorReviews : item.tutorCount" :showRateNumber="false" :size="'14'" />
                    <span class="reviews-ts ml-1" v-html="$Ph(item.tutorCount === 1 ? 'resultTutor_review_one' : `resultTutor_reviews_many`, reviewsPlaceHolder(item.tutorCount))"/>
                </div>
            </template>
        </div>
    </div>
</template>

<script>
import UserAvatar from '../helpers/UserAvatar/UserAvatar.vue';
import userRating from "../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";


export default {
    components:{UserAvatar,userRating},
    props:{
        item:{
            type:Object,
            required:true
        }
    },
    methods: {
        reviewsPlaceHolder(reviews) {
            return reviews === 0 ? Math.round(reviews).toString() : Math.round(reviews);
        },
    },
}
</script>

<style lang="less">
@import '../../styles/mixin.less';
.testimonialCard{
    padding: 0 20px;
    position: relative;
    width: 285px;
    height: 375px;
    border-radius: 8px;
    box-shadow: 0 2px 24px 0 rgba(0, 0, 0, 0.31);
    background-color: #ffffff;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    .card-t{
        position: absolute;
        top: 0;
        right: -1px;
        &:lang(he){
            transform: scaleX(-1);
        }
    }
    .testimonialCard-top{
        display: flex;
        flex-direction: column;
        align-items: center;
        .quote-card{
            margin-top: 24px;
        }
        .testimonialCard-txt{
            width: 100%;
            .giveEllipsisUpdated(16px, 1.75, 5, 140px);
            margin: 0;
            padding: 0;
            margin-top: 12px;
            font-size: 16px;
            font-weight: normal;
            font-stretch: normal;
            font-style: normal;
            line-height: 1.75;
            letter-spacing: normal;
            text-align: center;
            color: #43425d;
        }

        .t-name{
            width: 100%;
            text-align: center;
            font-size: 16px;
            font-weight: normal;
            font-stretch: normal;
            font-style: normal;
            line-height: 2.51;
            letter-spacing: normal;
            color: #848bbc;
        }
    }
    .tutorInfoT{
        display: flex;
        flex-direction: column;
        align-items: center;
        .tutorImg-t{
            pointer-events: all;
            margin-bottom: 12px;
        }
        .tutorName-t{
            width: 100%;
            text-align: center;
            font-size: 16px;
            font-weight: normal;
            font-stretch: normal;
            font-style: normal;
            line-height: 1.43;
            letter-spacing: normal;
            color: #4c59ff;
        }
        .user-rate-testimonialCard {
            display: inline-flex;
            // align-items: baseline;
            align-items: center;
            margin-bottom: 16px;
            .reviews-ts {
            font-size: 12px;
            letter-spacing: normal;
            color: #43425d;
            }
        }
    }
}
</style>