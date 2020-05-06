<template>
<div class="sbCarousel" style="width:100%;height:100%">
    <template v-if="isArrows">
        <v-btn v-show="!isFirstItemVisible" 
               :ripple="false" 
               class="sbCarousel_btn sbCarousel-prevBtn" 
               @click="prev" fab small color="white">
            <v-icon v-html="'sbf-arrow-left-carousel'"/>
            </v-btn>
            <v-btn v-show="!isLastItemVisible" :ripple="false"
                class="sbCarousel_btn sbCarousel-nextBtn" 
                @click="next" fab small color="white">
            <v-icon v-html="'sbf-arrow-right-carousel'"/>
            </v-btn>
    </template>
    <flicking   :class="uniqueID"
                :ref="uniqueID"
                :options="optionsObj"
                @move="handleMove"
                @select="select"
                @visibleChange="visibleChange"
                @hold-start="holdStart"
                @hold-end="holdEnd"
                @move-end="moveEnd">
    <slot></slot>
    </flicking>
    <!-- <div class="pagination flicking-pagination">
        <div class="flicking-pagination-item selected"></div>
        <div class="flicking-pagination-item"></div>
        <div class="flicking-pagination-item"></div>
    </div> -->
</div>
</template>

<script>
import {mapGetters} from 'vuex';

export default {
    name:'sbCarousel',
    props:{
        name:{
            type: String,
            default: 'sbCarouselRef'
        },
        maxWidth:{
            maxWidth: '100%'
        },
        infinite:{
            type: Boolean,
            default:false
        },
        contentClass:{
            type:String
        },

        arrows:{
            type: Boolean,
            default: true
        },
        slideStep:{
            type: Number,
            default: 1
        },
        gap:{
            default:36
        },
        overflow:{
            type: Boolean,
            default: false
        },
        centered:{
            type: Boolean,
            default: false
        },
        prevFun: {
            type: Function
        },
        nextFun: {
            type: Function
        },
        moveEnd:{
            type: Function,
            default: ()=>{}
        },
        renderOnlyVisible:{
            type:Boolean,
            default:false,
        },
        moveType:{
            type: String,
            default: 'freeScroll'
        }
    },
    data() {
        return {
            isFirstItemVisible: true,
            isLastItemVisible: false,
            isArrows: true,
            isDragging: false,
            optionsObj:{
                zIndex: 10,
                hanger: 0,
                anchor: 0,
                gap: this.gap, 
                circular: this.infinite, 
                moveType: this.moveType,
                bound: !this.infinite,
                overflow: this.overflow,
                duration:750,
                adaptive:true,
                renderOnlyVisible: this.renderOnlyVisible
            },
            stepMove:null,
        };
    },
    watch: {
        arrows:function(val){
            this.isArrows = val;
        },
        centered: {
            deep: true,
            handler(newVal) {
                if(newVal){
                    this.onCentered();
                } else{
                    this.onUnCenterd();
                }
            }
        }
    },
    computed: {
        ...mapGetters(['getIsTouchMove']),
        uniqueID(){
            return `${this.name}_${Math.random().toString(36).substr(2, 9)}`;
        }
    },
    methods: {
        next(){
            let carouselEl = this.$refs[this.uniqueID];
            let itemsCount = carouselEl.getPanelCount();
            let currentItemIndex = carouselEl.getIndex();
            if(currentItemIndex < itemsCount){
                carouselEl.moveTo(currentItemIndex+this.stepMove);
            }
        },
        prev(){
            let carouselEl = this.$refs[this.uniqueID];
            let itemsCount = carouselEl.getPanelCount();
            let currentItemIndex = carouselEl.getIndex();
            if(currentItemIndex < itemsCount && currentItemIndex > 0 ){
                carouselEl.moveTo(currentItemIndex-this.stepMove);
            }
        },
        onCentered(){
            this.optionsObj.hanger = '50%';
            this.optionsObj.anchor = '50%';
            this.optionsObj.bound = false;
        },
        onUnCenterd(){
            this.optionsObj.hanger = 0;
            this.optionsObj.anchor = 0;
            this.optionsObj.bound = true;
        },
        handleMove(event){
            this.$emit('move',event);
            let visiblesItems = this.$refs[this.uniqueID].getVisiblePanels();
            this.isLastItemVisible = visiblesItems.some(item=>item.nextSibling === null);
            this.isFirstItemVisible = visiblesItems.some(item=>item.prevSibling === null);
          },
        select(item){
            let dragging = this.isDragging || this.getIsTouchMove;
            if(!dragging){
                let vueComponent = item.panel.element.__vue__;
                this.$emit('select', vueComponent);
            }
        },
        visibleChange(event){
            this.$emit('visibleChange',event);
        },
        holdStart(){
            this.isDragging = true;
        },
        holdEnd(){
            this.isDragging = false;
        },
    },
    created() {
        this.isArrows = this.arrows;
        if(this.centered){
            this.onCentered();
        }
    },
    mounted() {
        this.stepMove = this.$refs[this.uniqueID].getVisiblePanels().length;
        if(this.stepMove > 1){
            this.stepMove -= 1;
        }
        let visiblesItems = this.$refs[this.uniqueID].getVisiblePanels();
        this.isLastItemVisible = visiblesItems.some(item=>item.nextSibling === null);
        this.isFirstItemVisible = visiblesItems.some(item=>item.prevSibling === null);
        if(!!this.$slots && this.$slots.default){
            this.$slots.default.forEach((slot)=>{
                slot.elm.draggable = false;
            });
        }
    }
}
</script>

<style lang="less">
@import "../../styles/mixin.less";

/*rtl:begin:ignore*/
.sbCarousel{
    position: relative;
    .sbCarousel_btn{
        color: #4c59ff;
        height: 50px !important;
        width: 50px !important;
        position: absolute;
        top: calc(~"50% - 20px");
        z-index: 11;
        &:before {
            background-color: transparent !important;
            transition: none !important;
        }
        &.sbCarousel-nextBtn{
            right: -20px;
        }
        &.sbCarousel-prevBtn{
            left: -20px;
        }
    }
}

/*rtl:end:ignore*/




</style>




