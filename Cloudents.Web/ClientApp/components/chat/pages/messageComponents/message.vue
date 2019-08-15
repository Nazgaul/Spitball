<template>
<div class="message_wrap" >
    <div class="message-wrapper" :class="{'myMessage': isMine}">
        <div class="message" :class="{'myMessage': isMine, 'imgMessage': message.type === 'file'}" >
            <div v-html="$chatMessage(message)"></div>
        </div>
    </div>
    <div class="time_wrapper" :class="{'myMessage': isMine}">
        <double-check v-show="isMine && !message.unreadMessage" />
        <span class="message-text-date">{{date}}</span>
    </div>
</div>
</template>

<script>
import {mapGetters, mapActions} from 'vuex';
import doubleCheck from './group-2.svg';
import {timeAgoFormat} from '../../../../services/language/timeAgoService';

export default {
    components: {
        doubleCheck
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
            return timeAgoFormat(this.message.dateTime)
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
    }
    .time_wrapper {
        margin-top: -2px;
        display: flex;
        justify-content: flex-end;
        align-items: center;
        .message-text-date {
            color: rgba(0, 0, 0, 0.38);
            font-size: 11px;
            display: flex;
            margin-left: 6px;
            margin-top: 2px;
            &.myMessage{
                justify-content: flex-start;
            }
        }
        &.myMessage {
            justify-content: flex-start;
        }
    }
}
</style>
