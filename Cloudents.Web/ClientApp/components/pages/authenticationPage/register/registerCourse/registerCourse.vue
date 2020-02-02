<template>
    <div id="registerCourse">
        <slot name="titleCourse"></slot>
        
        <courses>
            <template #fromRegisterBtn>
                <span></span>
            </template>

            <template #fromRegisterTitle>
                <span class="font-weight-bold">
                    {{$t('register_addCourse_title')}}
                </span>
            </template>
        </courses>
    </div>
</template>

<script>
const courses = () => import('../../../../courses/addCourses/addCourses.vue')
export default {
    components: { courses }
}
</script>

<style lang="less">
@import '../../../../../styles/mixin.less';

#registerCourse {
    .courses_title {
        font-size: 26px;
        font-weight: 600;
        margin-bottom: 60px;
    }

    .add-courses-wrap {
        @media(max-width: @screen-xs) {
            width: 100%;
        }
        #search-classes-list {
            max-height: 360px;
        }
    }
}

</style>