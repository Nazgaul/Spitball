<template>
    <div id="registerCourse">
        <slot name="titleCourse"></slot>
        
        <courses />
    </div>
</template>

<script>
const courses = () => import('../../../../courses/addCourses/addCourses.vue')

export default {
    // name: 'registerCourse',
    components: { courses },
    data: () => ({

    })
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';

#registerCourse {
    max-width: 500px;
    .courses_title {
        font-size: 26px;
        font-weight: 600;
        margin-bottom: 60px;
    }

    .add-courses-wrap {
        width: 500px;

        @media(max-width: @screen-xs) {
            width: 100%;
        }
        #search-classes-list {
            max-height: 360px;
        }
    }
}

</style>