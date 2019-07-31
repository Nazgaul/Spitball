<template>
<div class="message-wrapper" :class="{'myMessage': isMine}">
    <div class="message" :class="{'myMessage': isMine, 'imgMessage': message.type === 'file'}" v-html="$chatMessage(message, date)"></div>
</div>
    
</template>

<script>
import {mapGetters, mapActions} from 'vuex';
import checkMark from '../../../../font-icon/checkmark.svg';
import utilitiesService from '../../../../services/utilities/utilitiesService';
export default {
    components: {
        checkMark
    },
    props:{
        message:{
            type:Object
        }
    },
    computed:{
        ...mapGetters(['accountUser']),
        isMine(){
            return this.accountUser.id === this.message.userId
        },
        date() {
            return utilitiesService.dateFormater(this.message.dateTime);
        }
    }
}
</script>

<style lang='less'>
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
    padding: 10px;
    word-break: break-all; //firefox fallback
    word-break: break-word;
    display: flex;
    flex-direction: column;
    &.myMessage{
        text-align: left;
        margin: 5px 0;
        margin-left: unset;
        margin-right: auto;
        background-color: #dfe1ed;
        border-radius: 8px 8px 8px 0;
    }
    &.imgMessage{
        // height: 160px;
        background: transparent;
        padding: 0;
        margin: 0;
        border-radius: 3px;
        a {
            img {
               border: 2px solid #dcdbe1;
               border-radius: 4px;
            }
        }
    }
    .chat-checkmark {
        display: inline-block;
        transform: rotate(45deg);
        height: 12px;
        width: 6px;
        border-bottom: 2px solid blue;
        margin: 2px 0 0 10px;
        border-right: 2px solid blue;
    }
}
    .message-date{
        color: rgba(0, 0, 0, 0.38);
        display: flex;
        font-size: 11px;
        padding-top: 7px;
        justify-content: flex-end;
        &.myMessage{
            justify-content: flex-start;
        }
    }
}


</style>
