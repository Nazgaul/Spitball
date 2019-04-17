<template>
    <div class="user-image-wrap" :class="{'hide-block': hideImageBlock}">
        <img class="user-picture" style="height: 240px; width: 214px;"
             :src="profileImage">
        <div class="bottom-section" v-if="isTutorProfile">
            <user-rating :size="'20'" :rating="tutorRank" :readonly="true" class="px-4 line-height-1"></user-rating>
            <span class="reviews-quantity">
                    <span>{{reviewCount}}</span>
                    <span v-if="reviewCount > 1" class="ml-1" v-language:inner>profile_reviews</span>
                    <span v-else="reviewCount" class="ml-1" v-language:inner>profile_single_review</span>

                    </span>
        </div>
        <!-- <div class="bottom-section" v-else>
                    <span class="user-balance py-2">{{profUserBal | currencyLocalyFilter}}</span>
        </div> -->
        <div v-if="isMyProfile"
             class="hover-block d-flex transition-fast-in-fast-out darken-2 v-card--reveal display-3 white--text">
            <uploadImage></uploadImage>
        </div>
        <userOnlineStatus class="user-status" v-if="isTutorProfile" :isOnline="isOnline"></userOnlineStatus>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex';
    import userRating from '../userRating.vue';
    import uploadImage from '../uploadImage/uploadImage.vue';
    import userOnlineStatus from '../userOnlineStatus.vue';
    import utilitiesService from '../../../../../../services/utilities/utilitiesService';


    export default {
        components: {userRating, uploadImage, userOnlineStatus},
        name: "userImage",
        data() {
            return {
                hover: false,
                hideImageBlock: true
            }
        },
        props: {
            isMyProfile: {
                type: Boolean,
                default: false
            },
        },
        computed: {
            ...mapGetters(['getProfile', 'isTutorProfile']),
            profileImage() {
                if(this.getProfile){
                    if (this.getProfile && this.getProfile.user && this.getProfile.user.image && this.getProfile.user.image.length > 1) {
                        let url = utilitiesService.proccessImageURL(this.getProfile.user.image, 214,240);
                        return url;
                    } else {
                        return '../../images/placeholder-profile.png'
                    }
                }
            },
            reviewCount(){
                if (this.getProfile && this.getProfile.user && this.getProfile.user.tutorData) {
                    return this.getProfile.user.tutorData.reviewCount|| 0
                }
                return 0
            },
            tutorRank(){
                if (this.getProfile && this.getProfile.user && this.getProfile.user.tutorData) {
                    return this.getProfile.user.tutorData.rate || 0
                }
                return 0
            },
            isOnline() {
                if (this.getProfile && this.getProfile.user && this.getProfile.user.tutorData) {
                    return this.getProfile.user.tutorData.online || false
                }
                return false
            },
            profUserBal() {
                if (this.getProfile && this.getProfile.user) {
                    return this.getProfile.user.score
                }
            }
        },
        mounted(){
            let imageElm = document.querySelector(".user-picture");
            imageElm.addEventListener('load', ()=>{
                this.hideImageBlock = false;
            });
        }
    }
</script>

<style lang="less">
    @import '../../../../../../styles/mixin.less';

    .user-image-wrap {
        position: relative;
        height: 240px;
        max-width: 214px;
        &.hide-block{
            visibility: hidden;
        }
        @media (max-width: @screen-xs) {
            margin-bottom: 20px;
        }
        .user-balance {
            font-family: @fontOpenSans;
            font-size: 20px;
            font-weight: bold;
            color: @color-white;
            .small {
                font-size: 12px;
            }
        }
        .user-status {
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
            border-radius: 0 4px 0 4px;
            background-color: rgba(255, 255, 255, 0.7);
            top: 0;
            right: 0;
            display: flex;
            position: absolute;
            text-align: center;
            align-items: center;
            justify-content: center;
        }
        .user-picture {
            border-radius: 4px;
            @media (max-width: @screen-xs) {
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