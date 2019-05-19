<template>
    <div>
 

        <v-data-table :items="conversationsList"
                      class="elevation-1"
                      hide-actions
                      :headers="headers"
                      :expand="expand">
            <template slot="items" slot-scope="props">
                <tr @click="openItem(props.item)">
                    <td class="text-xs-left">{{ props.item.userName1 }}</td>
                    <td class="text-xs-left">{{ props.item.isTotur1 }}</td>
                    <td class="text-xs-left">{{ props.item.userName2 }}</td>
                    <td class="text-xs-left">{{ props.item.isTotur2 }}</td>
                    <td class="text-xs-left">{{ props.item.lastMessage }}</td>
                </tr>
            </template>
            <!--<template slot="expand" slot-scope="props">
            <v-card flat>
                <v-data-table :items="conversationsDetails"
                              class="elevation-1"
                              hide-actions>
                    <template slot="items" slot-scope="props">
                        <td class="text-s-left">name: {{ props.item.userName }} <br/>
                        email: {{props.item.email}} <br/>
                        phone: {{props.item.phoneNumber}}</td>
                    </template>
                </v-data-table>
                <v-data-table :items="conversationsMessages"
                              class="elevation-1"
                              hide-actions
                              :headers="messageHeaders">
                    <template slot="items" slot-scope="props">
                        <td class="text-xs-left">{{ props.item.name }}:</td>
                        <td class="text-xs-left">{{ props.item.text }}</td>
                    </template>
                </v-data-table>
                </v-card>
    </template>-->
        </v-data-table>
        </div>
</template>

<script>

    import { getConversationsList, getDetails, getMessages } from './conversationDetalisService'

    export default {
        data() {
            return {
                headers: [
                    { text: 'User Name', value: 'userName' },
                    { text: 'Is Totur', value: 'isTotur' },
                    { text: 'User Name', value: 'userName2' },
                    { text: 'Is Totur', value: 'isTotur2' },
                    { text: 'Last Message', value: 'lastMessage' }
                ],
                //messageHeaders: [
                //    { text: 'User Name', value: 'userName' },
                //    { text: 'Text', value: 'text' }
                //],
                showLoading: true,
                showNoResult: false,
                conversationsList: [],
                //conversationsDetails: [],
                //conversationsMessages:[],
                expand: false,
            }
        },
        methods: {
            openItem(item) {
                this.$router.push({ path: `conversationDetail/${item.id}` })
            }
        },
            created() {
                getConversationsList().then((list) => {
                    if (list.length === 0) {
                        this.showNoResult = true;
                    } else {
                        this.conversationsList = list;
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
    
</style>