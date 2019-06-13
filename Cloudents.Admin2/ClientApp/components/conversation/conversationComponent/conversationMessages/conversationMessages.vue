<template>
    
        
    <div class="px-4">
      <!-- <v-layout justify-center>
          <v-card>
              <v-container fluid grid-list-lg>
                <v-layout row wrap>
                  <v-flex xs12>
                    <v-card xs12 v-for="(user, index) in conversationsDetails" :key="index">
                      <v-card-text :class="{'student': studentName == user.userName, 'tutor': studentName != user.userName}" >
                          <v-layout justify-start row class="pl-2">
                            <v-flex xs3><b>{{ studentName != user.userName ? 'Tutor':'Student'}}:</b>{{user.userName}}</v-flex>
                            <div>&nbsp;&nbsp;&nbsp;</div>
                            <v-flex xs3><b>Email:</b>{{user.email}}</v-flex>
                            <v-flex xs3><b>Phone Number:</b>{{user.phoneNumber}}</v-flex>
                            <v-flex xs3><v-img v-if="user.image" :src="user.image" height="40px" contain></v-img></v-flex>
                          </v-layout>
                      </v-card-text>
                    </v-card>
                  </v-flex>
                </v-layout>
              </v-container>
          </v-card>
      </v-layout> -->
    
        <v-data-table :items="conversationsMessages"
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
            }
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