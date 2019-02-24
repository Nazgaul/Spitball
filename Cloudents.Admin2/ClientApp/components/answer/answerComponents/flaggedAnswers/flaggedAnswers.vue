<template>
    <div id="answer-wrapper-scroll">
        <div class="container">
            <v-layout justify-center>
                <v-flex xs12 style="background: #ffffff; max-width: 960px; min-width: 960px;">
                    <v-toolbar color="indigo" class="heading-toolbar" :height="'64px'" dark>
                        <v-toolbar-title>Flagged Answers</v-toolbar-title>
                    </v-toolbar>
                    <v-card v-for="(answer, index) in answers" :key="index" style="padding: 0 12px;">
                        <v-toolbar class="question-toolbar mt-4 back-color-purple">
                            <v-toolbar-title class="question-text-title">
                                Question: {{answer.questionText}}
                            </v-toolbar-title>
                            <v-spacer></v-spacer>
                            <span title="Fictive Or Original Question ">{{answer.markerEmail}}</span>
                            <v-spacer></v-spacer>
                            <!--<div class="answer-id" @click="doCopy(answer.id)">-->
                                <!--<span>Answer Id: {{answer.id}}</span>-->
                            <!--</div>-->
                        </v-toolbar>

                        <v-list two-line avatar>
                            <template>
                                <v-list-tile class="answers-list-tile">
                                    <v-list-tile-content class="answers-content">
                                        <v-list-tile-sub-title  class="answer-subtitle-reason">
                                            Reason: {{answer.reason}}
                                            <v-spacer></v-spacer>
                                            Flagged User Email: {{answer.flaggedUserEmail}}
                                        </v-list-tile-sub-title>
                                        <v-list-tile-sub-title class="answer-subtitle">Answer: {{answer.text}}
                                        </v-list-tile-sub-title>
                                    </v-list-tile-content>
                                    <v-list-tile-action class="answer-action">
                                        <v-list-tile-action-text></v-list-tile-action-text>
                                        <v-tooltip left>
                                        <v-btn slot="activator" icon @click="declineAnswer(answer, index)">
                                            <v-icon color="red">close</v-icon>
                                        </v-btn>
                                            <span>Delete</span>
                                        </v-tooltip>
                                    </v-list-tile-action>
                                    <v-list-tile-action class="answer-action">
                                        <v-list-tile-action-text></v-list-tile-action-text>
                                        <v-tooltip left>
                                        <v-btn  slot="activator" icon @click="aproveA(answer, index)">
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
            <div v-if="loading">Loading questions, please wait...</div>
            <div v-show="answers.length === 0 && !loading">No more flagged answers</div>
        </div>
    </div>
</template>

<script src="./flaggedAnswers.js"></script>

<style lang="scss" scoped>
    .v-list__tile__sub-title{
        &.answer-subtitle-reason{
            color: #3f51b5!important;
            font-weight: 500;
        }
    }

    .answer-id{
        cursor: pointer;
    }

</style>