<template>
    <router-link v-if="item.url" event @click.native.prevent="goToItem" :to="item.url" class="itemCarouselCard">
        <intersection>
            <img draggable="false" :id="`${item.id}-img`" class="itemCarouselImg" :src="$proccessImageUrl(item.preview,240,152)" alt="preview image">
        </intersection>
        <span class="itemCarouselCard_videoType" v-if="showVideoDuration">
            <vidSVG class="vidSvg" />
            <span class="vidTime">{{item.itemDuration}}</span>
        </span>
        <div class="item-cont pa-2">
            <div class="item-title text-truncate">{{item.title}}</div>
            <div class="item-course text-truncate">
                <b>{{$t('itemCardCarousel_course')}}</b>{{item.course}}
                 <div class="user-info" >{{$d(new Date(item.dateTime), 'short')}}</div>
            </div>
                <div class="item-user" v-if="!isProfilePage">
                    <UserAvatar :size="'34'" :user-name="item.user.name" :user-id="item.user.id" :userImageUrl="item.user.image"/> 
                    <div class="ml-2 user-info">
                        <div class="text-truncate" >{{item.user.name}}</div>
                    </div>
                </div>
            <div class="itemCard-bottom mt-2">
                <span class="item-purchases">{{item.views}} {{$tc('itemCardCarousel_view', item.views)}}</span>
                <span class="item-pts">{{$tc('itemCardCarousel_pts',item.price)}}</span>
            </div>
        </div>
    </router-link>
</template>

<script>
const vidSVG = () => import("../../components/results/svg/vid.svg");

import intersection from '../pages/global/intersection/intersection.vue';
import * as routeNames from '../../routes/routeNames'
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
    data() {
        return {
            routeNames
        }
    },
    computed: {
        showVideoDuration() {
            return (this.item && this.item.documentType === "Video" && this.item.itemDuration);
        },
        isProfilePage() {
            return this.$route.name === routeNames.Profile
        }
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
        }
    },  
}
</script>

<style lang="less">
.itemCarouselCard{
    width: 242px;
    height: 340px;
    background: white;
    border-radius: 8px;
    border: solid 1px #c1c3d2;
    color: #43425d !important;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    position: relative;
    .itemCarouselImg{
        border-bottom:  solid 1px #c1c3d2;
        border-top-left-radius: 8px;
        border-top-right-radius: 8px;
        width: 100%;
        height: 100%;
    }
    .itemCarouselCard_videoType {
        /*rtl:ignore*/
        align-items: center;
        display: flex;
        position: absolute;
        background: rgba(0, 0, 0, 0.8);
        color: white;
        right: 10px;
        top: 130px;
        padding-left: 4px;
        padding-right: 4px;
        height: 16px;
        .vidTime{
            font-size: 12px;
            padding-left: 4px;
            vertical-align: top;
        }
        /*rtl:ignore*/
    }
    .item-cont{
        height: inherit;
        display: flex;
        flex-direction: column;
        justify-content: space-between;

        .item-title{
            overflow: hidden !important;
            font-size: 14px;
            font-weight: bold;
        }
        .item-course{
            margin: 8px 0;
            font-size: 12px;
        }
        .item-user{
            margin-top: 30px;
            margin-bottom: 14px;
            display: flex;
            align-items: unset;
            
            .userImg-item{
                margin-right: 10px;
            }
        }
        .user-info{
            font-size: 12px;
            color: #43425d;
            min-width: 0;
            display: table;
            height: 100%;
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
                font-size: 12px;
                font-weight: 600;
                color: #43425d;
            }
            .item-pts{
                font-size: 14px;
                font-weight: bold;
            }

        }
    }
}
</style>