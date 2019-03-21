<template>
    <div class="messages-container">
        <div ml-2 class="avatar-container"><user-avatar :user-name="'gaby'" :user-id="123"/></div>
        <v-layout column class="messages-wrapper">
            <v-flex justify-end class="messages-header">
                <span>schedule</span>
                <span>invite</span>
            </v-flex>
            <v-flex class="messages-body">
                <message :message="singleMessage" v-for="(singleMessage, index) in messages" :key="index"></message>
            </v-flex>
            <v-flex class="messages-input">
                <chat-upload-file></chat-upload-file>
                <v-text-field solo type="text" placeholder="Type a message" @keyup.enter="sendMessage" v-model="messageText"></v-text-field>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
import message from "./messageComponents/message.vue"
import UserAvatar from '../../helpers/UserAvatar/UserAvatar.vue';
import chatUploadFile from './messageComponents/chatUploadFile.vue';
import {mapGetters, mapActions} from 'vuex';
export default {
    components:{
        message,
        UserAvatar,
        chatUploadFile
    },
    data(){
        return{
            messageText: ""
        }
    },
    computed:{
        ...mapGetters(['getMessages']),
        messages(){
            return this.getMessages;
        }
    },
    methods:{
        ...mapActions(['sendChatMessage']),
        sendMessage(){
            this.sendChatMessage(this.messageText)
            this.messageText = "";
        }
    }
}
</script>

<style lang="less">
    .messages-container{
        width: 100%;
        height: 100%;
        .avatar-container{
            position:absolute;
            top: 55px;
            left: 10px;
        }
        .messages-wrapper{
            height: 100%;
            .messages-header{
                display:flex;
                background-color: #f7f7f7;
                padding: 8px;
                max-height: 34px;
                min-height: 34px;
                span{
                    padding: 0 10px 0  10px;
                }
            }
            .messages-body{
                padding: 15px 10px 0 10px;
                overflow: auto;
            }
            .messages-input{
                display: flex;
                flex-direction: row-reverse;
                box-shadow: 0px -3px 0px 0px rgba(240,240,247,1);
                border-radius: 0 0 16px 16px;
                max-height: 45px;
                min-height: 45px;
                padding-right: 14px;
                .v-input__slot{
                    box-shadow: none !important;
                }
            }
        }
        
    }
</style>
