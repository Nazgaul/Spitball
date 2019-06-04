<template>
    <router-link @click.native="tutorCardClicked" :to="{name: 'profile', params: {id: tutorData.userId}}">
        <v-card class="tutor-card-wrap pa-12 elevation-0 cursor-pointer " :class="{'list-tutor-card': isInTutorList}">
            <div class="section-tutor-info">
                <v-layout>
                    <v-flex class="image-wrap d-flex" shrink>
                        <div v-if="!isLoaded">
                            <v-progress-circular indeterminate v-bind:size="50"></v-progress-circular>
                        </div>
                        <img class="tutor-image" v-show="isLoaded" @load="loaded" :src="userImageUrl" alt="tutor user image">
                    </v-flex>
                    <v-flex>
                        <v-layout align-start row wrap fill-height>
                            <v-flex xs12 grow>
                                <v-layout row justify-space-between align-baseline class="top-section">
                                    <v-flex grow>
                                    <span class="tutor-name"
                                          v-line-clamp:18="1">{{tutorData.name}}</span>
                                    </v-flex>
                                    <v-flex shrink>
                                        <!--keep this inline style to fix price / hour spacing-->
                                <span class="font-weight-bold pricing pr-1" v-if="showStriked" style="display: table;">
                                     <span class="subheading font-weight-bold">₪50</span>
                                    <span class="font-weight-regular caption" v-language:inner>resultTutor_hour</span>
                                </span>

                                <span class="font-weight-bold pricing pr-1" v-else>
                                     <!--keep this wraper to fix price / hour spacing-->
                                    <div class="d-inline-flex align-baseline">
                                <span class="subheading font-weight-bold">₪{{tutorData.price}}</span>
                            <span class="pricing caption"  v-language:inner>resultTutor_hour</span>
                                        </div>
                        </span>
                                        <v-flex shrink v-if="showStriked" class="strike-through">

                                                <span class="striked-price">₪{{tutorData.price}}</span>
                                                <span class="pricing striked-hour">
                            <span v-language:inner>resultTutor_hour</span>
                        </span>

                                        </v-flex>
                                    </v-flex>

                                </v-layout>
                            </v-flex>
                            <v-flex class="bottom-section">
                                <userRating
                                        v-if="true"
                                        class="rating-holder"
                                        :rating="tutorData.rating"
                                        :starColor="'#ffca54'"
                                        :rateNumColor="'#43425D'"
                                        :size="isInTutorList ? '16' : '20'"
                                        :rate-num-color="'#43425D'"></userRating>

                                <v-flex shrink class="tutor-courses text-truncate">
                                    <span class="blue-text courses-text ">{{tutorData.courses}}</span>
                                </v-flex>
                            </v-flex>

                        </v-layout>
                    </v-flex>
                </v-layout>
            </div>
        </v-card>
    </router-link>
</template>

<script>
    import userRating from '../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue';
    import utilitiesService from "../../../../services/utilities/utilitiesService";
    import analyticsService from '../../../../services/analytics.service';

    export default {
        name: "tutorCard",
        components: {
            userRating,
        },
        data() {
            return {
              isLoaded: false
            };
        },
        props: {
            tutorData: {},
            isInTutorList: {
                type: Boolean,
                default: false
            }
        },
        methods:{
            loaded() { this.isLoaded = true; },

            tutorCardClicked(){
                analyticsService.sb_unitedEvent('Tutor_Engagement', 'tutor_page');
            }
        },
        computed: {
            userImageUrl() {
                if(this.tutorData.image) {
                    // let size = this.isInTutorList ? [56, 64] : [76, 96];
                    //enlarged due to striked price addition, (didn't fit);
                     let size = [76, 96];
                    return utilitiesService.proccessImageURL(this.tutorData.image, ...size, 'crop');
                } else {
                    return './images/placeholder-profile.png';
                }
            },
            showStriked() {
                return this.tutorData.price <= 120 && this.tutorData.price > 50;
            }
        },
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

    .tutor-card-wrap {
        border-radius: 4px;
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.13);
        min-width: 304px;
        margin-bottom: 8px;
        // START styles for card rendered inside tutor list only
        &.list-tutor-card {
            margin-bottom: 4px;
            border-radius: 0;
            .strike-through {
                //text-decoration: line-through; //will not work cause of different font sizes
                position: relative;
                color: @colorBlackNew;
                display: table;
                .striked-price{
                    font-size: 12px;
                }
                .striked-hour{
                    font-size: 10px;
                }
                &:after {
                    content: '';
                    width: 100%;
                    border-bottom: solid 1px @colorBlackNew;
                    position: absolute;
                    left: 0;
                    top: 50%;
                    z-index: 1;
                }
            }
            &.pa-12 {
                padding: 16px 12px;
            }
            &:first-child {
                border-radius: 4px 4px 0 0;
            }
            &:last-child {
                border-radius: 0 0 4px 4px;
            }
            .top-section {
            }
            .tutor-name {
                font-size: 13px;
                font-weight: 700;
                line-height: 18px;
                max-width: 120px;
            }
            .rating-holder {
                margin-bottom: 0;
            }
            .tutor-courses {
                font-size: 12px;
            }
            .tutor-image {
                width: 76px;
                height: 96px;
                /*width: 56px;*/
                /*height: 64px;*/
            }
        }
        // END styles for card rendered inside tutor list only
        &.pa-12 {
            padding: 12px;
        }
        .user-rating-val {
            font-weight: bold;
        }
        .tutor-name {
            color: @profileTextColor;
            line-height: 20px;
            word-break: break-all;
            font-size: 16px;
            font-weight: bold;
        }
        .flex{
          &.image-wrap {
            margin-right: 12px;
            display: flex;
            justify-content: center;
            align-items: center;
            color: #5D62FD;
        }
          }
        .tutor-image {
            border-radius: 4px;
            width: 76px;
            height: 96px;
        }
        .rating-number {
            font-weight: bold;
        }
        .bottom-section {
            justify-self: flex-end;
            margin-top: auto
        }
        .rating-holder {
            margin-bottom: 8px;
        }
        .pricing {
            font-family: @fontOpenSans;
            color: @profileTextColor;
            font-size: 18px;
            line-height: 16px;
        }
        .strike-through {
            //text-decoration: line-through; //will not work cause of different font sizes
            font-size: 14px;
            position: relative;
            display: table;
            &:after {
                content: '';
                width: 100%;
                border-bottom: solid 1px @textColor;
                position: absolute;
                left: 0;
                top: 50%;
                z-index: 1;
            }
            .striked-hour{
                font-size: 12px;
            }
        }
        .small-text {
            font-size: 10px;
        }
        .tutor-courses {
            color: @colorBlue;
            max-width: 140px;
            font-size: 14px;
            min-height: 19px; //keep it to prevent rating stars shift
        }
        .courses-text {
            vertical-align: text-bottom;
            line-height: 1;
        }

    }
</style>
