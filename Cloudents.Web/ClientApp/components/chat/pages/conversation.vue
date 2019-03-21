<template>
    <div class="conversation-container">
        <v-layout class="conversation-wrapper" @click="openConversation(conversation)" align-center justify-start row v-for="conversation in converations" :key="conversation.conversationId">
            <v-flex ml-2 class="avatar-container"><user-avatar :user-name="conversation.name" :user-id="conversation.userId"/></v-flex>
            <v-flex class="user-detail-container">
                <v-flex class="top-detail-container">
                    <span>name {{conversation.name}}</span>
                    <span>date {{conversation.name}}</span>
                </v-flex> 
                <!-- <v-flex>
                    <span>message {{conversation}}</span>
                </v-flex>  -->
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
import UserAvatar from '../../helpers/UserAvatar/UserAvatar.vue';
import {mapGetters, mapActions} from 'vuex';
export default {
    components:{
        UserAvatar
    },
    computed:{
        ...mapGetters(['getConversations']),
        converations(){
            return this.getConversations;
        }
    },
    methods:{
        ...mapActions(['setActiveConversationId']),
        openConversation(conversation){
            this.setActiveConversationId(conversation.conversationId);
        }
    }
}
</script>

<style lang="less">
.conversation-container{
    width:100%;
    overflow: auto;
    .conversation-wrapper{
        cursor: pointer;
        &:hover{
            background: #f7f7f7;
        }
        .avatar-container{
            flex-grow:0;      
        }
        .user-detail-container{
            padding:12px;
            .top-detail-container{
                display:flex;
                justify-content: space-between;
            }
        }
    }
}
</style>
