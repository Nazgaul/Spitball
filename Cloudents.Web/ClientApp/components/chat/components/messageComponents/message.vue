<template>
<div class="message_wrap" >
    <div class="message-wrapper" :class="{'myMessage': isMine}">
        <div class="message" :class="{'myMessage': isMine, 'imgMessage': message.type === 'file'}" >
            <div v-html="$chatMessage(message)"></div>
        </div>
    </div>
    <div class="time_wrapper" :class="{'myMessage': isMine}">
        <span class="message-text-date">{{date}}</span>
        <double-check v-show="isMine && !message.unreadMessage" />
    </div>
    <div class="chat-loader" :class="{'my_message': isMine}" v-if="getChatLoader && lastMsgIndex">
        <v-progress-circular indeterminate v-bind:size="30" color="#43425d"></v-progress-circular>
    </div>
</div>
</template>

<script>
import {mapGetters, mapActions} from 'vuex';
import doubleCheck from '../../images/group-2.svg';
import timeAgoService from '../../../../services/language/timeAgoService';

export default {
    components: {
        doubleCheck
    },
    props:{
        message:{
            type: Object
        },
        lastMsgIndex: {}
    },
    computed:{
        ...mapGetters(['accountUser','getChatLoader']),

        isMine(){
            return this.accountUser.id === this.message.userId
        },
        date() {
            return timeAgoService.timeAgoFormat(this.message.dateTime)
        },
    }
}

</script>

<style lang='less'>
.message_wrap{
    margin-bottom: 14px;
    font-size: 14px;
    .message-wrapper{
        max-width: 70%;
        width: max-content; // firefox fallback
        width: fit-content;
        margin-left: unset;
        margin-right: auto;
        &.myMessage{
            margin-right: unset;
            margin-left: auto;
        }
        .message{
            margin: 5px 0;
            margin-left: auto;
            margin-right: unset;
            border-radius: 8px 8px 8px 0;
            background-color: #d4d2fe;
            padding: 4px 8px 6px 8px;
            word-break: break-all; //firefox fallback
            word-break: break-word;
            display: flex;
            flex-direction: column;
            color: #1d1d21;
            text-align: left;
            div {
                    white-space: pre-wrap;
                }
            &.myMessage{
                margin: 5px 0;
                padding: 4px 8px 6px 8px;
                margin-left: unset;
                margin-right: auto;
                background-color: #dfe1ed;
                border-radius: 8px 8px 0px 8px;
            }
            &.imgMessage{
                position: relative;
                background: transparent;
                padding: 0;
                margin: 0;
                border-radius: 3px;
                a {
                    img {
                    border: 2px solid #dcdbe1;
                    border-radius: 4px;
                    height: 144px;
                    border-radius: 4px;
                    }
                }
            }
        }
    }
    .time_wrapper {
        margin-top: -2px;
        display: flex;
        justify-content: flex-start;
        align-items: center;
        .message-text-date {
            color: rgba(0, 0, 0, 0.38);
            font-size: 11px;
            display: flex;
            margin-right: 6px;
            margin-top: 2px;
            &.myMessage{
                justify-content: flex-end;
            }
        }
        &.myMessage {
            justify-content: flex-end;
        }
    }
    .chat-loader {
        display: flex;
        justify-content: flex-end;
        &.my_message {
            justify-content: center;
        }
    }
}
</style>
