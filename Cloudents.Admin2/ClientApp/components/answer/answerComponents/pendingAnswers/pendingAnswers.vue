<template>
    <div>
        <h1>Pending Answers</h1>
        <div class="page-container">
            <div class="answer-co">
                <div v-show="asnwers.length > 0" class="answerItem" v-for="(answer,index) in answers" :key="index">
                    <div class="answer-left-body">
                        <div class="user-container">
                            <div>User Id: {{answer.user.id}}</div>
                            <div title="Number of Attachments">Attachments: (<b>{{answer.imagesCount}}</b>)</div>
                            <div>User Email: {{answer.user.email}}</div>
                        </div>
                        <div class="answer-container">
                            <div class="bottom-border bold bottom-space">Answer Id: {{answer.id}}</div>
                            <div>{{answer.text}}</div>
                        </div>
                    </div>
                    <div class="answer-right-body">
                        <button @click="aproveQ(answer, index)">Aprove</button>
                        <button class="decline" @click="declineAnswer(answer, index)">Delete</button>
                    </div>
                </div>
                <div v-if="loading">Loading answers, please wait...</div>
                <div v-show="answer.length === 0 && !loading">No more answers to approve</div>
            </div>
        </div>

    </div>
</template>

<script src="./pendingAnswers.js"></script>

<style lang="scss" scoped>
    .page-container {
        display: flex;
        .deleted-emails

    {
        position: absolute;
        color: #c25050;
        .bold

    {
        font-weight: 600;
    }

    }

    .answer-co {
        flex-grow: 1;
        .answerItem

    {
        display: flex;
        margin: 0 auto;
        flex-direction: row;
        border: 2px solid #c7c7c7;
        margin-bottom: 10px;
        width: 70%;
        min-width: 500px;
        border-radius: 29px;
        .answer-left-body

    {
        flex-grow: 10;
        display: flex;
        flex-direction: column;
        justify-content: center;
        padding: 10px;
        padding-left: 15px;
        background-color: #c7c7c7;
        border-radius: 20px;
        margin: 10px;
        .user-container

    {
        display: flex;
        flex-grow: 1;
        flex-direction: row;
        justify-content: space-between;
        width: 98%;
        border-bottom: 1px solid #e0e0e0;
    }

    .answer-container {
        display: flex;
        flex-grow: 5;
        flex-direction: column;
        width: 100%;
        text-align: left;
        .bottom-border

    {
        border-bottom: 1px solid #c7c7c7;
        width: fit-content;
    }

    .bold {
        font-weight: 600;
    }

    .bottom-space {
        margin-bottom: 5px;
    }

    }
    }

    .answer-right-body {
        position: relative;
        display: flex;
        flex-direction: column;
        flex-grow: 1;
        margin: 10px;
        justify-content: left;
        text-align: left;
        background-color: #c7c7c7;
        border-radius: 25px;
        padding: 7px;
        button

    {
        cursor: pointer;
        background-color: #affb93;
        border-radius: 25px;
        border: none;
        outline: none;
        cursor: pointer;
        height: 40px;
        margin-bottom: 5px;
        margin-top: 5px;
        &.decline

    {
        background-color: #fb9393
    }

    }
    }
    }
    }
    }
</style>