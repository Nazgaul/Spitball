<template functional>
    <v-layout column v-if="props.questionData">
        <question-card :cardData="props.questionData" class="user-question mb-3" detailed-view></question-card>
        <slot name="answer-form"></slot>
        <div class="answers_wrap">
            <h3 class="Answers ml-2 mb-3 " v-if="props.questionData.answers.length">Answers</h3>
            <question-card v-for="answer in props.questionData.answers" :typeAnswer="true" :key="answer.id"
                           :showApproveButton="props.questionData.cardOwner && !props.hasCorrectAnswer"
                           :isCorrectAnswer="props.questionData.correctAnswerId && props.questionData.correctAnswerId.toUpperCase() === answer.id.toUpperCase()"
                           :cardData="answer"
                           class="user-question mb-3 answer-card" detailed-view></question-card>
        </div>

    </v-layout>
    
</template>
