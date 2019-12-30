import userAvatar from "../UserAvatar/UserAvatar.vue";

export default {
    components: {UserAvatar: userAvatar},
    props:{
        name:{String},
        user:{
            required:true,
            type: Object
        },
        showExtended:{
            required:false,
            type: Boolean
        },
        cardData: {
            required: false,
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
        };
    }
}