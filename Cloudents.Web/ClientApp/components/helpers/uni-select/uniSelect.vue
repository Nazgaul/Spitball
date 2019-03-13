<template>
<div class="uni-select-component">
    <no-worries :fnStay="closeNoWorriesPopup" :fnLeave="closeInterface" v-show="beforeLeave"></no-worries>
    <!-- <changing-school :popupData="areYouSurePopup"></changing-school> -->
    <add-school-or-class :popupData="openAddSchoolOrClassData"></add-school-or-class>
    <div class="overlay" :class="{'active': beforeLeave || openAddSchoolOrClassData.show}"></div>
    <div class="back-img"></div>
    <div v-show="!beforeLeave" class="close-container" @click="openNoWorriesPopup">
        <v-icon>sbf-close</v-icon>
    </div>
    <div v-show="!beforeLeave" class="select-university-container"  :class="[`${currentStepCssClass}`]">
        <component :is="currentStep" :fnMethods="fnMethods" :enumSteps="enumSteps" style="z-index:1;"></component>
    </div>
</div>
</template>

<script src="./uniSelect.js"></script>

<style lang="less" src="./uniSelect.less"></style>

