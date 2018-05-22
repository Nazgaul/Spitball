<template>
    <div>
        <div class="user-question">
            <question-card :cardData="questionData"></question-card>
        </div>
        <div v-for="answer in questionData.answers" class="user-question">
            <question-card :isAnswer="true" :cardData="answer"></question-card>
        </div>
        <question-text-area :text="textAreaValue"></question-text-area>
        <v-btn block color="primary" @click=ask() :disabled="!validForm" class="add_answer">Add your answer</v-btn>
    </div>
</template>
<script>
    import questionTextArea from "./../helpers/question-text-area/questionTextArea.vue";
    import questionCard from "./../helpers/question-card/question-card.vue";
    export default {
        components: {questionTextArea, questionCard},
        props: {
            questionData: {},
            textAreaValue: ""
        },
        computed: {
            validForm() {
                return this.textAreaValue.length
            }
        },
    }
</script>
