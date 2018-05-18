export default {
    props:{
        displayRow:{
            type:Number,
            required:false,
            default:1
        },
        items:{
            type:Array,
            required:false,
            default:[]
        }
    },
    data() {
        return {
            classItem:''
        }
    },
    
    computed:{        
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        }         
    },
    mounted(){
        this.classItem = "xs" + Math.floor(12/this.displayRow);
    }
}