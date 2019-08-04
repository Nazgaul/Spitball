<template>
    <div class="tutor-carousel-slider-wrapper mb-4">
        <h3 class="subtitle-1 mb-4" v-language:inner="'resultTutor_title'"/>
        <div class="tutor-carousel-slider-container"
            v-touch="{
            left: () => moveCarousel('left'),
            right: () => moveCarousel('right')
            }"
            :style="{ transform: 'translateX' + '(' + currentOffset + 'px' + ')'}">

            <div v-for="(tutor, index) in tutorList" :key="index" class="tutor-carousel-card pa-2">
                <div>
                    <h4 class="caption font-weight-bold mb-1" v-language:inner="'resultTutor_subtitle'"/>
                    <h3 class="body-2 font-weight-bold">{{tutor.name}}</h3>
                
                <template>
                    <div class="user-rank mt-1 mb-2 align-center" :class="{'user-rank-hidden': tutor.reviews === 0}">
                        <user-rating :size="'16'" :rating="tutor.rating" :showRateNumber="false" />
                        <div class="reviews caption ml-1" v-html="$Ph(`resultTutor_reviews_many`, reviewsPlaceHolder(tutor.reviews))"></div>
                    </div>
                </template>

                </div>
                <div class="user-price mb-3">
                    <div v-if="!isLoaded" class="mr-2 user-image tutor-card-loader">
                        <v-progress-circular indeterminate v-bind:size="50"></v-progress-circular>
                    </div>
                    <img v-show="isLoaded" class="user-image" @error="onImageLoadError" @load="loaded" :src="getImgUrl(tutor.image)" :alt="tutor.name">
                    <div class="">
                        <div class="striked" v-if="showStriked(tutor.price)"> &#8362;{{tutor.price}}</div>
                        <div>
                            <span v-if="showStriked(tutor.price)" class="price font-weight-bold">&#8362;{{discountedPrice(tutor.price)}}</span>
                            <span class="price font-weight-bold" v-else>&#8362;{{tutor.price}}</span>
                            <div class="caption hour" v-language:inner="'resultTutor_hour'"></div>
                        </div>
                    </div>
                    
                </div>

                <div class="user-bio overflow-hidden" v-html="ellipsizeTextBox(tutor.bio)"></div>

                <v-btn class="btn-chat white--text text-truncate" small round block color="#4452fc" @click.prevent="sendMessage(tutor)">
                    <div class="text-truncate" v-html="$Ph('resultTutor_send_button', tutor.name)" ></div>
                </v-btn>
            </div>

        </div>
    </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import utilitiesService from '../../../../services/utilities/utilitiesService';
import { LanguageService } from "../../../../services/language/languageService.js";

import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";

export default {
    components: {
        userRating
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
        ...mapGetters(['getTutorList']),

        atEndOfList() {     
            if(this.isRtl){
                return (this.currentOffset >= this.paginationFactor * 1 * (this.getTutorList.length - this.windowSize));
            } else{
                return (this.currentOffset <= this.paginationFactor * -1 * (this.getTutorList.length - this.windowSize));
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
        ...mapActions(['getTutorListCourse']),

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
            return utilitiesService.proccessImageURL(path, 66, 74);
        },
        reviewsPlaceHolder(reviews) {
          return reviews === 0 ? reviews.toString() : reviews;
        },
        onImageLoadError(event) {
            event.target.src = "../../../images/placeholder-profile.png";
        },
        showStriked(price) {
            return price > this.minimumPrice;
        },
        discountedPrice(price) {
            let discountedAmount = price - this.discountAmount;
            return discountedAmount >  this.minimumPrice ? discountedAmount : this.minimumPrice;
        },
        ellipsizeTextBox(text) {
            let maxChars = 105;
            let showBlock = text.length > maxChars;
            let newText = showBlock ? text.slice(0, maxChars) + '...' : text;
            let hideText = showBlock ? `<span style="display:none">${text.slice(maxChars)}</span>` : '';
            return `${newText} ${hideText}`;
        },
        setCardsCarousel() {
            // calculate cards on screen
            this.$nextTick(()=>{
                let cardsDisplayed = 2;
                let totalCardWidth = document.querySelector('.tutor-carousel-slider-wrapper').offsetWidth / cardsDisplayed;
                let mobileCards = document.querySelectorAll('.tutor-carousel-card');
                let cardWidth = (totalCardWidth * 90) /100;
                let marginCardLeft = (totalCardWidth * 5) /100;
                let marginCardRight = (totalCardWidth * 5) /100;
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
    },
    created() {
        if(this.$vuetify.breakpoint.smAndDown) {
            let course = this.$route.params.courseName.replace(/-/g, ' '); 
            this.getTutorListCourse(course);
        }
    }
}
</script>
<style lang="less">
    @import "../../../../styles/mixin.less";

    @purple: #43425d;

    .tutor-carousel-slider-wrapper {
        width: 100%;
        h3 {
            color: @purple;
        }
        .tutor-carousel-slider-container {
            display: flex;
            transition: transform 150ms ease-out;
            .tutor-carousel-card {
                border-radius: 4px;
                background: #fff;
                .heightMinMax(248px);
                h3,h4 {
                    color: @purple;
                }
                .user-rank {
                    display: inline-flex;
                    &.user-rank-hidden {
                        visibility: hidden;
                    }
                }
                .user-price {
                    display: flex;
                    color: @purple;
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
                                font-size: 22px;
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
                .user-bio {
                    font-family: Open Sans,sans-serif;
                    .giveEllipsisUpdated(11px, normal, 3,38px);
                    line-height: 1.2 !important;
                    min-height: 38px;
                    text-align: left;
                    color: @purple;
                    position: relative;
                    font-size: 11px;
                }
                .btn-chat {
                    margin-top: 14px;
                    font-size: 12px;
                    button {
                        text-transform: inherit;
                    }
                }
            }
        }
    }
</style>