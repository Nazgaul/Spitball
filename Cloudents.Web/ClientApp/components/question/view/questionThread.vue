<template>
    <div>
        <div class="user-question">
            <question-card :cardData="questionData"></question-card>
        </div>
        <div v-for="answer in questionData.answers" class="user-question">
            <question-card :isAnswer="true" :cardData="answer"></question-card>
        </div>
        <question-text-area v-model="textAreaValue"></question-text-area>
        <v-btn block color="primary" @click=answer() :disabled="!validForm" class="add_answer">Add your answer</v-btn>
    </div>
</template>
<script>
    import questionTextArea from "./../helpers/question-text-area/questionTextArea.vue";
    import questionCard from "./../helpers/question-card/question-card.vue";
    import questionService from '../../../services/questionService';

    export default {
        components: {questionTextArea, questionCard},
        props: {
            questionId: {Number},
        },
        data() {
            return {
                textAreaValue: '',
                files: [],
                questionData: {}
            }
        },
        computed: {
            validForm() {
                return this.textAreaValue.length
            }
        },
        methods: {
            answer() {
                var self = this;
                questionService.answerQuestion(this.questionId, this.textAreaValue, this.files)
                    .then(function () {
                        self.textAreaValue = '';
                        questionService.getQuestion(self.questionId)
                            .then(function (response) {
                                self.questionData = response.data
                            });
                    });
            }
        },
        created() {
            var self = this;
            questionService.getQuestion(this.questionId)
                .then(function (response) {
                    self.questionData = response.data

                });
        }
    }
</script>
