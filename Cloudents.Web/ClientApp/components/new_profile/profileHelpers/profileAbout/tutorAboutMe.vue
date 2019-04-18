<template>
        <v-layout class="aboutme-section">
            <v-flex xs12>
                <v-card class="px-4 pt-4 about-me-card">
                    <v-layout column>
                        <v-flex xs12 sm12 md12>
                            <div>
                                <div class="about-title subheading font-weight-bold  mb-2" v-language:inner>profile_who_am_i</div>
                            </div>
                            <div class="mt-2">
                             <p class="about-text body-2">{{aboutMe | truncate(isOpen, '...', textLimit)}}
                             </p>
                            </div>
                        </v-flex>
                        <v-flex class="read-more-action mt-2 mb-2" v-if="readMoreVisible">
                            <v-divider style="widabout-textth: 100%; height: 2px;"></v-divider>
                            <a class="read-more-text pt-3 pb-1" @click="isOpen = !isOpen">
                                <span v-show="!isOpen" v-language:inner>profile_read_more</span>
                                <span v-show="isOpen" v-language:inner>profile_expand_less</span>
                            </a>
                        </v-flex>
                    </v-layout>
                </v-card>
            </v-flex>
        </v-layout>
</template>

<script>
    import { mapGetters } from 'vuex';
    export default {
        name: "tutorAboutMe",
        data() {
            return {
                textLimit: 150,
                defOpen: false

            }
        },
        computed: {
            ...mapGetters(['getProfile']),
            aboutMe() {
                if(this.getProfile &&  this.getProfile.about && this.getProfile.about.bio){
                    return this.getProfile.about.bio;
                }
                return '';

            },
            readMoreVisible(){
                return this.aboutMe.length >=  this.textLimit

            },
            isOpen :{
                get(){
                    return this.defOpen
                },
                set(val){
                    this.defOpen = val
                }

            }
        },
        filters: {
            truncate(val, isOpen, suffix, textLimit){
                if (val.length > textLimit && !isOpen) {
                    return val.substring(0, textLimit) + suffix;
                } else {
                    return val;
                }

            }
        }
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .aboutme-section{
        .about-me-card{
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.18);
            border-radius: 4px;
        }
        .about-title{
            height: 18px;
             line-height: 1;
            color: @profileTextColor;
        }
        .read-more-action{
            text-align: center;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            .read-more-text{
                font-family: @fontOpenSans;
                font-size: 12px;
                font-weight: 600;
                line-height: 1.31;
                color: @color-blue-new;
            }

        }
        .about-text{
            line-height: 1.59;
            color: @textColor;
            @media(max-width: @screen-xs){
                line-height: 1.53;

            }
        }
    }

</style>