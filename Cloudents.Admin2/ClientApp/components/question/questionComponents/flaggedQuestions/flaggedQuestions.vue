<template>
    <div>
        <h1>Pending Questions</h1>
        <div class="page-container">
            <div class="question-co">
                <div v-show="questions.length > 0" class="questionItem" v-for="(question,index) in questions" :key="index">
                    <div class="question-left-body">
                        <div class="user-container">
                            <!-- <div>User Id: {{question.user.id}}</div> -->
                            <!-- <div title="Number of Attachments">Attachments: (<b>{{question.imagesCount}}</b>)</div> -->
                            <div>Flagged Email: {{question.flaggedUserEmail}}</div>
                        </div>
                        <div class="question-container">
                            <div class="bottom-border bold bottom-space">Flagged Question Id: {{question.id}}</div>
                            <div>{{question.reason}}</div>
                        </div>
                    </div>
                    <div class="question-right-body">
                        <button @click="unflagQ(question, index)">Unflag</button>
                        <button class="decline" @click="declineQuestion(question, index)">Delete</button>
                    </div>
                </div>
                <div v-if="loading">Loading questions, please wait...</div>
                <div v-show="questions.length === 0 && !loading">No more flagged questions</div>
            </div>
        </div>
        
    </div>
</template>

<script src="./flaggedQuestions.js"></script>

<style lang="scss" scoped>
.page-container{
    display: flex;
    .deleted-emails{
            position: absolute;
            color:#c25050;
            .bold{
                font-weight: 600;
            }
        }
    .question-co{
        flex-grow: 1;
        .questionItem{
                display:flex;
                margin:0 auto;
                flex-direction: row;
                border: 2px solid #c7c7c7;
                margin-bottom: 10px;
                width: 70%;
                min-width:500px;   
                border-radius: 29px;  
                
            .question-left-body{
                    flex-grow: 10;
                    display: flex;
                    flex-direction: column;
                    justify-content: center;
                    padding:10px;
                    padding-left: 15px;
                    background-color: #c7c7c7;
                    border-radius: 20px;
                    margin: 10px;
                .user-container{
                        display: flex;
                        flex-grow: 1;
                        flex-direction: row;
                        justify-content: space-between;
                        width: 98%;
                        border-bottom: 1px solid #e0e0e0;
                }
                .question-container{
                        display: flex;
                        flex-grow: 5;
                        flex-direction: column;
                        width:100%;
                        text-align: left;
                        .bottom-border{
                            border-bottom: 1px solid #c7c7c7;
                            width: fit-content;
                        }
                        .bold{
                            font-weight: 600;
                        }
                        .bottom-space{
                            margin-bottom:5px;
                        }
                }
            }
            .question-right-body{
                    position:relative;
                    display: flex;
                    flex-direction: column;
                    flex-grow: 1;
                    margin:10px;
                    justify-content: left;
                    text-align: left;
                    background-color: #c7c7c7;
                    border-radius: 25px;
                    padding: 7px;
                    button{
                        cursor: pointer;
                        background-color: #affb93;
                        border-radius: 25px;
                        border: none;
                        outline: none;
                        cursor:pointer;
                        height: 40px;
                        margin-bottom: 5px;
                        margin-top: 5px;
                        &.decline{
                        background-color: #fb9393
                        }
                    }
                }
            }
        }
    }
    
</style>