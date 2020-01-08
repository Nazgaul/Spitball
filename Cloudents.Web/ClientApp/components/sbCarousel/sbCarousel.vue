<template>
<div class="sbCarousel" style="width:100%;height:100%" :class="{'isRtl': isRtl}">
    <!-- <template v-if="isArrows">
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
    </template> -->
    
    <hooper :class="uniqueID" :ref="uniqueID" :settings="hooperSettings">
        <slide v-for="(item, index) in items" :key="index">
            <slot name="slide" :item="item" :index="index" :isDragging="isDragging"></slot>
        </slide>
        <hooper-navigation v-if="arrows" slot="hooper-addons"></hooper-navigation>
    </hooper>
    <!-- <flicking   :class="uniqueID"
                :ref="uniqueID"
                :options="optionsObj"
                @move="handleMove"
                @select="select"
                @visibleChange="visibleChange"
                @hold-start="holdStart"
                @hold-end="holdEnd"
                @move-end="moveEnd">
    <slot></slot>
    </flicking> -->
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
/*rtl:begin:ignore*/
.sbCarousel{
    position: relative;
    .hooper{
        outline: none;
        height:100%;
        .hooper-navigation{
                button{
                    outline:none;
                    background: #FFF;
                    border-radius: 50%;
                    display: flex;
                    box-shadow: 0 3px 5px -1px rgba(0,0,0,.2), 0 6px 10px 0 rgba(0,0,0,.14), 0 1px 18px 0 rgba(0,0,0,.12);
                    &.is-disabled{
                        display: none;
                    }
                    svg{
                        fill: #4c59ff;
                    }
                }
                .hooper-prev{
                    right: auto  !important;
                    left: -15px  !important;
                }
                .hooper-next{
                    left: auto !important;
                    right: -15px  !important;
                }
            &.is-rtl{
                .hooper-prev{
                    left: auto  !important;
                    right: -15px  !important;
                }
                .hooper-next{
                    right: auto !important;
                    left: -15px  !important;
                }
            } 
        }
    }
    &.isRtl{
        .hooper{
            direction: rtl !important;
        }
    }
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




