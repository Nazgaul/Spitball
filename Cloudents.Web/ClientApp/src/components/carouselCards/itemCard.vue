<template>
    <div class="itemCarouselCard cursor-pointer mb-5 mb-sm-0" @click="openItemDialog">
        <div class="imageWrapper">
            <intersection>
                <img draggable="false" :id="`${item.id}-img`" class="itemCarouselImg" :src="srcImg" alt="preview image">
            </intersection>
            <div v-show="isVideo" class="videoSign">
                <img src="./videoSign.png" alt="">
            </div>
        </div>
        <div class="item-cont flex-grow-1 d-flex flex-column justify-space-between pa-4 pa-sm-3">
            <div class="item-title">{{item.title}}</div>
        </div>
    </div>
</template>

<script>
import intersection from '../pages/global/intersection/intersection.vue';

export default {
    components:{intersection},
    props:{
        item:{
            type:Object,
            required: true
        }
    },
    computed: {
        isVideo() {
            return this.item?.documentType === "Video";
        },
        srcImg(){
            let isMobile = this.$vuetify.breakpoint.xsOnly;
            if(isMobile){
                return this.$proccessImageUrl(this.item.preview,344,196)
            }else{
                return this.$proccessImageUrl(this.item.preview,248,150)
            }
        }
    },
    methods: {
        openItemDialog(){
            this.$store.dispatch('updateCurrentItem',this.item.id);
        }
    },  
}
</script>

<style lang="less">
@import '../../styles/mixin.less';
@import '../../styles/colors.less';

.itemCarouselCard{
    @media (max-width: @screen-xs) {
        height: 270px;
        margin: 0 auto;
        width: 100%;
        max-width: 344px;
    }
    width: 248px;
    height: 214px;
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
        .videoSign{
            position: absolute;
            top: calc(~"50% - 19px");
            right: calc(~"50% - 26px");
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
    .item-cont {



        .item-title{
            @media (max-width: @screen-xs) {
                font-size: 16px;
                line-height: 1.3;
                .giveMeEllipsis(2,24);
            }
            color: #363637;
            font-size: 14px;
            font-weight: 600;
            line-height: 1.43;
            .giveMeEllipsis(2,18);
        }

    }
}
</style>