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
#registerUniversity {
    .university_title {
        font-size: 26px;
        font-weight: 600;
        margin-bottom: 60px;
    }

    .add-university-wrap {
        width: 500px;
        .university-list {
            max-height: 360px;
        }
    }
}
</style>