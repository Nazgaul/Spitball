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
        word-break: break-word;
        &:first-child{
            margin-top: 4px;
        }
        .username {
            font-size: 10px;
        }
        .message {
            display: flex;
            flex-direction: column;

            &.own-message {
                align-items: flex-end;
            }
            &.not-own-message {
                align-items: flex-start;
            }
        }
        .chat-message-wrap-text {
            display: inline-flex;
            flex-direction: row;
            &.own-message{
                justify-items: flex-start;
                background: #f6f6f6;
                font-size: 12px;
                color: rgba(0, 0, 0, 0.87);
                background-attachment: fixed;
                padding: 6px 12px;
                border-radius: 16px;
                .message-text{
                    text-align: left;
                }

            }
            &.not-own-message{
                justify-items: flex-end;
                background-color: #e1e1e1;
                font-size: 12px;
                color: rgba(0, 0, 0, 0.87);
                padding: 6px 12px;
                border-radius: 16px;
                .message-text{
                    text-align: right;
                }
            }
        }
    }

</style>