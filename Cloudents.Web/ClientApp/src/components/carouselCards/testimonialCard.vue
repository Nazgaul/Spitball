<template>
    <div class="testimonialCard">
        <img class="card-t" src="./image/cardT.png" alt="">
        <div class="testimonialCard-top">
            <img class="quote-card" src="./image/quote.png" alt="">
            <p class="testimonialCard-txt">{{item.text}}</p>
            <div class="t-name text-truncate">{{item.userName}}</div>
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
                    <span class="reviews-ts ms-1">{{$tc('resultTutor_review_one', item.tutorCount)}}</span>
                   
                </div>
            </template>
        </div>
    </div>
</template>

<script>
import userRating from "../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";


export default {
    components:{userRating},
    props:{
        item:{
            type:Object,
            required:true
        }
    },
    methods: {
        // reviewsPlaceHolder(reviews) {
        //     return reviews === 0 ? Math.round(reviews).toString() : Math.round(reviews);
        // },
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
            padding: 0;
            margin: 12px 0 0;
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