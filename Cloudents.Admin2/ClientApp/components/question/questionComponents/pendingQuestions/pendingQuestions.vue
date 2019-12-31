<template>
    <div class="pendingQuestions">
        <h1 class="text-xs-center mb-4">Pending Questions</h1>
        <v-data-table
            :headers="headers"
            :items="questions"
            class="pendingQuestions_table mx-3"
            disable-initial-sort
            :loading="loading"
            :rows-per-page-items="[25, 50, 100,{text: 'All', value:-1}]">

            <template slot="items" slot-scope="props">
                <td class="pendingQuestions_item">{{props.item.text}}</td>
                <!-- <td class="pendingQuestions_item">{{props.item.user.email}}</td> -->
                <td class="pendingQuestions_item"><router-link :to="{name: 'userMainView', query: {id: props.item.user.id}}" target="_blank">{{props.item.user.id}}</router-link></td>
                <td class="pendingQuestions_item">
                    <v-tooltip top>
                        <v-btn slot="activator" icon @click="declineQuestion(props.item, props.index)">
                            <v-icon color="red">close</v-icon>
                        </v-btn>
                        <span>Delete</span>
                    </v-tooltip>
                    <v-tooltip top>
                        <v-btn slot="activator" icon @click="aproveQ(props.item, props.index)">
                            <v-icon color="green">done</v-icon>
                        </v-btn>
                        <span>Accept</span>
                    </v-tooltip>
                </td>
            </template>
        </v-data-table>
    </div>
</template>

<script>
import {getAllQuesitons, aproveQuestion} from './pendingQuestionsService'
import {deleteQuestion} from '../delete/deleteQuestionService'

export default {
    name: 'pendingQuestions',
    components: {},
    data: () => ({
        headers: [
            {text: 'Question', value: 'question', sortable: false},
            // {text: 'Email', value: 'email', sortable: false},
            {text: 'User Id', value: 'userId', sortable: false},
            {text: 'Actions', value: 'actions', sortable: false}
        ],
        questions: [],
        loading: true
    }),
    methods:{
        doCopy(id, type){
            let dataType = type || '';
            let self = this;
            this.$copyText(id).then(() => {
                self.$toaster.success(`${dataType} Copied` );
            }, () => {
            });

        },
        aproveQ(question, index){ 
            aproveQuestion(question.id).then(()=>{
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
            }, ()=>{
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
    .pendingQuestions {
        width: 100%;
    }
</style>