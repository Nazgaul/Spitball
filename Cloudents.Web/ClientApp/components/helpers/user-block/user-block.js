export default {
    props:{
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