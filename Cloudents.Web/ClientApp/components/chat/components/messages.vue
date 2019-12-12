<template>
    <div class="messages-container">
        <v-layout column class="messages-wrapper">

            <div class="messages-header">
                <div class="messages-study-room" :class="{'join-room': studyRoomExists || isStudyRoom, 'create-room': !studyRoomExists && isRoomTutor}" v-if="showStudyRoomInteraction || isStudyRoom" @click="createRoom">
                    <button v-if="studyRoomExists || isStudyRoom">
                        <v-icon style="font-size:16px; color:#fff; margin: 0 8px 0 0;">sbf-enter-icon</v-icon>&nbsp;
                        <span v-language:inner="'chat_studyRoom_enter'"></span>
                    </button>

                    <v-btn v-if="(!studyRoomExists && isRoomTutor) && !isStudyRoom " 
                           flat class="white--text messages-study-room-btn-create" 
                           :loading="loader">
                        <add-circle />&nbsp;&nbsp;&nbsp;<span v-language:inner="'chat_studyRoom_create'"></span>
                    </v-btn>
                </div>
            </div>

            <div class="messages-body">
                <message :message="singleMessage" v-for="(singleMessage, index) in messages" :key="index" :lastMsgIndex="index === messages.length - 1"></message>
            </div>

            <span class="error-file-span" v-if="fileError" v-language:inner="'chat_file_error'"></span>

            <div class="messages-input" :class="{'messages-input-disabled': !getIsSignalRConnected}">
                <span class="messages-mobile-button" v-if="typing" @click="sendMessage"><v-icon class="">sbf-path</v-icon></span>
                <chat-upload-file :typing="typing"></chat-upload-file>
                <v-textarea 
                    rows="1" 
                    class="pa-2 messages-textarea" solo 
                    type="text" hide-details 
                    :disabled="!getIsSignalRConnected" 
                    :placeholder="placeHolderText" 
                    v-language:placeholder
                    @keydown.enter.prevent="sendMessage" 
                    ref="chatTextArea"
                    v-model="messageText" auto-grow>
                </v-textarea>
            </div>

        </v-layout>
    </div>
</template>

<script>
import message from "./messageComponents/message.vue"
import chatUploadFile from './messageComponents/chatUploadFile.vue';

import UserAvatar from '../../helpers/UserAvatar/UserAvatar.vue';
import userOnlineStatus from '../../helpers/userOnlineStatus/userOnlineStatus.vue';

import {mapGetters, mapActions} from 'vuex';
import { LanguageService } from '../../../services/language/languageService';
import chatService from '../../../services/chatService';
import addCircle from '../images/add-circle-outline.svg';

import analyticsService from '../../../services/analytics.service';


export default {
    components:{
        message,
        addCircle,
        UserAvatar,
        chatUploadFile,
        userOnlineStatus
    },
    data(){
        return{
            messageText: "",
            placeHolderText: LanguageService.getValueByKey("chat_type_message"),
            emptyStateMessages: [],
            alreadyCreated: false,
            loader: false,
            lastMsgIndex: null
        }
    },
    computed:{
        ...mapGetters(['getshowStudentStudyRoom','getMessages', 'accountUser', 'getActiveConversationObj', 'getChatLoader', 'getIsSignalRConnected','getFileError']),
        fileError(){
            return this.getFileError
        },
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
            return this.isTutor && this.messages &&  this.messages.length > 0 && this.messages[0].userId !== this.accountUser.id && !this.messages[0].isDummy;
        },
        typing() {
            return !!this.messageText;
        },
        isStudyRoom(){
            return this.getshowStudentStudyRoom
        }
    },
    methods:{
        ...mapActions(['sendChatMessage', 'createStudyRoom']),
        sendMessage(){     
            let messageToSend = this.messageText.trim();
            if(messageToSend !== ''){
                this.sendChatMessage(this.messageText);
                this.messageText = "";
                this.$refs.chatTextArea.$el.querySelector('textarea').style.height = "32px";
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
            if((this.$route && this.$route.params && this.$route.params.id) &&
                (!!this.activeConversationObj && this.activeConversationObj.studyRoomId)){
                let paramId = this.$route.params.id;
                let studyRoomId = this.activeConversationObj.studyRoomId
                if(paramId == studyRoomId) return;
            }
            
            let conversationObj = this.activeConversationObj;
            this.loader = true;
            if(!!this.activeConversationObj.studyRoomId){
                let routeData = this.$router.resolve({
                    name: 'roomSettings',
                    params: {
                        id: this.activeConversationObj.studyRoomId
                    }
                });
                global.open(routeData.href, '_self');
                // this.$router.push(routeData.href);
            }else{
                if(!this.alreadyCreated){
                    let userId = conversationObj.userId;
                    this.createStudyRoom(userId).then(() => {
                      analyticsService.sb_unitedEvent('study_room', 'created', `tutorName: ${this.accountUser.name} tutorId: ${this.accountUser.id}`);
                      this.loader = false
                    });
                    this.alreadyCreated = true;
                }
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
            .error-file-span{
                padding: 10px 10px 40px;
                opacity: 0.8;
                background-color: red;
                color: white;
                text-align: center;
            }
            .messages-header{
                display:flex;
                justify-content: flex-end;
                .messages-study-room{
                        padding: 2px 0;
                        width: 100%;
                        color: #FFF;
                        text-align: center;
                        display: flex;
                        &.join-room {
                            background: #2ec293;
                        }
                        &.create-room {
                            background: #4452fc;
                        }
                        button {
                            display: flex;
                            align-items: center;
                            padding: 5px 10px;
                            margin: 0 auto;
                            font-weight: bold;
                            font-size: 14px;
                            outline: none;
                        }
                        .messages-study-room-btn-create {
                           font-size: 14px;
                           font-weight: bold;
                           margin: 0 auto;
                           height: auto;
                           padding: 5px 10px;
                           text-transform: capitalize;
                        }
                        .messages-study-room-btn-create::before {
                           content: "";
                           color: transparent;
                        }
                }
               
            }
            .messages-body{
                flex :2;
                padding: 12px 10px 20px 10px;
                margin: 0 0 4px 0;
                overflow-y: auto;
                -webkit-overflow-scrolling: touch;
                overscroll-behavior: none;
                .message_wrap:last-child {
                    @media(max-width: @screen-xs) {
                        margin-bottom: 40px;
                    }
                }
            }
            .messages-body-disabled {
                padding: 15px 10px 0 10px;
                margin: 22px 0 4px 0;
                overflow: auto;
            }
            .chat-upload-loader{
                position: absolute;
                width: 100%;
                right: 0;
                left: 0;
                bottom: 0;
                top: 0;
            }
            .messages-input{
                display: flex;
                flex-direction: row-reverse;
                box-shadow: 0px -3px 0px 0px rgba(240,240,247,1);
                background: #efefef;
                top: 6px;
                // padding-right: 54px;
                .responsive-property(padding-right, 54px, null, 64px);                
                position: relative;
                .messages-mobile-button {
                        position: absolute;
                        z-index: 5;
                        top: 10px;
                        right: 14px;
                    i { 
                        //Do not put it last because then the remark are gone
                        transform: rotateY(0deg)/*rtl:rotateY(180deg)*/; 
                        /*rtl:append:transform: rotateY(180deg);*/;
                        color: #FFF;
                        font-size: 12px;
                        background-color: @global-blue;
                        padding: 10px;
                        border-radius: 70%;
                        width: 30px;
                        height: 30px;
                    }
                }
                &.messages-input-disabled{
                    background: #b9b9b9;
                    border-radius: 0;
                    .v-input--is-disabled{
                        .v-input__slot{
                            background: #b9b9b9;
                        }
                    }
                }
                .messages-textarea {
                    font-size: 14px;
                    .v-input__control {
                        min-height: 30px;
                        .v-input__slot {
                            box-shadow: none !important;
                            padding: 0 40px 0 12px;
                            border: solid 1px #d8d9e5;
                            border-radius: 20px;
                            textarea{
                                margin-top:0;
                                max-height: 150px;
                                overflow: auto;
                                line-height: 16px;
                            }
                      }
                    }
                }
                @media(max-width: @screen-xs) {
                    position: absolute;
                    bottom: 0;
                    top: auto;
                    width: 100%;
                }
            }
        }
    }
</style>
