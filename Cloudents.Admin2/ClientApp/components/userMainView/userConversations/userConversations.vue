<template>
 <div class="container">
      <v-layout justify-center>
        <v-flex xs12 style="background: #ffffff; min-width: 960px;">
          <v-toolbar color="indigo" class="heading-toolbar" dark>
            <v-toolbar-title>Conversations</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-select
                class="mr-2 top-card-select"
                height="40px"
                hide-details
                box
                dense
                outline
                label="Waiting for"
                @change="filterWaitingFor()"
              ></v-select>
              <v-select
                :items="statusList"
                class="mr-2 top-card-select"
                height="40px"
                hide-details
                dense
                box
                round
                outline
                label="Status"
                @change="filterStatus()"
              ></v-select>
              <v-select
                class="top-card-select"
                height="40px"
                hide-details
                dense
                box
                round
                outline
                label="Assigned to"
                @change="filterAssgined()"
              ></v-select>
          </v-toolbar>

          <v-card class="blue lighten-4">
            <v-container fluid grid-list-lg>
              <v-layout row wrap>
                <v-expansion-panel class="elevation-0">
                  <v-expansion-panel-content
                    hide-actions
                    xs12
                    v-for="(conversation, index) in userConversations"
                    :key="index"
                    class="mb-3 elevation-4 card-conversation">
                    <div slot="header" class="card-conversation-wrap" @click="getConversationData(conversation.id)">
                      <v-layout class="card-converstaion-header">
                          <v-flex xs2 class="pl-3">Student</v-flex>
                          <v-flex xs2 class="pl-3" d-flex justify-space-between align-center>
                            <span>Tutor</span><v-btn outline round :to="{path: '/conversation/send', query: {studentId: conversation.studentId}}" small class="card-converstaion-header-btn">New Tutor</v-btn>
                          </v-flex>
                          <v-flex xs2 class="pl-3">Request for</v-flex>
                          <v-flex xs2 class="pl-3">Last msg</v-flex>
                          <v-flex xs2 class="pl-3">Study Room</v-flex>
                          <v-flex xs2 class="pl-3">Actions</v-flex>
                      </v-layout>
                      <v-layout class="card-converstaion-content py-3">
                          <v-flex xs2 class="card-converstaion-content-col-1 pl-3">
                              <v-layout row wrap align-center justify-start>
                                  <span class="grey--text caption pa-2">Name</span>
                                  <span class="body-1  font-weight-bold" color="81C784">{{conversation.userName}}</span>
                              </v-layout>
                              <v-layout row wrap align-center justify-start>
                                  <span class="grey--text caption pa-2">Email</span>
                                  <span class="">{{conversation.userEmail}}</span>
                              </v-layout>
                              <v-layout row wrap align-center justify-start>
                                  <span class="grey--text caption pa-2">Phone</span>
                                  <span class="">{{conversation.userPhoneNumber}}</span>
                              </v-layout>
                          </v-flex>
                          <v-divider vertical></v-divider>
                          <v-flex xs2 class="card-converstaion-content-col-2 pl-3">
                              <v-layout row wrap align-center justify-start>
                                  <span class="grey--text caption pa-2">Name</span>
                                  <span class="body-1  font-weight-bold ">{{conversation.tutorName}}</span>
                              </v-layout>
                              <v-layout row wrap align-center justify-start>
                                  <span class="grey--text caption pa-2">Email</span>
                                  <span class="">{{conversation.tutorEmail}}</span>
                              </v-layout>
                              <v-layout row wrap align-center justify-start>
                                  <span class="grey--text caption pa-2">Phone</span>
                                  <span class="">{{conversation.tutorPhoneNumber}}</span>
                              </v-layout>
                          </v-flex>
                          <v-divider vertical></v-divider>
                          <v-flex xs2 class="card-converstaion-content-col-3 pl-3">
                              <v-layout row wrap align-center>
                                  <p class="subheading pl-2 pt-1 font-weight-bold">dsadasd</p>
                              </v-layout>
                          </v-flex>
                          <v-divider vertical></v-divider>
                          <v-flex xs2 class="card-converstaion-content-col-4 pl-3">
                              <v-layout row wrap column>
                                  <p class="body-1 pl-2 pt-1 font-weight-bold">dsadasd</p>
                                  <p class="pl-2">({{conversation.lastMessage.toLocaleString()}})</p>
                              </v-layout>
                          </v-flex>
                          <v-divider vertical></v-divider>
                          <v-flex xs2 class="card-converstaion-content-col-5 pl-3">
                              <v-layout column>
                                  <p class="text-xs-center pt-1">{{conversation.studyRoomExists ? 'Yes' : 'No'}}</p>
                                  <v-btn small round class="white--text">Student</v-btn>
                              </v-layout>
                          </v-flex>
                          <v-divider vertical></v-divider>
                          <v-flex xs2 class="card-converstaion-content-col-6 pl-3">
                              <v-select
                                v-model="conversation.status"
                                :items="statusList"
                                @click.native.stop
                                class="card-converstaion-select pb-2"
                                hide-details
                                box
                                dense
                                outline
                                height="20"
                                color="success"
                                label="Status"
                                @change="changeStatus($event, conversation.id)"
                              ></v-select>
                              <v-select
                                :items="assignTo"
                                @click.native.stop
                                class="card-converstaion-select"
                                hide-details
                                dense
                                box
                                round
                                outline
                                label="Assign to"
                              ></v-select>
                          </v-flex>
                      </v-layout>
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
    import { statusList, assignTo, getFiltersParams, setAssignTo } from "../../conversation/conversationComponent/conversationDetalis/conversationDetalisService";
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
              statusList,
              assignTo
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
                  .then(messages => {
                      
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
            },
            changeNewTutor(user_id) {

            },
            changeStatus(selectedStatus,id){
              let status={
                "status": selectedStatus
              }
              setConversationsStatus(id, status)
            },
            filterWaitingFor() {

            },
            filterStatus() {
              
            },
            filterAssgined() {
              
            },
            handleAssingTo(id) {
              setAssignTo(id).then(assignedTo => {
                console.log(assignedTo);
              })
            }
        },
        created() {
            this.getUserConversationsData();
            getFiltersParams
        },

    }
</script>
