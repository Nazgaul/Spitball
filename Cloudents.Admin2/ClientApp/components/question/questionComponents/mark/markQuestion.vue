<template>
    <div>
        <h1>Questions</h1>
            <div v-show="questions.length > 0" class="questionItem" v-for="(question,index) in questions" :key="index">
                <div class="question-header">
                    <span style="flex-grow:5;" title="Question Text">{{question.questionText}}</span>
                    <span><button class="btn-mark" value="Open" @click="openQuestion(question.url)" title="Open Question">Open</button></span>
                    <span title="Fictive Or Original Question ">{{question.isFictive ? 'Fictive' : 'Original'}}</span>
                </div>
                <div class="question-body">
                    <span class="answer-text" title="Answer Text">{{question.answerText}}</span>
                    <span style="justify-content: right;text-align: right; min-width: 65px;"><button class="btn-mark" value="Open" @click="acceptQuestion(question)" title="Accept answer">Accept</button></span>
                </div>
            </div>
            <div v-show="questions.length === 0">Loading question list, please wait...</div>
    </div>
</template>

<script src="./markQuestion.js"></script>

<style lang="scss" scoped>
    .questionItem{
        display:flex;
        margin:0 auto;
        flex-direction: column;
        border: 2px solid #c7c7c7;
        margin-bottom: 10px;
        width: 70%;
        min-width:500px;   
        border-radius: 29px;     
        .btn-mark{
            cursor: pointer;
            background-color: #9d9d9d;
            border-radius: 25px;
            border: none;
            outline: none;
            cursor:pointer;
            height: 25px;
            color: white;
            font-weight: 600;
        }
        .question-header{
            display: flex;
            justify-content: center;
            padding:10px;
            background-color: #c7c7c7;
                border-radius: 50px;
            span{
                flex-grow: 1;
                vertical-align: middle;
                text-align: center;
                margin: auto;
                min-width: 65px;
                overflow-wrap: break-word;
                padding: 3px;
            }
        }
        .question-body{
            position:relative;
            display: flex;
            margin:10px;
            justify-content: left;
            text-align: left;
            span{
                flex-grow: 1;
            }
            .answer-text{
                word-break: break-all;
                padding-left: 15px;
            }
        }
}
</style>
