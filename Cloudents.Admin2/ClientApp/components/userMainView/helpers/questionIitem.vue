<template>
    <div class="item-wrap" data-app>
        <v-card class="elevation-5">
                <v-card-title class="question-text-title" @click="openQuestion(question.url)">
                    {{question.text}}
               
                <v-spacer></v-spacer>

                <span>{{question.create | dateFromISO}}&nbsp;&nbsp;</span>

                <span title="Fictive Or Original Question ">{{question.isFictive ? 'Fictive' : 'Original'}}</span>
                 
                 
                
                    <v-btn flat v-if="!isOk && !isDeleted" @click="isFlagged ? unflagSingleQuestion(question, index) : approveSingleQuestion(question, index)"
                                   :disabled="proccessedQuestions.includes(question.id)">
                                    <v-icon>check</v-icon>
                                    Accept
            </v-btn>
            <v-btn  flat v-if="!isDeleted" :disabled="proccessedQuestions.includes(question.id)"  @click="deleteQuestion(question, index)">
                 <v-icon>delete</v-icon>
                 Delete
            </v-btn>
            </v-card-title>

            <!-- <v-list two-line avatar v-show="question.answers">
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
            </v-list> -->
        </v-card>
    </div>
</template>

<script>
    import { unflagQuestion } from '../../question/questionComponents/flaggedQuestions/flaggedQuestionsService';
    import { deleteQuestion } from '../../question/questionComponents/delete/deleteQuestionService';
    import { aproveQuestion } from '../../question/questionComponents/pendingQuestions/pendingQuestionsService';
    import {mapActions} from 'vuex';
    export default {

        name: "questionIitem",
        data() {
            return {
                proccessedQuestions: [],
            }
        },
        props: {
            question: {
                type: Array,
                required: false
            },
            filterVal: {
                type: String,
                required: false
            },

        },
        computed:{
            ...mapActions(['deleteQuestionItem']),
            isOk() {
                return this.filterVal === 'ok'
            },
            isPending() {
                return this.filterVal === 'pending'
            },
            isFlagged() {
                return this.filterVal === 'flagged'
            },
            isDeleted() {
                return this.filterVal === 'deleted'
            }
        },
        methods: {
            
            isVisible(itemState) {
                return itemState.toLowerCase() === this.filterVal.toLowerCase();
            },
            deleteQuestion(question, index) {
                let id = question.id;
                let numberArr = [];
                numberArr.push(id);
                let self = this;
                deleteQuestion(numberArr)
                    .then(resp => {
                            self.$toaster.success(`Questions were deleted: ${id}`);
                            this.markAsProccessed(numberArr);
                            self.deleteQuestionItem;
                        },
                        (error) => {
                            self.$toaster.error('Something went wrong');
                        }
                    )
            },
            markAsProccessed(arrIds) {
                for (let i = 0; i < arrIds.length; i++) {
                    this.proccessedQuestions.push(arrIds[i])
                }
                return this.proccessedQuestions
            },

            approveSingleQuestion(question, Index) {
                let self = this;
                aproveQuestion(question.id).then(() => {
                    self.$toaster.success(`Question Aproved`);
                    this.markAsProccessed([question.id]);
                    self.deleteQuestionItem;
                    
                }, () => {
                    self.$toaster.error(`Question Aproved Failed`);
                })
            },
            unflagSingleQuestion(question, index) {
                unflagQuestion(question.id)
                    .then(resp => {
                            this.$toaster.success(`Question ${question.id} approved`);
                            this.markAsProccessed([question.id]);
                        },
                        (error) => {
                            this.$toaster.error('Something went wrong');
                        })
            }
        }, 
        
    }
</script>

<style  lang="scss">


</style>