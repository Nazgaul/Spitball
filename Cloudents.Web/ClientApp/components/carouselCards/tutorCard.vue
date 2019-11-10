<template>
    <router-link event @click.native.prevent="goToProfile" :to="{name: 'profile', params: {id: tutor.userId, name:tutor.name}}" class="tutorCarouselCard">
        <div class="tutorCarousel-top">
        <userAvatarRect draggable="false"
              :userName="tutor.name" 
              :userImageUrl="tutor.image" 
              class="tutorCarouselImg" 
              :width="240" 
              :height="152"></userAvatarRect>
        <div class="ts-content">
            <h1 class="tutor-name text-truncate">{{tutor.name}}</h1>
            <h2 class="tutor-university text-truncate">{{tutor.university}}</h2>
            <p class="tutor-bio my-2">{{tutor.bio}}</p>
        </div>
        </div>
        <div class="tutorCarousel-bottom">
        <div>
            <v-btn depressed color="#4c59ff" class="tutor-btn">
            <span class="text-truncate">
                <span v-language:inner="'tutorCardCarousel_tutor_btn'" /> {{tutor.name}}
            </span>
            </v-btn>
        </div>
        <div class="ts-bottom">
            <div class="rank">
            <template>
                <div class="user-rate-ts" v-if="tutor.reviews > 0">
                    <userRating :rating="tutor.rating" :showRateNumber="false" :size="'14'" />
                    <span class="reviews-ts ml-1" v-html="$Ph(tutor.reviews === 1 ? 'resultTutor_review_one' : `resultTutor_reviews_many`, reviewsPlaceHolder(tutor.reviews))"/>
                </div>
                <div class="user-rate-ts align-center" v-else>
                    <star class="mr-1 icon-star" />
                    <span class="reviews-ts" v-html="$Ph(`resultTutor_collecting_review`, reviewsPlaceHolder(tutor.reviews))"/>
                </div>
            </template>
            </div>
            <div class="ts-price">
            <span class="price-mark">{{tutor.price}}</span>/
            <span v-language:inner="'tutorCardCarousel_hour'" />
            </div>
        </div>
        </div>
    </router-link>
</template>

<script>
import userRating from "../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import userAvatarRect from '../helpers/UserAvatar/UserAvatarRect.vue';
import star from "./image/stars-copy.svg";
export default {
    components:{userRating,star,userAvatarRect},
    props:{
        tutor:{
            type:Object,
            required: true
        },
        fromCarousel:{
            type:Boolean,
            required: false,
            default: false
        }
    },
    methods: {
        reviewsPlaceHolder(reviews) {
            return reviews === 0 ? reviews.toString() : reviews;
        },
        goToProfile(enter){
            if(this.fromCarousel){
                return false;
            }else{
                this.enterProfilePage();
            }
        },
        enterProfilePage(){
            this.$router.push({
                name: 'profile',
                params: {id: this.tutor.userId, name: this.tutor.name}
            })
        }
    },

}
</script>

<style lang="less">
@import '../../styles/mixin.less';
.tutorCarouselCard {
    overflow: hidden;
    width: 242px;
    height: 340px;
    background: white;
    border-radius: 8px;
    border: solid 1px #c1c3d2;
    color: #43425d;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    .tutorCarousel-top {
    .tutorCarouselImg {
        // border-top-left-radius: 8px;
        // border-top-right-radius: 8px;
    }
    .ts-content {
        padding: 4px 8px 0 8px;
        .tutor-name {
        font-size: 14px;
        font-weight: bold;
        font-stretch: normal;
        font-style: normal;
        line-height: normal;
        letter-spacing: normal;
        }
        .tutor-university {
        font-size: 14px;
        font-weight: normal;
        font-stretch: normal;
        font-style: normal;
        line-height: normal;
        letter-spacing: normal;
        }
        .tutor-bio {
        .giveMeEllipsis(2, 30);
        margin: 0;
        padding: 0;
        font-size: 13px;
        font-weight: normal;
        font-stretch: normal;
        font-style: normal;
        line-height: 1.54;
        letter-spacing: normal;
        }
    }
    }
    .tutorCarousel-bottom {
    padding: 0 8px 8px 8px;
    .tutor-btn {
        border-radius: 8px;
        color: white;
        font-size: 14px;
        font-weight: 600;
        text-transform: capitalize !important;
        height: 34px !important;
        min-width: 100%;
        margin: 0;
        margin-bottom: 16px;
    }
    .ts-bottom {
        display: flex;
        justify-content: space-between;
        align-items: baseline;
        .rank {
        .user-rate-ts {
            display: inline-flex;
            align-items: baseline;
            .reviews-ts {
            font-size: 12px;
            letter-spacing: normal;
            color: #43425d;
            }
            .icon-star {
              width: 14px;
              align-self: center;
            }
        }
        }
        .ts-price {
        font-size: 12px;
        font-weight: normal;
        line-height: 1;
        .price-mark {
            font-weight: bold;
            font-size: 16px;
        }
        }
    }
    }
}
</style>