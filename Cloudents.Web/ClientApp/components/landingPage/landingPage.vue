<template>
    <div class="page">
    <nav>

    </nav>
    <main>
    <section>One</section>
    <section class="two">
        <div class="section-part-left">dsgfsdfds</div>
        <div class="section-part-right">
        <question-card :cardData="randomQuestionData"></question-card>
        <extended-text-area
                uploadUrl="/api/upload/ask"
                v-model="textAreaValue"
                :actionType="actionType"
                :error="errorTextArea"
        ></extended-text-area>
        </div>
    </section>
    <section>Three</section>
    <section>Four</section>
    <section>Five</section>
    <section>Six</section>
    <section>Seven</section>
    </main>
    </div>
</template>


<style src="./landingPage.less" scoped lang="less"></style>
<script src="./landingPage.js"></script>