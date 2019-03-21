<template>
    <v-layout class="bio-text-container" column>
        <v-flex>
            <h4 class="intro-name">Hey there, I’m {{userName}}!</h4>
        </v-flex>
        <v-flex class="pt-3">
            <span class="bio-about-intro">{{userDescription}}</span>
        </v-flex>
    </v-layout>
</template>

<script>
    import { mapGetters } from 'vuex';
    export default {
        name: "userAboutMessage",
        data() {
            return {
            }
        },
        computed: {
            ...mapGetters(['getProfile']),
            userDescription() {
                if(this.getProfile && this.getProfile.user){
                    return this.getProfile.user.description;
                }
            },
            userName(){
                if(this.getProfile && this.getProfile.user){
                    return this.getProfile.user.name;
                }
            },
        },

    }
</script>

<style lang="less">
    @import '../../../../../styles/mixin.less';
    .bio-text-container{
        .intro-name{
            font-family: @fontOpenSans;
            font-size: 30px;
            font-weight: 600;
            line-height: 0.57;
            color: @profileTextColor;
        }
        .bio-about-intro{
            font-family: @fontOpenSans;
            font-size: 22px;
            font-style: italic;
            line-height: 1.36;
            letter-spacing: normal;
            color: @profileTextColor;
        }
    }

</style>