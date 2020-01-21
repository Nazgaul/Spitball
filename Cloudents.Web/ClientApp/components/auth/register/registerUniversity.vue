<template>
    <div id="registerUniversity">
        <div class="text-center university_title" v-language:inner="'university_choose_title'"></div>

        <slot name="studentCollege"></slot>
        
        <slot name="studentSchool"></slot>

        <university />
    </div>
</template>

<script>
const university = () => import('../../university/addUniversity/addUniversity.vue');

export default {
    name: 'registerUniversity',
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