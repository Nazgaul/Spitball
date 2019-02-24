<template>
    <div class="item-wrap" data-app>
        <v-card v-for="(question, index) in questions" :key="index" v-if="isVisible(question.state)">
            <v-toolbar class="question-toolbar mt-4 back-color-purple">
                <v-toolbar-title class="question-text-title" @click="openQuestion(question.url)">
                    {{question.text}}
                </v-toolbar-title>
                <v-spacer></v-spacer>

                <span>{{question.create | dateFromISO}}&nbsp;&nbsp;</span>

                <span title="Fictive Or Original Question ">{{question.isFictive ? 'Fictive' : 'Original'}}</span>
                <div class="question-actions-container">
                    <v-tooltip left attach="tooltip-1">
                        <v-btn slot="activator" icon @click="deleteQuestionByID(question)" class="tooltip-1">
                            <v-icon color="red">close</v-icon>
                        </v-btn>
                        <span>Delete Question</span>
                    </v-tooltip>
                    <v-tooltip left attach="tooltip-2">
                        <v-btn slot="activator" icon @click="aproveQ(question, index)"  class="tooltip-2">
                            <v-icon color="green">done</v-icon>
                        </v-btn>
                        <span>Accept Question</span>
                    </v-tooltip>

                </div>
            </v-toolbar>

            <v-list two-line avatar v-show="question.answers">
                <template v-for="(answer, index) in question.answers">
                    <v-list-tile class="answers-list-tile">
                        <v-list-tile-content class="answers-content">
                            <v-list-tile-sub-title class="answer-subtitle">{{answer.text}}
                            </v-list-tile-sub-title>
                        </v-list-tile-content>
                        <v-list-tile-action class="answer-action">
                            <v-list-tile-action-text></v-list-tile-action-text>
                            <v-btn icon @click="deleteAnswerByID(question, answer)">
                                <v-icon color="red">close</v-icon>
                            </v-btn>
                        </v-list-tile-action>
                        <v-list-tile-action class="answer-action">
                            <v-list-tile-action-text></v-list-tile-action-text>
                            <span v-show="answer.imagesCount > 0" title="Number of Attchments"
                                  class="font-size-14">
                                                <b>{{answer.imagesCount}}</b>
                                                <v-icon class="font-size-16">attach_file</v-icon>
                                            </span>
                            <v-btn icon @click="acceptQuestion(question, answer)">
                                <v-icon color="green">done</v-icon>
                            </v-btn>
                        </v-list-tile-action>

                    </v-list-tile>
                    <v-divider
                            v-if="index + 1 < question.answers.length"
                            :key="index"
                    ></v-divider>
                </template>
            </v-list>
        </v-card>
    </div>
</template>

<script>
    import { deleteQuestion } from '../../question/questionComponents/delete/deleteQuestionService';
    import { aproveQuestion } from '../../question/questionComponents/pendingQuestions/pendingQuestionsService';
    import {mapActions} from 'vuex';
    export default {

        name: "questionIitem",
        props: {
            questions: {
                type: Array,
                required: false
            },
            filterVal: {
                type: String,
                required: false
            },

        },

        methods: {
            ...mapActions(['deleteQuestionItem']),
            isVisible(itemState) {
                return itemState.toLowerCase() === this.filterVal.toLowerCase();
            },
            deleteQuestionByID(question) {
                let id = question.id;
                let numberArr = [];
                numberArr.push(id);
                let self = this;
                deleteQuestion(numberArr)
                    .then(resp => {
                            self.$toaster.success(`Questions were deleted: ${id}`);
                            let questionIndex = self.questions.indexOf(question);
                            self.deleteQuestionItem(questionIndex)
                        },
                        (error) => {
                            self.$toaster.error('Something went wrong');
                            console.log('component delete error', error)
                        }
                    )
            },
            aproveQ(question) {
                let self = this;
                aproveQuestion(question.id).then(() => {
                    let questionIndex = self.questions.indexOf(question);
                    self.deleteQuestionItem(questionIndex);
                    self.$toaster.success(`Question Aproved`);
                }, () => {
                    self.$toaster.error(`Question Aproved Failed`);
                })
            },
        },
    }
</script>

<style  lang="scss">
    .item-wrap{
        .question-toolbar{
            max-width: 100%;
            background-color: transparent!important;
            box-shadow: none;
            border-bottom: 1px solid grey;
        }
        .v-card{
            max-width: 100%;
        }
    }

</style>