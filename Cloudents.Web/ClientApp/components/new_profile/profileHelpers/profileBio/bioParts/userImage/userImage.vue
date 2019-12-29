<template>
    <div class="user-image-wrap">
        <user-avatar-rect
            :userName="userName"
            :userImageUrl="profileImage"
            class="mr-3 d-block"
            :userId="userId"
            :width="214"
            :height="240"
            :fontSize="36"
            :borderRadius="4"
        />
        <div class="bottom-section" v-if="isTutorProfile">
            <user-rating :size="'20'" :rating="tutorRank" :readonly="true" class="px-4 line-height-1"></user-rating>
            <span class="reviews-quantity">
                <span>{{reviewCount}}</span>
                <span v-if="reviewCount > 1" class="ml-1" v-language:inner>profile_reviews</span>
                <span v-else class="ml-1" v-language:inner>profile_single_review</span>
            </span>
        </div>
        <!-- <div class="bottom-section" v-else>
                    <span class="user-balance py-2">{{profUserBal | currencyLocalyFilter}}</span>
        </div> -->
        <div v-if="isMyProfile"
             class="hover-block d-flex transition-fast-in-fast-out darken-2 v-card--reveal display-3 white--text">
            <uploadImage></uploadImage>
        </div>
        <userOnlineStatus class="user-status" v-if="isTutorProfile" :userId="userId"></userOnlineStatus>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex';

    
    import userRating from '../userRating.vue';
    import uploadImage from '../uploadImage/uploadImage.vue';
    import userOnlineStatus from '../../../../../helpers/userOnlineStatus/userOnlineStatus.vue';
    import userAvatarRect from '../../../../../helpers/UserAvatar/UserAvatarRect.vue';

    export default {
        components: {userRating, uploadImage, userOnlineStatus, userAvatarRect},
        name: "userImage",
        props: {
            isMyProfile: {
                type: Boolean,
                default: false
            },
        },
        methods:{
            onImageLoadError(event) {
                event.target.src = require("../../../../../images/placeholder-profile.png");
            }
        },
        computed: {
            ...mapGetters(['getProfile', 'isTutorProfile','getProfileImageLoading']),
            userName(){
                return (this.getProfile && this.getProfile.user.name)? this.getProfile.user.name : '';
            },
            profileImage() {
                if(this.getProfile){
                    if (this.getProfile && this.getProfile.user && this.getProfile.user.image && this.getProfile.user.image.length > 1) {
                        // let url = utilitiesService.proccessImageURL(this.getProfile.user.image, 214,240);
                        return this.getProfile.user.image;
                    } 
                }
                return '';
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
            userId(){
                if (this.getProfile && this.getProfile.user) {
                    return this.getProfile.user.id
                }
                return -1;
            },
            // profUserBal() {
            //     if (this.getProfile && this.getProfile.user) {
            //         return this.getProfile.user.score
            //     }
            // }
        },
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
        .loader-wrap{
            display: flex;
            height: 240px;
            width: 214px;
        }
        @media (max-width: @screen-xs) {
            margin-bottom: 20px;
        }
        .user-balance {
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
            border: 1px solid @systemBackgroundColor;
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
            font-size: 13px;
            line-height: 1.92;
            letter-spacing: -0.3px;
            color: rgba(255, 255, 255, 0.54);
        }

    }

</style>