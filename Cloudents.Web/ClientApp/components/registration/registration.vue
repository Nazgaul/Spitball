<template>
    <div class="registration">
        <button class="back-button" @click="$_back" v-if="$vuetify.breakpoint.xsOnly">
            <v-icon right>sbf-close</v-icon>
        </button>
        <div class="form-wrap">
            <register-email v-if="step===0" @next="nextStep"></register-email>
            <register-phone v-else-if="step===1" @next="nextStep"></register-phone>
            <register-username v-else-if="step===2" @next="nextStep"></register-username>
            <register-account v-else-if="step===3"></register-account>
        </div>
        <div class="img-wrap"></div>
        <div class="progress">
            <div v-for="page in [0,1,2,3]" :class="{highlighted: page===step}"></div>
        </div>
    </div>
</template>

<style src="./registration.less" lang="less"></style>
<script src="./registration.js"></script>
