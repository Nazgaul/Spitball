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
                <!-- :plugins="plugins" -->
    
    <flicking   :class="uniqueID"
                :ref="uniqueID"
                :options="optionsObj"
                @move="handleMove"
                @select="select"
                @visibleChange="visibleChange"
                @hold-start="holdStart"
                @hold-end="holdEnd"
              >
    <slot></slot>
    </flicking>
    <!-- <div class="pagination flicking-pagination">
        <div class="flicking-pagination-item selected"></div>
        <div class="flicking-pagination-item"></div>
        <div class="flicking-pagination-item"></div>
    </div> -->
</div>
</template>

<script src="./sbCarousel.js"/>

<style lang="less">
@import "../../styles/mixin.less";
// @import './cs.css';
.sbCarousel{
    position: relative;
    .sbCarousel_btn{
        color: #4c59ff;
        height: 50px !important;
        width: 50px !important;
        position: absolute;
        top: calc(~"50% - 25px");
        z-index: 10000000;
        &:before {
            background-color: transparent !important;
            transition: none !important;
        }
        &.sbCarousel-nextBtn{
            right: -28px;
        }
        &.sbCarousel-prevBtn{
            left: -28px;
        }
    }
}





</style>




