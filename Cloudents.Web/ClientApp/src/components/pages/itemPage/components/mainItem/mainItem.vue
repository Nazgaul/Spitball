<template>
    <div class="mainItem">
        <template v-if="!videoLoader">
            <div class="text-xs-center mainItem__loader">
                <img :src="require('./doc-preview-animation.gif')" alt="Photo" :class="{'video_placeholder': $vuetify.breakpoint.smAndDown}">
            </div>
        </template>
        <div v-if="videoLoader">
            <template v-if="isVideo && videoSrc">
                <div style="margin: 0 auto;background:black" class="text-center mainItem__item">
                <v-fade-transition>
                    <unlockItem v-if="showAfterVideo && !isPurchased" :type="document.documentType"/>
                </v-fade-transition>
                <sbVideoPlayer 
                    @videoEnded="updateAfterVideo()"
                    :id="`${document.id}`"
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
                        <!-- <div style="max-height: 500px;overflow-y: scroll;border: 1px solid #ddd;" :style="{height: `${dynamicWidthAndHeight.height}px`}"> -->
                        <div :style="{height: `${dynamicWidthAndHeight.height}px`}">
                            <v-scroll-x-reverse-transition>
                                <unlockItem v-touch="{
                                    left:() => handleSwipe(false),
                                    right:() => handleSwipe(true)}"  
                                    class="unlockItem_swipe" v-if="showUnlockPage" :type="document.documentType" :docLength="docPreview.length"/>
                            </v-scroll-x-reverse-transition>                
                            <v-carousel
                                :touch="{left: () => handleSwipe(false),right: () => handleSwipe(true)}"
                                :show-arrows="false" hide-delimiters 
                                :height="dynamicWidthAndHeight.height" 
                                v-model="docPage">
                                <v-carousel-item v-for="(doc, index) in docPreview" :key="index">
                                    <img :src="doc" draggable="false" class="mainItem__item__wrap--img" alt="">
                                </v-carousel-item>
                            </v-carousel>
                        </div>
                    </div>
                </template>
            </div>
        </div>
    </div>
</template>

<script>
import {  mapGetters } from 'vuex';
const sbVideoPlayer = () => import('../../../../sbVideoPlayer/sbVideoPlayer.vue');
const unlockItem = () => import('../unlockItem/unlockItem.vue');

export default {
    name: 'mainItem',
    components: {
        sbVideoPlayer,
        unlockItem,
    },
    props: {
        document: {
            type: Object
        }
    },
    data() {
        return {
            showAfterVideo:false,
        }
    },
    watch:{
        '$route.params.id'(){
            //reset the document with the v-if, fixing issue that moving from video to document wont reset the video ELEMENT
            // let self = this;
            this.docPage = 0;
        },
    },
    computed: {
        ...mapGetters(['getDocumentLoaded']),
        docPage:{
            get(){
                return this.$store.getters.getCurrentPage;
            },
            set(val){
                this.$store.commit('setItemPage',val)
            }
        },

        showUnlockPage(){
            return (this.docPage > 1 && !this.isPurchased)
        },
        isPurchased() {
            return this.document?.id ? this.document.isPurchased : false;
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
                    return this.$proccessImageUrl(
                        preview,
                        this.dynamicWidthAndHeight.width,
                        this.dynamicWidthAndHeight.height,
                        "pad",
                        "ffffff"
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
            let width = Math.min(global.innerWidth,945);
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
            return this.document?.title || null;
        },
        selector() {
            let element = document.querySelector('.itemPage__main')
            if(!element) return '';
            return element;
        },
    },
    methods: {
        prevDoc() {
            this.docPage--;
        },
        nextDoc() {
            this.docPage++;
        },
        updateAfterVideo(){
            this.showAfterVideo = true;
        },
        handleSwipe(dir){
            if(dir){
                this.$vuetify.rtl ? this.nextDoc() : this.prevDoc();
            }else{
                this.$vuetify.rtl ? this.prevDoc() : this.nextDoc();
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
                max-height: 530px;
                width: 100% !important;
                @media (max-width: @screen-xs) {
                    max-height: initial;
                }
            }
        }
        &__item {
            position: relative;
            background: #fff;
            &__wrap {
                position: relative;
                height: 100%;
               // max-width: 720px;
                margin: 0 auto;
                .sbCarousel {
                    div:first-child {
                        height: 100% !important;
                        z-index: 4 !important;
                    }
                }
                &--img  {
                    border-radius: 8px 8px 0 0;
                    width: 100%;

                    @media (max-width: @screen-xs) {
                        border-radius: 0;
                    }
                }
            }
        }
    }
    
</style>