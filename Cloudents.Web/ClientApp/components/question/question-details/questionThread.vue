<template functional>
    <v-layout column v-if="props.questionData">
        <question-card :cardData="props.questionData" class="user-question mb-3" detailed-view></question-card>
        <slot name="answer-form"></slot>
        <slot name="currently-watching"></slot>  
        <div class="answers_wrap">
            <div class="answer-container">
                <h3 class="Answers ml-2" style="padding-top: 12px;padding-left: 12px;font-weight: 600;" v-if="props.questionData.answers.length" v-language:inner>Answers</h3>
                <question-card v-for="answer in props.questionData.answers" :typeAnswer="true" :key="answer.id"
                    :showApproveButton="props.questionData.cardOwner && !props.hasCorrectAnswer"
                    :hasAnswer="props.questionData.correctAnswerId"
                    :isCorrectAnswer="props.questionData.correctAnswerId && props.questionData.correctAnswerId.toUpperCase() === answer.id.toUpperCase()"
                    :cardData="answer"
                    class="user-question answer-card" detailed-view></question-card>
            </div>
            
        </div>
    </v-layout>
</template>
