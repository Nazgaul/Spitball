<template>
    <div class="item-wrap" data-app>
        <v-card class="answer-card" v-for="(answer, index) in answers" :key="index" v-if="isVisible(answer.state)">
            <v-toolbar class="answer-toolbar mt-4 back-color-purple">
                <v-toolbar-title class="answer-text-title">
                    <span class="question-text-label">Answer Text</span>

                    {{answer.text}}
                </v-toolbar-title>
                <v-spacer></v-spacer>
                <span title="Fictive Or Original Question ">{{answer.flaggedUserEmail}}</span>
                <v-spacer></v-spacer>
                <v-flex>
                    <v-tooltip left  attach="tooltip-1">
                        <v-btn slot="activator" icon @click="declineAnswer(answer, index)" class="tooltip-1">
                            <v-icon color="red">close</v-icon>
                        </v-btn>
                        <span>Delete</span>
                    </v-tooltip>
                    <v-list-tile-action-text></v-list-tile-action-text>
                    <v-tooltip left  attach="tooltip-2">
                        <v-btn slot="activator" icon @click="aproveA(answer, index)" class="tooltip-2">
                            <v-icon color="green">done</v-icon>
                        </v-btn>
                        <span>Accept</span>
                    </v-tooltip>
                </v-flex>
            </v-toolbar>

            <v-list two-line avatar>
                <template>
                    <v-list-tile class="answers-list-tile">
                        <v-list-tile-content class="answers-content">
                            <v-list-tile-sub-title class="answer-subtitle">
                                <span class="question-text-label">Question Text</span>
                                {{answer.questionText}}
                            </v-list-tile-sub-title>
                        </v-list-tile-content>
                    </v-list-tile>
                </template>
            </v-list>
        </v-card>
    </div>
</template>

<script>
    import { deleteAnswer } from '../../answer/answerComponents/delete/deleteAnswerService'
    import { aproveAnswer } from '../../answer/answerComponents/flaggedAnswers/flaggedAnswersService'
    import {mapActions} from 'vuex';
    export default {
        name: "answerItem",
        props: {
            answers: {},
            filterVal: {
                type: String,
                required: false
            },
        },
        methods: {
            ...mapActions(['deleteAnswerItem']),
            isVisible(itemState) {
                return itemState.toLowerCase() === this.filterVal.toLowerCase();
            },
            doCopy(id, type) {
                let dataType = type || '';
                let self = this;
                this.$copyText(id).then((e) => {
                    self.$toaster.success(`${dataType} Copied`);
                }, (e) => {
                })

            },
            declineAnswer(answer, index) {
                let self = this;
                let id = answer.id;
                deleteAnswer([id]).then(() => {
                    self.deleteAnswerItem(index);
                    self.$toaster.success(`Answer Deleted`);
                }, err => {
                    self.$toaster.error(`Answer Delete Failed`);
                })
            },
        
        aproveA(answer, index) {
                let self = this;
                let id = answer.id;
                aproveAnswer(id).then(() => {
                    self.$toaster.success(`Answer Aproved`);
                    self.deleteAnswerItem(index);
                }, () => {
                    self.$toaster.error(`Answer Aproved Failed`);
                })
            },
        },
    }
</script>

<style lang="scss">
    .item-wrap {
        .question-text-label {
            font-weight: 500;
            color: #000;
        }

        .v-toolbar__content {
            height: 100% !important; //vuetify overwrite
            padding: 12px 24px;
        }
        .answer-toolbar {
            height: 100%;
        }
        .answer-id {
            cursor: pointer;
        }

        .answer-card {
            max-width: 1280px;
        }
        .answer-text-title {
            white-space: pre-line;
            text-align: left;
            max-width: 960px;
            font-size: 14px;
        }
        .answer-toolbar {
            max-width: 100%;
            background-color: transparent!important;
            box-shadow: none;
            border-bottom: 1px solid grey;
            .answer-text-title {

            }
        }
        .v-card {
            max-width: 100%;
        }
    }

</style>