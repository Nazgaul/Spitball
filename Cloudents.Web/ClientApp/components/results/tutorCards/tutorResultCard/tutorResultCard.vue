<template>
    <div class="tutor-card-wrap-desk cursor-pointer">
        <router-link :to="{name: 'profile', params: {id: tutorData.userId}}">
        <v-layout>
            <div class="section-tutor-info">
                <v-layout>
                    <v-flex class="mr-3" shrink>
                        <img class="tutor-image" :src="userImageUrl" alt="">
                    </v-flex>
                    <v-flex>
                        <v-layout align-start justify-space-between column fill-height>
                            <v-flex shrink class="pb-3">
                                <span class="tutor-name font-weight-bold" v-line-clamp:18="1">{{tutorData.name}}</span>
                            </v-flex>
                            <v-flex grow>
                                <span class="tutor-about subheading" v-line-clamp:18="2">
                                    {{tutorData.bio}}
                                </span>
                            </v-flex>
                            <v-flex shrink class="tutor-courses">
                                <span class="blue-text subheading" v-line-clamp:18="1">{{tutorData.courses}}</span>
                            </v-flex>
                        </v-layout>
                    </v-flex>
                </v-layout>
            </div>
            <v-layout row wrap align-start justify-start grow
                      class="price-review-column section-tutor-price-review ml-1">
                <v-flex xs12 sm12 md12 grow>
                    <v-flex xs12 shrink class="pricing">
                        <span>₪</span>
                        <span class="font-weight-bold headline">{{tutorData.price}}</span>
                        <span class="caption">
                            <span v-language:inner>resultTutor_hour</span>
                        </span>
                    </v-flex>
                    <v-flex xs12 sm12 md12 shrink class="pt-2">
                        <userRating
                                v-if="tutorData.reviews > 0"
                                :rating="tutorData.rating"
                                :starColor="'#ffca54'"
                                :rateNumColor="'#43425D'"
                                :size="'24'"
                                :rate-num-color="'#43425D'"></userRating>
                    </v-flex>
                    <v-flex xs12 sm12 md12 class="pt-1" shrink>
                            <span class="blue-text body-2" v-if="tutorData.reviews > 0">{{tutorData.reviews}}
                                <span v-if="tutorData.reviews > 1" v-language:inner>resultTutor_reviews_many</span>
                                <span v-else v-language:inner>resultTutor_review_one</span>
                            </span>
                        <span class="body-2" v-else v-language:inner>resultTutor_no_reviews</span>
                    </v-flex>
                </v-flex>

                <v-flex xs12 sm12 md12 class="d-flex btn-bottom-holder text-xs-center">
                    <v-btn style="max-width: 80%; margin: 0 auto;" round class="blue-btn rounded elevation-0 ma-0" block>
                        <span class="font-weight-bold text-capitalize" v-language:inner>resultTutor_btn_view</span>
                    </v-btn>
                </v-flex>
            </v-layout>
        </v-layout>
        </router-link>
    </div>
</template>


<script>
    import userRank from '../../../helpers/UserRank/UserRank.vue';
    import userRating from '../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue';
    import userAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';
    import utilitiesService from "../../../../services/utilities/utilitiesService";

    export default {
        name: "tutorResultCard",
        components: {
            userRank,
            userRating,
            userAvatar
        },
        data() {
            return {};
        },
        props: {
            tutorData: {},
        },
        computed: {
            userImageUrl() {
                if(this.tutorData.image) {
                    return utilitiesService.proccessImageURL(this.tutorData.image, 166, 186);
                } else {
                    return './images/placeholder-profile.png';
                }

            }
        },
    };
</script>


<style lang="less" src="./tutorResultCard.less">

</style>