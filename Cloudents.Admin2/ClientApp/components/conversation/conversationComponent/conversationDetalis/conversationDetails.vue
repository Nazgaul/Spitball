<template>
  <div>
    <div class="container">
      <v-layout justify-center>
        <v-flex xs12 style="background: #ffffff; max-width: 80%; min-width: 960px;">
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
              ></v-select>
              <v-select
                class="mr-2 top-card-select"
                height="40px"
                hide-details
                dense
                box
                round
                outline
                label="Status"
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
              ></v-select>
          </v-toolbar>

          <v-card class="blue lighten-4">
            <v-container fluid grid-list-lg>
              <v-layout row wrap>

                <v-expansion-panel class="elevation-0">
                  <v-expansion-panel-content
                    hide-actions
                    xs12
                    v-for="(conversation, index) in conversationsList"
                    :key="index"
                    class="mb-3 elevation-4 card-conversation">
                    <div slot="header" class="card-conversation-wrap" @click="getConversationData(conversation.id)">
                      <v-layout class="card-converstaion-header">
                          <v-flex xs2 class="pl-3">Student</v-flex>
                          <v-flex xs2 class="pl-3" d-flex justify-space-between align-center><span>Tutor</span><v-btn outline round small class="card-converstaion-header-btn">New Tutor</v-btn></v-flex>
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
                                  <p class="body-1 pl-2 pt-1 font-weight-bold">{{conversation.hoursFromLastMessage}}h</p>
                                  <p class="pl-2">({{conversation.lastMessage.toLocaleString('he-IL')}})</p>
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



                <!-- <v-expansion-panel class="elevation-0">
                  <v-expansion-panel-content
                    expand-icon=" "
                    xs12
                    v-for="(conversation, index) in conversationsList"
                    :key="index"
                    class="mb-3 elevation-1"
                  >
                    <div slot="header">
                      <v-card-text @click="getConversationData(conversation.id)">
                        <v-layout column wrap>

                          <v-layout justify-start row class="pl-2 text-xs-left">
                            <v-flex xs3>
                              <b>Tutor</b>
                              {{conversation.tutorPhoneNumber? conversation.tutorPhoneNumber : '-'}}
                            </v-flex>
                            <v-flex xs3>
                              {{conversation.tutorName? conversation.tutorName : '-'}}
                            </v-flex>
                            <v-flex xs3>
                              {{conversation.tutorEmail? conversation.tutorEmail : '-'}}
                            </v-flex>
                            <v-flex xs3>
                              <span style="max-width: fit-content; border-radius: 35px; padding: 10px;" :class="[`color-${conversation.autoStatus}`]">
                              {{conversation.autoStatus}}
                              </span>
                            <select @click.stop='' style="margin-left: 15px;padding: 5px;" outline @change="changeStatus($event, conversation.id)" :class="[`color-${conversation.status}`]" v-model="conversation.status">
                              <option value="default" class="color-default"></option>
                              <option value="noMatch" class="color-noMatch">No Match</option>
                              <option value="scheduled" class="color-scheduled">Scheduled</option>
                              <option value="active" class="color-active">Active</option>
                            </select> 

                            </v-flex>
                          </v-layout>

                          <v-layout justify-start row class="pl-2 text-xs-left">
                            <v-flex>
                              <v-layout justify-start row>
                                <v-flex xs3>
                                  <b>Student:</b>
                                  {{conversation.userPhoneNumber? conversation.userPhoneNumber: '-'}}
                                </v-flex>
                                <v-flex xs3>
                                  {{conversation.userName? conversation.userName : '-'}}
                                </v-flex>
                                <v-flex xs3>
                                  {{conversation.userEmail? conversation.userEmail : '-'}}
                                </v-flex>
                                <v-flex>
                                  <b>Last:</b>
                                  {{conversation.lastMessage.toLocaleString('he-IL')}}
                                </v-flex>
                              </v-layout>
                            </v-flex>
                          </v-layout>
                          
                          <v-layout row class="pl-3 text-xs-left">
                            <v-flex xs3>
                              <span><b>Study Room: </b> {{conversation.studyRoomExists}}</span>
                            </v-flex>
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
                </v-expansion-panel> -->
                
              </v-layout>
            </v-container>
          </v-card>
        </v-flex>
      </v-layout>
      <div v-if="showLoading" style="text-align: center">Loading conversations, please wait...</div>
      <div v-show="conversationsList.length === 0 && !showLoading">No conversations</div>
    </div>
  </div>
</template>

<script>
import conversationMessages from "../conversationMessages/conversationMessages.vue";
import {
  getDetails,
  getMessages,
  setConversationsStatus,
  getConversationsListPage,
  statusList
} from "./conversationDetalisService";

export default {
  data() {
    return {
      headers: [
        { text: "Tutor Id" },
        { text: "Tutor Name" },
        { text: "Student Id" },
        { text: "Student" },
        { text: "Last Message" }
      ],
      showLoading: true,
      showNoResult: false,
      conversationsList: [],
      expand: false,
      conversationsMsg: [],
      loadMessage: false,
      currentSelectedId: null,
      page: 0,
      isCompleted: false,
      EXPECTED_AMOUNT: 50,
      isLoading: false,
      statusList
    };
  },
  components: {
    conversationMessages
  },
  methods: {
    changeStatus(selectedStatus,id){
      let status={
        "status": selectedStatus
      }
      setConversationsStatus(id, status)
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
    },
    handleScroll() {     
      let bottomOfWindow = document.documentElement.scrollTop + window.innerHeight === document.documentElement.offsetHeight;      
      if (bottomOfWindow) {
          this.page = this.page + 1
          if(!this.isCompleted && !this.isLoading){
            this.isLoading = true;
            this.getConversations()
          }
          
      }
    },
    getConversations() {            
      getConversationsListPage(this.page).then(list => {
        if (list.length < this.EXPECTED_AMOUNT) {
          this.isCompleted = true;
        } 
          this.conversationsList = [...this.conversationsList, ...list];
        },
        err => {
          console.log(err);
        }
      ).finally(()=>{
        this.isLoading = false;
        this.showLoading = false;
      });
    }
  },
  created() {
    window.addEventListener("scroll", this.handleScroll);
    this.getConversations();
  },
  destroyed() {
    window.removeEventListener("scroll", this.handleScroll);
  }
};
</script>

<style lang="scss">
.elevation-1 {
  width: 100%;
}

.student {
  background-color: lightgray;
}
.color-default{
  background: #d7dde2;
}
.color-noMatch{
  background: #d23535;
}
.color-scheduled{
  background: #95ca31;
}
.color-active{
  background: #eab73e;
}
.color-Tutor {
  background: #d23535;
  color: white;
}
.color-Student {
  background: #eab73e;
  color: white;
}
.color-Conversation  {
  background: #95ca31;
  color: white;
}

</style>