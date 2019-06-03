<template>
  <div class="container">
    <v-layout justify-center>
        <v-flex xs12 style="background: #ffffff; max-width: 960px; min-width: 960px;">
          <v-toolbar color="indigo" class="heading-toolbar" :height="'64px'" dark>
            <v-toolbar-title>Flagged Questions</v-toolbar-title>
          </v-toolbar>
        <v-card v-for="(question, index) in questions" :key="index" style="padding: 0 12px;">
          <v-toolbar class="question-toolbar mt-4 back-color-purple">
            <v-toolbar-title class="question-text-title">
              {{question.text}}
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <div class="user-email"  @click="doCopy(question.flaggedUserEmail, 'Flagged User Email')">
              <span>{{question.flaggedUserEmail}}</span>
            </div>
            <div class="question-id ml-2"  @click="doCopy(question.id, 'Question ID')">
              <span>Question ID:{{question.id}}</span>
            </div>
            <div class="question-actions-container">
            </div>
          </v-toolbar>

          <v-list two-line avatar>
            <template>
              <v-list-tile class="answers-list-tile">
                <v-list-tile-content class="answers-content">
                  <v-list-tile-sub-title class="answer-subtitle">{{question.reason}}
                  </v-list-tile-sub-title>
                </v-list-tile-content>
                <v-list-tile-action class="answer-action">
                  <v-list-tile-action-text></v-list-tile-action-text>
                  <v-tooltip left>
                  <v-btn slot="activator" icon @click="declineQuestion(question, index)">
                    <v-icon color="red">close</v-icon>
                  </v-btn>
                    <span>Delete</span>
                  </v-tooltip>
                </v-list-tile-action>
                <v-list-tile-action class="answer-action">
                  <v-list-tile-action-text></v-list-tile-action-text>
                  <v-tooltip left>
                  <v-btn slot="activator" icon  @click="unflagQ(question, index)">
                    <v-icon color="green">done</v-icon>
                  </v-btn>
                    <span>Accept</span>
                  </v-tooltip>
                </v-list-tile-action>
              </v-list-tile>
            </template>
          </v-list>
        </v-card>
          </v-flex>
    </v-layout>
        <div v-if="loading" align="center">Loading questions, please wait...</div>
        <div v-show="questions.length === 0 && !loading" align="center">No more flagged questions</div>
  </div>
</template>

<script src="./flaggedQuestions.js"></script>

<style lang="scss" scoped>
  .user-email, .question-id{
    cursor: pointer;
  }
  .v-toolbar__title {
    &.question-text-title {
      font-size: 14px;
      white-space: normal;
      text-align: left;
      max-width: 85%;
      &:hover {
        cursor: default;
      }
    }
  }

  .question-text-title {
    white-space: pre-line;
  }

  .question-toolbar {
    .v-toolbar__content {
      height: auto !important;
      text-align: left;
      padding: 12px 24px;
    }
  }


.flagged-question-container {
    margin:0 auto;
  .page-container {
    display: flex;
    .deleted-emails {
      position: absolute;
      color: #c25050;
      .bold {
        font-weight: 600;
      }
    }
    .question-co {
      flex-grow: 1;
      .questionItem {
        display: flex;
        margin: 0 auto;
        flex-direction: row;
        border: 2px solid #c7c7c7;
        margin-bottom: 10px;
        width: 70%;
        min-width: 500px;
        border-radius: 29px;

        .question-left-body {
          flex-grow: 10;
          display: flex;
          flex-direction: column;
          justify-content: center;
          padding: 10px;
          padding-left: 15px;
          background-color: #c7c7c7;
          border-radius: 20px;
          margin: 10px;
          .user-container {
            display: flex;
            flex-grow: 1;
            flex-direction: row;
            justify-content: space-between;
            width: 98%;
            border-bottom: 1px solid #e0e0e0;
          }
          .question-container {
            display: flex;
            flex-grow: 5;
            flex-direction: column;
            width: 100%;
            text-align: left;
            .bottom-border {
              border-bottom: 1px solid #c7c7c7;
              width: fit-content;
            }
            .bold {
              font-weight: 600;
            }
            .bottom-space {
              margin-bottom: 5px;
            }
          }
        }
        .question-right-body {
          position: relative;
          display: flex;
          flex-direction: column;
          flex-grow: 1;
          margin: 10px;
          justify-content: left;
          text-align: left;
          background-color: #c7c7c7;
          border-radius: 25px;
          padding: 7px;
          button {
            cursor: pointer;
            background-color: #affb93;
            border-radius: 25px;
            border: none;
            outline: none;
            cursor: pointer;
            height: 40px;
            margin-bottom: 5px;
            margin-top: 5px;
            &.decline {
              background-color: #fb9393;
            }
          }
        }
      }
    }
  }
}
</style>