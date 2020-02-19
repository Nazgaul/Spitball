<template>
 <div class="item-wrap" data-app>
        <v-card class="elevation-5">
                <v-card-text :style="{ cursor: 'pointer'}" @click="openItem(conversation)">
                    <v-flex xs3><b>Tutor Id:</b>{{conversation.tutorId}}</v-flex>
                    <v-flex xs3><b>Tutor Name:</b>{{conversation.tutorName}}</v-flex>
              <br>
                  <v-flex xs3><b>Student Id:</b>{{conversation.userId}}</v-flex>
                  <v-flex xs3><b>Tutor Name:</b>{{conversation.userName}}</v-flex>
                  <v-flex xs3><b>Last Message:</b>{{conversation.lastMessage.toLocaleString()}}</v-flex>
                </v-card-text>
        </v-card>
    </div>
</template>

<script>
export default {
  name: "conversationItem",
  data() {
    return {};
  },
  props: {
    conversation: {
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

<style  lang="less">

</style>