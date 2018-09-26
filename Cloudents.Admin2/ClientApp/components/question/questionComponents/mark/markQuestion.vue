<template>
    <div>
        <h1>Questions</h1>
            <div class="questionItem" v-for="(question,index) in questions" :key="index">
                <div class="question-header">
                    <span style="flex-grow:5;" title="Question Text">{{question.questionText}}</span>
                    <span><button value="Open" @click="openQuestion(question.url)" title="Open Question" style="cursor:pointer;">Open</button></span>
                    <span title="Fictive Or Original Question ">{{question.isFictive ? 'Fictive' : 'Original'}}</span>
                </div>
                <div class="question-body">
                    <span class="answer-text" title="Answer Text">{{question.answerText}}</span>
                    <span style="justify-content: right;text-align: right; min-width: 65px;"><button style="cursor:pointer;" value="Open" @click="acceptQuestion(question)" title="Accept answer">Accept</button></span>
                </div>
            </div>
    </div>
</template>

<script src="./markQuestion.js"></script>

<style lang="scss" scoped>
    .questionItem{
        display:flex;
        margin:0 auto;
        flex-direction: column;
        border: 1px solid black;
        margin-bottom: 10px;
        width: 70%;
        min-width:500px;
        .question-header{
            display: flex;
            justify-content: center;
            padding:10px;
            border-bottom: 1px solid black;
            background-color: #c0eaac;
            span{
                border-right: 1px solid #9dc889;
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
            }
        }
}
</style>
