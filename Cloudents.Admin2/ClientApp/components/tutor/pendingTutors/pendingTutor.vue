<template>
  <div class="panding-tutor-container">
    <div class="container">
      <v-layout justify-center>
        <v-flex xs12 style="background: #ffffff; max-width: 80%; min-width: 960px;">
          <v-toolbar color="indigo" class="heading-toolbar" dark>
            <v-toolbar-title>Pending Tutors</v-toolbar-title>
          </v-toolbar>
          <v-card v-for="(tutor, index) in tutors" :key="index" style="padding: 0 12px;">
            <v-toolbar class="tutor-toolbar mt-4 back-color-purple">
              <v-toolbar-title
                class="tutor-text-title cursor-default"
              >Name: {{tutor.firstName}} {{tutor.lastName}}</v-toolbar-title>
              <v-spacer></v-spacer>
              <div class="user-email" @click="doCopy(tutor.email, 'User Email')">
                <span>Email: {{tutor.email}}</span>
              </div>
              <div class="user-id ml-2" @click="doCopy(tutor.bio, 'User ID')">
                <span>Bio: {{tutor.bio}}</span>
              </div>
              <div class="user-id ml-2" @click="doCopy(tutor.bio, 'User ID')">
                <span>Price: {{tutor.price}}</span>
              </div>
              <br>
              <div class="user-id ml-2" @click="doCopy(tutor.bio, 'User ID')">
                <span>Courses: {{tutor.courses}}</span>
              </div>
              <div class="tutor-actions-container">
                <v-tooltip left>
                  <v-btn slot="activator" icon @click="decline(tutor, index)">
                    <v-icon color="red">close</v-icon>
                  </v-btn>
                  <span>Delete</span>
                </v-tooltip>
                <v-tooltip left>
                  <v-btn slot="activator" icon @click="aprove(tutor, index)">
                    <v-icon color="green">done</v-icon>
                  </v-btn>
                  <span>Accept</span>
                </v-tooltip>
              </div>
            </v-toolbar>
          </v-card>
        </v-flex>
      </v-layout>
      <div v-if="loading">Loading tutors, please wait...</div>
      <div v-show="tutors.length === 0 && !loading">No more pending tutors</div>
    </div>
  </div>
</template>

<script>
import { getAllTutors, aproveTutor, deleteTutor } from "./pendingTutorsService";

export default {
  data() {
    return {
      tutors: [],
      loading: true
    };
  },
  methods: {
    doCopy(id, type) {
      let dataType = type || "";
      let self = this;
      this.$copyText(id).then(
        () => {
          self.$toaster.success(`${dataType} Copied`);
        },
        () => {}
      );
    },
    aprove(tutor, index) {
      aproveTutor(tutor.id).then(
        () => {
          this.tutors.splice(index, 1);
          this.$toaster.success(`Tutor Aproved`);
        },
        () => {
          this.$toaster.error(`Tutor Aproved Failed`);
        }
      );
    },
    decline(tutor, index) {
      let id = tutor.id;
      deleteTutor(id).then(
        () => {
          this.tutors.splice(index, 1);
          this.$toaster.success(`Tutor Declined`);
        },
        () => {
          this.$toaster.error(`Tutor Declined Failed`);
        }
      );
    }
  },
  created() {
    getAllTutors().then(questionsResp => {
      this.tutors = questionsResp;
      this.loading = false;
    });
  }
};
</script>

<style lang="scss" scoped>
.panding-tutor-container {
  margin: 0 auto;
  .user-id,
  .user-email {
    cursor: pointer;
  }

  .cursor-default {
    cursor: default !important;
  }

  .v-list__tile__content {
    &.answers-content {
      .v-list__tile__sub-title {
        &.answer-subtitle {
          color: rgba(0, 0, 0, 0.87);
          white-space: pre-line;
          padding: 8px;
        }
      }
    }
  }

  .tutor-actions-container {
    visibility: hidden;
  }
  .v-card {
    padding: 0 !important;
    .tutor-toolbar {
      max-width: 1280px;
      padding: 15px 0;
      cursor: default;
      .v-toolbar__content {
        height: unset !important;
        text-align: left;
        &:hover {
          .tutor-actions-container {
            visibility: visible;
          }
        }
      }
    }
  }

  .tutor-text-title {
    white-space: pre-line;
    cursor: default;
  }
}
</style>