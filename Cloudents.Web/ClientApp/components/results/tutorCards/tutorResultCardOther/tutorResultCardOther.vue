<template>
    <router-link class="tutor-result-card-other pa-2 mb-3 row wrap justify-space-between overflow-hidden " @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}">
        <div class="mb-3 top-card justify-space-between">
            <img :class="[isUserImage ? '' : 'tutor-no-img']" class="mr-2 user-image" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
            <div class="top-card-wrap">
                <h3 class="subheading font-weight-bold tutor-name text-truncate mb-1">{{tutorData.name}}</h3>

                <div class="striked" v-if="showStriked">₪{{tutorData.price}}</div>

                <v-layout row class="moreDetails" :class="{'mt-3': !showStriked}" align-center>
                    <div column class="price-box column">
                        <template>
                            <span v-if="showStriked" class="title font-weight-bold">&#8362;{{discountedPrice}}</span>
                            <span v-else class="font-weight-bold">&#8362;{{tutorData.price}}</span>
                        </template>
                        <div class="caption" v-language:inner="'resultTutor_hour'"></div>
                    </div>

                    <v-layout column align-center class="user-rates">
                        <userRating class="rating-holder" :rating="tutorData.rating" :showRateNumber="false" />
                        <div class="caption reviews" v-html="$Ph(`resultTutor_reviews_many`, reviewsPlaceHolder(tutorData.reviewsCount,tutorData.reviews))"></div>
                    </v-layout>
                    
                    <template>
                        <!-- card-a -->
                        <v-btn class="btn-chat ab-cardA" color="#4452fc" @click.prevent="sendMessage(tutorData)">
                            <iconChat/>
                        </v-btn>

                        <!-- card-b -->
                        <v-layout column align-center class="ab-cardB user-classes subheading">
                            <div>{{tutorData.classes}}</div>
                            <div v-language:inner="'resultTutor_classes'"></div>
                        </v-layout>
                    </template>

                </v-layout>
            </div>
        </div>

        <v-layout class="tutor-bio">{{tutorData.bio}}</v-layout>

        <v-layout row class="btn-footer ab-cardB">
            <div class="send-msg text-xs-center text-truncate" >
                <v-btn 
                    round 
                    small 
                    color="#848bbc" 
                    depressed 
                    class="white--text caption py-3 px-2" 
                    @click.prevent="sendMessage(tutorData)" 
                    :class="{'tutor-btn': isTutor}" 
                    v-html="$Ph('resultTutor_send_button', tutorData.name)">
                </v-btn>
            </div>
            <div class="more-documents text-xs-center text-truncate card-transform" v-if="isTutor">
                <v-btn round small color="#5158af" depressed class="caption py-3 px-2" v-language:inner="'resultTutor_btn_more_doc'"></v-btn>
            </div>
        </v-layout>
    </router-link>
</template>

<script>
import analyticsService from '../../../../services/analytics.service';
import utilitiesService from "../../../../services/utilities/utilitiesService";
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import iconChat from './icon-chat.svg';
import chatService from '../../../../services/chatService';
import {mapActions, mapGetters} from 'vuex';

export default {
    components: {
        userRating,
        iconChat
    },
    props: {
        tutorData: {},
    },
    data() {
        return {
            isLoaded: false,
            minimumPrice: 55,
            discountAmount: 70
        }
    },
    computed: {
        ...mapGetters(['accountUser']),
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
                return "./images/placeholder-profile.png";
            }
        },
        showStriked() {
            let price = this.tutorData.price;
            return price > this.minimumPrice;
        },
    },
    methods: {
        ...mapGetters(['getProfile']),
        ...mapActions(['setActiveConversationObj', 'openChatInterface','updateRequestDialog','updateCurrTutor']),
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
            event.target.src = "./images/placeholder-profile.png";
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
        // image stretching
        max-height: 78px; 
        min-height: 78px;
        .top-card-wrap {
            width:100%;
            max-height:83px;
        }
    }
    .user-image {
        border-radius: 4px;
    }
    .tutor-no-img {
        width: 64px;
        height: auto;
    }

    .tutor-name {
        color: @purple;
        max-width: 200px; // for eplipsis purpose
    }

    .striked {
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

    .moreDetails {
        color: @purple;
        .price-box {
            font-size: 22px;
        }
        .rating-holder {
            div {
                margin: 0 !important; //vuetify
                i {
                    font-size: 16px !important; //vuetify
                }
            }
        }
        .user-rates {
            .reviews {
                color: #4452fc;
            }
        }
    }
    .btn-chat {
        border-radius: 16px 0 0 16px;
        min-width: 20px;
        margin-right: -12px;
        div {
            justify-content: normal;
        }
    }
    .tutor-bio {
        min-height: 42px;
        min-height: 42px;
        .giveMeEllipsis(2, 23px);
        display: block;
        color: @purple;
    }
    .btn-footer {
        justify-content: space-evenly;
        width: 100%;
        .send-msg {
            button {
                line-height: 0;
                color: @purple;
                text-transform: lowercase;
                .widthMinMax(200px);
                &.tutor-btn {
                    .widthMinMax(140px);
                }
            }
        }
        .more-documents {
            button {
                border: solid 1px #5158af;
                background: #fff !important;
                line-height: 0;
                text-transform: lowercase;
                .widthMinMax(140px);
            }
        }
    }
}

</style>
