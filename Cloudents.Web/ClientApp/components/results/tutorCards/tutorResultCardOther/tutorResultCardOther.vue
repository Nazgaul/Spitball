<template>
    <router-link @click.native.prevent="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId, name:tutorData.name}}">
        <v-layout class="tutor-result-card-other pa-2 mb-3" row wrap>
            <v-layout row class="mb-2">
                <img :class="[isUserImage ? '' : 'tutor-no-img']" class="mr-2 user-image" @error="onImageLoadError" @load="loaded" :src="userImageUrl" :alt="tutorData.name">
                <div>
                    <h3 class="subheading font-weight-bold tutor-name text-truncate mb-1">{{tutorData.name}}</h3>
                    <div class="striked"> &#8362;{{discountedPrice}}</div>
                    <v-layout row class="moreDetails" align-baseline>
                        <v-layout column class="mr-5 price-box">
                            <span class="title price font-weight-bold"><span class="subheading font-weight-bold">&#8362;</span>{{tutorData.price}}</span>
                            <div class="subheading">for hour</div> <!-- v-language:inner="'tutorCard-hours'" -->
                        </v-layout>

                        <v-layout column align-center>
                            <userRating class="rating-holder mt-2 mb-1" :rating="tutorData.rating" :showRateNumber="false" />  <!-- :size="isInTutorList ? '16' : '20'" -->
                            <router-link to=""><div class="caption">reviews {{tutorReviews}}</div></router-link> <!-- v-language:inner="'tutorCard-reviews'" -->
                        </v-layout>
                        
                        <template>
                            <!-- *****A***** -->

                            <!-- <v-btn class="btn-chat" color="#4452fc" @click.prevent="">
                                <iconChat/>
                            </v-btn> -->

                            <!-- *****B***** -->
                            <v-layout column align-center class="ml-4">
                                <div class="subheading font-weight-regular">32</div> <!-- number of classes -->
                                <div>classes</div> <!-- v-language:inner="'tutorCard-classes'" -->
                            </v-layout>
                        </template>

                    </v-layout>
                </div>
            </v-layout>
            <v-layout class="tutor-bio">
                <p class="mb-2">{{readMore(tutorData.bio)}}</p>
            </v-layout>
            <v-layout row class="btn-footer">
                <div class="send-msg text-xs-center">
                    <v-btn round small color="#848bbc" depressed class="white--text caption">שלחו {{tutorData.name}} הודעה</v-btn> <!-- v-language:inner="'tutorCard-send-button'" -->
                </div>
                <!-- tutor document button -->
                <div class="more-documents text-xs-center" v-if="isTutor">
                    <v-btn round small color="#5158af" depressed class="caption">עוד מסמכים שלי</v-btn> <!-- v-language:inner="'tutorCard-more-document'" -->
                </div>
            </v-layout>
        </v-layout>
    </router-link>
</template>

<script>
import utilitiesService from "../../../../services/utilities/utilitiesService";
import userRating from "../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue";
import iconChat from './icon-chat.svg';

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
            if(this.tutorData) {
                return this.tutorData.isTutor;
            }
        },
        isTutorData() {
            return this.tutorData ? true : false;
        },
        isTutorName() {
            return this.tutorData ? this.tutorData.name : null;
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
        }
    },
    methods: {
        loaded() {
          this.isLoaded = true;
        },
        tutorCardClicked() {
            
        },
        onImageLoadError(event) {
            event.target.src = "./images/placeholder-profile.png";
        },
        readMore(text) {
            let pArr = document.querySelector('.tutor-bio > p');
            // console.log(pArr);
            
            // let lines = text.split("\n").length;
            return text;
        }
    }
}
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

@purple: #43425d;

.tutor-result-card-other {
    max-width: 330px;
    // max-height: 160px;
    border-radius: 4px;
    position: relative;
    overflow: hidden;
    background: #fff;
    .user-image {
        border-radius: 4px;
        object-fit: contain;
    }
    .tutor-no-img {
        width: 66px;
        height: 92px;
    }

    .tutor-name {
        color: @purple;
        max-width: 200px;
    }

    .striked{
        max-width: fit-content;
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
            max-width: 36px;
            div {
                width: max-content;
            }
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
            width: 100%;
            button {
                padding: 15px 30px;
                line-height: 0;
                color: @purple;
            }
        }
        .more-documents {
            button {
                padding: 15px 30px;
                border: solid 1px #5158af;
                background: #fff !important;
                line-height: 0;
                color: #5158af;
            }
        }
    }
    
}

</style>
