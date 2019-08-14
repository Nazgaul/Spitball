<template>
<div class="message_wrap" >
    <div class="message-wrapper" :class="{'myMessage': isMine}">
        <div class="message" :class="{'myMessage': isMine, 'imgMessage': message.type === 'file'}" >
            <div v-html="$chatMessage(message, date)"></div>
        </div>
    </div>
    <div class="time_wrapper" :class="{'myMessage': isMine}">
        <div v-show="isMine && !message.unreadMessage" :class="[`${!rtl ? 'chat-checkmark' : 'chat-checkmark checkmark-rtl'}${message.unreadMessage ? ' unread-message':''}`]"></div>
        <span class="message-text-date">{{date}}</span>
    </div>
</div>
</template>

<script>
import {mapGetters, mapActions} from 'vuex';
import checkMark from '../../../../font-icon/checkmark.svg';
import utilitiesService from '../../../../services/utilities/utilitiesService';
import timeago from 'timeago.js';

export default {
    components: {
        checkMark
    },
    props:{
        message:{
            type: Object
        }
    },
    data() {
        return {
            rtl: global.isRtl
        }
    },
    computed:{
        ...mapGetters(['accountUser']),
        isMine(){
            return this.accountUser.id === this.message.userId
        },
        date() {
            return timeago().format(new Date(this.message.dateTime));
        },
    }
}

</script>

<style lang='less'>
.message_wrap{
    margin-bottom: 14px;
    .message-wrapper{
        max-width:211px;
        width: max-content; // firefox fallback
        width: fit-content;
        margin-left: auto;
        margin-right: unset;
        &.myMessage{
            margin-left: unset;
            margin-right: auto;
        }
    .message{
        text-align: right;
        margin: 5px 0;
        margin-left: auto;
        margin-right: unset;
        border-radius: 8px 8px 0 8px;
        background-color: #d4d2fe;
        padding: 4px 8px 6px 8px;
        word-break: break-all; //firefox fallback
        word-break: break-word;
        display: flex;
        flex-direction: column;
        color: #1d1d21;
        &.myMessage{
            text-align: left;
            margin: 5px 0;
            padding: 4px 8px 6px 8px;
            margin-left: unset;
            margin-right: auto;
            background-color: #dfe1ed;
            border-radius: 8px 8px 8px 0;
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
        
    .message-file-date {
        color: rgba(0, 0, 0, 0.38);
        font-size: 11px;
        position: absolute;
        bottom: 10px;
        right: 10px;
        color: #fff;
        }
    }
    .time_wrapper {
        margin-top: -2px;
        display: flex;
        justify-content: flex-end;
        .message-text-date {
            color: rgba(0, 0, 0, 0.38);
            font-size: 11px;
            display: flex;
            margin-left: 6px;
            margin-top: 2px;
            &.myMessage{
                justify-content: flex-start;
            }
            .unread-message {
                visibility: hidden;
            }
        }
        .chat-checkmark {
            display: inline-block;
            transform: rotate(40deg);
            height: 11px;
            width: 6px;
            border-bottom: 1.5px solid #5bbdb7;
            margin: 2px 0 0 10px;
            border-right: 1.5px solid #5bbdb7;
            &.checkmark-rtl {
                transform: rotate(-45deg);
                border-left: 2px solid #5bbdb7;
                border-right: 0;
            }
        }
        &.myMessage {
            margin-left: -6px;
            justify-content: flex-start;
        }
    }
}
</style>
