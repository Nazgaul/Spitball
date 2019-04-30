<template>

    <v-card class="tutor-card-wrap px-3 pb-small mb-2 elevation-0 cursor-pointer " @click.native="goToTutorProfile(tutorData.userId)">
        <v-layout align-center justify-space-between class="pt-small pb-1">
            <v-flex shrink class="mr-2 tutor-name font-weight-bold caption">
                {{tutorData.name}}
            </v-flex>
            <!-- <v-flex grow>
                <user-rank class="tutor-rank" :score="tutorData.score"></user-rank>
            </v-flex> -->
            <v-flex @click.prevent.stop="sendMessage" class="rounded" shrink style="z-index: 3;">
                <commentIcon class="chat-icon"></commentIcon>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center class="pb-2">
            <v-flex shrink>

                <user-avatar size="40"
                             :userImageUrl="userImageUrl"
                             :user-id="tutorData.userId"
                             :user-name="tutorData.name"/>
            </v-flex>
            <v-flex class="pl-3 tutor-text caption" v-line-clamp="2">
                {{tutorData.courses}}
            </v-flex>
        </v-layout>
        <v-divider></v-divider>
        <v-layout class="pt-1" align-center justify-center>
            <v-flex xs6 grow>
                <userRating
                        :rating="tutorData.rating"
                        :starColor="'#43425D'"
                        :rateNumColor="'#43425D'"
                        :size="'15'"
                        :rate-num-color="'#43425D'"></userRating>
            </v-flex>
            <v-flex xs8 class="text-xs-right">
            <span class="pricing">
                 <span class="caption pricing">₪</span>
                <span class="font-weight-medium subheading pricing">{{tutorData.price}}</span>
                <!--<span class="small-text pricing" v-language:inner>app_currency_dynamic</span>-->
                 <span class="pricing small-text">/&nbsp;<span v-language:inner>tutorList_per_hour</span></span>
            </span>
            </v-flex>
        </v-layout>
    </v-card>

</template>

<script>
    import userRank from '../UserRank/UserRank.vue';
    import userRating from '../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue';
    import commentIcon from '../../../font-icon/message-icon.svg';
    import userAvatar from '../../helpers/UserAvatar/UserAvatar.vue';
    import chatService from '../../../services/chatService';
    import { mapActions, mapGetters } from 'vuex';

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
                return this.tutorData.image;
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
    @import '../../../styles/mixin.less';

    .pb-small {
        padding-bottom: 12px;
    }

    .tutor-card-wrap {
        border-radius: 4px;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.13);
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
        .tutor-rank {
            font-size: 10px;
            margin-top: 0;
            line-height: 14px;
            width: 60px;
        }
        .pricing {
            font-family: @fontOpenSans;
            color: @purpleLight;
        }
        .small-text {
            font-size: 10px;
        }
        .tutor-text {
            font-family: @fontOpenSans;
            color: @profileTextColor;
            line-height: 1.33;
        }
        .rounded {
            min-height: 32px;
            min-width: 32px;
            border-radius: 50%;
            background-color: @profileTextColor;
        }
        .chat-icon {
            margin: 8px auto;
            display: block;
            fill: @color-white;
            cursor: pointer;
            width: 20px;
            height: 16px;
        }
    }
</style>