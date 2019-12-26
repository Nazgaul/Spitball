<template>
        <v-layout column class="reviews-container" v-if="reviews && reviews.length >0">
            <v-flex class="mb-4">
                <span class="review-title" v-language:inner>profile_reviews</span>
            </v-flex>
            <v-flex  v-for="(singleReview, index) in reviews" :key="index"  class="single-review">
                <reviewItem :reviewData="singleReview"></reviewItem>
            </v-flex>
        </v-layout>
</template>

<script>
    import { mapGetters } from "vuex";
    import reviewItem from './reviewItem/reviewItem.vue'
    export default {
        name: "reviewsList",
        components:{
            reviewItem
        },
        data() {
            return {
            }
        },
        computed: {
            ...mapGetters(['getProfile']),
            reviews() {
                if(this.getProfile && this.getProfile.about && this.getProfile.about.reviews) {
                    return this.getProfile.about.reviews
                }
                return null
            }

        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

.reviews-container{
    margin-top: 64px;
    .review-title{
        font-size: 22px;
        font-weight: 600;
        line-height: 0.77;
        color: @global-purple;

    }
    .single-review{
        border-top:  solid 1px @separatorColor;
      &:last-child{
          border-bottom: solid 1px @separatorColor;
      }
    }
}
</style>