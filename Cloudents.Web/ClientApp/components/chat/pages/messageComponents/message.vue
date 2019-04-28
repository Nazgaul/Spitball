<template>
<div class="message-wrapper" :class="{'myMessage': isMine}">
    <span class="message-date" :class="{'myMessage': isMine}">{{date}}</span>
    <div class="message" :class="{'myMessage': isMine, 'imgMessage': message.type === 'file'}" v-html="$chatMessage(message)"></div>
</div>
    
</template>

<script>
import {mapGetters, mapActions} from 'vuex';
import utilitiesService from '../../../../services/utilities/utilitiesService';
export default {
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
    background-color: #f5f5f5;
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
        background-color: #dfdeff;
        border-radius: 8px 8px 8px 0;
    }
    &.imgMessage{
        height: 160px;
    }
}
    .message-date{
        color: rgba(0, 0, 0, 0.38);
        display: flex;
        font-size: 11px;
        &.myMessage{
            justify-content: flex-end;
        }
    }
}


</style>
