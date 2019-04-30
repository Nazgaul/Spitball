<template>

    <v-card class="tutor-card-wrap px-3 py-12 mb-2 elevation-0 cursor-pointer "
            @click.native="goToTutorProfile(tutorData.userId)">
        <div class="section-tutor-info">
            <v-layout>
                <v-flex class="image-wrap d-flex" shrink >
                    <img class="tutor-image" :src="userImageUrl" alt="">
                </v-flex>
                <v-flex>
                    <v-layout align-start row wrap fill-height>
                        <v-flex xs12 grow>
                            <v-layout row justify-space-between align-baseline class="top-section">
                                <v-flex grow class="">
                                    <span class="tutor-name font-weight-bold subheading"
                                          v-line-clamp:18="1">{{tutorData.name}}</span>
                                </v-flex>
                                <v-flex shrink>
                                    <span class="font-weight-bold pricing">₪{{tutorData.price}}</span>
                                    <span class="pricing caption">
                            <span v-language:inner>resultTutor_hour</span>
                        </span>
                                </v-flex>
                            </v-layout>
                        </v-flex>
                        <v-flex class="bottom-section">
                            <!--<userRating-->
                                    <!--:rating="tutorData.rating"-->
                                    <!--:starColor="'#ffca54'"-->
                                    <!--:rateNumColor="'#43425D'"-->
                                    <!--:size="'20'"-->
                                    <!--:rate-num-color="'#43425D'"></userRating> -->
                            <userRating
                                    class="rating-holder"
                                    :rating="4.86"
                                    :starColor="'#ffca54'"
                                    :rateNumColor="'#43425D'"
                                    :size="'20'"
                                    :rate-num-color="'#43425D'"></userRating>
                            <v-flex shrink class="tutor-courses">
                                <span class="blue-text body-1" v-line-clamp:18="1">{{tutorData.courses}}</span>
                            </v-flex>
                        </v-flex>

                    </v-layout>
                </v-flex>
            </v-layout>
        </div>
    </v-card>
</template>

<script>
    import userRank from '../../../helpers/UserRank/UserRank.vue';
    import userRating from '../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue';
    import commentIcon from '../../../../font-icon/message-icon.svg';
    import userAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';
    import chatService from '../../../../services/chatService';
    import { mapActions, mapGetters } from 'vuex';
    import utilitiesService from "../../../../services/utilities/utilitiesService";

    export default {
        name: "tutorCard",
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
                    return utilitiesService.proccessImageURL(this.tutorData.image, 76, 96);
                } else {
                    return './images/placeholder-profile.png';
                }

            }
        },
        methods: {
            ...mapActions(['updateLoginDialogState', 'setActiveConversationObj', 'openChatInterface', 'changeFooterActiveTab']),
            goToTutorProfile(userId) {
                this.$router.push({name: 'profile', params: {id: userId}});
            },
            sendMessage() {
                if(this.accountUser == null) {
                    this.updateLoginDialogState(true);
                } else {
                    let currentConversationObj = chatService.createActiveConversationObj(this.tutorData);
                    this.setActiveConversationObj(currentConversationObj);
                    let isMobile = this.$vuetify.breakpoint.smAndDown;
                    this.openChatInterface();
                }
            }
        }
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .tutor-card-wrap {
        border-radius: 4px;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.13);
        min-width: 304px;
        /*&:first-child{*/
        /*border-radius:  0 0 4px 4px;*/
        /*}*/
        &.py-12 {
            padding: 12px 0;
        }
        .user-rating-val {
            font-weight: bold;
        }
        .tutor-name {
            color: @profileTextColor;
            line-height: 12px;
            word-break: break-all;
        }
        .image-wrap{
            margin-right: 12px;
        }
        .tutor-image {
            border-radius: 4px;
            width: 76px;
            height: 96px;
        }
        .top-section{

        }
        .bottom-section{
            justify-self: flex-end;
            margin-top: auto
        }
        .rating-holder{
            margin-bottom: 12px;
        }
        .pricing {
            font-family: @fontOpenSans;
            color: @profileTextColor;
            font-size: 18px;
            line-height: 16px;
        }
        .small-text {
            font-size: 10px;
        }
        .tutor-text {
            font-family: @fontOpenSans;
            color: @profileTextColor;
            line-height: 1.33;
        }
        .tutor-courses {
            color: @colorBlue;
            max-width: 140px;
        }
        .rounded {
            min-height: 32px;
            min-width: 32px;
            border-radius: 50%;
            background-color: @profileTextColor;
        }

    }
</style>