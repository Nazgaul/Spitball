<template>
    <div class="review-submitted-wrap">
        <v-layout row wrap align-center justify-center class="pb-4">
            <v-flex xs12 md12 sm12 class="text-xs-center pt-4">
                <v-icon class="star-icon">sbf-star-rating-full</v-icon>
            </v-flex>
            <v-flex xs12 md12 sm12 class="text-xs-center pt-4">
                <span class="font-weight-black final-review-title">Thank you for rating</span>
            </v-flex>
            <v-flex xs12 md12 sm12 class="text-xs-center pt-3 pb-5">
                <span class="body-2 blue-text">Go to your review</span>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
    export default {
        name: "finalScreen"
    };
</script>

<style lang="less">
    @import '../../../../../styles/mixin.less';
    .review-submitted-wrap{
        .final-review-title{
            font-size: 18px;
            color: @profileTextColor;
        }
        .star-icon{
            font-size: 86px;
            color: #ffca54;
        }
        .blue-text{
            color: @colorBlue;
        }
    }
</style>