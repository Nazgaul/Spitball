<template>
  <div>
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
                    v-for="(conversation, index) in conversationsList"
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
                        </v-layout>
                      </v-card-text>

                             <select :class="conversation.status" @change="changeStatus($event,conversation.id)">
                               <option value="default" class="grey"></option>
                               <option value="NoMatch" class="red">No Match</option>
                               <option value="Scheduled" class="green">Scheduled</option>
                               <option value="Active" class="orange">Active</option>
                             </select> 
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
      <div v-show="conversationsList.length === 0 && !showLoading">No conversations</div>
    </div>
  </div>
</template>

<script>
import conversationMessages from "../conversationMessages/conversationMessages.vue";
import {
  getConversationsList,
  getDetails,
  getMessages,
  setConversationsStatus
} from "./conversationDetalisService";

export default {
  data() {
    return {
      selectClass: 'grey',
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
      currentSelectedId: null
    };
  },
  components: {
    conversationMessages
  },
  methods: {
    changeStatus(ev,id){
      let selected = {
        "status": ev.target.value
      }
        setConversationsStatus(id,selected).then(()=>{
          if(selected === "NoMatch"){this.selectClass = 'red'}
          if(selected === "Scheduled"){this.selectClass = 'green'}
          if(selected === "Active"){this.selectClass = 'orange'}
          if(selected === "default"){this.selectClass = 'grey'}
        })
    }
  },
  created() {
    getConversationsList().then(
      list => {
        if (list.length === 0) {
          this.showNoResult = true;
        } else {
          this.conversationsList = list;
        }
        this.showLoading = false;
      },
      err => {
        console.log(err);
      }
    );
  },
  methods: {
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
</style>