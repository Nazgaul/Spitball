<template>
    <div class="chat-bubble">
        <div class="message" :class="[isMine ? 'own-message' : 'not-own-message' ]">
            <!--<div class="username">{{message.data ? message.data.identity : ''}}</div>-->
            <div class="chat-message-wrap-text" :class="[isMine ? 'own-message' : 'not-own-message' ]">
                <div class="message-text">
                    {{message.data ? message.data.text : ''}}
                </div>

            </div>
        </div>
    </div>
</template>

<script>
    import { mapGetters } from 'vuex'

    export default {
        name: "message.vue",
        props: {
            message: {},
        },
        computed: {
            ...mapGetters({
                'userIdentity': 'userIdentity'
            }),
            isMine() {
                return this.userIdentity === this.message.data.identity
            }
        },

    }
</script>

<style scoped lang="less">
    .chat-bubble {
        margin-bottom: 8px;
        .username {
            font-size: 10px;
        }
        .message {
            display: flex;
            flex-direction: column;

            &.own-message {
                align-items: flex-start;
            }
            &.not-own-message {
                align-items: flex-end;
            }
        }
        .chat-message-wrap-text {
            display: inline-flex;
            flex-direction: row;
            &.own-message{
                justify-items: flex-start;
                color: white;
                background: linear-gradient(to bottom, #00D0EA 0%, #0085D1 100%);
                background-attachment: fixed;
                padding: 16px;
                border-radius: 16px;
                .message-text{
                    text-align: left;
                }

            }
            &.not-own-message{
                justify-items: flex-end;
                background-color: #eee;
                color: #000000;
                padding: 16px;
                border-radius: 16px;
                .message-text{
                    text-align: right;
                }
            }
        }
    }

</style>