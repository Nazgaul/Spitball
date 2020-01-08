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
        arrows:{
            type: Boolean,
            default: true
        },
        itemsToShow:{
            type:Number
        },
        itemsToSlide:{
            type:Number
        },
        isCarouselReady:{
            type:Function
        }
    },
    data() {
        return {
            isRtl:global.isRtl,
            isArrows: true,
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
    mounted(){
        //Need this, to let the vue engine recognize the $refs.
        this.isMounted = true;
        if(this.isCarouselReady){
            //tell the parent component that the carousel is ready and can be watched
            this.isCarouselReady();
        }
    },
    created() {
        this.isArrows = this.arrows;
    },
}