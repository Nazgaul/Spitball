<template>
        <div class="uni-select-component">
            <div class="overlay" :class="{'active': beforeLeave || areYouSurePopup.show}"></div>
            <div class="back-img"></div>
            <div  class="close-container" @click="openNoWorriesPopup">
                <v-icon>sbf-close</v-icon>
            </div>
            <div  class="select-university-container">


            </div>
        </div>
</template>

<script>
    export default {
        name: "onBoardGuide"
    }
</script>

<style lang="less">
    .select-university-container {
        width: 499px;
        height: 516px;
        //opacity: 0.91;
        border-radius: 4px;
        box-shadow: 0 1px 17px 0 rgba(16, 27, 147, 0.39);
        //background-image: linear-gradient(140deg, #4b8ffe, #4452fc);
        background-image: linear-gradient(140deg, rgba(75, 143, 254, 1), rgba(68, 82, 252, 0.91));
        z-index: 9;
        background-repeat: no-repeat;
        margin-top: 26px;
        margin-right: 18%;
        padding-left: 32px;
        padding-right: 32px;
    }
</style>