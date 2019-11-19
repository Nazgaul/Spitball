<template>
    <v-layout class="bio-text-container" column>
        <v-flex>
            <h4 class="intro-name">
                <!--<span class="icon-wrap" :class="[$vuetify.breakpoint.xsOnly ? 'mr-2' : 'ml-2']"><v-icon>sbf-hand-icon</v-icon></span>&nbsp;-->
                <span class="text-wrap headline font-weight-bold">
                    <span v-language:inner>profile_hey_there</span>
                    <span class="word-break">&nbsp;{{userName}}!</span>
                  </span>
            </h4>
        </v-flex>
        <v-flex :class="[$vuetify.breakpoint.smAndUp ? 'pt-4' : 'pt-1']">
            <div class="bio-about-intro ">{{userDescription}}</div>
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
            ...mapGetters(['getProfile', 'isTutorProfile']),
            userDescription() {
                if (this.getProfile && this.getProfile.user) {
                    return this.getProfile.user.description;
                }
            },
            userName(){
                if(this.isTutorProfile){
                    if(this.getProfile && this.getProfile.user && this.getProfile.user.tutorData){
                        return `${this.getProfile.user.tutorData.firstName} ${this.getProfile.user.tutorData.lastName}`
                    }
                }else{
                    if (this.getProfile && this.getProfile.user) {
                        return this.getProfile.user.name;
                    }
                }

            },
            // userName() {
            //     if (this.getProfile && this.getProfile.user) {
            //         return this.getProfile.user.name;
            //     }
            // },
        },

    }
</script>

<style lang="less">
    @import '../../../../../styles/mixin.less';

    .bio-text-container {
        .word-break{
            word-break: break-word;
        }
        .intro-name {
            font-size: 30px;
            font-weight: 600;
            color: @global-purple;
            line-height: 1;
            align-items: flex-start;
            word-break: break-all;
            @media (max-width: @screen-xs) {
                font-size: 18px;
                font-weight: 600;
                line-height: 0.94;
                display: flex;
            }
        }
        .icon-wrap {
            vertical-align: middle;
            display: inline-flex;
            order: 2;
            i {
                font-size: 32px;
                color: @global-purple;
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
            font-size: 20px;
            font-style: italic;
            line-height: 1.36;
            letter-spacing: normal;
            color: @global-purple;
            word-break: break-word;
            white-space: pre-line;
            @media(max-width: @screen-xs){
                font-size: 16px;
                font-style: italic;
                line-height: 1.38;
                color: @global-purple;
            }
        }
    }

</style>