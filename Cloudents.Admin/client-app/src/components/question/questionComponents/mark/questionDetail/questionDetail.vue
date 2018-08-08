<template>
    <div class="question-table">
        <span>{{questionDetail.questionId}}</span>
        <span>{{questionDetail.questionText}}</span>
        <span>{{questionDetail.answerText}}</span>
        <span>{{questionDetail.link}}</span>
        <span>{{questionDetail.aproveUrl}}</span>
    </div> 
</template>
<script lang="ts" src="./questionDetail.ts"></script>

<style lang="scss" scoped>
.question-table{
    display:flex;
    border: 1px solid black;
    span{
        flex-grow: 1
    }
}
</style>

