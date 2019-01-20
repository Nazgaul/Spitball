<template>
    <div class="answer-item-wrap">
        <v-card v-for="(answer, index) in answers" :key="index">
            <v-toolbar class="question-toolbar mt-4 back-color-purple">
                <v-toolbar-title class="question-text-title">
                    {{answer.text}}
                </v-toolbar-title>
                <v-spacer></v-spacer>
                <span title="Fictive Or Original Question ">{{answer.flaggedUserEmail}}</span>
                <v-spacer></v-spacer>
                <div class="answer-id" @click="doCopy(answer.id)">
                    <span>Answer Id: {{answer.id}}</span>
                </div>
            </v-toolbar>

            <v-list two-line avatar>
                <template>
                    <v-list-tile class="answers-list-tile">
                        <v-list-tile-content class="answers-content">
                            <v-list-tile-sub-title class="answer-subtitle">{{answer.reason}}
                            </v-list-tile-sub-title>
                        </v-list-tile-content>
                        <v-list-tile-action class="answer-action">
                            <v-list-tile-action-text></v-list-tile-action-text>
                            <v-btn icon @click="declineAnswer(answer, index)">
                                <v-icon color="red">close</v-icon>
                            </v-btn>
                        </v-list-tile-action>
                        <v-list-tile-action class="answer-action">
                            <v-list-tile-action-text></v-list-tile-action-text>
                            <v-btn icon @click="aproveA(answer, index)">
                                <v-icon color="green">done</v-icon>
                            </v-btn>
                        </v-list-tile-action>

                    </v-list-tile>
                </template>
            </v-list>
        </v-card>
    </div>
</template>

<script>
    import { deleteAnswer} from '../../answer/answerComponents/delete/deleteAnswerService'

    export default {
        name: "answerItem",
        props: {
            answers: {},
            updateData: {
                type: Function,
                required: false

            },
        },
        methods: {
            declineAnswer(answer, index) {
                let self = this;
                let id = answer.id;
                deleteAnswer([id]).then(() => {
                    self.updateData(index);
                    self.$toaster.success(`Answer Deleted`);
                }, err => {
                    this.$toaster.error(`Answer Delete Failed`);
                })
            }
        },
        created(){
            console.log(this.answers)
        }
    }
</script>

<style scoped lang="scss">


</style>