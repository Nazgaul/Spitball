<template>

    <v-card class="tutor-card-wrap px-3 pb-small mb-2 elevation-0 cursor-pointer " @click.native="goToTutorProfile(tutorData.userId)">
        <v-layout align-start justify-space-between class="pt-small pb-1">
            <v-flex shrink class="mr-2">
                <!--<v-flex xs2 sm2 md2 shrink>-->
                    <img class="tutor-image" :src="userImageUrl" alt="">
                <!--</v-flex>-->
            </v-flex>
            <v-flex @click.prevent.stop="sendMessage" xs10 sm10 md10>
                <v-layout row wrap align-start >
                    <v-flex xs6 sm6 md6 grow class="tutor-name font-weight-bold body-2">
                        <span>{{tutorData.name}}</span>
                    </v-flex>
                    <v-flex shrink xs6 sm6 md6 class="text-xs-right text-sm-right">
                        <span class="pricing">
                        <span class="font-weight-medium  pricing">₪{{tutorData.price}}</span>
                        <span class="pricing caption">/&nbsp;<span v-language:inner>tutorList_per_hour</span></span>
                        </span>
                    </v-flex>
                </v-layout>
                <v-layout column class="pt-1">
                    <v-flex xs12 sm12 md12 class="pb-1">
                        <userRating
                        :rating="tutorData.rating"
                        :starColor="'#ffca54'"
                        :rateNumColor="'#43425D'"
                        :size="'15'"
                        :rate-num-color="'#43425D'"></userRating>
                    </v-flex>

                    <v-flex xs4 sm4 md4 class="text-truncate pt-1" style="max-width: 140px">
                        <span class="tutor-courses caption">{{tutorData.courses}}</span>
                    </v-flex>
                </v-layout>

            </v-flex>
        </v-layout>
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
                if(this.tutorData.image){
                    return utilitiesService.proccessImageURL(this.tutorData.image, 56, 64)
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
                    let isMobile = this.$vuetify.breakpoint.smAndDown;
                    this.openChatInterface();
                }
            }
        }
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .pb-small {
        padding-bottom: 12px;
    }

    .tutor-card-wrap {
        border-radius: 4px;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.13);
        min-width: 304px;
        &:first-child{
            border-radius:  0 0 4px 4px;
        }

        .pt-small {
            padding-top: 12px;
        }
        .tutor-name {
            color: @profileTextColor;
            line-height: 12px;
            word-break: break-all;
        }
        .tutor-image{
            border-radius: 4px;
            width: 56px;
            height: 64px;
        }
        .tutor-rank {
            font-size: 10px;
            margin-top: 0;
            line-height: 14px;
            width: 60px;
        }
        .pricing {
            font-family: @fontOpenSans;
            color: @profileTextColor;
            font-size: 18px;
        }
        .small-text {
            font-size: 10px;
        }
        .tutor-text {
            font-family: @fontOpenSans;
            color: @profileTextColor;
            line-height: 1.33;
        }
        .tutor-courses{
            color: @colorBlue;
        }
        .rounded {
            min-height: 32px;
            min-width: 32px;
            border-radius: 50%;
            background-color: @profileTextColor;
        }

    }
</style>