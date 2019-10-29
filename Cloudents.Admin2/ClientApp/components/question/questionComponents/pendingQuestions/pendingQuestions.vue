<template>
    <div class="panding-question-container">
        <div class="container">
            <v-layout justify-center>
                <v-flex xs12 style="background: #ffffff; max-width: 960px; min-width: 960px;">
                    <v-toolbar color="indigo" class="heading-toolbar" :height="'64px'" dark>
                        <v-toolbar-title>Pending Questions</v-toolbar-title>
                    </v-toolbar>
                    <v-card v-for="(question, index) in questions" :key="index" style="padding: 0 12px;">
                        <v-toolbar class="question-toolbar mt-4 back-color-purple">
                            <v-toolbar-title class="question-text-title cursor-default">
                                {{question.text}}
                            </v-toolbar-title>
                            <v-spacer></v-spacer>
                            <div class="user-email"  @click="doCopy(question.user.email, 'User Email')">
                                <span>{{question.user.email}}</span>
                            </div>
                            <div class="user-id ml-2"  @click="doCopy(question.user.id, 'User ID')">
                                <span>User ID:{{question.user.id}}</span>
                            </div>
                            <div class="question-actions-container">
                                <v-tooltip left>
                                <v-btn slot="activator" icon @click="declineQuestion(question, index)">
                                    <v-icon color="red">close</v-icon>
                                </v-btn>
                                    <span>Delete</span>
                                </v-tooltip>
                                <v-tooltip left>
                                <v-btn slot="activator" icon @click="aproveQ(question, index)">
                                    <v-icon color="green">done</v-icon>
                                </v-btn>
                                    <span>Accept</span>
                                </v-tooltip>
                            </div>
                        </v-toolbar>
                    </v-card>
                </v-flex>
            </v-layout>
            <div v-if="loading">Loading questions, please wait...</div>
            <div v-show="questions.length === 0 && !loading">No more pending questions</div>
        </div>

    </div>
</template>

<script src="./pendingQuestions.js"></script>

<style lang="less" scoped>
    .user-id, .user-email {
        cursor: pointer;
    }
    .cursor-default{
        cursor: default!important;
    }
    .v-list__tile__content {
        &.answers-content {
            .v-list__tile__sub-title {
                &.answer-subtitle {
                    color: rgba(0, 0, 0, .87);
                    //white-space: pre-line;
                    padding: 8px;
                }
            }
        }
    }

    .question-actions-container {
        visibility: hidden;
    }

    .question-toolbar, .v-card {
        max-width: 1280px;
        &:hover {
            .question-actions-container {
                visibility: visible;
            }
        }
    }
    .question-toolbar {
        cursor: default;
        .v-toolbar__content {
            height: auto !important;
            text-align: left;
            padding: 12px 24px;
        }
    }

    .question-text-title {
        white-space: pre-line;
        cursor: default;
    }

    .panding-question-container {
        margin: 0 auto;
    }

</style>