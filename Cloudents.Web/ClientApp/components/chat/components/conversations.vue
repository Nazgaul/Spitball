<template>
    <div v-if="hasConversations" class="conversations-container">
        <v-layout class="conversations-wrapper"
        @click="openConversation(conversation)"
        justify-start
        v-for="conversation in converations"
        :key="conversation.conversationId">
            <conversation-comp :conversation="conversation"></conversation-comp>
        </v-layout>
    </div>
    <div v-else>
        <v-layout>
            <span class="conversations-empty-state" v-language:inner="'chat_conversations_empty_state'"></span>
        </v-layout>
    </div>
</template>

<script>
import {mapGetters, mapActions} from 'vuex';
import conversationComp from './conversationComponents/conversation.vue';
import chatService from '../../../services/chatService';

export default {
    components:{
        conversationComp
    },
    computed:{
        ...mapGetters(['getConversations']),
        converations(){
            return this.getConversations;
        },
        hasConversations(){
            return Object.keys(this.converations).length > 0;
        }
    },
    methods:{
        ...mapActions(['setActiveConversationObj']),
        openConversation(conversation){
            let currentConversationObj = chatService.createActiveConversationObj(conversation)
            this.setActiveConversationObj(currentConversationObj);
        }
    },
    mounted(){
        this.$forceUpdate();
    }
}
</script>

<style lang="less">
.conversations-container{
    width:100%;
    overflow: auto;
    overscroll-behavior: none;
    .conversations-wrapper{
        cursor: pointer;
    }
}
.conversations-empty-state{
    text-align: center;
    position: absolute;
    left: 0;
    right: 0;
    top: 112px;
}
</style>
