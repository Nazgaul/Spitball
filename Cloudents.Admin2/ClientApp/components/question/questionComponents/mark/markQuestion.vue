<template>
    <div id="question-wrapper-scroll">
        <h1>Questions</h1>
        <div v-show="questions.length > 0" class="questionItem" v-for="(question,index) in questions" :key="index">
            <div class="question-header">
                <span style="flex-grow:5;" class="question-text-title"  title="Question Text">{{question.text}}</span>
                <div class="question-actions-container">
                <span>
                    <v-btn round value="Open" @click="openQuestion(question.url)" title="Open Question">Open
                     <v-icon light right>open_in_browser</v-icon>
                    </v-btn>
                </span>
                <span title="Fictive Or Original Question ">{{question.isFictive ? 'Fictive' : 'Original'}}</span>
                <span title="Number of Attchments">(<b>{{question.imagesCount}}</b>)</span>
                </div>
            </div>
            <div class="question-body" v-for="answer in question.answers" :key="answer.id">
                <span class="answer-text" title="Answer Text">{{answer.text}}</span>
                <span style="justify-content: right;text-align: right; min-width: 65px;">
                    <span title="Number of Attchments">(<b>{{answer.imagesCount}}</b>)</span>
                    <v-btn  round color="#97ed82"  value="Open" @click="acceptQuestion(question, answer)" title="Accept answer">
                        Accept
                     <v-icon light right>check</v-icon>
                    </v-btn>
                </span>
            </div>

        </div>

        <div v-show="questions.length === 0 && !loading">No question to mark as correct</div>
        <div v-show="loading">Loading question list, please wait...</div>

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
            &:hover{
               background-color: #9dc08c
            }
        }
        .question-header{
            display: flex;
            justify-content: center;
            padding:10px 25px;
            background-color: #f8f8f8;
            /*background-color: #c7c7c7;*/
            border-top-left-radius: 25px;
            border-top-right-radius: 25px;
            .question-actions-container{
                display: flex;
                flex-direction: row;
                justify-content: center;
            }
            span{
                flex-grow: 1;
                vertical-align: middle;
                text-align: center;
                margin: auto;
                min-width: 65px;
                overflow-wrap: break-word;
                padding: 3px;

            }
            .question-text-title{
                font-size: 16px;
                font-weight: 600;
                color: rgba(0,0,0,.87);
                text-align: left;
            }
        }
        .question-body{
            position:relative;
            display: flex;
            margin:10px;
            justify-content: left;
            text-align: left;
            border-bottom: 1px solid #c7c7c7;
            border-radius: 25px;
            padding: 7px;
            align-items: center;
            &:hover{
                background-color: #e8e8e8;
            }
            span{
                flex-grow: 1;
            }
            .answer-text{
                word-break: break-all;
                padding-left: 15px;
                flex-basis: 85%;
            }
        }
}
</style>
