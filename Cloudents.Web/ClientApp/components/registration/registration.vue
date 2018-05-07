<template>
    <div class="registration">
        <div class="form-wrap">
            <register-email v-if="step===0"></register-email>
            <register-phone v-else-if="step===1" @next="step++"></register-phone>
            <register-username v-else-if="step===2" @next="step++"></register-username>
            <register-account v-else-if="step===3"></register-account>
        </div>
        <div class="img-wrap"></div>
        <div>Step: {{step}}</div>
    </div>
</template>

<style src="./registration.less" lang="less"></style>
<script src="./registration.js"></script>
