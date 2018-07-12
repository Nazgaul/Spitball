<template>
    <div class="step-account">
        <step-template>
            <div slot="step-data" class="limited-width done">
                <h1>CONGRATS!</h1>
                <p class="congrats">You were just rewarded your first <br/><span class="blue-points">{{initialPointsNum}} SBL.</span></p>
                <p class="congrats">You can spend them to get help with your <br/> Homework questions.</p>
                <button class="continue-btn" @click="finishRegistration">Let's Start</button>
            </div>
            <img slot="step-image" :src="require(`../../img/done.png`)"/>
        </step-template>

    </div>
</template>
<script src="./registerAccount.js"></script>
<style src="./registerAccount.less" lang="less"></style>
