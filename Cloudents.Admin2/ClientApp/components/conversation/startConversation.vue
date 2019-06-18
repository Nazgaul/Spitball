<template>
  <div>
    <v-toolbar color="purple" class="mb-3">
      <v-toolbar-title class="white--text">Send a message</v-toolbar-title>
    </v-toolbar>

    <v-text-field v-model="studentId" label="Student Id"></v-text-field>
    <v-text-field v-model="tutorId" label="Tutor Id"></v-text-field>
    <v-btn :disabled="disable" @click="submit()">Send</v-btn>

    <v-snackbar v-model="snackbar" :color="color" :timeout="5000" top>
      {{ text }}
      <v-btn dark flat @click="snackbar = false">Close</v-btn>
    </v-snackbar>
  </div>
</template>

<script>
import { connectivityModule } from "../../services/connectivity.module";
export default {
  data() {
    return {
      tutorId: '',
      studentId: '',
      disable: false,
      color: "green",
      snackbar: false,
      text: ""
    };
  },
  methods: {
    submit() {
      this.disable = true;

      connectivityModule.http
        .post("AdminConversation/start", {
          userId: this.studentId,
          tutorId: this.tutorId
        })
        .then(
          () => {
            this.color = "green";
            this.text = "send successfully";
            this.snackbar = true;
            this.tutorId = "";
            this.studentId = "";
          },
          () => {
            this.color = "red";
            this.text = "error sending";
            this.snackbar = true;
          }
        )
        .finally(() => {
          this.disable = false;
        });

      console.log(this.tutorId, this.studentId);
    }
  }
};
</script>

