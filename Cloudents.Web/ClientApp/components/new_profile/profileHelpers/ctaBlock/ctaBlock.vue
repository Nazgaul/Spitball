<template>
    <v-layout class="cta-section my-5">
        <v-flex xs12>
            <v-layout align-center justify-center column>
                <v-flex v-if="isTutorProfile && !isMyProfile" xs12 sm4  class="text-center">
                    <div class="cta-title mb-4 subtitle-1 font-weight-bold" v-language:inner>profile_cta_section_title
                    </div>
                </v-flex>
                <v-flex class="text-center  mt-3 ">
                    <contactBtn v-if="isTutorProfile && !isMyProfile"></contactBtn>
                    <becomeTutorBtn v-else-if="!isTutorProfile && isMyProfile && !tutorPending"></becomeTutorBtn>
                </v-flex>
            </v-layout>
        </v-flex>
    </v-layout>
</template>

<script>
    import {mapGetters} from 'vuex';
    import contactBtn from '../../profileHelpers/contactBtn/contactBtn.vue';
    import becomeTutorBtn from '../../profileHelpers/becomeTutorBtn/becomeTutorBtn.vue';

    export default {
        name: "ctaBlock",
        components: {contactBtn, becomeTutorBtn},
        computed: {
            ...mapGetters(['isTutorProfile', 'accountUser', 'getProfile', 'getIsTutorState']),
            isMyProfile() {
                if (!!this.getProfile) {
                    // return false
                    return this.accountUser && this.accountUser.id && this.getProfile ? this.getProfile.user.id == this.accountUser.id : false;
                }
            },
            tutorPending(){
               return this.getIsTutorState && this.getIsTutorState ==='pending';
            }
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .cta-section {
        .cta-title {
            color: @global-purple;
            line-height: 0.85;
        }
    }


</style>