<template functional>
    <v-layout column v-if="props.questionData">
        <question-card :cardData="props.questionData" class="user-question mb-3"></question-card>
        <slot name="answer-form"></slot>
        <div>
            <question-card v-for="answer in props.questionData.answers" :isAnswer="true" :cardData="answer"
                           class="user-question mb-3"></question-card>
        </div>
    </v-layout>
</template>