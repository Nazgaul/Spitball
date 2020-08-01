<template functional>
    <div class="createCourse mb-6 pa-5 d-flex align-center justify-space-between">
        <div class="createTitle" v-t="'create_course'"></div>
        <div>
            <v-btn
                class="createCourseBtn cancelBtn me-3"
                to="/"
                color="#fff"
                width="132"
                height="40"
                v-t="'cancel'"
                depressed
                rounded
                outlined
            >
            </v-btn>
            <v-btn
                @click="parent.saveCourseInfo()"
                class="createCourseBtn white--text"
                color="#4452fc"
                width="132"
                height="40"
                depressed
                rounded
            >
                <span v-t="'save'"></span>
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
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    max-width: 1077px; // eidan request
    position: sticky;
    z-index: 9;
    top: 70px;
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