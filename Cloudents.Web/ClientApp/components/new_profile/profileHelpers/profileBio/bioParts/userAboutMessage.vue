<template>
    <v-layout class="bio-text-container" column>
        <v-flex>
            <h4 class="intro-name">
                <span class="icon-wrap" :class="[$vuetify.breakpoint.xsOnly ? 'mr-2' : 'ml-2']"><v-icon>sbf-hand-icon</v-icon></span>
                <span class="text-wrap">
                    <span v-language:inner>profile_hey_there</span>
                    <span>&nbsp;{{userName}}!</span>
                  </span>
            </h4>
        </v-flex>
        <v-flex :class="[$vuetify.breakpoint.smAndUp ? 'pt-3' : 'pt-1']">
            <span class="bio-about-intro">{{userDescription}}</span>
        </v-flex>
    </v-layout>
</template>

<script>
    import { mapGetters } from 'vuex';

    export default {
        name: "userAboutMessage",
        data() {
            return {}
        },
        computed: {
            ...mapGetters(['getProfile']),
            userDescription() {
                if (this.getProfile && this.getProfile.user) {
                    return this.getProfile.user.description;
                }
            },
            userName() {
                if (this.getProfile && this.getProfile.user) {
                    return this.getProfile.user.name;
                }
            },
        },

    }
</script>

<style lang="less">
    @import '../../../../../styles/mixin.less';

    .bio-text-container {
        .intro-name {
            font-family: @fontOpenSans;
            font-size: 30px;
            font-weight: 600;
            color: @profileTextColor;
            display: inline-flex;
            line-height: 1;
            align-items: flex-start;
            @media (max-width: @screen-xs) {
                font-size: 18px;
                font-weight: 600;
                line-height: 0.94;
                display: flex;
            }
        }
        .icon-wrap {
            display: inline-flex;
        }
        .icon-wrap {
            order: 2;
            i {
                font-size: 32px;
                color: @profileTextColor;
            }
            @media (max-width: @screen-xs) {
                order: 1;
                i {
                    font-size: 24px;
                }
            }
        }
        .text-wrap{
            @media (max-width: @screen-xs) {
                order: 21;
            }
        }

        .bio-about-intro {
            font-family: @fontOpenSans;
            font-size: 22px;
            font-style: italic;
            line-height: 1.36;
            letter-spacing: normal;
            color: @profileTextColor;
            @media(max-width: @screen-xs){
                font-size: 16px;
                font-style: italic;
                line-height: 1.38;
                color: @profileTextColor;
            }
        }
    }

</style>