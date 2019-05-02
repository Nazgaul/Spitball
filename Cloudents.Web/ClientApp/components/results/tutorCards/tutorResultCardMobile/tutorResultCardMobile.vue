<template>
    <v-card class="tutor-card-wrap px-3 py-12 mb-2 elevation-0 cursor-pointer " :class="{'list-tutor-card': isInTutorList}"
            @click.native="goToTutorProfile(tutorData.userId)">
        <div class="section-tutor-info">
            <v-layout>
                <v-flex class="image-wrap d-flex" shrink >
                    <img class="tutor-image" :src="userImageUrl" alt="tutor user image">
                </v-flex>
                <v-flex>
                    <v-layout align-start row wrap fill-height>
                        <v-flex xs12 grow>
                            <v-layout row justify-space-between align-baseline class="top-section">
                                <v-flex grow class="">
                                    <span class="tutor-name"
                                          v-line-clamp:18="1">{{tutorData.name}}</span>
                                </v-flex>
                                <v-flex shrink>
                                    <span class="font-weight-bold pricing pr-1">₪{{tutorData.price}}</span>
                                    <span class="pricing caption">
                            <span v-language:inner>resultTutor_hour</span>
                        </span>
                                </v-flex>
                            </v-layout>
                        </v-flex>
                        <v-flex class="bottom-section">
                            <userRating
                            v-if="tutorData.reviews > 0"
                            class="rating-holder"
                                    :rating="tutorData.rating"
                                    :starColor="'#ffca54'"
                                    :rateNumColor="'#43425D'"
                                    :size="isInTutorList ? '16' : '20'"
                                    :rate-num-color="'#43425D'"></userRating>

                            <v-flex shrink class="tutor-courses">
                                <span class="blue-text" v-line-clamp:18="1">{{tutorData.courses}}</span>
                            </v-flex>
                        </v-flex>

                    </v-layout>
                </v-flex>
            </v-layout>
        </div>
    </v-card>
</template>

<script>
    import userRating from '../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue';
    import chatService from '../../../../services/chatService';
    import { mapActions, mapGetters } from 'vuex';
    import utilitiesService from "../../../../services/utilities/utilitiesService";

    export default {
        name: "tutorCard",
        components: {
            userRating,
        },
        data() {
            return {};
        },
        props: {
            tutorData: {},
            isInTutorList:{
                type: Boolean,
                default: false
            }
        },
        computed: {
            ...mapGetters(['accountUser', 'getConversations']),
            userImageUrl() {
                if(this.tutorData.image) {
                    let size = this.isInTutorList ? [56, 64] : [76, 96];
                    return utilitiesService.proccessImageURL(this.tutorData.image, ...size, 'crop');
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
            //         let isMobile = this.$vuetify.breakpoint.smAndDown;
            //         this.openChatInterface();
            //     }
            // }
        }
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .tutor-card-wrap {
        border-radius: 4px;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.13);
        min-width: 304px;
        // START styles for card rendered inside tutor list only
        &.list-tutor-card{
            .tutor-name {
                font-size: 14px;
                font-weight: 500;
                line-height: 14px;
            }
            .rating-holder{
                margin-bottom: 0;
            }
            .tutor-courses {
                font-size: 12px;
            }
            .tutor-image {
                width: 56px;
                height: 64px;
            }
        }
        // END styles for card rendered inside tutor list only
        &.py-12 {
            padding: 12px 0;
        }
        .user-rating-val {
            font-weight: bold;
        }
        .tutor-name {
            color: @profileTextColor;
            line-height: 20px;
            word-break: break-all;
            font-size: 16px;
            font-weight: bold;
        }
        .image-wrap{
            margin-right: 12px;
        }
        .tutor-image {
            border-radius: 4px;
            width: 76px;
            height: 96px;
        }
        .bottom-section{
            justify-self: flex-end;
            margin-top: auto
        }
        .rating-holder{
            margin-bottom: 8px;
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
        .tutor-courses {
            color: @colorBlue;
            max-width: 140px;
            font-size: 14px;
            min-height: 19px; //keep it to prevent rating stars shift
        }

    }
</style>