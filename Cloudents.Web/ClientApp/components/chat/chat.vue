<template>
    <v-container v-show="visible" py-0 px-0 class="chat-container" :class="{'minimized': isMinimized}">
        <v-layout @click="toggleMinimizeChat(false)" class="chat-header">
            <v-icon @click="OriginalChatState">sbf-close</v-icon>
            <span class="chat-header-text">{{headerTitle}}</span>
            <span class="other-side">
                <v-icon @click.stop="toggleMinimizeChat">{{isMinimized ? 'sbf-toggle-enlarge' : 'sbf-minimize'}}</v-icon>
                <v-icon @click="closeChatWindow">sbf-close</v-icon>
            </span>
        </v-layout>
        <v-layout v-show="!isMinimized" class="general-chat-style">
            <component :is="`chat-${state}`"></component>
        </v-layout>
    </v-container>
</template>


<script>
    import chatConversation from "./pages/conversations.vue"
    import chatMessages from "./pages/messages.vue"
    import {mapGetters, mapActions} from 'vuex'
    
    export default {
        components:{
            chatConversation,
            chatMessages
        },
        data(){
            return{
                enumChatState: this.getEnumChatState()
            }
        },
        computed:{
            ...mapGetters(['getChatState', 'getIsChatVisible', 'getIsChatMinimized', 'getCurrentConversationObj']),
            state(){
                return this.getChatState;
            },
            visible(){
                return this.getIsChatVisible;
            },
            isMinimized(){
                return this.getIsChatMinimized;
            },
            headerTitle(){
                if(this.state === this.enumChatState.conversation){
                    return "Messages";
                }else{
                    if(!!this.getCurrentConversationObj){
                        return this.getCurrentConversationObj.name;
                    }else{
                        //get user from server to show name
                    }
                }
            }
        },
        methods:{
            ...mapActions(['updateChatState', 'getAllConversations', 'toggleChatMinimize', 'closeChat']),
            ...mapGetters(['getEnumChatState']),
            OriginalChatState(){
                if(!this.isMinimized){
                    this.updateChatState(this.enumChatState.conversation);
                }
            },
            toggleMinimizeChat(val){
                this.toggleChatMinimize(val);
            },
            closeChatWindow(){
                this.closeChat();
            }
        },
        created(){
            this.getAllConversations();
        }
    }
</script>
<style lang="less">
@import '../../styles/mixin.less';
.chat-container{
    .scrollBarStyle(3px, #43425d);
    position: fixed;
    bottom: 0;
    right: 130px;
    width: 320px;
    height: 595px;
    background: #fff;
    border-radius: 4px;
    box-shadow: 0 3px 13px 0 rgba(0, 0, 0, 0.1);
    &.minimized{
        height: unset;
    }
    .chat-header{
        background-color:#43425d;
        border-radius: 4px 4px 0 0;
        padding:10px;
        color:#fff;
        .chat-header-text{
            font-family: @fontOpenSans;
            font-size: 11px;
            font-weight: bold;
            color: #ffffff;
        }
        i{
            color: #a5a4bf;
            font-size:16px;
            margin-right: 14px;
        }
        .other-side{
            margin-left:auto;
            i{
                margin-right: 0;
                margin-left: 14px;
            }
        }
    }
    .general-chat-style{
        height:93%; //minus chat header
        width:100%;        
    }
}
</style>
