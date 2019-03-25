<template>
    <div class="conversations-container">
        <v-layout class="conversations-wrapper"
        @click="openConversation(conversation)"
        justify-start
        row
        v-for="conversation in converations"
        :key="conversation.conversationId">
        <conversation-comp :conversation="conversation"></conversation-comp>
        </v-layout>
    </div>
</template>

<script>
import {mapGetters, mapActions} from 'vuex';
import conversationComp from './conversationComponents/conversation.vue'
export default {
    components:{
        conversationComp
    },
    computed:{
        ...mapGetters(['getConversations']),
        converations(){
            return this.getConversations;
        },
    },
    methods:{
        ...mapActions(['setActiveConversationObj']),
        openConversation(conversation){
            let currentConversationObj = {
                userId:conversation.userId,
                conversationId: conversation.conversationId
            }
            this.setActiveConversationObj(currentConversationObj);
        }
    }
}
</script>

<style lang="less">
.conversations-container{
    width:100%;
    overflow: auto;
    .conversations-wrapper{
        cursor: pointer;
        &:hover{
            background: #f7f7f7;
        }
    }
}
</style>
