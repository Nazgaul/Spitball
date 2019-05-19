<template>
    <div>
 

        <v-data-table :items="conversationsList"
                      class="elevation-1"
                      hide-actions
                      :headers="headers"
                      :expand="expand">
            <template slot="items" slot-scope="props">
                <tr @click="openItem(props.item)" :style="{ cursor: 'pointer'}">
                    <td class="text-xs-left" style="background-color: lightgray">{{ props.item.tutorName }}</td>
                    <td class="text-xs-left" style="background-color: lightblue">{{ props.item.userName }}</td>
                    <td class="text-xs-left">{{ props.item.lastMessage.toLocaleString() }}</td>
                </tr>
            </template>
           
        </v-data-table>
        </div>
</template>

<script>

    import { getConversationsList, getDetails, getMessages } from './conversationDetalisService'

    export default {
        data() {
            return {
                headers: [
                    { text: 'Tutor' },
                    { text: 'Student' },
                    { text: 'Last Message' }
                ],
                showLoading: true,
                showNoResult: false,
                conversationsList: [],
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

    .student {
        background-color: lightgray;
    }

 
    
</style>