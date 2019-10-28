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
            
        }
    },
    data() {
        return {
            isFirstItemVisible: true,
            isLastItemVisible: false,
            options:{
                hanger: 0,
                anchor: 0,
                circular: this.infinite, 
                moveType: 'freeScroll',
                bound: true
            }
        }
    },
    methods: {
        next(){
            let carouselEl = this.$refs[this.name]
            let itemsCount = carouselEl.getPanelCount()
            let currentItemIndex = carouselEl.getIndex()
            if(currentItemIndex < itemsCount){
                carouselEl.moveTo(currentItemIndex+this.slideStep)
            }
        },
        prev(){
            let carouselEl = this.$refs[this.name]
            let itemsCount = carouselEl.getPanelCount()
            let currentItemIndex = carouselEl.getIndex()
            if(currentItemIndex < itemsCount && currentItemIndex > 0 ){
                carouselEl.moveTo(currentItemIndex-this.slideStep)
            }
        },



        handleMove(event){
            this.$emit('move',event)
            let visiblesItems = this.$refs[this.name].getVisiblePanels()
            this.isLastItemVisible = visiblesItems.some(item=>item.nextSibling === null)
            this.isFirstItemVisible = visiblesItems.some(item=>item.prevSibling === null);
          },
        select(item){
            this.$emit('select',item)
        },
        visibleChange(event){
            this.$emit('visibleChange',event)
        }
    },
}