<template>
    <div class="step-account">
        <step-template>
            <div slot="step-data" class="limited-width done">
                <h1 class="congrats-heading">CONGRATS!</h1>
                <h2 class="congrats-heading">You are rewarded with</h2>
                <h2 class="congrats-heading"><span class="blue-points">{{initialPointsNum}} SBL</span></h2>
                <img class="money-done-img"  :src="require(`../../img/money-done.png`)"/>
                <p class="congrats">You can spend them to get help with your <br/> Homework questions.</p>
                <button class="continue-btn" @click="finishRegistration">Let's Start</button>
            </div>
            <img  slot="step-image" :src="require(`../../img/done.png`)"/>
        </step-template>

    </div>
</template>
<script src="./registerAccount.js"></script>
<style src="./registerAccount.less" lang="less"></style>
