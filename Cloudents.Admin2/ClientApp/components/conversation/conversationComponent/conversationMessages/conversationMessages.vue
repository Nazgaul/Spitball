<template> 
    <div class="expansion-panel elevation-4 pa-4">
        <h3>Conversation</h3>
        <table v-show="!loadMessage">
            <tr v-for="msg in messages">
                <td>{{ studentName === msg.name ? 'student' :'tutor' }}</td>
                <td>{{ msg.text }}</td>
                <td>{{ new Date(msg.dateTime).toLocaleString('he-IL') }}</td>
            </tr>
        </table>
    </div>
</template>

<script>
    import { getDetails, getMessages } from './conversationMessagesService'

    export default {
        data() {
            return {
                messageHeaders: [
                    { text: 'User Name' },
                    { text: 'Text' },
                    { text: 'Date' }
                ],
            }
        },
        props: {
            messages: {
              type: Array,
            },
            loadMessage:{
                type:Boolean,
            }
        },
        computed: {
            studentName() {
                let lastMessage = this.messages.length - 1;
                return this.messages[lastMessage] ? this.messages[lastMessage].name : ''
            }
        },
    }
</script>

<style lang="less">
tr{
    &.student {
        background-color: lightblue;
        }
    &.tutor {
        background-color: lightgray;
        }
}
.expansion-panel {
    height: 256px;
    background: #fcfcfc;
    border-bottom-left-radius: 8px;
    border-bottom-right-radius: 8px;
    overflow: auto;
    table {
        border-top: solid 1px #979797;
        border-bottom: solid 1px #979797;
        width: 100%;
        tr {
            height: 56px;
        }
        tr:nth-child(odd) {
            background: #f7f7f7;
        }
        tr:nth-child(even) {
            background: #fcfcfc;
        }
    }
}
</style>