<template functional>
    <div class="homeBtns d-block d-sm-flex mx-auto">
        <button class="btn btnLearn d-block d-sm-inline-block mb-4 mb-sm-0" v-t="'homePage_btn_learn'" @click="props.action"></button>
        <a class="btn btnTeach d-block d-sm-inline-block" href="https://www.teach.spitball.co/" target="_blank" v-t="'homePage_btn_teach'"></a>
    </div>
</template>

<style lang="less">
@import "../../../styles/mixin.less";
.homeBtns {
    .responsive-property(justify-content, space-evenly, null, null);

    .btn {
        .responsive-property(width, 230px, null, 260px);
        color: #fff;
        outline: none;
        padding: 11px 0;
        border-radius: 28px;
        font-size: 16px;
        font-weight: 600;
        @media (max-width: @screen-xs) {
            margin: 0 auto;
        }
        &.btnLearn {
            background-color: #4c59ff;

        }
        &.btnTeach {
            background-color: #41c4bc;
        }   
    }
}
</style>