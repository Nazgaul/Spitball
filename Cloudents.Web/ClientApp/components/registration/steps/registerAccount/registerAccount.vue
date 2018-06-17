<template>
    <div class="step-account">
        <step-template>
            <div slot="step-data" class="limited-width done">
                <h1>CONGRATS!</h1>
                <p>You just earned your first 10 tokens.</p>
                <button class="continue-btn" @click="finishRegistration">Lets Start</button>
            </div>
            <img slot="step-image" :src="require(`../../img/done.png`)"/>
        </step-template>

    </div>
</template>
<script src="./registerAccount.js"></script>
<style src="./registerAccount.less" lang="less"></style>
