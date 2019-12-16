<template>
    <router-link event @click.native.prevent="goToItem" v-if="item.url" :to="item.url" class="itemCarouselCard">
    <img draggable="false" :id="`${item.id}-img`" class="itemCarouselImg" :src="$proccessImageUrl(item.preview,240,152)" alt="">
    <div class="item-cont pa-2">
        <h1 class="item-title text-truncate">{{item.title}}</h1>
        <div class="item-course text-truncate">
            <b v-language:inner="'itemCardCarousel_course'"/> {{item.course}}</div>
        <div class="item-university text-truncate">
            <b v-language:inner="'itemCardCarousel_university'"/> {{item.university}}</div>
        <div class="item-user">
            <UserAvatar :size="'34'" :user-name="item.user.name" :user-id="item.user.id" :userImageUrl="item.user.image"/> 
            <div class="ml-2 user-info">
                <div class="text-truncate" >{{item.user.name}}</div>
                <div>{{$options.filters.fullMonthDate(item.dateTime)}}</div>
            </div>
        </div>
        <div class="itemCard-bottom">
            <span class="item-purchases">{{item.views}} <span v-language:inner="item.views > 1?'itemCardCarousel_views':'itemCardCarousel_view'"/> </span>
            <span class="item-pts">{{item.price}} <span v-language:inner="'itemCardCarousel_pts'"/></span>
        </div>
    </div>
    </router-link>
</template>

<script>
import UserAvatar from '../helpers/UserAvatar/UserAvatar.vue';
export default {
    components:{UserAvatar},
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
    methods: {
        goToItem(enter){
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
    .itemCarouselImg{
        border-bottom:  solid 1px #c1c3d2;
        border-top-left-radius: 8px;
        border-top-right-radius: 8px;
    }
    .item-cont{
        .item-title{
            font-size: 14px;
            font-weight: bold;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
        }
        .item-course{
            margin: 8px 0;
            font-size: 12px;
            font-weight: normal;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
        }
        .item-university{
            font-size: 12px;
            font-weight: normal;
            font-stretch: normal;
            font-style: normal;
            line-height: normal;
            letter-spacing: normal;
        }
        .item-user{
            margin-top: 30px;
            margin-bottom: 14px;
            display: flex;
            align-items: unset;
            .userImg-item{
                margin-right: 10px;
            }
            .user-info{
                font-size: 12px;
                font-weight: normal;
                font-stretch: normal;
                font-style: normal;
                line-height: normal;
                letter-spacing: normal;
                color: #43425d;
            }
        }
        .itemCard-bottom{
            display: flex;
            justify-content: space-between;
            align-items: center;
            .item-purchases{
                font-size: 12px;
                font-weight: 600;
                font-stretch: normal;
                font-style: normal;
                line-height: normal;
                letter-spacing: normal;
                color: #43425d;
            }
            .item-pts{
                font-size: 14px;
                font-weight: bold;
                font-stretch: normal;
                font-style: normal;
                line-height: normal;
                letter-spacing: normal;
            }

        }
    }
}
</style>