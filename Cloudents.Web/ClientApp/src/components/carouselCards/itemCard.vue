<template>
    <router-link v-if="item.url" :to="item.url" class="itemCarouselCard">
        <div class="imageWrapper" :class="{'subscribed': isSubscribed && !isLearnRoute}">
            <intersection>
                <img draggable="false" :id="`${item.id}-img`" class="itemCarouselImg" :src="$proccessImageUrl(item.preview,240,152)" alt="preview image">
            </intersection>
            <div class="overlay text-center px-8" v-if="isSubscribed && !isLearnRoute">
                <div class="unlockText white--text mb-3">{{subscribeText}}</div>
                <v-btn class="btn" color="#fff" rounded block @click.prevent="goSubscription">
                    <span>{{subscribeBtnText}}</span>
                </v-btn>
                <!-- <div class="overladyDuration white--text">{{item.itemDuration}}</div> -->
            </div>
        </div>

<!-- pa-3 -->
        <div class="item-cont flex-grow-1 d-flex flex-column justify-space-between pa-2">
            <div class="itemCarouselCard_videoType d-flex align-center justify-space-between mb-1">
                <div class="itemDate" >{{$d(item.dateTime, 'short')}}</div>
                <div class="d-flex align-center" v-if="showVideoDuration">
                    <span class="vidTime pr-1">{{item.itemDuration}}</span>
                    <vidSVG class="vidSvg" width="17" />
                </div>
            </div>
            <div class="item-title text-truncate mb-1">{{item.title}}</div>
            <div class="item-course text-truncate">
                <span class="font-weight-bold" v-t="'itemCardCarousel_course'"></span>
                <span>{{item.course}}</span>
            </div>
            <div class="item-user d-flex align-center" v-if="!isProfilePage">
                <UserAvatarNew :fontSize="11" :width="34" :height="34" :user-name="item.user.name" :user-id="item.user.id" :userImageUrl="item.user.image"/> 
                <div class="ml-2 user-info">
                    <div class="text-truncate" >{{item.user.name}}</div>
                </div>
            </div>
            <div class="itemCard-bottom">
                <!-- <span v-if="item.price" class="item-purchases">{{$tc('itemCardCarousel_purchased', item.purchased)}}</span>
                <span v-else class="item-purchases">{{$tc('itemCardCarousel_downloaded', item.downloads)}}</span> -->
                <documentPrice :price="item.price" :isSubscribed="isSubscribed" />
            </div>
        </div>
    </router-link>
</template>

<script>
import * as routeNames from '../../routes/routeNames';

import documentPrice from '../pages/global/documentPrice/documentPrice.vue'
import intersection from '../pages/global/intersection/intersection.vue';
import vidSVG from '../../components/results/svg/vid.svg'

export default {
    components:{vidSVG, intersection, documentPrice},
    props:{
        item:{
            type:Object,
            required: true
        },
        fromCarousel:{
            type:Boolean,
            required: false,
            default: false
        }
    },
    computed: {
        showVideoDuration() {
            return (this.item && this.item.documentType === "Video" && this.item.itemDuration);
        },
        isLearnRoute() {
            return this.$route.name === routeNames.Learning
        },
        isProfilePage() {
            return this.$route.name === routeNames.Profile
        },
        isSubscribed() {
            return this.item.priceType === 'Subscriber'
        },
        subscribeText() {
            return this.isMobile ? this.$t('resultNote_subscribe_mobile_text') : this.$t('resultNote_subscribe_desktop_text')
        },
        subscribeBtnText() {
            let price = this.$price(this.item.price, 'USD')
            return this.isMobile ? this.$t('resultNote_subscribe_mobile_btn', [price]) : this.$t('resultNote_subscribe_desktop_btn', [price])
        },
        // subscribedPrice() {
        //     return this.item.price
        // },
    },
    methods: {
        goSubscription() {
            if(!this.isProfilePage) {
                this.$router.push({
                    name: routeNames.Profile,
                    params: {
                        id: this.item.user.id,
                        name: this.item.user.name
                    },
                    hash: '#subscription'
                })
            } else {
                this.$vuetify.goTo('#subscription')
            }
        }
    },  
}
</script>

<style lang="less">
@import '../../styles/mixin.less';
@import '../../styles/colors.less';

.itemCarouselCard{
    // OLD
    width: 242px;
    height: 320px;
    // New
    // width: 219px;
    // height: 263px;
    background: white;
    border-radius: 6px;
    border: solid 1px #c1c3d2;
    color: @global-purple !important;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    position: relative;

    .imageWrapper {
        position: relative;
        margin: 0;
        min-height: 151px;
        .v-lazy {
            display: flex;
            height: 100%; // extra div added for overlay subscription box, image issue
        }

        &.subscribed {
            &:before {
                content: '';
                position: absolute;
                background: rgba(0, 0, 0, .7);
                height: 100%;
                width: 100%;
                border-radius: 6px 6px 0 0;
            }
            .overlay {
                position: absolute;
                top: 50%;
                right: 0;
                left: 0;
                transform: translate(0,-50%);
                .unlockText {
                    white-space: pre;
                    font-size: 15px;
                    font-weight: 600;
                    line-height: 1.47;
                }
                .btn {
                    color: @global-purple; // old
                    width: 100%;
                    // min-width: 153px;
                    // color: @global-purple;
                    font-weight: 600;
                }
                .overladyDuration {
                    position: absolute;
                    right: 6px;
                    margin-top: 14px;
                    font-size: 11px;
                }
            }
        }
        .itemCarouselImg {
            border-bottom:  solid 1px #c1c3d2;
            border-top-left-radius: 6px;
            border-top-right-radius: 6px;
            width: 100%;
            height: 100%;

            img {
                width: 100%;
            }
        }
    }
    .itemCarouselCard_videoType {
        /*rtl:ignore*/
        .vidTime{
            font-size: 13px;
            vertical-align: top;
        }
        .vidSvg {
            height: 100%;
            path {
                fill: #69687d;
            }
        }

        .itemDate {
            color: #989bac;
            font-size: 13px;
        }
        /*rtl:ignore*/
    }
    .item-cont {
        .item-title {
            overflow: hidden !important;
            // .giveMeEllipsis(2, 22px);
            font-size: 15px;
            font-weight: 600;
            line-height: 1.5;
            color: @global-purple;
        }
        .item-course{
            font-size: 12px;
        }
        .item-user {
            margin-top: 22px;
            .userImg-item{
                margin-right: 10px;
            }
        }
        .user-info {
            font-size: 13px;
            color: @global-purple;
            min-width: 0;
            .text-truncate {
                display: table-cell;
                vertical-align: middle;
            }
        }
        .itemCard-bottom{
            display: flex;
            //justify-content: space-between;
            justify-content: flex-end;
            //align-items: center;
            // .item-purchases{
            //     font-size: 13px;
            //     font-weight: bold;
            //     color: @global-purple;
            // }
            // .item-pts{
            //     font-size: 14px;
            //     font-weight: bold;
            // }
            .documentPrice {
                .docFree {
                font-size: 13px;
                color: @global-purple;
                font-weight: 600;
                }
            }
        }
    }
}
</style>