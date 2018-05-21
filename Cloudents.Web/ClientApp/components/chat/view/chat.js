import conversationsList from "./../helpers/conversations-list/conversations-list.vue";
import miniChat from "./../private-chat/private-chat.vue";

export default {
    components:{conversationsList, miniChat},
    data() {
        return {
            currentRoom:null,
            rooms:[
                {
                    user: 'Irma Rogers',
                    lastMsg:'Selling iphone 8 plus 256 gb'                    
                },
                {
                    user: 'Edi56790',
                    lastMsg:'Algebra - question'                    
                }
            ]
            
        }
    },
    methods:{
        selectRoom(room){
            this.currentRoom = room;
        }
    },
    computed:{        
        isMobile(){return this.$vuetify.breakpoint.xsOnly;}
    }
}