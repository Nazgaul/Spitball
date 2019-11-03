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
        }
    },
    data() {
        return {
            isFirstItemVisible: true,
            isLastItemVisible: false,
            isArrows: true,
            isDragging: false,
            optionsObj:{
                hanger: 0,
                anchor: 0,
                gap: this.gap, 
                circular: this.infinite, 
                moveType: 'freeScroll',
                bound: !this.infinite,
                overflow: this.overflow
            },
        }
    },
    watch: {
        arrows:function(val){
            this.isArrows = val
        },
        centered: {
            deep: true,
            handler(newVal, oldVal) {
                if(newVal){
                    this.onCentered()
                } else{
                    this.onUnCenterd()
                }
            }
        }
    },
    computed: {
        uniqueID(){
            return `${this.name}_${Math.random().toString(36).substr(2, 9)}`;
        }
    },
    methods: {
        next(){
            let carouselEl = this.$refs[this.uniqueID]
            let itemsCount = carouselEl.getPanelCount()
            let currentItemIndex = carouselEl.getIndex()
            if(currentItemIndex < itemsCount){
                carouselEl.moveTo(currentItemIndex+this.slideStep)
            }
        },
        prev(){
            let carouselEl = this.$refs[this.uniqueID]
            let itemsCount = carouselEl.getPanelCount()
            let currentItemIndex = carouselEl.getIndex()
            if(currentItemIndex < itemsCount && currentItemIndex > 0 ){
                carouselEl.moveTo(currentItemIndex-this.slideStep)
            }
        },
        onCentered(){
            this.optionsObj.hanger = '50%'
            this.optionsObj.anchor = '50%'
            this.optionsObj.bound = false
        },
        onUnCenterd(){
            this.optionsObj.hanger = 0
            this.optionsObj.anchor = 0
            this.optionsObj.bound = true
        },
        handleMove(event){
            this.$emit('move',event)
            let visiblesItems = this.$refs[this.uniqueID].getVisiblePanels()
            this.isLastItemVisible = visiblesItems.some(item=>item.nextSibling === null)
            this.isFirstItemVisible = visiblesItems.some(item=>item.prevSibling === null);
          },
        select(item){
            if(!this.isDragging){
                let vueComponent = item.panel.element.__vue__;
                this.$emit('select', vueComponent)
            }
        },
        visibleChange(event){
            this.$emit('visibleChange',event)
        },
        holdStart(){
            this.isDragging = true;
        },
        holdEnd(){
            this.isDragging = false;
        },
    },
    created() {
        this.isArrows = this.arrows
        if(this.centered){
            this.onCentered()
        }
    },
    mounted() {
        let el = document.querySelector(`.${this.uniqueID}`)
        
        if(!!this.$slots && this.$slots.default){
            this.$slots.default.forEach((slot)=>{
                slot.elm.draggable = false;
            })
        }
    }
}