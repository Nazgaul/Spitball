<template>
    <div class="user-image-wrap">
        <img class="user-picture" style="height: 240px; width: 214px;"
              src="../../../../images/placeholder.png">
        <div class="bottom-section">
            <user-rating :rating="4.5" class="px-4 line-height-1" ></user-rating>
            <span class="reviews-quantity">({{quantityReviews}} Reviews)</span>
        </div>
    </div>

</template>

<script>
    import userRating from '../userRating.vue'
    export default {
        components: {userRating},
        name: "userImage",
        data() {
            return {
                quantityReviews: 12
            }
        },
    }
</script>

<style lang="less">
    @import '../../../../../../styles/mixin.less';
    .user-image-wrap{
        position: relative;
        height: 240px;
        max-width: 214px;
        .line-height-1{
            line-height: 1;
        }
        .user-picture{

        }
        .bottom-section{
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            position: absolute;
            bottom: 0;
            right: 0;
            left: 0;
            padding: 8px 0;
            background-color: rgba(31, 31, 45, 0.73);
        }
        .reviews-quantity{
            font-family: @fontOpenSans;
            font-size: 13px;
            line-height: 1.92;
            letter-spacing: -0.3px;
            color:   rgba(255, 255, 255, 0.54);
        }

    }

</style>