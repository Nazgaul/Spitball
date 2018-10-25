<template>
<div class="uni-select-component">
    <div class="back-img">
            <div class="close-container" @click="closeInterface">
                <v-icon>sbf-close</v-icon>
            </div>
        </div>
        <component :is="currentStep" :fnMethods="fnMethods" :enumSteps="enumSteps" style="z-index:1;"></component>
</div>
</template>

<script src="./uniSelect.js"></script>

<style lang="less" src="./uniSelect.less"></style>

