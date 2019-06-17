<template> 
    <div class="px-4" v-show="!loadMessage">
        <v-data-table :items="messages"
                      hide-actions
                      disable-initial-sort
                      :headers="messageHeaders"
                      class="elevation-2">
            <template slot="items" slot-scope="props">
                <tr :class="{'student':studentName === props.item.name,'tutor':studentName !== props.item.name}">
                    <td>{{ props.item.name }}</td>
                    <td>{{ props.item.text }}</td>
                    <td>{{ props.item.dateTime.toLocaleString() }}</td>
                </tr>
            </template>
        </v-data-table>
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
                // conversationsDetails: [],
                // conversationsMessages: []
            }
        },
        props: {
            // id: {
            //   type: String,
            //   required: true
            // },
            messages: {
              type: Array,
            },
            loadMessage:{
                type:Boolean,
            }
        },
        computed: {
            studentName() {
                return this.messages[0] ? this.messages[0].name : ''
            }
        },
    }
</script>

<style lang="scss">
tr{
    &.student {
        background-color: lightblue;
        }
    &.tutor {
        background-color: lightgray;
        }
}
</style>