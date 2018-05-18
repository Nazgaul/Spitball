export default {
    props:{
        isAnswer:{
            type:Boolean,
            required:false,
            default:false
        },
        myQuestion:{
            type:Boolean,
            required:false,
            default:false
        }
    },
    data() {
        return {
            
        }
    },
    
    computed:{        
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        }         
    },
}