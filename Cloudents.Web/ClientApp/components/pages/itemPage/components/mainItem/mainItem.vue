<template>
    <div class="mainItem">
        <template v-if="!videoLoader">
            <div class="text-xs-center mainItem__loader">
                <img :src="require('./doc-preview-animation.gif')" alt="Photo" :class="{'video_placeholder': $vuetify.breakpoint.smAndDown}">
            </div>
        </template>
        <div v-if="!isLoad && videoLoader">
            <template v-if="isVideo && videoSrc">
                <div style="margin: 0 auto;background:black" class="text-center mainItem__item mb-3">
                <v-fade-transition>
                    <unlockItem v-if="showAfterVideo && !isPurchased" :type="document.documentType"/>
                </v-fade-transition>
                <sbVideoPlayer 
                    @videoEnded="updateAfterVideo()"
                    :id="`${document.details.id}`"
                    :height="videoHeight" 
                    :width="videoWidth" 
                    style="margin: 0 auto" 
                    :isResponsive="true" 
                    :src="videoSrc"
                    :title="courseName"
                    :poster="`${document.preview.poster}?width=${videoWidth}&height=${videoHeight}&mode=crop&anchorPosition=bottom`"
                />
                </div>
            </template>
            <div class="mainItem__item" v-else>
                <template v-if="docPreview">
                    <div class="mainItem__item__wrap">
                        <div :style="{height: `${dynamicWidthAndHeight.height}px`}">
                            <v-scroll-x-reverse-transition>
                                <unlockItem  v-touch="{
                                    left:() => handleSwipe(true),
                                    right:() => handleSwipe(false)}"  class="unlockItem_swipe" v-if="showUnlockPage" :type="document.documentType" :docLength="docPreview.length"/>
                            </v-scroll-x-reverse-transition>
                            
                            <sbCarousel
                                ref="itemPageChildComponent"
                                :gap="20"
                                :centered="true"
                                :arrows="false"
                                :moveEnd="setDocPage"
                                :renderOnlyVisible="true"
                                :moveType="'snap'"
                                >
                                <lazyImage 
                                    v-for="(doc, index) in docPreview"
                                    :src="doc"
                                    :key="index"
                                    class="mainItem__item__wrap--img"
                                    draggable="false"
                                    :element="selector"
                                    >
                                </lazyImage>
                                    <!-- <img draggable="false" class="mainItem__item__wrap--img" :src="doc" alt="" v-for="(doc, index) in docPreview" :key="index"> -->
                            </sbCarousel>
                        </div>
                        <div class="mainItem__item__wrap__paging">
                            <v-layout class="mainItem__item__wrap__paging__actions">
                                <button class="mainItem__item__wrap__paging__actions--left"  @click="isRtl ? nextDoc() : prevDoc()" v-if="showDesktopButtons">
                                    <v-icon class="mainItem__item__wrap__paging__actions--img" v-html="'sbf-arrow-left-carousel'"/>
                                </button>
                                <div class="mx-4 mainItem__item__wrap__paging--text" v-html="$Ph('documentPage_docPage', [docPage, docPreview.length])"></div>            

                                <button class="mainItem__item__wrap__paging__actions--right" @click="isRtl ? prevDoc() : nextDoc()" v-if="showDesktopButtons">
                                    <v-icon class="mainItem__item__wrap__paging__actions--img" v-html="'sbf-arrow-right-carousel'"/>
                                </button>
                            </v-layout>
                        </div>
                    </div>
                </template>
            </div>
        </div>
    </div>
</template>

<script>
import {  mapGetters } from 'vuex';

import utillitiesService from "../../../../../services/utilities/utilitiesService";

import sbCarousel from '../../../../sbCarousel/sbCarousel.vue';
import sbVideoPlayer from '../../../../sbVideoPlayer/sbVideoPlayer.vue';
import lazyImage from '../../../global/lazyImage/lazyImage.vue';
import unlockItem from '../unlockItem/unlockItem.vue';

export default {
    name: 'mainItem',
    components: {
        sbCarousel,
        sbVideoPlayer,
        lazyImage,
        unlockItem,
    },
    props: {
        document: {
            type: Object
        },
        isLoad:{
            type: Boolean
        }
    },
    data() {
        return {
            docPage: 1,
            isRtl: global.isRtl,
            showAfterVideo:false,
        }
    },
    watch:{
        '$route'(){
            //reset the document with the v-if, fixing issue that moving from video to document wont reset the video ELEMENT
            let self = this;
            this.isLoad = true;
            this.docPage = 1;
            setTimeout(()=>{
                self.isLoad = false;
            })
        },
    },
    computed: {
        ...mapGetters(['getDocumentLoaded']),
        showUnlockPage(){
            // touchmove 
            return (this.docPage > 2 && !this.isPurchased)
        },
        showDesktopButtons() {
            if(this.docPreview) {
                if(this.$vuetify.breakpoint.xsOnly || this.docPreview.length < 2) return false;
                return true;
            }
            return false;
        },
        isPurchased() {
            if (this.document.details) {
                return this.document.details.isPurchased;
            }
            return false;
        },

        videoSrc:{
            get(){
                if(this.document && this.document.preview && this.document.preview.locator){
                    return this.document.preview.locator;
                }else{
                    return null;
                }
            }, 
            set(val){
                this.document.preview.locator = val;
            }
        },
        isVideo(){
            return this.document.documentType === 'Video';
        },
        docPreview() {
            if(this.isVideo) return
                // TODO temporary calculated width container
            if (this.document.preview) {
                if(this.document.preview[0].indexOf("base64") > -1) {
                    return this.document.preview;
                }
                let result = this.document.preview.map(preview => {
                    return utillitiesService.proccessImageURL(
                        preview,
                        this.dynamicWidthAndHeight.width,
                        this.dynamicWidthAndHeight.height,
                        "pad"
                    );
                });
                return result;
            }
            return null
        },
        videoLoader() {
            if(this.getDocumentLoaded) {
                if(this.isVideo) {
                    return !!(this.document && this.document.preview && this.document.preview.locator);
                } else {
                    return true;
                }
            } else {
                return false;
            }
        },
        calculateWidthByScreenSize(){
            let width = global.innerWidth < 960 ? global.innerWidth : 703;
            if(this.$vuetify.breakpoint.width >= 1264 && this.$vuetify.breakpoint.width <= 1410) {
                width = 550;
            }
            if(this.$vuetify.breakpoint.width > 800 && this.$vuetify.breakpoint.width < 960){
                return 698;
            }else if (this.$vuetify.breakpoint.width <= 800) {
                if (this.$vuetify.breakpoint.xs) {
                    width = global.innerWidth;
                }
                if (this.$vuetify.breakpoint.smAndUp) {
                    width = global.innerWidth - 102;
                }
            }
            return width;
        },
        dynamicWidthAndHeight() {
            return {
                width: this.calculateWidthByScreenSize,
                height: Math.ceil(this.calculateWidthByScreenSize / 0.785)
            }
        },
        videoHeight(){
            return Math.floor((this.videoWidth/16)*9); 
        },
        videoWidth(){
            return this.calculateWidthByScreenSize;
        },
        courseName() {
            if (this.document.details && this.document.details.name) {
                return this.document.details.name;
            }
            return null
        },
        selector() {
            let element = document.querySelector('.itemPage__main')
            if(!element) return '';
            return element;
        },
    },
    methods: {
        setDocPage(){  
            let uniqueID = this.$refs.itemPageChildComponent.uniqueID;
            let currentIndex = this.$refs.itemPageChildComponent.$refs[uniqueID].getIndex();
            this.docPage = currentIndex + 1;
        },
        prevDoc() {
            this.$refs.itemPageChildComponent.prev();
        },
        nextDoc() {
            this.$refs.itemPageChildComponent.next();
        },
        updateAfterVideo(){
            this.showAfterVideo = true;
        },
        handleSwipe(dir){
            if(dir){
                this.nextDoc()
            }else{
                this.prevDoc()
            }
        }
    },
}
</script>

<style lang="less">
    @import '../../../../../styles/mixin';

    .mainItem {
        &__loader {
            img {
                width: 100% !important;
            }
        }
        &__item {
            position: relative;
            &__wrap {
                position: relative;
                height: 100%;
                .sbCarousel {
                    div:first-child {
                        height: 100% !important;
                        z-index: 4 !important;
                    }
                }
                &--img  {
                    border-radius: 8px 8px 0 0;
                    width: 100%;
                }
                &__paging{
                    &__actions {
                            display: flex;
                            justify-content: center;
                            background: #fff;
                            padding: 14px 0;
                            border-radius: 0 0 8px 8px;
                        &--img {
                            transform: none /*rtl:scaleX(-1)*/;
                            color: #4c59ff !important; //vuetify
                            font-size: 14px !important; //vuetify
                            font-weight: 600;
    
                            &:before {
                                font-weight: 600 !important;
                            }
                        }
                        &--left {
                            width: 32px;
                            padding: 2px 6px 6px 6px;
                            border-radius: 38px 0 0 38px;
                            border: solid 1px #d7d7d7;
                            outline: none;
                            background: #fff;
                        }
                        &--right {
                            width: 32px;
                            padding: 2px 6px 6px 6px;
                            border-radius: 0 38px 38px 0;
                            border: solid 1px #d7d7d7;
                            outline: none;
                            background: #fff;
                        }
                    }
                    &--text {
                        min-width: 120px;
                        text-align: center;
                        display: flex;
                        align-items: center;
                        font-size: 16px;
                        color: #4d4b69;
                        font-weight: 600;
                    }
                }
            }
        }
    }
    
</style>