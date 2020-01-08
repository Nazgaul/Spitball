import {mapGetters} from 'vuex';
import {
    Hooper,
    Slide,
    Navigation as HooperNavigation
    } from './hooper/hooper';
import './hooper/hooper.css';

export default {
    name:'sbCarousel',
    components:{
        Hooper, 
        Slide,
        HooperNavigation
    },
    props:{
        items:{
            type:Array,
        },
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
        },
        itemsToShow:{
            type:Number
        },
        itemsToSlide:{
            type:Number
        },
    },
    data() {
        return {
            isRtl:global.isRtl,
            isFirstItemVisible: true,
            isLastItemVisible: false,
            isArrows: true,
            // isDragging: false,
            // optionsObj:{
            //     zIndex: 10,
            //     hanger: 0,
            //     anchor: 0,
            //     gap: this.gap, 
            //     circular: this.infinite, 
            //     moveType: this.moveType,
            //     bound: !this.infinite,
            //     overflow: this.overflow,
            //     duration:750,
            //     adaptive:true,
            //     renderOnlyVisible: this.renderOnlyVisible
            // },
            hooperSettings:{
                rtl: global.isRtl,
                itemsToShow: this.itemsToShow,
                itemsToSlide: this.itemsToSlide,
                wheelControl: false,
                keysControl: false,
                trimWhiteSpace: true,
                transition: 750,
            },
            isMoving: false,
            stepMove:null,
            isMounted: false,
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
        },
        itemsToShow(val){
            if(val){
                let carouselEl = this.$refs[this.uniqueID];
                carouselEl.config.itemsToShow = val;
                carouselEl.config.itemsToSlide = val;
                carouselEl.updateWidth();
            }
        }
    },
    computed: {
        ...mapGetters(['getIsTouchMove']),
        uniqueID(){
            return `${this.name}_${Math.random().toString(36).substr(2, 9)}`;
        },
        isDragging(){
            if(!this.isMounted){
                return false
            }else{
                let carouselRef = this.$refs[this.uniqueID];
                return carouselRef.isDragging
            }
        }
    },
    methods: {
        beforeSlide(){
            console.log('beforeSlide');
        },
        slide(){
            console.log('Slide');
        },
        afterSlide(){
            console.log('afterSlide');
        },
        next(){
            let carouselEl = this.$refs[this.uniqueID];
            carouselEl.slideNext();
            // let itemsCount = carouselEl.getPanelCount();
            // let currentItemIndex = carouselEl.getIndex();
            // if(currentItemIndex < itemsCount){
            //     carouselEl.moveTo(currentItemIndex+this.stepMove);
            // }
        },
        prev(){
            let carouselEl = this.$refs[this.uniqueID];
            carouselEl.slidePrev();
            // let itemsCount = carouselEl.getPanelCount();
            // let currentItemIndex = carouselEl.getIndex();
            // if(currentItemIndex < itemsCount && currentItemIndex > 0 ){
            //     carouselEl.moveTo(currentItemIndex-this.stepMove);
            // }
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
    mounted(){
        this.isMounted = true;
    },
    created() {
        this.isArrows = this.arrows;
        if(this.centered){
            this.onCentered();
        }
    },
    
    // mounted() {
    //     this.stepMove = this.$refs[this.uniqueID].getVisiblePanels().length;
    //     if(this.stepMove > 1){
    //         this.stepMove -= 1;
    //     }
    //     let visiblesItems = this.$refs[this.uniqueID].getVisiblePanels();
    //     this.isLastItemVisible = visiblesItems.some(item=>item.nextSibling === null);
    //     this.isFirstItemVisible = visiblesItems.some(item=>item.prevSibling === null);
    //     if(!!this.$slots && this.$slots.default){
    //         this.$slots.default.forEach((slot)=>{
    //             slot.elm.draggable = false;
    //         });
    //     }
    // }
}