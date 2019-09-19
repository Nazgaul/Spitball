<template>
    <router-link class="tutor-result-card-other pa-2 mb-3 row wrap justify-space-between overflow-hidden ab-default-card" 
    @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}">
        <div class="mb-3 top-card justify-space-between">
            <div v-if="!isLoaded" class="mr-2 user-image tutor-card-loader">
              <v-progress-circular indeterminate v-bind:size="50"></v-progress-circular>
            </div>
            <img v-show="isLoaded" class="mr-2 user-image" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
            <div class="top-card-wrap">
                <h3 class="subheading font-weight-bold tutor-name text-truncate" v-html="$Ph('resultTutor_private_tutor', tutorData.name)"></h3>

                <template>
                    <div class="striked" v-if="showStriked">₪{{tutorData.price}}</div>
                    <div v-else class="striked"></div>
                </template>
                

                <v-layout row class="moreDetails" align-center>
                    <div column class="price-box column mr-2">
                        <template>
                            <span v-if="showStriked" class="font-weight-bold"><span class="price-sign">&#8362;</span>{{discountedPrice}}</span>
                            <span v-else class="font-weight-bold"><span class="price-sign">&#8362;</span>{{tutorData.price}}</span>
                        </template>
                        <div class="caption" v-language:inner="'resultTutor_hour'"></div>
                    </div>

                    <v-layout column align-center class="user-rates">
                        <div v-if="isReviews" :class="{'mr-5': !isReviews}">
                            <userRating :size="'15'" class="rating-holder" :rating="tutorData.rating" :showRateNumber="false" color="#ffca54"/>
                            <div class="caption text-xs-center reviews" v-html="$Ph(tutorData.reviews === 1 ? `resultTutor_review_one` : `resultTutor_reviews_many`, reviewsPlaceHolder(tutorData.reviewsCount,tutorData.reviews))"></div>        
                        </div>
                        <div v-else class="no-reviews">
                            <star class="rating-holder" />
                            <span class="caption" :class="{'font-weight-bold': uploader}" v-language:inner="'resultTutor_no_reviews_mobile'"></span>
                        </div>
                    </v-layout>
                    
                    <template>
                        <!-- card-a -->
                        <v-btn class="btn-chat ab-cardA" color="#4452fc" @click.prevent="sendMessage(tutorData)">
                            <iconChat/>
                        </v-btn>

                        <!-- card-b -->
                        <v-layout column align-center class="ab-cardB user-classes" :class="{'user-classes-hidden': tutorData.lessons === 0}">
                            <div>{{tutorData.lessons}}</div>
                            <div class="user-classes-text" v-language:inner="tutorData.lessons !== 1 ? 'resultTutor_classes' : 'resultTutor_class'"></div>
                        </v-layout>
                    </template>

                </v-layout>
            </div>
        </div>

        <!-- <v-layout class="tutor-bio mb-2" v-html="ellipsizeTextBox">{{tutorData.bio}}</v-layout> -->
        <v-layout class="tutor-bio mb-2">{{tutorData.bio}}</v-layout>

        <v-layout row class="btn-footer ab-cardB">
            <div class="send-msg text-xs-center text-truncate" :class="{'no-uploader': !uploader}">
                <v-btn 
                    round 
                    small 
                    color="#4452fc" 
                    depressed 
                    class="white--text caption py-3 px-3 mb-0" 
                    @click.prevent="sendMessage(tutorData)" 
                    :class="{'tutor-btn': isTutor}" 
                    v-html="$Ph('resultTutor_send_button', showFirstName)">
                </v-btn>
            </div>
            
            <div class="more-documents text-xs-center text-truncate card-transform" v-if="uploader">
                <v-btn @click.stop.prevent="goMoreDocs" round small color="#5158af" depressed class="caption py-3 px-2" v-language:inner="'resultTutor_btn_more_doc'"></v-btn>
            </div>
        </v-layout>
    </router-link>
</template>

<script>
import analyticsService from '../../../../services/analytics.service';
import utilitiesService from "../../../../services/utilities/utilitiesService";
import { LanguageService } from "../../../../services/language/languageService.js";
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import iconChat from './icon-chat.svg';
import chatService from '../../../../services/chatService';
import {mapActions, mapGetters} from 'vuex';
import star from '../stars-copy.svg';

export default {
    components: {
        userRating,
        iconChat,
        star
    },
    props: {
        tutorData: {},
        uploader: {
            type: Boolean
        }
    },
    data() {
        return {
            isLoaded: false,
            minimumPrice: 55,
            discountAmount: 70
        }
    },
    computed: {
        ...mapGetters(['accountUser', 'getActivateTutorDiscounts']),
        isTutor() {
            if(this.isTutorData) {
                return this.tutorData.isTutor;
            }
        },
        isTutorData() {
            return this.tutorData ? true : false;
        },
        isUserImage() {
            return this.isTutorData && this.tutorData.image ? true : false;
        },
        isReviews() {
            return this.tutorData.reviewsCount > 0 || this.tutorData.reviews > 0 ? true : false
        },
        discountedPrice() {
            let price = this.tutorData.price;
            let discountedAmount = price - this.discountAmount;
            return discountedAmount > this.minimumPrice ? discountedAmount.toFixed(0) : this.minimumPrice;
        },
        userImageUrl() {
            if (this.tutorData.image) {
                    let size = [64, 78];
                    return utilitiesService.proccessImageURL(
                    this.tutorData.image,
                    ...size,
                    "crop"
                    );
            } else {
                return "../../../images/placeholder-profile.png";
            }
        },
        showStriked() {
            if(!this.getActivateTutorDiscounts) return false;
            let price = this.tutorData.price;
            return price > this.minimumPrice;
        },
        showFirstName() {
            return this.tutorData.name.split(' ')[0];
        },
        // ellipsizeTextBox() {
        //     let text = this.tutorData.bio;
        //     let maxChars = 105;
        //     let showBlock = text.length > maxChars;
        //     let newText = showBlock ? text.slice(0, maxChars) + '...' : text;
        //     let hideText = showBlock ? `<span style="display:none">${text.slice(maxChars)}</span>` : '';
        //     let readMore = showBlock ? `<span class="read-more" style="${showBlock ? 'display: contents' : ''}">${LanguageService.getValueByKey('resultTutor_read_more')}</span>` : '';
        //     return `${newText} ${readMore} ${hideText}`;
        // }
    },
    methods: {
        ...mapGetters(['getProfile']),
        ...mapActions(['setActiveConversationObj', 'openChatInterface','updateRequestDialog','updateCurrTutor', 'setTutorRequestAnalyticsOpenedFrom']),
        loaded() {
          this.isLoaded = true;
        },
        tutorCardClicked() {
            if(this.fromLandingPage){
                analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_landing_page");
            } else {
                analyticsService.sb_unitedEvent("Tutor_Engagement", "tutor_page");
            };
        },
        onImageLoadError(event) {
            event.target.src = "../../../images/placeholder-profile.png";
        },
        reviewsPlaceHolder(reviewsOwner, reviews) {
            let review;
            if(reviewsOwner || reviewsOwner === 0) {
                review = reviewsOwner === 0 ? reviewsOwner.toString() : reviewsOwner;
            } else if(reviews || reviews === 0) {
                review = reviews === 0 ? reviews.toString() : reviews;
            }
            return review;
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
        goMoreDocs(){
            this.$router.push({name: 'profile', params: {id: this.tutorData.userId, name: this.tutorData.name,tab:4}});
        }
    }
}
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

@purple: #43425d;
.tutor-result-card-other {
    border-radius: 4px;
    position: relative;
    background: #fff;
    display: flex;
    flex-direction: column;
    &.ab-default-card {
        .ab-cardA {
            display: none;
        }
        .ab-cardB {
            display: flex;
        }
        .user-rates {
            padding: 0 !important;
        }
    }
    .ab-cardA {
        display: flex;
    }
    .ab-cardB {
        display: none;
    }
    .top-card {
        display: flex;
        width: 100%;
        max-height: 78px; 
        min-height: 78px;
        .top-card-wrap {
            width:100%;
            display: flex;
            flex-direction: column;
            // max-height:83px;
        }
    }
    .tutor-card-loader{
        display: flex;
        justify-content: center;
        align-items: center;
    }
    .user-image {
        border-radius: 4px;
        width: 64px;
        height: auto;
    }

    .tutor-name {
        color: @purple;
        max-width: 200px; // for eplipsis purpose
        margin-bottom: 6px;
            .flexSameSize();
    }

    .striked {
        max-width: max-content;
        position: relative;
        margin-bottom: 2px;
        font-family: arial;
        font-size: 12px;
        color: @colorBlackNew;
        .heightMinMax(14px);
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

    .moreDetails {
        display: flex;
        align-items: flex-end;
        color: @purple;
        margin-bottom: -2px;
        .price-box {
            line-height: 14px;
            font-size: 22px;
            span {
                font-family: Arial;
                font-size: 22px;
            }
            .price-sign {
                font-size: 16px;
            }
            .caption {
                margin-top: 1px;
            }
        }
        .rating-holder {
            padding-bottom: 2px;
            div {
                margin: 0 !important;
            }
        }
        .user-rates {
            padding: 0 42px 0 0;
            .reviews {
                color: #4452fc;
            }
            .no-reviews {
                display: flex;
                flex-direction: column;
                margin-top: -4px;
                svg {
                    margin: 0 auto;
                }
            }
        }
        .user-classes-text {
            font-size: 12px;
        }
        .user-classes-hidden {
            visibility: hidden;
        }
    }
    .btn-chat {
        border-radius: 16px 0 0 16px;
        min-width: 20px;
        position: absolute;
        right: -12px;
        top: 44px;
        div {
            justify-content: normal;
        }
    }
    .tutor-bio {
        font-size: 13px;
        display: block;
        color: @purple;
        .giveMeEllipsis(2,18px);
        .read-more {
            position: absolute;
            color: #4452fc;
        }
    }
    .btn-footer {
        justify-content: space-evenly;
        width: 100%;
        .send-msg {
            button {
                line-height: 0;
                color: @purple;
                text-transform: inherit;
                margin-left: 0;
                // .widthMinMax(200px);
                &.tutor-btn {
                    font-weight: 600;
                    // .widthMinMax(132px);
                }
            }
            &.no-uploader {
                button {
                    // .widthMinMax(170px);
                }
            }

        }
        .more-documents {
            button {
                border: solid 1px #5158af;
                background: #fff !important;
                line-height: 0;
                text-transform: inherit;
                margin: 6px 0;
                // .widthMinMax(120px);
            }
        }
    }
}

</style>
