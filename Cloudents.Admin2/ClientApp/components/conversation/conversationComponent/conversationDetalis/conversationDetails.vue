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
                <v-flex
                  xs12
                  v-for="(conversation, index) in conversationsList"
                  :key="index"
                  @click="openItem(conversation)"
                  :style="{ cursor: 'pointer'}"
                >
                  <v-card>
                    <v-card-text>
                      <v-layout column>
                        <v-layout justify-start row class="pl-2 text-xs-left">
                          <v-flex  xs3   >
                            <b>Tutor Id:</b>
                            {{conversation.tutorId}}
                          </v-flex>
                          <div>&nbsp;&nbsp;&nbsp;</div>
                          <v-flex xs3>
                            <b>Tutor Name:</b>
                            {{conversation.tutorName}}
                          </v-flex>
                        </v-layout>
                          
                     
                    
                            <v-layout  justify-start row class="pl-2 text-xs-left">
                                <v-flex >
                                    <v-layout  justify-start row >
                         <v-flex  xs3 >
                            <b>User Id:</b>
                            {{conversation.userId}}
                         </v-flex>
                          <div>&nbsp;&nbsp;&nbsp;</div>
                          <v-flex  xs3 >
                            <b>User Name:</b>
                            {{conversation.userName}}
                          </v-flex>
                          <v-flex xs3>
                            <b>Last Message:</b>
                            {{conversation.lastMessage.toLocaleString()}}
                          </v-flex>
                          </v-layout>
                                </v-flex>
                            </v-layout>
                      </v-layout>
                    </v-card-text>
                  </v-card>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card>
        </v-flex>
      </v-layout>
      <div v-if="loading">Loading conversations, please wait...</div>
      <div v-show="conversationsList.length === 0 && !loading">No conversations</div>
    </div>
  </div>
  
</template>

<script>
import {
  getConversationsList,
  getDetails,
  getMessages
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
      expand: false
    };
  },
  methods: {
    openItem(item) {
      this.$router.push({ path: `conversationDetail/${item.id}` });
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