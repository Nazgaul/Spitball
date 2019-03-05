<template>
    <v-container class="py-0" id="tutoring-chat-holder">
        <v-layout row>
            <v-flex>
                <div class="chat-header px-3 py-2" @click="minimizeChat()">
                        <span class="title-chat">Chat</span>
                        <span class="chat-size-ctrl" @click.stop="minimizeChat()">
                              <v-icon v-if="visibleChat" class="video-size-icon">sbf-minimize</v-icon>
                              <v-icon v-else class="video-size-icon">sbf-toggle-enlarge</v-icon>
                        </span>
                </div>
            </v-flex>
        </v-layout>

    <v-layout row class="chat-container" v-show="visibleChat">
        <v-flex xs12 sm12>
            <div class="messages-container" id="messages-container">
                <message :message="singleMessage" v-for="singleMessage in messages"></message>
            </div>
            <div class="chat-input-block">
                <v-text-field solo type="text" placeholder="Write a message..." @keyup.enter="send" v-model="messageText">
                </v-text-field>
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
                visibleChat: true
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
            minimizeChat() {
                this.visibleChat = !this.visibleChat;
            },
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

<style  lang="less">
    @import '../../../styles/mixin.less';
    #tutoring-chat-holder{
        .chat-header{
            display: flex;
            flex-direction: row;
            align-items: center;
            justify-content: space-between;
            border-radius: 4px;
            background-color: #eeeeee;
            min-width: 388px;
            .title-chat{
                color: #000000;
                width: 30px;
                font-size: 14px;
                font-weight: 600;
                letter-spacing: -0.4px;
            }
                .chat-size-icon{
                    color: #7b7b7b;
                    font-size: 14px;
                }

        }
        .messages-container{
            height: 100%;
            max-height: 270px;
            max-width: 388px;
            min-width: 388px;
            overflow: auto;
            padding: 0 8px;
        }
        .chat-input-block{
            display: flex;
            background-color: #cccccc;
            border-radius: 0 0 16px 16px;
            max-height: 45px;

        }
        .chat-container{
            .scrollBarStyle(3px, #0085D1);
            background-color: #ffffff;
            bottom: 0;
            height: 320px;
            max-width: 388px;
            min-width: 388px;
            font-family: 'Open Sans', sans-serif;

        }
    }


</style>