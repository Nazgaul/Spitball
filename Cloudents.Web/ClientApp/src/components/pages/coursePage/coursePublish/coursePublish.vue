<template>
    <div class="visibleSection pa-3 mb-4">
        <div class="d-flex align-center justify-space-between mb-3">
            <div class="courseStickyTitle" v-t="'visible'"></div>
            <v-switch
                v-model="courseVisible"
                class="ma-0 pa-0"
                hide-details
            ></v-switch>
        </div>

        <div class="courseStickyTitle" v-t="'course_visible'"></div>
    </div>
</template>

<script>
export default {
    name: 'coursePublish',
    computed: {
        courseVisible: {
            get() {
                return this.$store.getters.getCourseVisible
            },
            set(val) {
                this.$store.commit('setShowCourse', val)
            }
        }
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.visibleSection {
    background: #fff;
    height: max-content;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    border-radius: 6px;
    .courseStickyTitle {
        font-size: 16px;
        color: @global-purple;
    }
    }
</style>