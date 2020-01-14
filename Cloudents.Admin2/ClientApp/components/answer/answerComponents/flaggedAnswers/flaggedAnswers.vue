<template>
    <div class="flaggedAnswer">
        <h1 class="text-xs-center mb-4">Flagged Answers</h1>
        <v-data-table
            :headers="headers"
            :items="answers"
            class="flaggedAnswer_table mx-3"
            disable-initial-sort
            :loading="loading"
            expand
            :rows-per-page-items="[5, 10, 25,{text: 'All', value:-1}]">

            <template slot="items" slot-scope="props">
                <tr @click="props.expanded = !props.expanded">
                    <td class="flaggedAnswer_item">{{props.item.questionText}}</td>
                    <!-- <td class="flaggedAnswer_item">{{props.item.markerEmail}}</td> -->
                    <!-- <td class="flaggedAnswer_item">{{props.item.id}}</td> -->
                    <td class="flaggedAnswer_item">{{props.item.reason}}</td>
                    <td class="flaggedAnswer_item">
                        <v-tooltip top>
                            <v-btn slot="activator" icon @click.stop="declineAnswer(props.item, props.index)">
                                <v-icon color="red">close</v-icon>
                            </v-btn>
                            <span>Delete answer</span>
                        </v-tooltip>
                        <v-tooltip top>
                            <v-btn slot="activator" icon @click.stop="aproveA(props.item, props.index)">
                                <v-icon color="green">done</v-icon>
                            </v-btn>
                            <span>Approve answer</span>
                        </v-tooltip>
                    </td>
                </tr>
            </template>
            <template slot="expand" slot-scope="props">
                <v-card flat>
                    <v-card-text><span class="font-weight-bold mr-1">Answer:</span> {{props.item.text}}</v-card-text>
                    <v-card-text><span class="font-weight-bold mr-1">Email:</span> {{props.item.flaggedUserEmail}}</v-card-text>
                    <hr>
                </v-card>
            </template>
        </v-data-table>
    </div>
</template>

<script>
import { getAllAnswers, aproveAnswer } from './flaggedAnswersService'
import { deleteAnswer} from '../delete/deleteAnswerService'

export default {
    name: 'flaggedAnswer',
    data: () => ({
        headers: [
            {text: 'Question', value: 'question', sortable: false},
            // {text: 'Email', value: 'email', sortable: false},
            // {text: 'Question Id', value: 'questionId', sortable: false},
            {text: 'Reason', value: 'reason', sortable: false},
            {text: 'Actions', value: 'actions', sortable: false}
        ],
        answers: [],
        loading: true,
        expand: false,
    }),
    methods: {
        aproveA(answer, index) {
            aproveAnswer(answer.id).then(() => {
                this.answers.splice(index, 1);
                this.$toaster.success(`Answer Aproved`);
            }, () => {
                this.$toaster.error(`Answer Aproved Failed`);
            });
        },
        declineAnswer(answer, index) {
            let id = answer.id;
            deleteAnswer([id]).then(() => {
                this.answers.splice(index, 1);
                this.$toaster.success(`Answer Declined`);
            },() => {
                this.$toaster.error(`Answer Declined Failed`);
            });
        }
    },
    created() {
        getAllAnswers().then((answersResp) => {
            this.answers = answersResp;
            this.loading = false;
        });
    }
}
</script>

<style lang="less">
    .flaggedAnswer {
        width: 100%;
        .flaggedAnswer_table {
            thead {
                th {
                    width: 30% !important;
                }
            }
        }
    }
</style>