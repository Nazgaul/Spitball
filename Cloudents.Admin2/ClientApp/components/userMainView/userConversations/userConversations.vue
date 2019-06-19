<template>
 <div class="container">
      <v-layout justify-center>
        <v-flex xs12 style="background: #ffffff; max-width: 80%; min-width: 960px;">
          <v-toolbar color="indigo" class="heading-toolbar" dark>
            <v-toolbar-title>Conversations</v-toolbar-title>
          </v-toolbar>

          <v-card class="blue lighten-4">
            <v-container fluid grid-list-lg>
              <v-layout row wrap>
                <v-expansion-panel class="elevation-0">
                  <v-expansion-panel-content
                    xs12
                    v-for="(conversation, index) in userConversations"
                    :key="index"
                    class="mb-3 elevation-1"
                  >
                    <div slot="header">
                      <v-card-text @click="getConversationData(conversation.id)">
                        <v-layout column>
                          <v-layout justify-start row class="pl-2 text-xs-left">
                            <v-flex xs3>
                              <b>Tutor Tel:</b>
                              {{conversation.tutorPhoneNumber}}
                            </v-flex>
                            <v-flex xs3>
                              <b>Tutor Name:</b>
                              {{conversation.tutorName}}
                            </v-flex>
                            <v-flex xs3>
                              <b>Tutor Email:</b>
                              {{conversation.tutorEmail}}
                            </v-flex>
                            <v-flex xs3>
                              <b>Status:</b>
                              {{conversation.status}}
                            </v-flex>
                          </v-layout>
                          <v-layout justify-start row class="pl-2 text-xs-left">
                            <v-flex>
                              <v-layout justify-start row>
                                <v-flex xs3>
                                  <b>Student Tel:</b>
                                  {{conversation.userPhoneNumber}}
                                </v-flex>
                                <v-flex xs3>
                                  <b>Student Name:</b>
                                  {{conversation.userName}}
                                </v-flex>
                                <v-flex xs3>
                                  <b>Student Email:</b>
                                  {{conversation.userEmail}}
                                </v-flex>
                                <v-flex>
                                  <b>Last Message:</b>
                                  {{conversation.lastMessage.toLocaleString()}}
                                </v-flex>
                              </v-layout>
                            </v-flex>
                          </v-layout>
                           <v-layout justify-start row class="pl-2 text-xs-left">
                              <v-flex xs3>
                              <span><b>Hours From Last Message: </b> {{conversation.hoursFromLastMessage}}</span>
                              </v-flex>
                             </v-layout>
                        </v-layout>
                      </v-card-text>
                    </div>
                    <conversationMessages
                      :loadMessage="loadMessage"
                      :id="conversation.id"
                      :messages="conversationsMsg"
                    />
                  </v-expansion-panel-content>
                </v-expansion-panel>
              </v-layout>
            </v-container>
          </v-card>
        </v-flex>
      </v-layout>
      <div v-if="showLoading">Loading conversations, please wait...</div>
      <div v-show="!userConversations && !showLoading">No conversations</div>
    </div>
</template>

<script>
    import conversationItem from '../helpers/conversationItem.vue';
    import conversationMessages from "../../conversation/conversationComponent/conversationMessages/conversationMessages.vue";
    import { mapGetters, mapActions } from 'vuex';
    import { getDetails, getMessages } from '../../conversation/conversationComponent/conversationMessages/conversationMessagesService'

    export default {
        name: "userConversations",
        components: {
          conversationItem,
          conversationMessages
        },
        data() {
            return {
              conversationsMsg: [],
              loadMessage: false,
              currentSelectedId: null,
              showNoResult: false,
              showLoading: true,
            }
        },
        props: {
            userId: {}
        },
       
        computed: {
            ...mapGetters(["userConversations"]),
        },
        methods: {
            ...mapActions(["getUserConversations"]),          
            getUserConversationsData() {

                let id = this.userId;

                this.getUserConversations({id}).then(item => {
                  this.showLoading = false
                });                  
            },
            getConversationData(conversation_id) {
              
              if (this.currentSelectedId !== conversation_id) {
                this.currentSelectedId = conversation_id;
                this.loadMessage = true;


                getMessages(conversation_id)
                  .then(
                    messages => {
                      if (messages.length === 0) {
                        this.showNoResult = true;
                      } else {
                        this.conversationsMsg = messages;
                      }
                      this.showLoading = false;
                    },
                    err => {
                      console.log(err);
                    }
                  )
                  .finally(() => {
                    this.loadMessage = false;
                  });
              }
            }
        },
        created() {
            this.getUserConversationsData();
        },

    }
</script>

<style scoped>

</style>