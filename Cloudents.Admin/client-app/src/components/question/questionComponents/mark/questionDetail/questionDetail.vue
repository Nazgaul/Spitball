<template>
    <div class="question-table">
        <span>{{questionDetail.questionId}}</span>
        <span class="break-word padding">{{questionDetail.questionText}}</span>
        <span class="break-word padding">{{questionDetail.answerText}}</span>
        <span class="url" @click="visitUrl(questionDetail.url)">{{questionDetail.url}}</span>
        <span class="url" @click="aproveQuestion(questionDetail)">aprove</span>
    </div> 
</template>
<script lang="ts" src="./questionDetail.ts"></script>

<style lang="scss" scoped>
.question-table{
    display:flex;
    border: 1px solid #9a9a9a;
    justify-content: center;
    align-items: center;
    margin-bottom: 10px;
    span{
        flex-grow: 1;
        width:1px;
    }
    &:nth-child(odd){
        background-color: #b6b6b6;
    }

    .url{
        cursor: pointer;
    }
    .break-word{
        word-wrap: break-word;
    }
    .padding{
        padding: 10px;
    }
}
</style>

