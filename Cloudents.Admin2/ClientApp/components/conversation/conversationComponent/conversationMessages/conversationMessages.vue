<template>
    <div>
        
        <v-data-table :items="conversationsDetails"
                      class="elevation-1"
                      hide-actions
                      hide-headers>
            <template slot="items" slot-scope="props">
                <td class="text-s-left">
                    name: {{ props.item.userName }} <br />
                    email: {{props.item.email}} <br />
                    phone: {{props.item.phoneNumber}}
                </td>
            </template>
        </v-data-table>
        <v-data-table :items="conversationsMessages"
                      class="elevation-1"
                      hide-actions
                      disable-initial-sort
                      :headers="messageHeaders">
            <template slot="items" slot-scope="props">
                <td :class="{'student': studentName == props.item.name}" class="text-xs-left">{{ props.item.name }}</td>

                <td :class="{'student': studentName == props.item.name}" class="text-xs-left">{{ props.item.text }}</td>
                <td :class="{'student': studentName == props.item.name}" class="text-xs-left">{{ props.item.dateTime.toLocaleString() }}</td>
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
                    { text: 'User Name', value: 'userName' },
                    { text: 'Text', value: 'text' },
                    { text: 'Date', value: 'Date' }
                ],
                conversationsDetails: [],
                conversationsMessages: []
            }
        },
        props: {
            id: {}
        },
        computed: {
            studentName() {
                return this.conversationsMessages[0] ? this.conversationsMessages[0].name : ''
            }
        },
        created() {
            getDetails(this.id).then((list) => {
                if (list.length === 0) {
                    this.showNoResult = true;
                } else {
                    this.conversationsDetails = list;
                }
                this.showLoading = false;
            }, (err) => {
                console.log(err)
                })

            getMessages(this.id).then((messageslist) => {
                if (messageslist.length === 0) {
                    this.showNoResult = true;
                } else {
                    this.conversationsMessages = messageslist;
                }
                this.showLoading = false;
            }, (err) => {
                console.log(err)
            })
        }
    }
</script>

<style lang="scss">
    .elevation-1 {
        width: 100%
    }

    .text-s-left {
        color: red;
        font-weight: bold;
        font-size: large;
    }
    .text-xs-left {
        background-color: white;
    }
    .student {
        background-color: aqua;
    }
</style>