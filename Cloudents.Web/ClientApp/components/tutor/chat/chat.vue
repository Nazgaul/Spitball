<template>
    <v-container class="py-0">
    <v-layout row class="chat-container">
        <v-flex xs12 sm12>
            <!--<div class="chat-container" v-on:scroll="onScroll" >-->
            <div class="messages-container" id="messages-container">
                <message :message="singleMessage" v-for="singleMessage in messages"></message>
            </div>
            <div class="chat-input-block">
                <v-text-field solo type="text" placeholder="Type here..." @keyup.enter="send" v-model="messageText">
                </v-text-field>
                <!--<v-btn @click="send">Send</v-btn>-->
            </div>
        </v-flex>
    </v-layout>
    </v-container>
</template>

<script>
    import {mapGetters, mapActions} from 'vuex';
    import message from './helpers/message.vue';

    export default {
        name: "chat.vue",
        components: {message},
        data() {
            return {
                messageText: '',
            }
        },
        props: {
            id: {},
        },
        computed: {
            ...mapGetters({'messages': 'getChatMessages',
                'userIdentity': 'userIdentity'
            }),
            name() {
                return this.data;
            }
        },
        watch: {
            messages(newValue, oldValue) {
                if(newValue){
                    this.$nextTick(() => {
                        setTimeout(() => {
                            this.scrollToEnd()
                        }, 130);
                    });


                }
            }
        },
        methods: {
            ...mapActions(['addMessage', 'sendMessage']),
            scrollToEnd: function() {
                let container = this.$el.querySelector("#messages-container");
                container.scrollTop = container.scrollHeight;
            },
            send() {
                let messageObj = {
                    "text": this.messageText,
                    "type": 'tutoringChatMessage',
                    "identity" : this.userIdentity
                };
                this.sendMessage(messageObj);
                this.messageText ='';
            },
        },
    }
</script>

<style scoped lang="less">
    .chat-container{
            background-color: #ffffff;
            bottom: 0;
            height: 320px;
            max-width: 388px;
            min-width: 388px;
            font-family: 'Open Sans', sans-serif;
        .messages-container{
            height: 320px;
            max-width: 388px;
            min-width: 388px;
            overflow: auto;
        }
        .chat-input-block{
            display: flex;
            background-color: #cccccc;
            border-radius: 0 0 16px 16px;
            max-height: 45px;

        }

    }

</style>