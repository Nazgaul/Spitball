<template>
  <div :class="{'dialogLayout': isDialog}">
    <v-toolbar color="purple" class="mb-3">
      <v-toolbar-title class="white--text">Send a message</v-toolbar-title>
    </v-toolbar>

    <v-text-field v-model="studentId" label="Student Id" :class="{'px-4': isDialog}"></v-text-field>
    <v-text-field v-model="tutorId" label="Tutor Id" :class="{'px-4': isDialog}">></v-text-field>
    <v-text-field v-model="textMessage" label="text" :class="{'px-4': isDialog}">></v-text-field>
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
  props: {
    isDialog: {
      type: Boolean
    },
    userId: {},
    closeDialog: {
      type: Function
    }
  },
  data() {
    return {
      tutorId: '',
      defaultStudentId: '',
      disable: false,
      color: "green",
      snackbar: false,
      text: "",
      textMessage:"",
    };
  },
  methods: {
    submit() {
      this.disable = true;

      connectivityModule.http
        .post("AdminConversation/start", {
          userId: this.studentId,
          tutorId: this.tutorId,
          message: this.textMessage
        })
        .then(
          () => {
            this.closeDialog()
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
    },
  },
  computed:{
    studentId:{
      get(){
        return this.defaultStudentId || this.userId;
      },
      set(val){
        this.defaultStudentId = val;
      }
    }
  },
};
</script>

<style lang="scss">
  .dialogLayout {
    background-color: #fff;
  }
</style>

