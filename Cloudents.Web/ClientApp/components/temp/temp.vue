<template>
<div>
    <!-- <span>{{status}}</span> -->
    <v-btn color="success" @click="connect">Connect</v-btn>
    <ul>
    </ul>
</div>
</template>

<script>
import chatService from "./../../services/chatService";
export default {
  watch: {
    status: function() {
        debugger;
      return chatService.client != null;
    }
  },
  data: function() {
    return {
      status2: chatService.client != null
    };
  },
  methods: {
    connect: function() {
      chatService.connect();
    }
  }
};
</script>