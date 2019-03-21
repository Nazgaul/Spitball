<template>
        <v-layout column class="reviews-container mt-4">
            <v-flex v-for="singleReview in 15" class="single-review">
                <reviewItem :reviewData="singleReview"></reviewItem>
            </v-flex>
        </v-layout>
</template>

<script>
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
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';

.reviews-container{
    .single-review{
        border-top:  solid 1px @newGreyColor;
      &:last-child{
          border-bottom: solid 1px @newGreyColor;
      }
    }
}
</style>