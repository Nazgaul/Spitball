<template>
    
        
    <div class="container">
      <v-layout justify-center>
        <v-flex xs12 style="background: #ffffff; max-width: 80%; min-width: 960px;">
         
          <v-card>
            <v-container fluid grid-list-lg>
              <v-layout row wrap>
                <v-flex
                  xs12
                  v-for="(user, index) in conversationsDetails"
                  :key="index"
                >
                  <v-card>
                    <v-card-text :class="{'student': studentName == user.userName, 'tutor': studentName != user.userName}" >
                        <v-layout justify-start row 
                        class="pl-2">
                          <v-flex xs3>
                            <b>{{ studentName != user.userName ? 'Tutor':'Student'}}:</b>
                            {{user.userName}}
                          </v-flex>
                          <div>&nbsp;&nbsp;&nbsp;</div>
                          <v-flex xs3>
                            <b>Email:</b>
                            {{user.email}}
                          </v-flex>
                          <v-flex xs3>
                            <b>Phone Number:</b>
                            {{user.phoneNumber}}
                          </v-flex>
                          
                          
                          <v-flex xs3>
                                <v-img v-if="user.image" :src="user.image"
                                        height="40px"
                                        contain></v-img>
                            </v-flex>
                            </v-layout>
                    </v-card-text>
                  </v-card>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card>
        </v-flex>
      </v-layout>
    
        <v-data-table :items="conversationsMessages"
                      class="elevation-1"
                      hide-actions
                      disable-initial-sort
                      :headers="messageHeaders"
                      >
            <template slot="items" slot-scope="props">
                <td :class="{'student': studentName == props.item.name, 'tutor': studentName != props.item.name}" class="text-xs-left">{{ props.item.name }}</td>
                <td :class="{'student': studentName == props.item.name, 'tutor': studentName != props.item.name}" class="text-xs-left">{{ props.item.text }}</td>
                <td :class="{'student': studentName == props.item.name, 'tutor': studentName != props.item.name}" class="text-xs-left">{{ props.item.dateTime.toLocaleString() }}</td>
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
                    { text: 'Text'},
                    { text: 'Date' }
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
                return this.conversationsMessages[0] ?
                        this.conversationsMessages[0].name : ''
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
        width: 80%;
        margin-top: 30px;
        margin-left: 10%;
    }

    .text-s-left {
        font-weight: bold;
        font-size: large;
        text-align:left;
    }
    .text-xs-left {
        background-color: white;
    }
    .student {
        background-color: lightblue;
    }
    .tutor {
        background-color: lightgray;
    }

    .userInfo {
        margin-left:40%;
    }
</style>