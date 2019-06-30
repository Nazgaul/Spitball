<template>
  <div>
    <div class="container">
      <v-layout justify-center>
        <v-flex xs12 style="background: #ffffff; min-width: 960px;">
          <v-toolbar color="indigo" class="heading-toolbar" dark>
            <v-toolbar-title>Conversations</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-select
                v-model="filterWaitingFor"
                :items="filters.waitingFor"
                class="mr-2 top-card-select"
                height="40px"
                hide-details
                box
                dense
                outline
                label="Waiting for"
                @change="handleFilter('waiting', filterWaitingFor)"
              ></v-select>
              <v-select
                v-model="filterStatus"
                :items="filters.status"
                class="mr-2 top-card-select"
                height="40px"
                hide-details
                dense
                box
                round
                outline
                label="Status"
                @change="handleFilter('status', filterStatus)"
              ></v-select>
              <v-select
                v-model="filterAssignTo"
                :items="filters.assignTo"
                class="top-card-select"
                height="40px"
                hide-details
                dense
                box
                round
                outline
                label="Assigned to"
                @change="handleFilter('assignTo', filterAssignTo)"
              ></v-select>
          </v-toolbar>

          <v-card class="blue lighten-4" style="max-width:100%;">
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
                          <v-flex xs2 class="pl-3" d-flex justify-space-between align-center>
                            <span>Tutor</span><v-btn @click.native.stop="openStartConversationDialog(conversation.userId)" outline round small class="card-converstaion-header-btn">New Tutor</v-btn>
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
                                  <router-link :to="{name: 'userQuestions', params: {userId: conversation.userId}}" target="_blank" class="body-1  font-weight-bold" color="81C784">{{conversation.userName}}</router-link>
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
                                  <router-link :to="{name: 'userQuestions', params: {userId: conversation.userId}}"  target="_blank"  class="body-1  font-weight-bold ">{{conversation.tutorName}}</router-link>
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
                                  <p @click.stop="openSpitballTutorPage(conversation.requestFor)" class="subheading pl-2 pt-1 font-weight-bold">{{conversation.requestFor}}</p>
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
                              <v-layout column justify-center align-center>
                                  <span class="text-xs-center py-1 mb-4 elevation-2 white--text" :class="[conversation.studyRoomExists ? 'studyRoomExists' : 'studyRoomNotExists']">{{conversation.studyRoomExists ? 'Yes' : 'No'}}</span>
                                  <span class="text-xs-center py-1 elevation-2 white--text" :class="statusColor(conversation.autoStatus)">{{conversation.autoStatus}}</span>
                              </v-layout>
                          </v-flex>
                          <v-divider vertical></v-divider>
                          <v-flex xs2 class="card-converstaion-content-col-6 pl-3">
                              <v-select
                                v-model="conversation.status"
                                :items="filters.status"
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
                                v-model="conversation.assignTo"
                                :items="filters.assignTo"
                                @click.native.stop
                                class="card-converstaion-select"
                                hide-details
                                dense
                                box
                                round
                                outline
                                label="Assign to"
                                @change="handleAssingTo($event, conversation.id)"
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
      <div v-if="showLoading" style="text-align: center">Loading conversations, please wait...</div>
      <div v-show="conversationsList.length === 0 && !showLoading">No conversations</div>
    </div>
    <v-dialog
      v-model="dialog.startConversation"
      width="500"
      v-if="dialog.startConversation"
    >
    <startConversation :isDialog="true" :userId="currentStudentId" :closeDialog="closeDialog"></startConversation>
    </v-dialog>
  </div>
</template>

<script>
import conversationMessages from "../conversationMessages/conversationMessages.vue";
import startConversation from "../../startConversation.vue";
import {
  getDetails,
  getMessages,
  setConversationsStatus,
  getConversationsListPage,
  getFiltersParams,
  getFilters,
  setAssignTo
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
      dialog: {
        startConversation: false,
      },
      filters: {},
      filterAssignTo: 'None',
      filterWaitingFor: '',
      filterStatus: 'Default',
      currentStudentId: ''
    };
  },
  components: {
    conversationMessages,
    startConversation
  },
  props:{
    userId:{}
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
    getConversations(id) {            
      getConversationsListPage(id, this.page).then(list => {
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
    },
    handleAssingTo(assignTo, id) {  
      setAssignTo(id, assignTo)
    },
    handleFilter(params, payload) {
      let assign = this.filterAssignTo === 'None' ? '' : `assignTo=${this.filterAssignTo}&`;
      let status = this.filterStatus === 'Unassigned' ? '' :  `status=${this.filterStatus}&`;
      let autoStatus = this.filterWaitingFor === 'Unassigned' ? '' :  `autoStatus=${this.filterWaitingFor}&`;
      let query = `${assign}${status}${autoStatus}`
      getFilters(this.userId, query).then(res => {        
        this.conversationsList = res;
      })
    },
    openSpitballTutorPage(subject) {
      window.open(`https://www.spitball.co/tutor?term=${subject}`, '_blank');
    },
    // openCrm(id) {
    //   this.$router.push({name: 'userQuestions', params: {userId: id}});
    // },
    openStartConversationDialog(id) {
      this.currentStudentId = id
      this.dialog.startConversation = true
    },
    statusColor(status) {
      if(status === 'Tutor') return 'tutor-status'
      if(status === 'Student') return 'student-status'
      return 'status'
    },
    closeDialog() {
      this.dialog = false
    }
  },
  created() {
    window.addEventListener("scroll", this.handleScroll);
    getFiltersParams().then(conversationFilters=>{
      this.filters = conversationFilters;
    });
    this.getConversations(this.userId);
  },
  destroyed() {
    window.removeEventListener("scroll", this.handleScroll);
  }
};
</script>


<style lang="scss">
.heading-toolbar {
    height: 74px;
    padding-top: 5px;
  .top-card-select{
    max-width: 130px;
  }
}
.card-conversation {
  border-radius: 8px;
  .v-expansion-panel__header{
    padding: 12px;
    border-radius: 8px;
    box-shadow: 0 2px 4px -1px rgba(0,0,0,.2),0 4px 5px 0 rgba(0,0,0,.14),0 1px 10px 0 rgba(0,0,0,.12)
  }
  .card-conversation-wrap {

    .card-converstaion-header {
      background-color:#d0deff;
      border-radius: 8px 8px 0 0;
      font-weight: 600;
      align-items: center;
      .card-converstaion-header-btn {
        text-transform: none;
        color: #4452fc;
        flex: 0 0 auto !important;
      }
    }
    .card-converstaion-content-col-4 {
      flex-direction: column;
    }
    .card-converstaion-content-col-5 {
      span {
        border-radius: 20px;
        align-self: center;
        width: 38%;
      }
      .studyRoomExists {
        background: green;
      }
      .studyRoomNotExists {
        background: #ff0000;
      }
      .tutor-status {
        background-color: #ee9e35;
      }
      .student-status {
        background-color: #5bbdb7
      }
      .status {
        background-color: #0000ff
      }
    }
    .card-converstaion-content-col-6 {
      .card-converstaion-select {
        .v-text-field--box .v-input__slot, .v-text-field--outline .v-input__slot{
          min-height: auto !important;
        }
        .v-input__slot {
          background-color: rgba(68, 82, 252, 0.06) !important;
          border: 1px solid rgba(68, 82, 252, 0.56);
        }
        .v-input__slot:hover {
          border: 1px solid rgba(68, 82, 252, 0.56) !important;
        }  
      }
    }
  }
  .body-1,.subheading {
    color:#4452fc
  }
}
</style>