<template>
    <router-link @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}">
        <v-layout class="tutor-result-card-other pa-2 mb-3 default-card" row wrap>
            <v-layout row class="mb-2">
                <img :class="[isUserImage ? '' : 'tutor-no-img']" class="mr-2 user-image" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
                <div>
                    <h3 class="subheading font-weight-bold tutor-name text-truncate mb-1">{{tutorData.name}}</h3>

                    <div class="striked" v-if="showStriked">₪{{tutorData.price}}</div>

                    <v-layout row class="moreDetails" :class="{'isprice': !showStriked}" align-baseline>
                        <div column class="price-box column">
                            <span v-if="showStriked">
                                <span class="title font-weight-bold">{{discountedPrice}}</span>
                                <span class="font-weight-bold">&#8362;</span>
                            </span>
                            <span v-else>
                                <span class="title font-weight-bold">{{tutorData.price}}</span>
                                <span class="font-weight-bold">&#8362;</span>
                            </span>
                            <div class="caption" v-language:inner="'resultTutor_hour'"></div>
                        </div>

                        <v-layout column align-center class="user-rates ml-4">
                            <userRating class="rating-holder mt-2 mb-1" :rating="tutorData.rating" :showRateNumber="false" />  <!-- :size="isInTutorList ? '16' : '20'" -->
                            <div class="caption reviews" v-html="$Ph(`resultTutor_reviews_many`, reviewsPlaceHolder(tutorData.reviewsCount || tutorData.reviews))"></div>
                        </v-layout>
                        
                        <template>
                            <!-- card-a -->
                            <v-btn class="btn-chat cardA" color="#4452fc" @click.prevent="openChatWindow">
                                <iconChat/>
                            </v-btn>

                            <!-- card-b -->
                            <v-layout column align-center class="ml-4 cardB user-classes subheading">
                                <div>32</div>
                                <div v-language:inner="'resultTutor_classes'"></div>
                            </v-layout>
                        </template>

                    </v-layout>
                </div>
            </v-layout>
            <v-layout class="tutor-bio">
                <p class="mb-2">{{tutorData.bio}}</p>
            </v-layout>
            <v-layout row class="btn-footer cardB">
                <div class="send-msg text-xs-center text-truncate" >
                    <v-btn 
                        round 
                        small 
                        color="#848bbc" 
                        depressed 
                        class="white--text caption" 
                        @click.prevent="openChatWindow" 
                        :class="{'tutor-btn': isTutor}" 
                        v-html="$Ph('resultTutor_send_button', tutorData.name)">
                    </v-btn>
                </div>
                <div class="more-documents text-xs-center text-truncate card-transform" v-if="isTutor">
                    <v-btn round small color="#5158af" depressed class="caption" v-language:inner="'resultTutor_btn_more_doc'"></v-btn>
                </div>
            </v-layout>
        </v-layout>
    </router-link>
</template>

<script>
import utilitiesService from "../../../../services/utilities/utilitiesService";
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import iconChat from './icon-chat.svg';
import {mapActions} from 'vuex';

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
        isTutor() {
            if(this.isTutorData) {
                return this.tutorData.isTutor;
            }
        },
        isTutorData() {
            return this.tutorData ? true : false;
        },
        isTutorName() {
            return this.isTutorData ? this.tutorData.name : null;
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
        tutorReviews() {
            return this.tutorData.reviewsCount || this.tutorData.reviews;
        },
        showStriked() {
            let price = this.tutorData.price;
            return price > this.minimumPrice;
        },
    },
    methods: {
        ...mapActions(['openChatInterface']),
        loaded() {
          this.isLoaded = true;
        },
        tutorCardClicked() {
            
        },
        onImageLoadError(event) {
            event.target.src = "./images/placeholder-profile.png";
        },
        reviewsPlaceHolder(reviews) {
            return reviews === 0 ? reviews.toString() : reviews
        },
        openChatWindow(){
            this.openChatInterface();
        },
    }
}
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

@purple: #43425d;

.tutor-result-card-other {
    &.default-card {
        .cardA {
            display: none;
        }
        .cardB {
            display: flex;
        }
    }
    .cardA {
            display: flex;
        }
        .cardB {
            display: none;
        }
    max-width: 330px;
    // max-height: 160px;
    border-radius: 4px;
    position: relative;
    overflow: hidden;
    background: #fff;
    .user-image {
        border-radius: 4px;
    }
    .tutor-no-img {
        width: 64px;
        height: auto;
    }

    .tutor-name {
        color: @purple;
        max-width: 200px;
    }

    .striked{
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
        &.isprice {
            margin-top: 17px;
        }
        color: @purple;
        .price-box {
            max-width: 90px;
            div {
                white-space: nowrap;
                max-width: 191px;
                text-overflow: ellipsis;
                overflow: hidden;
            }
        }
        .user-rates {
            min-width: 80px;
            .reviews {
                color: #4452fc;
            }
        }
        .user-classes {
            max-width: 67px;
            overflow: hidden;
            white-space: nowrap;
            text-overflow: ellipsis;
        }
    }

    .rating-holder {
        div {
            margin: 0 !important;
            i {
                font-size: 16px !important;
            }
        }
    }

    .btn-chat {
        position: absolute;
        border-radius: 0px;
        border-radius: 16px 0 0 16px;
        right: -50px;
        div {
            justify-content: normal;
        }
    }

    .tutor-bio {
        width: 100%;
        overflow: hidden;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-box-orient: vertical;
        -webkit-line-clamp: 2;
        line-height: 22px;
        max-height: 50px;
        min-height: 50px;
        color: @purple;
    }
    .btn-footer {
        justify-content: space-evenly;
        width: 100%;
        .send-msg {
            button {
                padding: 15px 12px;
                line-height: 0;
                color: @purple;
                text-transform: lowercase;
                min-width: 200px;
                max-width: 200px;
                &.tutor-btn {
                    min-width: 140px;
                    max-width: 140px;
                }
            }
        }
        .more-documents {
            button {
                padding: 15px 12px;
                border: solid 1px #5158af;
                background: #fff !important;
                line-height: 0;
                color: #5158af;
                text-transform: lowercase;
                min-width: 140px;
                max-width: 140px;
            }
        }
    }

    
}

</style>
