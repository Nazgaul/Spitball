<template functional>
    <v-layout column v-if="props.questionData">
        <question-card :cardData="props.questionData" class="user-question mb-4" detailed-view></question-card>
        <slot name="answer-form"></slot>
        <slot name="currently-watching"></slot>  
        <div class="answers_wrap" ref="answers">
            <div class="answer-container">
                <h3 class="Answers ml-2" style="padding-top: 12px;padding-left: 12px;font-weight: 600; font-size:16px;" v-if="props.questionData.answers.length" v-language:inner>questionTheard_Answers</h3>
                <answer-card 
                    v-for="(answer, index) in props.questionData.answers"
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