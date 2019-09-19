<template>
    <div v-touch="{
            left: () => moveCarousel('left'),
            right: () => moveCarousel('right')
            }" class="tutor-carousel-slider-wrapper mb-4">
        <h3 class="subtitle-1 mb-4" v-language:inner="'resultTutor_title'"/>
        <div class="tutor-carousel-slider-container" :style="{ transform: 'translateX' + '(' + currentOffset + 'px' + ')'}">
            <router-link v-for="(tutor, index) in tutorList" :key="index" class="tutor-carousel-card" 
                @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutor.userId, name: tutor.name}}">
                <div>
                    <h4 class="caption font-weight-bold mb-1" v-language:inner="'resultTutor_subtitle'"/>
                    <h3 class="body-2 font-weight-bold tutor-name-restriction">{{tutor.name}}</h3>
                <template>
                    <div class="user-rank mt-1 mb-2 align-center" v-if="tutor.reviews > 0">
                        <user-rating :size="'16'" :rating="tutor.rating" :showRateNumber="false" />
                        <div class="reviews caption ml-1" v-html="$Ph(tutor.reviews === 1 ? 'resultTutor_review_one' : `resultTutor_reviews_many`, reviewsPlaceHolder(tutor.reviews))"></div>
                    </div>
                    <div v-else class="user-rank mt-1 mb-2 align-center">
                        <star />
                        <span class="no-reviews font-weight-bold caption ml-1" v-language:inner="'resultTutor_no_reviews_mobile'"></span>
                    </div>
                </template>

                </div>
                <div class="user-price">
                    <div v-if="!isLoaded" class="mr-2 user-image tutor-card-loader">
                        <v-progress-circular indeterminate v-bind:size="50"></v-progress-circular>
                    </div>
                    <img v-show="isLoaded" class="user-image" @error="onImageLoadError" @load="loaded" :src="getImgUrl(tutor.image)" :alt="tutor.name">
                    <div>
                        <div class="striked" v-if="showStriked(tutor.price)"> &#8362;{{tutor.price}}</div>
                        <div>
                            <span v-if="showStriked(tutor.price)" class="price font-weight-bold"><span class="price-sign">&#8362;</span>{{discountedPrice(tutor.price)}}</span>
                            <span class="price font-weight-bold" v-else><span class="price-sign">&#8362;</span>{{tutor.price}}</span>
                            <div class="caption hour" v-language:inner="'resultTutor_hour'"></div>
                        </div>
                    </div>
                    
                </div>

                <div class="user-bio overflow-hidden">{{tutor.bio}}</div>

                <v-btn class="btn-chat white--text text-truncate" small round block color="#4452fc" @click.prevent="sendMessage(tutor)">
                    <div class="text-truncate" v-html="$Ph('resultTutor_send_button', showFirstName(tutor.name))" ></div>
                </v-btn>
            </router-link>

        </div>
    </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import utilitiesService from '../../../../services/utilities/utilitiesService';
import { LanguageService } from "../../../../services/language/languageService.js";
import analyticsService from "../../../../services/analytics.service";
import chatService from '../../../../services/chatService';
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import star from '../stars-copy.svg';

export default {
    components: {
        userRating,
        star
    },
    props: {
        courseName: {
            type: String
        }
    },
    data() {
        return {
            isLoaded: false,
            minimumPrice: 55,
            discountAmount: 70,
            currentOffset: 0,
            windowSize: window.innerWidth > 757 ? 2 : 1,
            paginationFactor: 0,
            isRtl: global.isRtl
        }
    },
    watch: {
        tutorList(newVal, oldVal) {
            if(!!newVal){
                this.setCardsCarousel();
            }
        }
    },
    computed: {
        ...mapGetters(['getTutorList', 'accountUser', 'getActivateTutorDiscounts']),

        atEndOfList() {     
            if(this.isRtl){
                return (this.currentOffset >= this.paginationFactor * 1 * ((this.getTutorList.length - 1) - this.windowSize));
            } else{
                return (this.currentOffset <= this.paginationFactor * -1 * ((this.getTutorList.length - 1) - this.windowSize));
            }
        },
        atHeadOfList() {
            return this.currentOffset === 0;
        },
        tutorList() {
            return this.getTutorList;
        },
        isMobile() {
            return this.$vuetify.breakpoint.smAndDown;
        }
    },
    methods: {
        ...mapActions(['getTutorListCourse', 'updateCurrTutor', 'setTutorRequestAnalyticsOpenedFrom', 'updateRequestDialog', 'setActiveConversationObj','openChatInterface']),

        loaded() {
            this.isLoaded = true;
        },
        moveCarousel(dir) {
            let self = this;
            let direction = dir === "left" ? 1 : -1;
            if (self.isRtl) {
            direction = direction*-1;
            if (direction === 1 && !self.atEndOfList) {
                self.currentOffset += self.paginationFactor;
            } else if (direction === -1 && !self.atHeadOfList) {
                self.currentOffset -= self.paginationFactor;
            }
            } else {
            if (direction === 1 && !self.atEndOfList) {
                self.currentOffset -= self.paginationFactor;
            } else if (direction === -1 && !self.atHeadOfList) {
                self.currentOffset += self.paginationFactor;
            }
            }
        },
        moveCarouselClick(direction) { 
            if(this.isRtl) {
                if (direction === 1 && !this.atEndOfList) {
                this.currentOffset += this.paginationFactor;
                } else if (direction === -1 && !this.atHeadOfList) {
                    this.currentOffset -= this.paginationFactor;
                } 
            } else {
                if (direction === 1 && !this.atEndOfList) {
                    this.currentOffset -= this.paginationFactor;
                } else if (direction === -1 && !this.atHeadOfList) {
                    this.currentOffset += this.paginationFactor;
                } 
            }
        },
        getImgUrl(path) {
            return utilitiesService.proccessImageURL(path, 64, 74);
        },
        reviewsPlaceHolder(reviews) {
          return reviews === 0 ? reviews.toString() : reviews;
        },
        onImageLoadError(event) {
            event.target.src = "../../../images/placeholder-profile.png";
        },
        showStriked(price) {
            if(!this.getActivateTutorDiscounts) return false;
            return price > this.minimumPrice;
        },
        discountedPrice(price) {
            let discountedAmount = price - this.discountAmount;
            return discountedAmount >  this.minimumPrice ? discountedAmount : this.minimumPrice;
        },
        setCardsCarousel() {
            // calculate cards on screen
            this.$nextTick(()=>{
                let cardsDisplayed = 2;
                let totalCardWidth = document.querySelector('.tutor-carousel-slider-wrapper').offsetWidth / cardsDisplayed;
                let mobileCards = document.querySelectorAll('.tutor-carousel-card');
                let cardWidth = (totalCardWidth * 95) /100;
                let marginCardLeft = (totalCardWidth * 2.5) /100;
                let marginCardRight = (totalCardWidth * 2.5) /100;
                mobileCards.forEach((card)=>{
                    card.style.minWidth = `${cardWidth}px`;
                    card.style.marginLeft = `${marginCardLeft}px`;
                    card.style.marginRight = `${marginCardRight}px`;
                })

                //calculate offset to swipe
                const element = mobileCards[0];
                let margins = (totalCardWidth * 5) /100;
                let width = element.offsetWidth;
                if(!this.isMobile) {
                    this.paginationFactor = width + (margins*2);
                } else{
                    let style = element.currentStyle || window.getComputedStyle(element);
                    let margin = parseFloat(style.marginLeft) + parseFloat(style.marginRight);
                    this.paginationFactor = width + margin;
                }
            })
        },
        isUserImage(img) {
            return img ? true : false;
        },
        showFirstName(name) {
            return name.split(' ')[0];
        },
        sendMessage(user) {
            if (this.accountUser == null) {
                analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:GUEST`);
                this.updateCurrTutor(user);
                this.setTutorRequestAnalyticsOpenedFrom({
                    component: 'tutorCard',
                    path: this.$route.path
                });
                this.updateRequestDialog(true);
            } else {
                analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:${this.accountUser.id}`);
                let conversationObj = {
                    userId: user.userId,
                    image: user.image,
                    name: user.name,
                    conversationId: chatService.createConversationId([user.userId, this.accountUser.id]),
                }
                let currentConversationObj = chatService.createActiveConversationObj(conversationObj)
                this.setActiveConversationObj(currentConversationObj);
                let isMobile = this.$vuetify.breakpoint.smAndDown;
                this.openChatInterface();                    
            }
        },
        tutorCardClicked() {
            if(this.fromLandingPage){
                analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_landing_page");
            } else {
                analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_page");
            };
        },
    },
    created() {
        if(this.$vuetify.breakpoint.smAndDown) {
            this.getTutorListCourse(this.courseName);
        }
    }
}
</script>
<style lang="less">
    @import "../../../../styles/mixin.less";

    @purple: #43425d;

    .tutor-carousel-slider-wrapper {
        width: 100%;
        overflow: hidden;
        height:315px;
        h3 {
            color: @purple;
        }
        .tutor-carousel-slider-container {
            display: flex;            
            transition: transform 150ms ease-out;
            .tutor-carousel-card {
                padding: 8px;
                border-radius: 4px;
                background: #fff;
                h3,h4 {
                    color: @purple;
                }
                .tutor-name-restriction{
                    white-space: nowrap;
                    text-overflow: ellipsis;
                    overflow: hidden;
                }
                .user-rank {
                    display: inline-flex;
                    // flex-direction: column;
                    svg {
                        width: 16px;
                        height: 16px;
                    }
                }
                .user-price {
                    display: flex;
                    color: @purple;
                    margin-bottom: 12px;
                    .user-image {
                        margin-right: 10px;
                        border-radius: 4px;
                         width: 66px;
                        height: 74px;
                    }
                    .tutor-card-loader {
                        display: flex;
                        justify-content: center;
                        align-content: center;
                    }
                    div {
                        display: flex;
                        flex-direction: column;
                        justify-content: flex-end;
                        .striked {
                            font-size: 12px;
                            max-width: max-content;
                            position: relative;
                            color: @colorBlackNew;
                            &:after {
                                content: "";
                                width: 100%;
                                border-bottom: solid 1px @colorBlackNew;
                                position: absolute;
                                left: 0;
                                top: 50%;
                                z-index: 1;
                            }
                        }
                        div {
                            display: flex;
                            flex-direction: column;
                            justify-content: flex-end;
                            .price {
                                font-family: Arial;
                                font-size: 22px;
                                .price-sign {
                                    font-size: 16px;
                                }
                            }
                            .hour {
                                align-items: end;
                            }
                        }
                    }
                }

                .reviews {
                    color: #4452fc;
                }
                .no-reviews {
                    display: flex;
                    justify-content: center;
                    margin-top: 2px;
                    color: #43425d;
                }
                .user-bio {
                    font-family: Open Sans,sans-serif;
                    .giveMeEllipsis(3,40px);
                    min-height: 40px;
                    line-height: 14px;
                    text-align: left;
                    color: @purple;
                    font-size: 12px;
                }
                .btn-chat {
                    margin-top: 14px;
                    font-size: 12px;
                    text-transform: inherit;
                    button {
                        text-transform: inherit;
                    }
                }
            }
        }
    }
</style>