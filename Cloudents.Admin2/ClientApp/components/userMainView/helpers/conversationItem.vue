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
                  v-for="(conversation, index) in conversations"
                  :key="index"
                  @click="openItem(conversation)"
                  :style="{ cursor: 'pointer'}"
                >
                  <v-card>
                    <v-card-text>
                      <v-layout column>
                        <v-layout justify-start row class="pl-2 text-xs-left">
                          <v-flex xs3>
                            <b>Tutor Id:</b>
                            {{conversation.tutorId}}
                          </v-flex>
                          <v-flex xs3>
                            <b>Tutor Name:</b>
                            {{conversation.tutorName}}
                          </v-flex>
                        </v-layout>
                        <v-layout justify-start row class="pl-2 text-xs-left">
                          <v-flex>
                            <v-layout justify-start row>
                              <v-flex xs3>
                                <b>User Id:</b>
                                {{conversation.userId}}
                              </v-flex>
                              <v-flex xs3>
                                <b>User Name:</b>
                                {{conversation.userName}}
                              </v-flex>
                              <v-flex xs4>
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
      <div v-show="conversations.length === 0 && !loading">No conversations</div>
    </div>
  </div>
</template>

<script>
export default {
  name: "conversationItem",
  data() {
    return {};
  },
  props: {
    conversations: {
      type: Array,
      required: false
    }
  },

  methods: {
    openItem(item) {
      this.$router.push({
        path: `/conversation/conversationDetail/${item.id}`
      });
    }
  }
};
</script>

<style  lang="scss">
.item-wrap {
  .conversation-toolbar {
    max-width: 100%;
    background-color: transparent !important;
    box-shadow: none;
    border-bottom: 1px solid grey;
  }
  .v-card {
    max-width: 100%;
  }
}
.conversation-text-title {
  font-size: 15px;
  text-align: left;
}
</style>