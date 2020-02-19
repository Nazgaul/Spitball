<template>
  <div :class="{'dialogLayout': isDialog}">
    <v-toolbar color="purple" class="mb-3">
      <v-toolbar-title class="white--text">Send a message</v-toolbar-title>
    </v-toolbar>

    <v-text-field :rules="[rules.integer]" v-model="studentId" label="Student Id" :class="{'px-4': isDialog}"></v-text-field>
    <v-text-field :rules="[rules.integer]" v-model="tutorId" label="Tutor Id" :class="{'px-4': isDialog}">></v-text-field>
    <v-text-field v-model="textMessage" label="text" :class="{'px-4': isDialog}">></v-text-field>
    <v-btn :disabled="disable" @click="submit()">Send</v-btn>

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
    showSnack: {
      type: Function
    }
  },
  data() {
    return {
      tutorId: '',
      defaultStudentId: '',
      disable: false,
      snackbar: false,
      text: "",
      textMessage:"",
      rules: {
        integer: (value) =>{
        return Number.isInteger(+value) || 'Numbers Only'
    },
      }
      
    };
  },
  methods: {
    submit() {
      this.disable = true;
      let snack;
      connectivityModule.http
        .post("AdminConversation/start", {
          userId: this.studentId,
          tutorId: this.tutorId,
          message: this.textMessage
        }).then(() => {
            snack = { color: "green", text: "SUCCESS: sending message", snackbar: true }
          },
          () => {
            snack = { color: "red", text: "ERROR: sending message, try again later", snackbar: true }
          })
        .finally(() => {
          this.showSnack(snack, 'startConversation')
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

<style lang="less">
  .dialogLayout {
    background-color: #fff;
  }
</style>

