<template>
    <div v-if="hasConversations" class="conversations-container">
        <v-layout class="" :class="['conversations-wrapper',{'conversation-active': isCurrentActive(conversation)}]"
        @click="openConversation(conversation)"
        justify-start
        v-for="conversation in converations"
        :key="conversation.conversationId">
            <conversation-comp :conversation="conversation"></conversation-comp>
        </v-layout>
    </div>
    <div v-else>
        <v-layout>
            <span class="conversations-empty-state" v-t="'chat_conversations_empty_state'"></span>
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
    props:{
        filterOptions:{
            required:false,
            type:Object
        }
    },
    computed:{
        ...mapGetters(['getConversations']),
        converations(){
            if(this.filterOptions){
                let filterdList = this.getConversations.filter(c=>c.name.toLowerCase().includes(this.filterOptions.keyWord.toLowerCase()))
                if(!this.filterOptions.isShowAll){
                    filterdList = filterdList.filter(c=>c.unread > 0)
                }
                return filterdList
            }else{
                return this.getConversations;
            }
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
            this.$router.push({...this.$route,params:{id:conversation.conversationId}}).catch(()=>{})
        },
        isCurrentActive({conversationId}){
            let currentActive = this.$store.getters.getActiveConversationObj?.conversationId;
            return conversationId == currentActive && !this.$vuetify.breakpoint.xsOnly;
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
        margin-right: 4px;
        cursor: pointer;
        &.conversation-active{
            background-color: #f0f3f6;
        }
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
