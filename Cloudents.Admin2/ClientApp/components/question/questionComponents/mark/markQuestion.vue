<template>
    <div class="container">
        <v-layout justify-center>
            <v-flex xs12 style="background: #ffffff; max-width: 960px; min-width: 960px;">
                <v-toolbar color="indigo" class="heading-toolbar" :height="'64px'" dark>
                    <v-toolbar-title>Mark as correct</v-toolbar-title>
                </v-toolbar>
                <div>
                    <div id="question-wrapper-scroll">
                        <v-card v-show="questions.length > 0" v-for="(question,index) in questions" :key="index">
                            <v-toolbar class="question-toolbar mt-4 back-color-purple">
                                <v-toolbar-title class="question-text-title" @click="openQuestion(question.url)">
                                    <span class="question-text-label d-block mb-1">Question Text</span>
                                    {{question.text}}
                                </v-toolbar-title>
                                <v-spacer></v-spacer>

                                <span title="date mr-2">{{question.create | dateFromISO}}&nbsp;&nbsp;</span>
                                <span title="Fictive Or Original Question ">{{question.isFictive ? 'Fictive' : 'Original'}}</span>
                                <div class="question-actions-container">
                                    <v-tooltip left>
                                        <v-btn slot="activator" icon @click="deleteQuestionByID(question)">
                                            <v-icon color="red">close</v-icon>
                                        </v-btn>
                                        <span>Delete question</span>
                                    </v-tooltip>
                                </div>
                                <!--<v-btn icon @click="openQuestion(question.url)">-->
                                <!--<v-icon>open_in_browser</v-icon>-->
                                <!--</v-btn>-->
                            </v-toolbar>

                            <v-list two-line avatar>
                                <span class="question-text-label text-xs-left text-md-left pl-4  font-weight-medium text-sm-left d-block mb-1">Answer Text</span>
                                <template v-for="(answer, index) in question.answers" v-show="question.answers">
                                    <v-list-tile class="answers-list-tile">
                                        <v-list-tile-content class="answers-content">
                                            <v-list-tile-sub-title class="answer-subtitle">{{answer.text}}
                                            </v-list-tile-sub-title>
                                        </v-list-tile-content>
                                        <v-list-tile-action class="answer-action">
                                            <v-list-tile-action-text></v-list-tile-action-text>
                                            <v-tooltip left>
                                            <v-btn slot="activator" icon @click="deleteAnswerByID(question, answer)">
                                                <v-icon color="red">close</v-icon>
                                            </v-btn>
                                                <span>Delete Answer</span>
                                            </v-tooltip>
                                        </v-list-tile-action>
                                        <v-list-tile-action class="answer-action">
                                            <v-list-tile-action-text></v-list-tile-action-text>
                                            <span v-show="answer.imagesCount > 0" title="Number of Attchments"
                                                  class="font-size-14">
                                                <b>{{answer.imagesCount}}</b>
                                                <v-icon class="font-size-16">attach_file</v-icon>
                                            </span>
                                            <v-tooltip left>
                                            <v-btn slot="activator" icon @click="acceptQuestion(question, answer)">
                                                <v-icon color="green">done</v-icon>
                                            </v-btn>
                                                <span>Accept Answer</span>
                                            </v-tooltip>
                                        </v-list-tile-action>

                                    </v-list-tile>
                                    <v-divider
                                            v-if="index + 1 < question.answers.length"
                                            :key="index"
                                    ></v-divider>
                                </template>
                            </v-list>
                        </v-card>

                        <div v-show="questions.length === 0 && !loading">No question to mark as correct</div>
                        <div v-show="loading">Loading question list, please wait...</div>
                    </div>
                </div>
            </v-flex>
        </v-layout>
    </div>
</template>

<script src="./markQuestion.js"></script>

<style lang="less">
    .v-icon {
        &.font-size-16 {
            font-size: 16px;
        }
    }

    .font-size-14 {
        font-size: 14px;
    }

    #question-wrapper-scroll {
        overflow-y: scroll;
        max-height: 98%;
        height: 98vh;
        padding: 0 12px;
    }

    .heading-toolbar {
        height: 64px;
        .v-toolbar__content {
            height: 64px;
        }
    }

    .v-toolbar__title {
        &.question-text-title {
            font-size: 14px;
            white-space: pre-line;
            text-align: left;
            max-width: 80%;
            &:hover {
                cursor: pointer;
            }
        }
    }

    .answers-list-tile {
        .v-list__tile {
            height: 100% !important;
        }
        .answer-action {
            visibility: hidden;
        }
        &:hover {
            .answer-action {
                visibility: visible;
            }
        }
    }

    /*.question-actions-container {*/
    /*visibility: hidden;*/
    /*}*/

    .question-toolbar{
        max-width: 1280px;
        background-color: transparent!important;
        box-shadow: none!important;
        border-bottom: 1px solid grey;
    }

    .v-card {
        max-width: 1280px;
    }

    .question-toolbar {
        .v-toolbar__content {
            height: auto !important;
            text-align: left;
            padding: 12px 24px;
        }
    }

    .v-list__tile__content {
        &.answers-content {
            .v-list__tile__sub-title {
                &.answer-subtitle {
                    color: rgba(0, 0, 0, .87);
                    white-space: pre-line;
                    padding: 8px;
                }
            }
        }
    }

    .question-text-title {
        white-space: pre-line;
    }


</style>
