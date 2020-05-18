<template>
    <router-link v-if="item.url" :to="item.url" class="itemCarouselCard">
        <div class="imageWrapper" :class="{'subscribed': isSubscribed && !isLearnRoute}">
            <intersection>
                <img draggable="false" :id="`${item.id}-img`" class="itemCarouselImg" :src="$proccessImageUrl(item.preview,240,152)" alt="preview image">
            </intersection>
            <div class="overlay text-center px-8" v-if="isSubscribed && !isLearnRoute">
                <div class="unlockText white--text mb-3" v-t="subscribeText"></div>
                <v-btn class="btn" color="#fff" rounded block @click.prevent="goSubscription">
                    <span v-t="{path: subscribeBtnText, args: { 0: subscribedPrice }}"></span>
                </v-btn>
            </div>
        </div>
        <div class="item-cont pa-2">
            <div class="itemCarouselCard_videoType d-flex align-center justify-space-between mb-1">
                <div class="itemDate" >{{$d(item.dateTime, 'short')}}</div>
                <div class="d-flex" v-if="showVideoDuration">
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
                <UserAvatar :size="'34'" :user-name="item.user.name" :user-id="item.user.id" :userImageUrl="item.user.image"/> 
                <div class="ml-2 user-info">
                    <div class="text-truncate" >{{item.user.name}}</div>
                </div>
            </div>
            <div class="itemCard-bottom mt-2">
                <span class="item-purchases">{{item.purchased}} {{$tc('itemCardCarousel_view', item.purchased)}}</span>
                <span class="item-pts">{{$tc('itemCardCarousel_pts',item.price)}}</span>
            </div>
        </div>
    </router-link>
</template>

<script>
import * as routeNames from '../../routes/routeNames';
import intersection from '../pages/global/intersection/intersection.vue';
import vidSVG from '../../components/results/svg/vid.svg'

export default {
    components:{vidSVG, intersection},
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
            return this.isMobile ? 'resultNote_subscribe_mobile_text' : 'resultNote_subscribe_desktop_text'
        },
        subscribeBtnText() {
            return this.isMobile ? 'resultNote_subscribe_mobile_btn' : 'resultNote_subscribe_desktop_btn'
        },
        subscribedPrice() {
            return this.item.subscriberPrice || '$15'
        },
    },
    methods: {
        goToItem(){
            if(this.fromCarousel){
                return false;
            }else{
                this.enterItemPage();
            }
        },
        enterItemPage(){
            this.$router.push(this.item.url)
        },
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
    width: 242px;
    height: 320px;
    background: white;
    border-radius: 8px;
    border: solid 1px #c1c3d2;
    color: @global-purple !important;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    position: relative;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);

    .imageWrapper {
        position: relative;
        &.subscribed {
            z-index: 10;
            &:before {
                content: '';
                position: absolute;
                background: rgba(0, 0, 0, .7);
                height: 100%;
                width: 100%;
                border-radius: 8px 8px 0 0;
            }
            .overlay {
                position: absolute;
                top: 50%;
                right: 0;
                left: 0;
                transform: translate(0,-50%);
                .unlockText {
                    font-size: 15px;
                    font-weight: 600;
                    line-height: 1.47;
                }
                .btn {
                    color: @global-purple;
                    font-weight: 600;
                }
            }
        }
        .itemCarouselImg{
            border-bottom:  solid 1px #c1c3d2;
            border-top-left-radius: 8px;
            border-top-right-radius: 8px;
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
        .vidSvg path{
            fill: #69687d;
        }
        .itemDate {
            color: #989bac;
            font-size: 13px;
        }
        /*rtl:ignore*/
    }
    .item-cont{
        // height: inherit;
        // display: flex;
        // flex-direction: column;
        // justify-content: space-between;

        .item-title{
            overflow: hidden !important;
            font-size: 15px;
            font-weight: bold;
        }
        .item-course{
            font-size: 12px;
        }
        .item-user{
            margin-top: 22px;
            .userImg-item{
                margin-right: 10px;
            }
        }
        .user-info{
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
            justify-content: space-between;
            align-items: center;
            .item-purchases{
                font-size: 13px;
                font-weight: bold;
                color: @global-purple;
            }
            .item-pts{
                font-size: 14px;
                font-weight: bold;
            }

        }
    }
}
</style>