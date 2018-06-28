<template>
    <div>
        <h2>Great Job!</h2>
        <p>
            Let’s wait for eidan2134 to see if accepts your answer.
            In the meantime, below is another question that you should be able to with.
        </p>
        <question-card :cardData="cardList[0]" class="user-question mb-3" detailed-view></question-card>

    </div>
</template>

<script src="questionSuggestPopUp.js"></script>

<style src="./questionSuggestPopUp.less" lang="less"></style>
