<template>
    <v-container v-show="visible" py-0 px-0 class="sb-chat-container" :style="{'height': height}" :class="{'minimized': isMinimized}">
        <v-layout class="chat-header">
            <v-icon :class="{'rtl':isRtl}" @click="OriginalChatState">{{inConversationState ? 'sbf-message-icon' : 'sbf-arrow-back-chat'}}</v-icon>
            <span class="chat-header-text">{{headerTitle}}</span>
            <span class="other-side" v-show="!isMobile">
                <v-icon @click="toggleMinimizeChat">{{isMinimized ? 'sbf-toggle-enlarge' : 'sbf-minimize'}}</v-icon>
                <v-icon @click="closeChatWindow">sbf-close-chat</v-icon>
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
    import { LanguageService } from '../../services/language/languageService'
    export default {
        components:{
            chatConversation,
            chatMessages
        },
        data(){
            return{
                enumChatState: this.getEnumChatState(),
                mobileFooterHeight: 48,
                isRtl: global.isRtl
            }
        },
        computed:{
            ...mapGetters(['getChatState', 'getIsChatVisible', 'getIsChatMinimized', 'getActiveConversationObj', 'getIsChatLocked', 'accountUser']),
            isMobile(){
                return this.$vuetify.breakpoint.smAndDown;
            },
            height(){
                if(this.isMobile){
                    return `${global.innerHeight - this.mobileFooterHeight}px`;
                }else{
                    if(this.isMinimized){
                        return 'unset';
                    }else{
                        return '595px';
                    }
                }
            },
            state(){
                return this.getChatState;
            },
            visible(){
                if(this.isMobile){
                    return true;
                }else{
                    if(this.accountUser === null){
                        return false;
                    }else{
                        return this.getIsChatVisible;
                    }
                }
            },
            isMinimized(){
                 if(this.isMobile){
                    return false;
                }else{
                    return this.getIsChatMinimized;
                }
            },
            headerTitle(){
                if(this.state === this.enumChatState.conversation){
                    return LanguageService.getValueByKey('chat_messages');
                }else{
                    if(!!this.getActiveConversationObj){
                        return this.getActiveConversationObj.userName;
                    }
                }
            },
            inConversationState(){
               return this.state === this.enumChatState.conversation
            }
        },
        methods:{
            ...mapActions(['updateChatState', 'getAllConversations', 'toggleChatMinimize', 'closeChat', 'openChatInterface']),
            ...mapGetters(['getEnumChatState']),
            OriginalChatState(){
                if(!this.getIsChatLocked){
                    this.updateChatState(this.enumChatState.conversation);
                    if(this.isMinimized){
                        this.openChatInterface();
                    }
                }
            },
            toggleMinimizeChat(){
                this.toggleChatMinimize();
            },
            closeChatWindow(){
                this.closeChat();
            }
        },
        created(){
        }
    }
</script>
<style lang="less">
@import '../../styles/mixin.less';
.sb-chat-container{
    .scrollBarStyle(3px, #43425d);
    position: fixed;
    bottom: 0;
    right: 130px;
    width: 320px;
    height: 595px;
    z-index: 3;
    background: #fff;
    border-radius: 4px;
    box-shadow: 0 3px 13px 0 rgba(0, 0, 0, 0.1);
    @media (max-width: @screen-xs) {
        position: unset;
        width: 100%;
        border-radius: unset;
    }
    &.minimized{
        height: unset;
    }
    .chat-header{
        background-color:#43425d;
        border-radius: 4px 4px 0 0;
        padding:10px;
        color:#fff;
        
        @media (max-width: @screen-xs) {
            border-radius: unset;
        }
        .chat-header-text{
            font-family: @fontOpenSans;
            font-size: 11px;
            font-weight: bold;
            color: #ffffff;
            word-break: break-all;
            text-overflow: ellipsis;
            width: 200px;
            white-space: nowrap;
            overflow: hidden;
        }
        i{
            color: #a5a4bf;
            font-size:16px;
            margin-right: 14px;
            &.sbf-arrow-back-chat{
                &.rtl{
                    transform: rotate(180deg);
                }
            }
            
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
        @media (max-width: @screen-xs) {
            height:92%;
        }    
    }
}
</style>
