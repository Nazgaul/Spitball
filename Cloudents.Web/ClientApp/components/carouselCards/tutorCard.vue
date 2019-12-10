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

            <p class="tutor-bio my-2">{{tutor.bio}}</p>
        </div>
        </div>
        <div class="tutorCarousel-bottom">
            <div class="ts-bottom">
                <router-link class="applyCoupon" :to="{name: 'profile', params: {id: tutor.userId, name: tutor.name},  query: {coupon: true}}" v-language:inner="'resultTutor_apply_coupon'"></router-link>


                <div class="ts-price">
                    <template>
                        <span v-if="isDiscount" class="ts-price-discount font-weight-bold">{{tutor.discountPrice | currencyFormat(tutor.currency)}}</span>
                        <span class="ts-price-original font-weight-bold" v-else>{{tutor.price | currencyFormat(tutor.currency)}}</span>
                    </template>
                    <span class="caption">
                        <span class="">/</span>
                        <span class="" v-language:inner="'tutorCardCarousel_hour'"></span>
                    </span>
                    <div class="striked ml-2" v-if="isDiscount">{{tutor.price | currencyFormat(tutor.currency)}}</div>
                </div>


                <!-- <div class="ts-price">
                    <span class="price-mark">{{tutor.price | currencyFormat(tutor.currency)}}</span>/
                    <span v-language:inner="'tutorCardCarousel_hour'" />
                </div> -->
            </div>
            <v-btn depressed color="#4c59ff" class="tutor-btn">
                <span class="text-truncate">
                    <button class="mr-1">
                        <div class="contact-me-button" v-html="$Ph('resultTutor_send_button', showFirstName)" ></div>
                    </button>
                </span>
            </v-btn>
        </div>
    </router-link>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import analyticsService from '../../services/analytics.service';
import { LanguageService } from "../../services/language/languageService.js";

import userRating from "../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import userAvatarRect from '../helpers/UserAvatar/UserAvatarRect.vue';

import star from "./image/stars-copy.svg";
export default {
    components:{userRating,star,userAvatarRect},
    data(){
        return{
            contactClickedbtn: false,
            flagLocalClick: false,
            flagRemoteClick: false,
        }
    },
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
    computed: {
        ...mapGetters(['accountUser']),
        showFirstName() {
            let maxChar = 5;
            let name = this.tutor.name.split(' ')[0];
            if(name.length > maxChar) {
                return LanguageService.getValueByKey('resultTutor_message_me');
            }
            return name;
        },
        isDiscount() {
            return this.tutor.discountPrice !== undefined;
        }
    },
    methods: {
        ...mapActions(['updateCurrTutor', 'setTutorRequestAnalyticsOpenedFrom', 'updateRequestDialog']),
        sendMessage(tutor) {
            analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:GUEST`);
            this.updateCurrTutor(tutor);
            this.setTutorRequestAnalyticsOpenedFrom({
                component: 'landingPage',
                path: this.$route.path
            });
            this.updateRequestDialog(true);
        },
        reviewsPlaceHolder(reviews) {
            return reviews === 0 ? reviews.toString() : reviews;
        },
        goToProfile(event){
            if(this.fromCarousel){
                    this.flagLocalClick = true;
                    if(event.target.querySelector('.contact-me-button') || event.target.classList.contains('contact-me-button')){
                        this.contactClickedbtn = true;
                    }else{
                        this.contactClickedbtn = false;
                    }
                    //this flag protects us from mouse up after drag
                    if(this.flagRemoteClick){
                        this.enterProfilePage();
                        this.flagRemoteClick = false;
                    }
                    return false;
            }
        },
        enterProfilePage(){
            this.flagRemoteClick = true;
            if(this.flagLocalClick){
                this.flagLocalClick = false;
                if(!this.contactClickedbtn){
                    this.$router.push({
                        name: 'profile',
                        params: {id: this.tutor.userId, name: this.tutor.name}
                    })
                }else{
                    this.sendMessage(this.tutor)
                }
            }
                
        },
    },
}
</script>

<style lang="less">
@import '../../styles/mixin.less';
.tutorCarouselCard {
    overflow: hidden;
    width: 242px;
    height: 362px;
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
        padding: 7px 8px 0 8px;
        .tutor-name {
        font-size: 14px;
        font-weight: bold;
        font-stretch: normal;
        font-style: normal;
        line-height: normal;
        letter-spacing: normal;
        margin-bottom: 4px;
        }
        .tutor-university {
        font-size: 14px;
        font-weight: normal;
        font-stretch: normal;
        font-style: normal;
        line-height: normal;
        letter-spacing: normal;
        margin-bottom: 4px;
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
        .rank {
            .user-rate-ts {
                display: inline-flex;
                // align-items: baseline;
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
    }
    }
    .tutorCarousel-bottom {
        // margin-top: 10px;
        padding: 0 8px 8px 8px;
    .tutor-btn {
        width: 100%;
        border-radius: 8px;
        color: white;
        font-size: 14px;
        font-weight: 600;
        text-transform: capitalize !important;
        min-width: 100%;
        margin: 0;
    }
    .ts-bottom {
        display: flex;
        justify-content: space-between;
        align-items: baseline;
        margin-bottom: 8px;

        @media(max-width: @screen-xs) {
            margin-bottom: 10px;
        }
        .applyCoupon {
          color: #4c59ff;
          font-weight: 600;
          font-size: 13px;
          margin-top: 6px;
        }
        .ts-price {
            display: flex;
            font-size: 12px;
            font-weight: normal;
            line-height: 1;
            align-items: flex-end;
            &-original {
                font-size: 18px;
            }
            &-discount {
                font-size: 18px;
            }
            .price-mark {
                color: #43425d;
                font-weight: bold;
                font-size: 16px;
            }
            .striked {
                margin: 0 0 0 auto;
                max-width: max-content;
                position: relative;
                color: #a0a4be;
                font-size: 14px;
                &:after {
                    content: "";
                    width: 100%;
                    border-bottom: solid 1px #a0a4be;
                    position: absolute;
                    left: -2px;
                    top: 50%;
                    z-index: 1;
                }
            }
        }
        }
    }
}
</style>