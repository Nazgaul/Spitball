<template>
    <router-link class="tutorCarouselCard ma-4" :to="{name: 'profile', params: {id: tutor.id, name:tutor.name}}">
        <div class="tutorCarousel-top">
            <userAvatarRect 
                draggable="false"
                :userName="tutor.name" 
                :userImageUrl="tutor.image" 
                class="tutorCarouselImg" 
                :width="240" 
                :height="152"
            >
            </userAvatarRect>
            <div class="ts-content">
                <h3 class="font-weight-bold text-truncate mb-1">{{tutor.name}}</h3>
                <div class="rank">
                    <div class="user-rate-ts" v-if="tutor.reviews > 0">
                        <userRating :rating="tutor.rating" :showRateNumber="false" :size="'14'" />
                        <span class="reviews-ts ml-1">{{$tc('resultTutor_review_one', tutor.reviews)}}</span>
                    </div>
                    <div class="user-rate-ts align-center" v-else>
                        <star class="mr-1 icon-star" width="14" />
                        <span class="reviews-ts">{{$tc('resultTutor_review_one', tutor.reviews)}}</span>
                    </div>
                </div>
                <p class="tutor-bio my-2">{{tutor.bio}}</p>
            </div>
        </div>
        <div class="tutorCarousel-bottom pa-2 pt-0">
            <div class="text-truncate ts_subjects" v-show="subjects">
                <span class="mr-1 font-weight-bold" v-t="'resultTutor_study-area'"></span>
                <span class="">{{subjects}}</span>
            </div>
            <div class="ts-bottom d-flex align-baseline justify-space-between">
                <router-link 
                    class="applyCoupon"
                    :to="{name: 'profile', params: {id: tutor.id, name: tutor.name},  query: {coupon: true}}"
                    v-t="'resultTutor_apply_coupon'"
                >
                </router-link>
                <div class="ts-price d-flex align-baseline">
                    <template>
                        <span v-if="isDiscount" class="ts-price-discount font-weight-bold">{{$n(tutor.discountPrice, 'currency')}}</span>
                        <span class="ts-price-original font-weight-bold" v-else>{{$n(tutor.price, 'currency')}}</span>
                    </template>
                    <span>
                        <span>/</span>
                        <span v-t="'tutorCardCarousel_hour'"></span>
                    </span>
                    <div class="striked ml-2" v-if="isDiscount">{{$n(tutor.price, 'currency')}}</div>
                </div>
            </div>
            <v-btn depressed block color="#4c59ff" class="tutor-btn white--text" @click.native.prevent="sendMessage(tutor)">
                <div class="text-truncate">{{$t('resultTutor_send_button',[showFirstName])}}</div>
            </v-btn>
        </div>
    </router-link>
</template>

<script>
import { mapActions } from 'vuex';
import analyticsService from '../../services/analytics.service';

import userRating from "../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import userAvatarRect from '../helpers/UserAvatar/UserAvatarRect.vue';

import star from "./image/stars-copy.svg";

export default {
    components:{
        userRating,
        star,
        userAvatarRect
    },
    props:{
        tutor:{
            type:Object,
            required: true
        }
    },
    computed: {
        showFirstName() {
            let maxChar = 5;
            let name = this.tutor.name.split(' ')[0];
            if(name.length > maxChar) {
                return this.$t('resultTutor_message_me');
            }
            return name;
        },
        isDiscount() {
            return this.tutor.discountPrice !== undefined;
        },
        subjects() {
            return this.tutor.subjects;
        },
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
        }
    }
}
</script>

<style lang="less">
@import '../../styles/mixin.less';
@import '../../styles/colors.less';

.tutorCarouselCard {
    overflow: hidden;
    width: 242px;
    height: 362px;
    background: white;
    border-radius: 8px;
    border: solid 1px #c1c3d2;
    color: @global-purple !important; //vuetify
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    .tutorCarousel-top {
        .ts-content {
            padding: 7px 8px 0 8px;
            .tutor-name {
            font-size: 14px;
            font-weight: bold;
            margin-bottom: 4px;
            }
            .tutor-bio {
            .giveMeEllipsis(2, 30);
                margin: 0;
                padding: 0;
                font-size: 13px;
                line-height: 1.54;
            }
            .rank {
                .user-rate-ts {
                    display: inline-flex;
                    .reviews-ts {
                        font-size: 12px;
                        letter-spacing: normal;
                        color: @global-purple;
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
        .ts_subjects {
            font-size: 13px;
        }
        .tutor-btn {
            border-radius: 8px;
            font-weight: 600;
        }
        .ts-bottom {
            margin-bottom: 8px;
            @media(max-width: @screen-xs) {
                margin-bottom: 10px;
            }
            .applyCoupon {
                color: @global-auth-text;
                font-weight: 600;
                font-size: 13px;
                margin-top: 6px;
            }
            .ts-price {
                font-size: 12px;
                font-weight: normal;
                line-height: 1;
                &-original {
                    font-size: 18px;
                }
                &-discount {
                    font-size: 18px;
                }
                .price-mark {
                    color: @global-purple;
                    font-weight: bold;
                    font-size: 16px;
                }
                .striked {
                    margin: 0 0 0 auto;
                    max-width: max-content;
                    color: #a0a4be;
                    font-size: 14px;
                    text-decoration: line-through;
                }
            }
        }
    }
}
</style>