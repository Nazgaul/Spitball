<template functional>
    <v-layout column v-if="props.questionData">
        <question-card :cardData="props.questionData" class="user-question mb-3" detailed-view></question-card>
        <slot name="answer-form"></slot>
        <div>
            <question-card v-for="answer in props.questionData.answers" :typeAnswer="true"
                           :showApproveButton="props.questionData.myQuestion"
                           :isCorrectAnswer="props.questionData.correctAnswer && props.questionData.correctAnswer.toUpperCase() === answer.id.toUpperCase()"
                           :cardData="answer"
                           class="user-question mb-3" detailed-view></question-card>
        </div>
    </v-layout>
</template>