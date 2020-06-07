export default {
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