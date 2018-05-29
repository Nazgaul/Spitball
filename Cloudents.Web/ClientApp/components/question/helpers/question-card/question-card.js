import userBlock from "./../../../helpers/user-block/user-block.vue";

export default {
    components:{userBlock},
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
        },
        cardData: {},
        answerBtn: {
            type: Boolean,
            default: false
        }
    },
    data() {
        return {
            user:{
                img:'https://cdn.pixabay.com/photo/2016/08/20/05/38/avatar-1606916_960_720.png',
                name:'User Name'
            }
        }
    },
    
    computed:{        
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        }         
    }
}