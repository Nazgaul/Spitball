<template>
    <div id="registerUniversity">
        <slot name="titleUniversity"></slot>

        <university>
            <template #fromRegister>
                <span></span>
            </template>
        </university>
    </div>
</template>

<script>
const university = () => import('../../../../university/addUniversity/addUniversity.vue');

export default {
    components: {
        university
    }
}
</script>

<style lang="less">
@import '../../../../../styles/mixin';

#registerUniversity {
    .university_title {
        font-size: 26px;
        font-weight: 600;
        margin-bottom: 60px;

        @media (max-width: @screen-xs) {
            margin-bottom: 30px;
        }
    }

    .add-university-wrap {
        width: inherit;
        .university-list {
            max-height: 360px;
        }
    }
}
</style>