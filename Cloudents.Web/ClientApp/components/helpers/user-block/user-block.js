import UserAvatar from "../UserAvatar/UserAvatar.vue";

export default {
    components: {UserAvatar},
    props:{
        name:{String},
        user:{
            required:true,
            type: Object
        },
        classType:{
            type:String,
            required:false,
            default:''
        },
        text:{
            type:String,
            required:false,
            default:''
        }
    },
    data() {
        return {
        }
    }
   
}