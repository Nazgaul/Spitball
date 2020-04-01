<template functional>
    <div class="homeBtns d-block d-sm-flex mx-auto">
        <button class="btn btnLearn d-block d-sm-inline-block mb-4 mb-sm-0" v-t="'homePage_btn_learn'" @click="props.actions.studentRegister()"></button>
        <button class="btn btnTeach d-block d-sm-inline-block" v-t="'homePage_btn_teach'" @click="props.actions.teacherRegister()"></button>
    </div>
</template>

<style lang="less">
@import "../../../styles/mixin.less";
.homeBtns {
    .responsive-property(justify-content, space-evenly, null, null);

    .btn {
        .responsive-property(width, 230px, null, 100%);
        color: #fff;
        outline: none;
        height: 44px;
        border-radius: 28px;
        font-size: 16px;
        @media (max-width: @screen-xs) {
            margin: 0 auto;
        }
        &.btnLearn {
            margin-right: 80px; // center the buttons to the middle section box's
            background-color: #4c59ff;
        }
        &.btnTeach {
            background-color: #41c4bc;
        }   
    }
}
</style>