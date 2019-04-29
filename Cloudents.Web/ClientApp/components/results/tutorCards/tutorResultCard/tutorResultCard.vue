<template>
    <div class="tutor-card-wrap-desk">
        <v-layout>
            <div class="section-tutor-info">
                <v-layout>
                    <v-flex class="mr-3">
                        <img class="tutor-image" :src="userImageUrl" alt="">
                    </v-flex>
                    <v-flex>
                        <v-layout align-start justify-space-between column fill-height>
                            <v-flex shrink class="pb-3">
                                <span class="tutor-name font-weight-bold">{{tutorData.name}}</span>
                            </v-flex>
                            <v-flex grow>
                                <span class="tutor-about subheading">
                                    Lorem ipsum dolor amet plaid shaman vinyl hashtag lyft stumptown readymade tofu flexitarian.
                                </span>
                            </v-flex>
                            <v-flex shrink class="text-truncate tutor-courses">
                                <span class="blue-text subheading">{{tutorData.courses}}</span>
                            </v-flex>
                        </v-layout>
                    </v-flex>
                </v-layout>
            </div>
            <div class="section-tutor-price-review ml-1">
                    <v-layout row wrap>
                        <v-flex xs12 sm12 md12>
                        <span class="pricing">
                        <span class="font-weight-medium headline  pricing">₪{{tutorData.price}}</span>
                        <span class="pricing caption">/&nbsp;<span >hour</span></span>
                        </span>
                        </v-flex>
                        <v-flex xs12 sm12 md12>
                            <userRating
                                    :rating="tutorData.rating"
                                    :starColor="'#ffca54'"
                                    :rateNumColor="'#43425D'"
                                    :size="'20'"
                                    :rate-num-color="'#43425D'"></userRating>

                        </v-flex>
                        <v-flex xs12 sm12 md12>
                            <span class="blue-text body-2">{{tutorData.reviews}}
                            <span>Reviews</span>
                            </span>
                        </v-flex>
                        <v-flex xs12 sm12 md12></v-flex>
                    </v-layout>
            </div>
        </v-layout>
    </div>
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
                if(this.tutorData.image){
                    return utilitiesService.proccessImageURL(this.tutorData.image, 166, 186)
                }else{
                    return './images/placeholder-profile.png'
                }

            }
        },
        methods: {
            ...mapActions(['updateLoginDialogState', 'setActiveConversationObj', 'openChatInterface', 'changeFooterActiveTab']),
            goToTutorProfile(userId){
                this.$router.push({name:'profile', params:{id: userId}})
            },
            sendMessage() {
                if(this.accountUser == null) {
                    this.updateLoginDialogState(true);
                } else {
                    let currentConversationObj = chatService.createActiveConversationObj(this.tutorData);
                    this.setActiveConversationObj(currentConversationObj);
                    this.openChatInterface();
                }
            }
        }
    };
</script>


<style lang="less">
    @import '../../../../styles/mixin.less';

    .tutor-card-wrap-desk {
        width: 100%;
        margin: 0 auto;
        .section-tutor-info{
            max-width: @cellWidth;
            background-color: @color-white;
            border-radius: 4px;
            padding: 16px;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
            @media (max-width: @screen-xs) {
                padding: 12px;
            }
        }
        .section-tutor-price-review{
            max-width: 260px;
            background-color: @color-white;
            border-radius: 4px;
            padding: 16px;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.24);
            @media (max-width: @screen-xs) {
                padding: 12px;
            }
        }

        .tutor-image{
            border-radius: 4px;
            width: 166px;
            height: 186px;
        }
        .tutor-name{
            opacity: 0.9;
            font-size: 18px;
            letter-spacing: -0.4px;
            color: @textColor;
        }
        .tutor-about{
            color: @profileTextColor;
        }
        .blue-text{
            color: @colorBlue;
        }
        .tutor-courses{
            max-width: 180px!important;
        }
    }

</style>