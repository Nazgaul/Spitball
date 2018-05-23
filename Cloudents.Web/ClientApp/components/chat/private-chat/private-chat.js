import userBlock from "../../helpers/user-block/user-block.vue"

export default {
    components:{userBlock},
    data() {
        return {
            messages:[],
            // messages:[{text:"Hello there", fromMe:false}],
            newMessage:'',
            user:{name:'User Name'}
        }
    },
    methods:{
        sendMsg(){
            if(this.newMessage.length){
                this.messages.push({text:this.newMessage, fromMe:true});
                this.newMessage = '';
            }
        }
    }
   
}