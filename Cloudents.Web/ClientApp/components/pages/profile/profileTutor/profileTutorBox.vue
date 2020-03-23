<template>
    <v-sheet class="profileTutorBox" color="#fff" elevation="6">

        <v-container>
            <div class="d-flex">
                <div class="mr-3">
                    <userAvatarRect
                        class="pUb_dS_img"
                        :userName="user.name"
                        :userImageUrl="user.image"
                        :width="130"
                        :height="160"
                        :userId="user.id"
                        :fontSize="36"
                        :borderRadius="8"
                    />
                </div>
                <div class="flex-grow-1">
                    <div class="nameWrap">
                        <span>{{$t('profileTutor_name')}}</span>
                        <span class="name">{{user.name}}</span>
                    </div>
                    <div></div>
                    <div></div>
                    <div></div>
                    <div></div>
                </div>
            </div>

            <v-row>
                <v-col cols="12" sm="3">3</v-col>
                <v-col cols="12" sm="3">4</v-col>
                <v-col cols="12" sm="3">5</v-col>
                <v-col cols="12" sm="3">6</v-col>
            </v-row>
        </v-container>

    </v-sheet>
</template>

<script>
import userAvatarRect from '../../../helpers/UserAvatar/UserAvatarRect.vue'
export default {
    components: {
        userAvatarRect
    },
    computed: {
        user() {
            return this.$store.getters.accountUser
        }
    }
}
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    @import '../../../../styles/colors.less';
    .profileTutorBox {
        max-width: 700px;
        width: 100%;
        margin: 0 auto;
        margin-top: -140px;

        .nameWrap {
            color: @global-purple;
        }
    }
</style>