<template>
    <div class="panding-tutor-container">
        <div class="container">
            <v-layout justify-center>
                <v-flex xs12 style="background: #ffffff; max-width: 80%; min-width: 960px;">
                    <v-toolbar color="indigo" class="heading-toolbar" dark>
                        <v-toolbar-title>Pending Tutors</v-toolbar-title>
                    </v-toolbar>
                    
                    <v-card class="blue lighten-4">
                        <v-container fluid
                                     grid-list-lg>
                            <v-layout row wrap>
                                <v-flex xs12 v-for="(tutor, index) in tutors" :key="index">
                                    <v-card>
                                        <v-card-title primary-title>
                                            <v-layout row>
                                                <v-flex xs7>

                                                    <div class="text-xs-left">
                                                        <div class="headline"><b> Name:</b> {{tutor.firstName}} {{tutor.lastName}}</div>
                                                        <span><b>Email:</b> {{tutor.email}}</span>
                                                        <div><b>Bio:</b> {{tutor.bio}}</div>
                                                    </div>

                                                </v-flex>
                                                <v-flex xs5>
                                                    <v-img v-if="tutor.image" :src="tutor.image"
                                                           height="125px"
                                                           contain></v-img>
                                                </v-flex>
                                            </v-layout>
                                        </v-card-title>
                                        <v-card-text>
                                            <!--<v-container fluid class="px-0">-->
                                            <v-layout justify-start row class="pl-2">
                                                <div><b>Created:</b> {{tutor.created.toLocaleString()}}</div>
                                                <div>&nbsp;&nbsp;</div>
                                                <div><b>Price:</b> {{tutor.price}}</div>
                                            </v-layout>
                                            <!--</v-container>-->
                                            <div class="text-xs-left">
                                                <b>Courses:</b>
                                                <v-container fluid grid-list-sm>
                                                    <v-layout row wrap>
                                                        <v-flex xs4 v-for="(course, index) in tutor.courses" :key="index">
                                                            {{course}}
                                                        </v-flex>
                                                    </v-layout>
                                                </v-container>
                                            </div>

                                        </v-card-text>
                                        <v-card-actions>
                                            <v-btn class="white--text" color="green" @click="aprove(tutor, index)">
                                                Approve
                                                <v-icon color="white">done</v-icon>
                                            </v-btn>
                                            <v-btn class="white--text" color="red" @click="decline(tutor, index)">
                                                Decline
                                                <v-icon color="white">close</v-icon>
                                            </v-btn>
                                        </v-card-actions>
                                    </v-card>

                                </v-flex>
                            </v-layout>
                        </v-container>
                    </v-card>
                    </v-flex>
                </v-layout>
                        <div v-if="loading">Loading tutors, please wait...</div>
            <div v-show="tutors.length === 0 && !loading">No pending tutors</div>
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
                    });
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
                        this.$toaster.success('Tutor Declined');
                    },
                    () => {
                        this.$toaster.error('Tutor Declined Failed');
                    }
                );
            }
        },
        created() {
            getAllTutors().then(questionsResp => {

                this.tutors = questionsResp;
                this.loading = false;
            }, (err) => {
                console.error(err);
                this.$toaster.error('Failed to bring data');
            });
        }
    };
</script>

<!--<style lang="less" scoped>
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
</style>-->
