<template>
    
        
    <div class="px-4" v-show="!loadMessage">
    
        <v-data-table :items="messages"
                      hide-actions
                      disable-initial-sort
                      :headers="messageHeaders"
                      class="elevation-2"  style="border:5px solid green">
            <template slot="items" slot-scope="props">
                <td :class="{'student':studentName == props.item.name,'tutor':studentName != props.item.name}" class="text-xs-left">{{ props.item.name }}</td>
                <td :class="{'student':studentName == props.item.name,'tutor':studentName != props.item.name}" class="text-xs-left">{{ props.item.text }}</td>
                <td :class="{'student':studentName == props.item.name,'tutor':studentName != props.item.name}" class="text-xs-left">{{ props.item.dateTime.toLocaleString() }}</td>
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
                conversationsDetails: [],
                conversationsMessages: []
            }
        },
        props: {
            id: {
              type: String,
              required: true
            },
            messages: {
              type: Array,
            },
            loadMessage:{
                type:Boolean,
            }
        },
        computed: {
            studentName() {
                return this.conversationsMessages[0] ? this.conversationsMessages[0].name : ''
            }
        },
    }
</script>

<style lang="scss">
  .text-xs-left {
        background-color: white;
    }
    td.student {
        background-color: lightblue;
    }
    .tutor {
        background-color: lightgray;
    }
</style>