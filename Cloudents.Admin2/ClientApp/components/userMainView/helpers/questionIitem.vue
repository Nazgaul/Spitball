<template>
    <div class="item-wrap" data-app>
        <v-card class="elevation-0 qCard mb-2">
            <v-card-title class="question-text-title justify-space-between grey lighten-2" @click="openQuestion(question.url)">
                <div><b>Created: </b><span>{{question.create | dateFromISO}}</span></div>
                <div>
                    <v-btn icon v-if="!isOk && !isDeleted" @click="isFlagged ? unflagSingleQuestion(question, index) : approveSingleQuestion(question, index)"
                    :disabled="proccessedQuestions.includes(question.id)">
                    <v-icon color="green">check</v-icon>
                    </v-btn>
                    <v-btn icon v-if="!isDeleted" :disabled="proccessedQuestions.includes(question.id)"  @click="deleteQuestion(question, index)">
                        <v-icon>delete</v-icon>
                    </v-btn>
                </div>
            </v-card-title>
            <v-card-text>
                <b>Text: </b>{{question.text}}
            </v-card-text>
            <!-- <span title="Fictive Or Original Question ">{{question.isFictive ? 'Fictive' : 'Original'}}</span> -->
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
                type: Object,
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

<style  lang="less">
.item-wrap{
    .qCard{
        border: 1px solid #ccc !important;
        .question-text-title{
            padding: 0px 16px;
        }
    }
}
</style>