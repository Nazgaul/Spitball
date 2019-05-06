<template>
    <div class="messages-container">
        <div ml-2 class="avatar-container">
            <user-avatar :size="'40'" :user-name="activeConversationObj.name" :user-id="activeConversationObj.userId" :userImageUrl="activeConversationObj.image"/>
            <userOnlineStatus class="user-status" :userId="activeConversationObj.userId"></userOnlineStatus>
        </div>
        <v-layout column class="messages-wrapper">
            <v-flex justify-end class="messages-header">
                <div v-if="showStudyRoomInteraction" @click="createRoom">
                    <span v-show="studyRoomExists" v-language:inner>chat_studyRoom_enter</span>
                    <span v-show="!studyRoomExists && isRoomTutor" v-language:inner>chat_studyRoom_create</span>
                </div>
                <div v-show="showStudyRoomInteraction">
                    <v-icon v-show="studyRoomExists" style="font-size:16px; color:#bcbccb;">sbf-studyroom-icon</v-icon>
                    <v-icon v-show="!studyRoomExists && isRoomTutor" style="font-size:16px; color:#bcbccb;">sbf-studyroom-icon</v-icon>
                </div>
              
            </v-flex>
            <v-flex class="messages-body">
                <message :message="singleMessage" v-for="(singleMessage, index) in messages" :key="index"></message>
            </v-flex>
            <v-flex class="messages-input">
                <chat-upload-file></chat-upload-file>
                <v-text-field solo type="text" :placeholder="placeHolderText" v-language:placeholder @keyup.enter="sendMessage" v-model="messageText"></v-text-field>
            </v-flex>
        </v-layout>
    </div>
</template>

<script>
import message from "./messageComponents/message.vue"
import UserAvatar from '../../helpers/UserAvatar/UserAvatar.vue';
import userOnlineStatus from '../../helpers/userOnlineStatus/userOnlineStatus.vue';
import chatUploadFile from './messageComponents/chatUploadFile.vue';
import {mapGetters, mapActions} from 'vuex';
import { LanguageService } from '../../../services/language/languageService'
export default {
    components:{
        message,
        UserAvatar,
        chatUploadFile,
        userOnlineStatus
    },
    data(){
        return{
            messageText: "",
            placeHolderText: LanguageService.getValueByKey("chat_type_message"),

        }
    },
    computed:{
        ...mapGetters(['getMessages', 'accountUser', 'getActiveConversationObj']),
        messages(){
            this.scrollToEnd();
            return this.getMessages;
        },
        isTutor(){
            return this.accountUser.isTutor;
        },
        activeConversationObj(){
            return this.getActiveConversationObj;
        },
        studyRoomExists(){
            if(this.activeConversationObj && this.activeConversationObj.studyRoomId){
                return this.activeConversationObj.studyRoomId.length > 1
            }
        },
        showStudyRoomInteraction(){
            return this.messages &&  this.messages.length > 0;
        },
        isRoomTutor(){
            return this.isTutor && this.messages &&  this.messages.length > 0 && this.messages[0].userId !== this.accountUser.id;
        }
    },
    methods:{
        ...mapActions(['sendChatMessage', 'createStudyRoom']),
        sendMessage(){
            let messageToSend = this.messageText.trim();
            if(messageToSend !== ''){
                this.sendChatMessage(this.messageText);
                this.messageText = "";
            }
        },
        scrollToEnd: function() {
            this.$nextTick(function(){
                let container = document.querySelector(".messages-body");
                if(container){
                    container.scrollTop = container.scrollHeight;
                }
            })
        },
        createRoom(){
            let conversationObj = this.activeConversationObj;
            if(!!this.activeConversationObj.studyRoomId){
                let routeData = this.$router.resolve({
                    name: 'tutoring',
                    params: {
                        id: this.activeConversationObj.studyRoomId
                    }
                });
                global.open(routeData.href, '_blank');
            }else{
                let userId = conversationObj.userId;
                this.createStudyRoom(userId);
            }
            
        }
    }
}
</script>

<style lang="less">
@import "../../../styles/mixin.less";
    .messages-container{
        width: 100%;
        height: 100%;
        .avatar-container{
            position:absolute;
            top: 55px;
            left: 10px;
            .user-status{
                position: absolute;
                bottom: 0px;
                right: -2px;
            }
        }
        .messages-wrapper{
            height: 100%;
            .messages-header{
                display:flex;
                background-color: #f7f7f7;
                padding: 8px 10px 8px 8px ;
                max-height: 34px;
                min-height: 34px;
                span{
                    padding: 0 10px 0  10px;
                    cursor: pointer;
                    font-size: 12px;
                    color: @purpleNewColor;
                }
            }
            .messages-body{
                padding: 15px 10px 0 10px;
                margin: 22px 0 4px 0;
                overflow: auto;
            }
            .messages-input{
                display: flex;
                flex-direction: row-reverse;
                box-shadow: 0px -3px 0px 0px rgba(240,240,247,1);
                border-radius: 0 0 16px 16px;
                max-height: 45px;
                min-height: 45px;
                padding-right: 20px;
                .v-input__slot{
                    box-shadow: none !important;
                    
                }
            }
        }
        
    }
</style>
