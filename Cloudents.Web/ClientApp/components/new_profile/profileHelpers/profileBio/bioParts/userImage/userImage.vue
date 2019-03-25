<template>
        <div class="user-image-wrap" slot-scope="{ hover }">
            <img class="user-picture" style="height: 240px; width: 214px;"
                 :src="profileImage">
            <div class="bottom-section">
                <user-rating :rating="4.5" :readonly="true" class="px-4 line-height-1"></user-rating>
                <span class="reviews-quantity">({{quantityReviews}} Reviews)</span>
            </div>
                <div
                        class="hover-block d-flex transition-fast-in-fast-out darken-2 v-card--reveal display-3 white--text">
                    <uploadImage></uploadImage>
                </div>
            <userOnlineStatus class="user-status" :isOnline="true"></userOnlineStatus>
        </div>
</template>

<script>
    import {mapGetters} from 'vuex';
    import userRating from '../userRating.vue';
    import uploadImage from '../uploadImage/uploadImage.vue';
    import userOnlineStatus from '../userOnlineStatus.vue';


    export default {
        components: {userRating, uploadImage, userOnlineStatus},
        name: "userImage",
        data() {
            return {
                quantityReviews: 12,
                hover: false,
            }
        },
        computed: {
            ...mapGetters(['getProfile']),
            profileImage() {
                if( this.getProfile && this.getProfile.user){
                    return  `${global.location.origin}${this.getProfile.user.image}?width=214&height=240`
                }
                return ''

            }
        },
    }
</script>

<style lang="less">
    @import '../../../../../../styles/mixin.less';

    .user-image-wrap {
        position: relative;
        height: 240px;
        max-width: 214px;
        @media(max-width: @screen-xs){
            margin-bottom: 20px;
        }
        .user-status{
            position: absolute;
            top: 12px;
            left: 8px;
        }
        .line-height-1 {
            line-height: 1;
        }
        .hover-block {
            width: 38px;
            height: 45px;
            border-radius: 4px;
            background-color: rgba(255, 255, 255, 0.38);
            top: 0;
            right: 0;
            display: flex;
            position: absolute;
            text-align: center;
            align-items: center;
            justify-content: center;
        }
        .user-picture {
            @media(max-width: @screen-xs){
                border-radius: 4px;
                box-shadow: 0 3px 8px 0 rgba(0, 0, 0, 0.41);
            }
        }
        .bottom-section {
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
            border-radius: 0 0 4px 4px;
        }
        .reviews-quantity {
            font-family: @fontOpenSans;
            font-size: 13px;
            line-height: 1.92;
            letter-spacing: -0.3px;
            color: rgba(255, 255, 255, 0.54);
        }

    }

</style>