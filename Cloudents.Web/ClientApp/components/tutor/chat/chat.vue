<template>
    <v-layout row>
        <v-flex xs12 sm10 order-xs2 style="position: relative;">
            <!--<div class="chat-container" v-on:scroll="onScroll" >-->
            <div class="chat-container" >
                <message :messages="messages"></message>
            </div>
            <div class="typer">
                <input type="text" placeholder="Type here..." @keyup.enter="sendMessage" v-model="messageText">
            </div>
        </v-flex>
    </v-layout>
</template>

<script>
    import chatService from './chatService';
    import message from './helpers/message.vue';
    import { dataTrack } from '../tutorService';

    export default {
        name: "chat.vue",
        components: {message},
        data() {
            return {
                messageText: {
                    type: String,
                    default: ''
                },
                messages: [],
            }
        },
        props: {
            id: {},
        },
        methods: {
            sendMessage() {
                let messageObj = {
                    "text": this.messageText,
                    "type": dataTrack
                };
                let mess = chatService.createMessageItem(messageObj);
                this.addUserMessageToAll(mess);
                chatService.sendChatMessage(messageObj)
                this.messageText ='';
            },
            addUserMessageToAll(message) {
                this.messages.push(message);
            },
        },
    }
</script>

<style scoped>

</style>