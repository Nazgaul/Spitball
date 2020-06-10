<template>
<div class="message_wrap">
    <component :message="message" :is="currentMessageComponent"></component>
    <div class="chat-loader" :class="{'my_message': isMine}" v-if="getChatLoader && message.isLastMessage">
        <v-progress-circular indeterminate v-bind:size="30" color="#43425d"></v-progress-circular>
    </div>
</div>
</template>

<script>
import {mapGetters} from 'vuex';
import myMessage from './myMessage.vue';
import remoteMessage from './remoteMessage.vue';
export default {
    components: {
        myMessage,
        remoteMessage,
    },
    props:{
        message:{
            type: Object
        },
    },
    computed:{
        ...mapGetters(['accountUser','getChatLoader']),
        isMine(){
            return this.accountUser.id === this.message.userId
        },
        currentMessageComponent(){
            return this.isMine? 'myMessage' : 'remoteMessage'
        }
    },

}

</script>

<style lang='less'>
.message_wrap{
    margin-bottom: 16px;
    font-size: 14px;
    .chat-loader {
        display: flex;
        justify-content: flex-end;
        &.my_message {
            justify-content: center;
        }
    }
}
</style>
