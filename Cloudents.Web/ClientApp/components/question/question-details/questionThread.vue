<template>
    <v-layout column v-if="questionData">
        <question-card :cardData="questionData" class="user-question mb-4" detailed-view></question-card>
        <slot name="answer-form"></slot>
        <div class="answers_wrap" ref="answers">
            <div class="answer-container">
                <h3 class="Answers ml-2" style="padding-top: 12px;padding-left: 12px;font-weight: 600; font-size:16px;" v-if="questionData.answers.length" v-language:inner>questionTheard_Answers</h3>
                <answer-card 
                    v-for="(answer, index) in questionData.answers"
                    :key="answer.id"
                    :cardData="answer"
                    :typeAnswer="true"
                    class="user-question answer-card"
                    :id="`answer-${index}`"
                    detailed-view
                >
                </answer-card>
            </div>
            
        </div>
    </v-layout>
</template>
<script>
import questionCard from "./../helpers/new-question-card/new-question-card.vue";
import answerCard from "./../helpers/question-card/question-card.vue";

export default { 
    components:{questionCard,answerCard},
    props:['questionData']
}
</script>