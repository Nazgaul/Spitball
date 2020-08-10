<template functional>
    <div class="createCourse mb-6 pa-5 d-flex align-center justify-space-between">
        <div class="createTitle">
            {{parent.$route.params.id ? parent.$t('update_course') : parent.$t('create_course')}}
        </div>
        <div>
            <v-btn
                class="createCourseBtn cancelBtn me-3"
                :to="{name: props.courseRoute}"
                color="#fff"
                width="132"
                height="40"
                v-t="'cancel'"
                rounded
                depressed
                outlined
            >
            </v-btn>
            <v-btn
                @click="parent.saveCourseInfo()"
                :loading="parent.loading"
                class="createCourseBtn white--text"
                color="#4452fc"
                width="132"
                height="40"
                depressed
                rounded
            >
                <span>{{parent.step === 3 ? parent.$t('save') : parent.$t('next')}}</span>
            </v-btn>
        </div>
    </div>
</template>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.createCourse {
    background: #fff;
    border-radius: 6px;
    .createTitle {
        font-size: 20px;
        font-weight: 600;
        color: @global-purple;
    }
    .createCourseBtn {
        font-weight: 600;
        &.cancelBtn {
            border: solid 1px #4452fc !important;
            color: #4452fc !important;
        }
    }
}
</style>