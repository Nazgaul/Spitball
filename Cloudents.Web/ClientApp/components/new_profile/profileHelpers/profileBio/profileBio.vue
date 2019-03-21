<template>
    <v-layout class="profile-bio" align-center>
        <v-flex xs12>
            <v-card class="px-3 py-4">
                <v-layout v-bind="xsColumn" align-start>
                    <v-flex  order-xs2 order-sm1 order-md1>
                        <user-image></user-image>
                    </v-flex>
                    <v-flex xs12 order-xs1 order-sm2 order-md2 :class="[$vuetify.breakpoint.smAndUp ?  'pl-4' : '']">
                        <div>
                            <div class="user-name mb-2">
                                <div class="d-flex align-start">
                                    <span class="line-height-1">
                                    {{userName}}
                                    </span>

                                </div>
                                <div class="d-flex align-start">
                                    <userRank class="ml-3" :score="userScore"></userRank>
                                </div>

                            </div>
                            <div class="user-university">{{university}}</div>
                        </div>
                        <div class="mt-5">
                            <userAboutMessage></userAboutMessage>
                        </div>
                    </v-flex>
                </v-layout>
            </v-card>
        </v-flex>
    </v-layout>
</template>

<script>
    import { mapGetters } from 'vuex';
    import userImage from './bioParts/userImage/userImage.vue';
    import userAboutMessage from './bioParts/userAboutMessage.vue';
    import userRank from '../../../helpers/UserRank/UserRank.vue'
    export default {
        name: "profileBio",
        components: {userImage, userAboutMessage, userRank},
        data() {
            return {
                userStars: 3
            }
        },
        computed: {
            ...mapGetters(['getProfile']),
            xsColumn() {
                const xsColumn = {};
                if (this.$vuetify.breakpoint.xsOnly) {
                    xsColumn.column = true;

                }
                return xsColumn
            },
            university() {
                if (this.getProfile && this.getProfile.user) {
                    return this.getProfile.user.universityName;
                }
            },
            userName(){
                if (this.getProfile && this.getProfile.user) {
                    return this.getProfile.user.name;
                }
            },
            userScore(){
                if (this.getProfile && this.getProfile.user) {
                    return this.getProfile.user.score;
                }
            }
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .profile-bio {
        max-width: 760px;
        .user-name {
            display: flex;
            flex-direction: row;
            align-items: flex-end;
            font-family: @fontOpenSans;
            font-size: 18px;
            font-weight: bold;
            letter-spacing: -0.4px;
            color: @profileTextColor;
        }
        .line-height-1{
            line-height: 1;
        }
        .user-university {
            font-size: 14px;
            letter-spacing: -0.3px;
            color: @textColor;
        }
    }

</style>