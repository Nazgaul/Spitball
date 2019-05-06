<template>
    <div class="tutor-card-wrap-desk cursor-pointer"  @click="goToTutorProfile(tutorData.userId)">
        <v-layout>
            <div class="section-tutor-info">
                <v-layout>
                    <v-flex class="mr-3" shrink>
                        <img class="tutor-image" :src="userImageUrl" alt="">
                    </v-flex>
                    <v-flex>
                        <v-layout align-start justify-space-between column fill-height>
                            <v-flex shrink class="pb-3">
                                <span class="tutor-name font-weight-bold" v-line-clamp:18="1">{{tutorData.name}}</span>
                            </v-flex>
                            <v-flex grow>
                                <span class="tutor-about subheading" v-line-clamp:18="2">
                                    {{tutorData.bio}}
                                </span>
                            </v-flex>
                            <v-flex shrink class="tutor-courses">
                                <span class="blue-text subheading" v-line-clamp:18="1">{{tutorData.courses}}</span>
                            </v-flex>
                        </v-layout>
                    </v-flex>
                </v-layout>
            </div>
            <v-layout row wrap align-start justify-start grow
                      class="price-review-column section-tutor-price-review ml-1">
                <v-flex xs12 sm12 md12 grow>
                    <v-flex xs12 sm12 md12 shrink>
                        <span class="font-weight-bold headline pricing">₪{{tutorData.price}}&nbsp;</span>
                        <span class="pricing caption">
                            <span v-language:inner>resultTutor_hour</span>
                        </span>
                    </v-flex>
                    <v-flex xs12 sm12 md12 shrink class="pt-2">
                        <userRating
                                v-if="tutorData.reviews > 0"
                                :rating="tutorData.rating"
                                :starColor="'#ffca54'"
                                :rateNumColor="'#43425D'"
                                :size="'24'"
                                :rate-num-color="'#43425D'"></userRating>
                    </v-flex>
                    <v-flex xs12 sm12 md12 class="pt-1" shrink>
                            <span class="blue-text body-2" v-if="tutorData.reviews > 0">{{tutorData.reviews}}
                                <span v-if="tutorData.reviews > 1" v-language:inner>resultTutor_reviews_many</span>
                                <span v-else v-language:inner>resultTutor_review_one</span>
                            </span>
                        <span class="body-2" v-else v-language:inner>resultTutor_no_reviews</span>
                    </v-flex>
                </v-flex>

                <v-flex xs12 sm12 md12 class="d-flex btn-bottom-holder text-xs-center">
                    <v-btn style="max-width: 80%; margin: 0 auto;" round class="blue-btn rounded elevation-0 ma-0" block>
                        <span class="font-weight-bold text-capitalize" v-language:inner>resultTutor_btn_view</span>
                    </v-btn>
                </v-flex>
            </v-layout>
        </v-layout>
    </div>
</template>

<script>
    import userRank from '../../../helpers/UserRank/UserRank.vue';
    import userRating from '../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue';
    import commentIcon from '../../../../font-icon/message-icon.svg';
    import userAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';
    import { mapActions, mapGetters } from 'vuex';
    import utilitiesService from "../../../../services/utilities/utilitiesService";

    export default {
        name: "tutorResultCard",
        components: {
            userRank,
            userRating,
            commentIcon,
            userAvatar
        },
        data() {
            return {};
        },
        props: {
            tutorData: {},
        },
        computed: {
            ...mapGetters(['accountUser', 'getConversations']),
            userImageUrl() {
                if(this.tutorData.image) {
                    return utilitiesService.proccessImageURL(this.tutorData.image, 166, 186);
                } else {
                    return './images/placeholder-profile.png';
                }

            }
        },
        methods: {
            ...mapActions(['updateLoginDialogState', 'setActiveConversationObj', 'openChatInterface']),
            goToTutorProfile(userId) {
                this.$router.push({name: 'profile', params: {id: userId}});
            },
            // sendMessage() {
            //     if(this.accountUser == null) {
            //         this.updateLoginDialogState(true);
            //     } else {
            //         let currentConversationObj = chatService.createActiveConversationObj(this.tutorData);
            //         this.setActiveConversationObj(currentConversationObj);
            //         this.openChatInterface();
            //     }
            // }
        }
    };
</script>


<style lang="less">
    @import '../../../../styles/mixin.less';

    .tutor-card-wrap-desk {
        width: 100%;
        margin: 0 auto;
        .rating-number{
            font-weight: bold;
        }
        .section-tutor-info {
            width: @cellWidth;
            background-color: @color-white;
            border-radius: 4px;
            padding: 16px;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
            @media (max-width: @screen-xs) {
                padding: 12px;
            }
        }
        .user-rating-val {
            font-weight: bold;
        }
        .section-tutor-price-review {
            width: 260px;
            background-color: @color-white;
            border-radius: 4px;
            padding: 16px;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
            @media (max-width: @screen-xs) {
                padding: 12px;
            }
        }
        .btn-bottom-holder {
            justify-self: flex-end;
            margin-top: auto;
            justify-content: center;

        }
        .blue-btn {
            background-color: @colorBlue !important;
            color: @color-white;
            height: 42px;
        }
        .tutor-image {
            border-radius: 4px;
            width: 166px;
            height: 186px;
        }
        .tutor-name {
            opacity: 0.9;
            font-size: 18px;
            letter-spacing: -0.4px;
            color: @textColor;
        }
        .tutor-about {
            color: @profileTextColor;
        }
        .blue-text {
            color: @colorBlue;
        }
        .tutor-courses {
            max-width: 180px !important;
        }
    }

</style>