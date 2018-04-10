<template functional>
    <a class="studyblue d-block cell" target="_blank" :href="'https://www.studyblue.com/notes/answers/'+props.searchterm+'?utm_source=spitball&utm_medium=referral'" @click="goog_report_conversion('https://www.studyblue.com/notes/answers/'+props.searchterm+'?utm_source=spitball&utm_medium=referral')">
        <v-layout row align-center>
            <img src="./img/study-blue-logo.png" alt="">
            <div class="cell-text">Not finding what you're looking for? <br class="hidden-xs-only"/>Click here to ask StudyBlue's Homework Help</div>
        </v-layout>
    </a>
</template>
<style src="./studyblueCard.less" lang="less"></style>