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
    border: 1px solid #9a9a9a;
    height: 90px;
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
}
</style>

