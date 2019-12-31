<template>
  <div class="flaggedQuestion">
        <h1 class="text-xs-center mb-4">Flagged Questions</h1>
        <v-data-table
            :headers="headers"
            :items="questions"
            class="flaggedQuestion_table mx-3"
            disable-initial-sort
            :loading="loading"
            :rows-per-page-items="[25, 50, 100,{text: 'All', value:-1}]">

            <template slot="items" slot-scope="props">
                <tr>
                    <td class="flaggedQuestion_item">{{props.item.text}}</td>
                    <td class="flaggedQuestion_item">{{props.item.id}}</td>
                    <td class="flaggedQuestion_item">{{props.item.reason}}</td>
                    <td class="flaggedQuestion_item">
                        <v-tooltip top>
                            <v-btn slot="activator" icon @click="declineQuestion(props.item, props.index)">
                                <v-icon color="red">close</v-icon>
                            </v-btn>
                            <span>Delete question</span>
                        </v-tooltip>
                        <v-tooltip top>
                            <v-btn slot="activator" icon @click="unflagQ(props.item, props.index)">
                                <v-icon color="green">done</v-icon>
                            </v-btn>
                            <span>Approve question</span>
                        </v-tooltip>
                    </td>
                </tr>
            </template>
        </v-data-table>
  </div>
</template>

<script>
import {getAllQuesitons, unflagQuestion} from './flaggedQuestionsService'
import {deleteQuestion} from '../delete/deleteQuestionService'

export default {
    name: 'flaggedQuestion',
    data: () => ({
          headers: [
            {text: 'Question', value: 'question', sortable: false},
            // {text: 'Email', value: 'email', sortable: false},
            {text: 'Question Id', value: 'questionId', sortable: false},
            {text: 'Reason', value: 'reason', sortable: false},
            {text: 'Actions', value: 'actions', sortable: false}
          ],
          questions: [],
          loading: true
    }),
    methods:{
        unflagQ(question, index){
            unflagQuestion(question.id).then(()=>{
                this.questions.splice(index, 1);
                this.$toaster.success(`Question Aproved`);
            }, ()=>{
                this.$toaster.error(`Question Aproved Failed`);
            });
        },
        declineQuestion(question, index){
            let id = question.id;
            deleteQuestion([id]).then(()=>{
                this.questions.splice(index, 1);
                this.$toaster.success(`Question Declined`);
            },()=>{
                this.$toaster.error(`Question Declined Failed`);
            });
        }   
    },
    created(){
        getAllQuesitons().then((questionsResp)=>{
            this.questions = questionsResp;
            this.loading = false;
        });
    }
}
</script>

<style lang="less" scoped>
    .flaggedQuestion {
        width: 100%;
        .flaggedQuestion_item {
            &:last-child {
                display: flex;
                flex-shrink: 0;
                flex-grow: 0;
            }
        }
    }
</style>