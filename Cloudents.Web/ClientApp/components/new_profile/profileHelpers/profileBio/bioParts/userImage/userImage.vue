<template>
    <v-hover>
        <div class="user-image-wrap" slot-scope="{ hover }">

            <img class="user-picture" style="height: 240px; width: 214px;"
                 :src="profileImage">
            <div class="bottom-section">
                <user-rating :rating="4.5" class="px-4 line-height-1"></user-rating>
                <span class="reviews-quantity">({{quantityReviews}} Reviews)</span>
            </div>
            <transition name="scale-transition">
                <div
                        v-if="hover"
                        class="hover-block d-flex transition-fast-in-fast-out darken-2 v-card--reveal display-3 white--text">
                    <uploadImage></uploadImage>
                </div>
            </transition>
        </div>
    </v-hover>

</template>

<script>
    import {mapGetters} from 'vuex';
    import userRating from '../userRating.vue';
    import uploadImage from '../uploadImage/uploadImage.vue';

    export default {
        components: {userRating, uploadImage},
        name: "userImage",
        data() {
            return {
                quantityReviews: 12,
                hover: false
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
        .line-height-1 {
            line-height: 1;
        }
        .hover-block {
            height: 100%;
            bottom: 0;
            background-color: #ffca54;
            display: flex;
            position: absolute;
            right: 0;
            left: 0;
            text-align: center;
            align-items: center;
            justify-content: center;
        }
        .user-picture {

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