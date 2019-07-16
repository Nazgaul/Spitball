<template>
  <div>
    <div class="pa-2">
      <v-layout justify-center>
        <v-flex xs12 style="background: #ffffff;">
          <v-toolbar color="indigo" class="heading-toolbar" dark>
            <v-toolbar-title>Conversations</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn flat @click="clearFilters">Clear</v-btn>
            <v-select
                v-model="filterWaitingFor"
                :items="filters.waitingFor"
                class="mr-2 top-card-select"
                height="40px"
                hide-details
                menu-props="lazy"
                box
                dense
                outline
                label="Waiting for"
                @change="handleFilter()"
              ></v-select>
              <v-select
                v-model="filterStatusName"
                class="mr-2 top-card-select"
                height="40px"
                hide-details
                dense
                box
                readonly
                menu-props="lazy"
                round
                outline
                label="Status"
                @click.native.stop="dialog.status = !dialog.status"
              ></v-select>
              <v-select
                v-model="filterAssignTo"
                :items="filters.assignTo"
                class="top-card-select"
                height="40px"
                hide-details
                dense
                box
                menu-props="lazy"
                round
                outline
                label="Assigned to"
                @change="handleFilter()"
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
                                  <router-link :to="{name: 'userQuestions', params: {userId: conversation.userId}}" target="_blank" class="body-1 text-truncate font-weight-bold" color="81C784">{{conversation.userName}}</router-link>
                              </v-layout>
                              <v-layout row wrap align-center justify-start>
                                  <span class="grey--text caption pa-2">Email</span>
                                  <span class="text-truncate">{{conversation.userEmail}}</span>
                              </v-layout>
                              <v-layout row wrap align-center justify-start>
                                  <span class="grey--text caption pa-2">Phone</span>
                                  <span class="text-truncate">{{conversation.userPhoneNumber}}</span>
                              </v-layout>
                          </v-flex>
                          <v-divider vertical></v-divider>
                          <v-flex xs2 class="card-converstaion-content-col-2 pl-3">
                              <v-layout row wrap align-center justify-start>
                                  <span class="grey--text caption pa-2">Name</span>
                                  <router-link :to="{name: 'userQuestions', params: {userId: conversation.tutorId}}" target="_blank" class="body-1 text-truncate font-weight-bold ">{{conversation.tutorName}}</router-link>
                              </v-layout>
                              <v-layout row wrap align-center justify-start>
                                  <span class="grey--text caption pa-2">Email</span>
                                  <span class="text-truncate">{{conversation.tutorEmail}}</span>
                              </v-layout>
                              <v-layout row wrap align-center justify-start>
                                  <span class="grey--text caption pa-2">Phone</span>
                                  <span class="text-truncate">{{conversation.tutorPhoneNumber}}</span>
                              </v-layout>
                          </v-flex>
                          <v-divider vertical></v-divider>
                          <v-flex xs2 class="card-converstaion-content-col-3 pl-3">
                              <v-layout row wrap align-center>
                                  <p @click.stop="openSpitballTutorPage(conversation.requestFor)" class="subheading pl-2 popenSpitballTutorPaget-1 font-weight-bold">{{conversation.requestFor}}</p>
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
                              <v-btn flat class="mb-2" @click.native.stop="dialog.status = !dialog.status">Status</v-btn>
                              <v-btn flat @click.native.stop="dialog.assign = !dialog.assign">Assign to</v-btn>
                          </v-flex>
                      </v-layout>
                    </div>
                    <conversationMessages :loadMessage="loadMessage" :id="conversation.id" :messages="conversationsMsg"/>
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

    <v-dialog v-model="dialog.startConversation" width="500" v-if="dialog.startConversation">
        <startConversation :isDialog="true" :userId="currentStudentId" :closeDialog="closeDialog"></startConversation>
    </v-dialog>

    <v-dialog v-model="dialog.status" width="500" v-if="dialog.status">
        <statusDialogs :statusFilters="filters.status" :setStatusFilter="setStatusFilter" :handleFilter="handleFilter" />
    </v-dialog>

  </div>
</template>

<script>
import conversationMessages from "../conversationMessages/conversationMessages.vue";
import startConversation from "../../startConversation.vue";
import statusDialogs from "../conversationDialogs/statusDialogs.vue";
import assignToDialog from "../conversationDialogs/assignToDialog.vue";

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
  props:{
    userId:{}
  },
  components: {
    conversationMessages,
    startConversation,
    statusDialogs,
    assignToDialog
  },
  data() {
    return {
      page: 0,
      currentSelectedId: null,
      showLoading: true,
      showNoResult: false,
      loadMessage: false,
      isCompleted: false,
      isLoading: false,
      filterAssignTo: '',
      filterWaitingFor: '',
      filterStatusName: '',
      filterStatusId: '',
      currentStudentId: '',
      conversationsList: [],
      conversationsMsg: [],
      filters: {},
      dialog: {
        startConversation: false,
        status: false,
      },
    };
  },
  methods: {
    getConversationData(conversation_id) {
      if (this.currentSelectedId !== conversation_id) {
        this.currentSelectedId = conversation_id;
        this.loadMessage = true;
        getMessages(conversation_id).then(messages => {
            if (messages.length === 0) {
              this.showNoResult = true;
            } else {
              this.conversationsMsg = messages;
            }
            this.showLoading = false;
            },
          err => {
            console.log(err);
        })
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
      let filter = this.getFiltersQuery();
      let id = this.userId;
      getConversationsListPage(id, this.page, filter).then(list => {
        if (list.length === 0) {
          this.isCompleted = true;
        }
        this.conversationsList = [...this.conversationsList, ...list];
      },
      err => {
        console.log(err);
      })
      .finally(() => {
        this.isLoading = false;
        this.showLoading = false;
      });
    },
    getFiltersQuery() {
      let assign = this.filterAssignTo === '' ? '' : `assignTo=${this.filterAssignTo}&`;
      let status = this.filterStatusName === '' ? '' :  `status=${this.filterStatusId}&`;
      let autoStatus = this.filterWaitingFor === '' ? '' :  `autoStatus=${this.filterWaitingFor}&`;
      return `${assign}${status}${autoStatus}`;
    },
    handleFilter() {
      let query = this.getFiltersQuery();
      getFilters(this.userId, query).then(res => {
        this.conversationsList = res;
        this.page = 0;
        this.isCompleted = false;
      })
    },
    openSpitballTutorPage(subject) {
      window.open(`https://www.spitball.co/tutor?term=${subject}`, '_blank');
    },
    openStartConversationDialog(id) {
      this.currentStudentId = id;
      this.dialog.startConversation = true;
    },
    statusColor(status) {
      if(status === 'Tutor') return 'tutor-status'
      if(status === 'Student') return 'student-status'
      return 'status'
    },
    closeDialog() {
      this.dialog = false
    },
    setStatusFilter(id, name) {
      this.filterStatusName = name;
      this.filterStatusId = id
      this.dialog.status = false;
      this.handleFilter()
    },
    clearFilters() {
      if(!this.filterAssignTo && !this.filterStatusName && !this.filterWaitingFor) return

      this.filterAssignTo = '';
      this.filterStatusName = '';
      this.filterWaitingFor = '';
      this.handleFilter();
    }
  },
  created() {
    if(this.$route.name === 'userConversations' && !this.userId) return this.$router.push('/')

    window.addEventListener("scroll", this.handleScroll);
    getFiltersParams().then(conversationFilters=>{
      this.filters = conversationFilters;
    });

    this.getConversations();
  },
  destroyed() {
    window.removeEventListener("scroll", this.handleScroll);
  }
};
</script>

<style lang="scss">
.v-menu__content.theme--light.menuable__content__active {
  .v-list__tile.v-list__tile--link.theme--light {
    padding: 0 10px;
    height: 30px;
  }
}

.heading-toolbar {
    position: sticky !important;
    top: 0;
    z-index: 1;
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
      button {
        display: block;
        margin: 0 auto;
      }
    }
  }
  .body-1,.subheading {
    color:#4452fc
  }
}
.dialog-wrap {
  background-color: #fff; 
}
</style>