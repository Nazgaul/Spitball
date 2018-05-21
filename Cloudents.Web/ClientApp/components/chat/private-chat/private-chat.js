export default {
    data() {
        return {
            messages:[],
            // messages:[{text:"Hello there", fromMe:false}],
            newMessage:''
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