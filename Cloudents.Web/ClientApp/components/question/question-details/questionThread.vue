<template functional>
    <v-layout column v-if="props.questionData">
        <question-card :cardData="props.questionData" class="user-question mb-3" detailed-view></question-card>
        <slot name="answer-form"></slot>
        <div>
            <h3 class="Answers ml-3 mb-3" v-if="props.questionData.answers.length">Answers</h3>
            <question-card v-for="answer in props.questionData.answers" :typeAnswer="true" :key="answer.id"
                           :showApproveButton="props.questionData.cardOwner && !props.hasCorrectAnswer"
                           :isCorrectAnswer="props.questionData.correctAnswerId && props.questionData.correctAnswerId.toUpperCase() === answer.id.toUpperCase()"
                           :cardData="answer"
                           class="user-question mb-3 answer-card" detailed-view></question-card>
        </div>
         <v-dialog v-model="showDialog" max-width="600px" content-class="registration-dialog">
            <v-card>
                <button class="close-btn" @click="showDialog = false">
                    <v-icon>sbf-close</v-icon>
                </button>
                <v-card-text class="limited-width">
                    <h1>Are you sure you want to exit?</h1>
                    <p>Exit from this process will delete all your progress and information</p>
                    <button class="continue-btn" @click="$_back">Exit</button>
                </v-card-text>
            </v-card>
        </v-dialog>
    </v-layout>
    
</template>