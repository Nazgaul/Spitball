<template>
    <v-container class="profile-bio">
        <v-layout>
            <v-flex xs12>
                <v-card class="px-3 py-4">
                    <v-layout>
                        <v-flex class="mr-3">
                           <user-image></user-image>
                        </v-flex>
                        <v-flex>
                                <div>
                                    <div class="user-name mb-2">Supermodel</div>
                                    <div class="user-university">Foster the People</div>
                                </div>
                            <div class="mt-5">
                                <userAboutMessage></userAboutMessage>
                            </div>
                        </v-flex>
                    </v-layout>
                </v-card>
            </v-flex>

        </v-layout>
    </v-container>
</template>

<script>
    import userImage from './bioParts/userImage/userImage.vue';
    import userAboutMessage from './bioParts/userAboutMessage.vue';
    export default {
        name: "profileBio",
        components: {userImage, userAboutMessage },
        data() {
            return {
                userStars: 3
            }
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .profile-bio{
        .user-name{
            font-family: @fontOpenSans;
            font-size: 18px;
            font-weight: bold;
            letter-spacing: -0.4px;
            color: @profileTextColor;
        }
        .user-university{
            font-size: 14px;
            letter-spacing: -0.3px;
            color: @textColor;
        }
    }

</style>