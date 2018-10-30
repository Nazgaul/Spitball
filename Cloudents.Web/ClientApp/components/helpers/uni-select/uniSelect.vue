<template>
<div class="uni-select-component">
    <div class="back-img"></div>
    <div v-show="!beforeLeave" class="close-container" @click="openNoWorriesPopup">
        <v-icon>sbf-close</v-icon>
    </div>
        <component v-show="!beforeLeave" :is="currentStep" :fnMethods="fnMethods" :enumSteps="enumSteps" style="z-index:1;"></component>
        <no-worries :fnStay="closeNoWorriesPopup" :fnLeave="closeInterface" v-show="beforeLeave"></no-worries>
        <changing-school :popupData="areYouSurePopup"></changing-school>
        
</div>
</template>

<script src="./uniSelect.js"></script>

<style lang="less" src="./uniSelect.less"></style>

