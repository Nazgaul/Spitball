<template>
  <div>
    <div class="">
      <v-layout justify-center>
        <v-flex xs12 style="background: #ffffff;">
          <v-toolbar color="indigo" class="heading-toolbar elevation-0" dark>
            <v-toolbar-title>Conversations</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-btn flat @click="clearFilters">Clear</v-btn>
            <v-combobox
                v-model="filterWaitingFor"
                :items="filters.waitingFor"
                class="mr-2 top-card-select"
                hide-details
                menu-props="lazy"
                box
                outline
                small
                label="Waiting for"
                validate-on-blur
                @change="handleFilter"
              ></v-combobox>
              <v-combobox
                v-model="filterStatusName"
                class="mr-2 top-card-select"
                hide-details
                box
                readonly
                menu-props="lazy"
                round
                outline
                label="Status"
                @click.native.stop="openStatusDialog(false)"
              ></v-combobox>
              <v-combobox
                v-model="filterAssignTo"
                :items="filters.assignTo"
                class="top-card-select"
                hide-details
                box
                menu-props="lazy"
                round
                outline
                label="Assigned to"
                @change="handleFilter"
              ></v-combobox>
          </v-toolbar>

          <v-card style="max-width:100%;" class="elevation-0">
            <div class="pa-1">
              <v-layout row wrap>

                <v-expansion-panel class="elevation-0">
                  <v-expansion-panel-content
                    hide-actions
                    xs12
                    v-for="(conversation, index) in conversationsList"
                    :key="index"
                    class="mb-3 card-conversation">
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
                                  <span class="text-xs-center py-1 mb-4 elevation-0 white--text" :class="[conversation.studyRoomExists ? 'studyRoomExists' : 'studyRoomNotExists']">{{conversation.studyRoomExists ? 'Yes' : 'No'}}</span>
                                  <span class="text-xs-center py-1 elevation-0 white--text" :class="statusColor(conversation.autoStatus)">{{conversation.autoStatus}}</span>
                              </v-layout>
                          </v-flex>
                          <v-divider vertical></v-divider>
                          <v-flex xs2 class="card-converstaion-content-col-6 pl-3">
                              <v-btn 
                                color="#b4d6f3"
                                block
                                depressed
                                class="mb-2 px-4 "
                                @click.native.stop="openStatusDialog(true, conversation.id, index)"
                              >
                                  {{conversation.status.name || 'New'}}
                              </v-btn>
                              <v-btn 
                                color="#b4d6f3" 
                                block
                                depressed
                                class="px-4" 
                                @click.native.stop="openAssignDialog(conversation.assignTo, conversation.id, index)"
                              >
                                  {{conversation.assignTo || 'Assign'}}
                              </v-btn>
                          </v-flex>
                      </v-layout>
                    </div>
                    <conversationMessages :loadMessage="loadMessage" :id="conversation.id" :messages="conversationsMsg"/>
                  </v-expansion-panel-content>
                </v-expansion-panel>                
              </v-layout>
            </div>
          </v-card>
        </v-flex>
      </v-layout>
      <div v-if="showLoading" style="text-align: center">Loading conversations, please wait...</div>
      <div v-show="conversationsList.length === 0 && !showLoading">No conversations</div>
    </div>

    <v-dialog v-model="dialog.startConversation" width="500" v-if="dialog.startConversation">
        <startConversation :isDialog="true" :userId="currentStudentId" :showSnack="showSnack"></startConversation>
    </v-dialog>

    <v-dialog v-model="dialog.status" width="500" v-if="dialog.status">
        <statusDialogs 
          :isSet="isSet"
          :changeStatus="changeStatus"
          :statusFilters="filters.status" 
          :setStatusFilter="setStatusFilter" 
          :handleFilter="handleFilter" 
          :currentStatus="currentStatus" />
    </v-dialog>

    <v-dialog v-model="dialog.assign" width="500" v-if="dialog.assign">
        <assignDialogs
          :assignFilters="filters.assignTo" 
          :setAssigned="setAssigned"
          :currentAssigned="currentAssigned"
        />
    </v-dialog>

    <v-snackbar v-model="snackBar.snackbar" :color="snackBar.color" :timeout="5000" top>
      {{ snackBar.text }}
      <v-btn dark flat @click="snackBar.snackbar = false">Close</v-btn>
    </v-snackbar>

  </div>
</template>

<script>
import conversationMessages from "../conversationMessages/conversationMessages.vue";
import startConversation from "../../startConversation.vue";
import statusDialogs from "../conversationDialogs/statusDialogs.vue";
import assignDialogs from "../conversationDialogs/assignDialog.vue";

import {
  getDetails,
  getMessages,
  setConversationsStatus,
  getConversationsListPage,
  getFiltersParams,
  getFilters,
  setAssignTo,
  createGroupStatus
} from "./conversationDetalisService";

export default {
  props:{
    userId:{}
  },
  components: {
    conversationMessages,
    startConversation,
    statusDialogs,
    assignDialogs
  },
  data() {
    return {
      page: 0,
      itemIndex: null,
      currentSelectedId: null,
      showLoading: true,
      showNoResult: false,
      loadMessage: false,
      isCompleted: false,
      isLoading: false,
      isSet: false,
      filterAssignTo: '',
      filterWaitingFor: '',
      filterStatusName: '',
      filterStatusId: '',
      currentStudentId: '',
      conversationsList: [],
      conversationsMsg: [],
      currentStatus: {},
      currentAssigned: '',
      filters: {},
      dialog: {
        startConversation: false,
        status: false,
        assign: false
      },
      snackBar: {
        snackbar: false,
        text: '',
        color: ''
      },
    };
  },
  computed: {
    isFiltersEmpty() {
      if(!this.filterAssignTo && !this.filterStatusName && !this.filterWaitingFor) {
        return true;
      }
      return false;
    }
  },
  methods: {
    changeStatus(selectedStatus) {
      this.dialog.status = false;
      let statusId = {"status": selectedStatus.id};
      setConversationsStatus(this.currentSelectedId, statusId).then(() => {
        let newSelectedStatus = createGroupStatus(selectedStatus);
        this.conversationsList[this.itemIndex].status = newSelectedStatus;
        this.showSnack({
          snackbar: true,
          text: 'SUCCESS: change status',
          color: 'green'
        }, 'status');
      },
      err => {
        this.showSnack({
          snackbar: true,
          text: 'ERROR: change status',
          color: 'red'
        }, 'status');
      })
    },
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
      let assign = !this.filterAssignTo ? '' : `assignTo=${this.filterAssignTo}&`;
      let status = !this.filterStatusName ? '' :  `status=${this.filterStatusId}&`;
      let autoStatus = !this.filterWaitingFor ? '' :  `autoStatus=${this.filterWaitingFor}&`;
      return `${assign}${status}${autoStatus}`;
    },
    handleFilter() {
      if(this.isFiltersEmpty) return;

      let query = this.getFiltersQuery();
      this.requestFilters(query)
    },
    requestFilters(query) {
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
     openStatusDialog(setStatus, conversation_id, itemIndex) {
      this.dialog.status = true;
      this.itemIndex = itemIndex;
      this.isSet = setStatus;
      this.currentSelectedId = conversation_id;
    },
    setStatusFilter(statusObj) {
      this.filterStatusName = statusObj.name;
      this.filterStatusId = statusObj.id;
      this.currentStatus = statusObj;
      this.dialog.status = false;
      this.currentStudentId = '';
      this.handleFilter();
    },
    clearFilters() {
      if(this.isFiltersEmpty) return;
      
      this.currentStatus = {};
      this.filterAssignTo = '';
      this.filterStatusName = '';
      this.filterWaitingFor = '';
      let query = this.getFiltersQuery();
      this.requestFilters(query);
    },
    openAssignDialog(assignedTo, conversation_id, itemIndex) {
      this.dialog.assign = true;
      this.itemIndex = itemIndex;
      this.currentAssigned = assignedTo;
      this.currentSelectedId = conversation_id;
    },
    setAssigned(assignedTo) {
      setAssignTo(this.currentSelectedId, assignedTo).then(() => {
        let currentItem = this.conversationsList[this.itemIndex].assignTo
        if(currentItem !== assignedTo) {
          this.conversationsList[this.itemIndex].assignTo = assignedTo;
        }
        this.currentAssigned = assignedTo;
        this.showSnack({
          snackbar: true,
          text: 'SUCCESS: change status',
          color: 'green'
        }, 'assign');
      },
      () => {
        this.showSnack({
          snackbar: true,
          text: 'ERROR: change status',
          color: 'red'
        }, 'assign')
      }).finally(() => {
          this.dialog.assign = false;
      })
    },
    showSnack(snackObj, dialogName) {
      this.snackBar.snackbar = snackObj.snackbar;
      this.snackBar.text = snackObj.text;
      this.snackBar.color = snackObj.color;
      this.dialog[dialogName] = false;
      this.currentStudentId = '';
    }
  },
  created() {
    if(this.$route.name === 'userConversations' && !this.userId) return this.$router.push('/');

    window.addEventListener("scroll", this.handleScroll);
    getFiltersParams().then(conversationFilters => {
      this.filters = conversationFilters;
    });

    this.getConversations();
  },
  destroyed() {
    window.removeEventListener("scroll", this.handleScroll);
  }
};
</script>

<style lang="less">
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
    margin-bottom: 10px;
  .top-card-select{
    max-width: 254px;
  }
}
.card-conversation {
  // border-radius: 8px;
  .v-expansion-panel__header{
    padding: 0;
    // border-radius: 8px;
    border: 1px solid #ccc;
    // box-shadow: 0 2px 4px -1px rgba(0,0,0,.2),0 4px 5px 0 rgba(0,0,0,.14),0 1px 10px 0 rgba(0,0,0,.12)
  }
  .card-conversation-wrap {

    .card-converstaion-header {
      background-color:#d0deff;
      // border-radius: 8px 8px 0 0;
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