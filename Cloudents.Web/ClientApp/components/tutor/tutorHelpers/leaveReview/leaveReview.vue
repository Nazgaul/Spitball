<template>
    <div class="leave-review-wrap">
        <v-layout align-center justify-space-between class="heading">
            <v-flex xs11 sm11 md11 grow>
                <span class="heading-text font-weight-bold">Rating and review </span>
            </v-flex>
            <v-flex xs1 sm1 md1>
                    <v-icon>sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-layout row wrap class="pt-4 pb-2">
            <v-flex xs12 sm12 md12 class="text-xs-center text-sm-center">
                <span class="review-title font-weight-bold">Was this Session Helpful?</span>
            </v-flex>
            <v-flex xs12 sm12 md12 class="text-xs-center text-sm-center">
                <span class="body-2">Press a star to rate ""name""</span>
            </v-flex>
        </v-layout>
        <v-layout align-center justify-center class="py-2">
            <v-flex xs12 sm6 md6 class="text-xs-center">
                <userRating  :rating="0"
                             :rateNumColor="'#43425D'"
                             :size="'26'"
                             :readonly="false"
                             :rate-num-color="'#43425D'"></userRating>
            </v-flex>
        </v-layout>
        <v-layout>
            <v-flex xs12 sm8 md8>sf</v-flex>
        </v-layout>
        <v-layout>
            <v-flex xs12 sm8 md8>wer</v-flex>
        </v-layout>
    </div>

</template>

<script>
    import userRating from '../../../new_profile/profileHelpers/profileBio/bioParts/userRating.vue';

    export default {
        components: {userRating},
        name: "leaveReview"
    };
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .leave-review-wrap{
        width: 100%;
        background-color: @color-white;
        .heading{
            background-color: @systemBackgroundColor;
            .heading-text{
                color: @profileTextColor;

            }
        }
        .review-texts{

        }
        .review-title{
            font-size: 18px;
            color: @profileTextColor;
        }
    }

</style>