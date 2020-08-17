<template>
    <div class="courseShare">
        <div class="promotePublishWrap">
            <promotePublish :document="{id: courseId}"/>
        </div>
        <marketingBlogs />  
    </div>
</template>

<script>
const promotePublish = () => import('../../marketing/promote/promotePublish.vue');
const marketingBlogs = () => import('../../marketing/marketingBlogs/marketingBlogs.vue');

export default {
    name: 'courseShare',
    props: {
        currentCreatedCourseId: {}
    },
    components: {
        promotePublish,
        marketingBlogs
    },
    computed: {
        courseId(){
            return this.currentCreatedCourseId || this.$route.params?.id;
        }       
    },
}
</script>

<style lang="less">
.courseShare {
    .promotePublishWrap {
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        padding: 50px 0;
        border-radius: 6px;
        background: #fff;
        margin-bottom: 20px;
        .promotePublish {
            margin-top: 0;
        }
    }
    .spitballBlogs {
        box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    }
}
</style>