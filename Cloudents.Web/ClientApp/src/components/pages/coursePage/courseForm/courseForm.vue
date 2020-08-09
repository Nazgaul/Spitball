<template>
    <div class="d-flex courseForm">
        <div class="courseLeftSide">
            <courseInfo ref="courseInfo" />
            <div class="courseTeachingWrapper mb-6">
                <div class="pa-5">
                    <div class="courseTeachingTitle" v-t="'set_Teaching'"></div>
                </div>
                <courseTeaching v-for="n in numberOfLecture" :key="n" :index="n" />
                <div class="addLecture d-flex pa-5 pt-0">
                    <v-icon size="14" color="#4c59ff">sbf-plus-regular</v-icon>
                    <button class="ms-1" v-t="'another_lecture'" @click.prevent="$store.commit('setNumberOfLecture', numberOfLecture + 1)"></button>
                </div>
            </div>
            <courseUpload ref="courseUpload" />
        </div>
        <div class="courseRightSide ms-6">
            <coursePublish />
            <coursePromote />
        </div>
    </div>
</template>

<script>
import { MyCourses } from '../../../../routes/routeNames'

import courseInfo from '../courseInfo/courseInfo.vue';
import courseTeaching from '../courseTeaching/courseTeaching.vue';
import courseUpload from '../courseUpload/courseUpload.vue';
import coursePublish from '../coursePublish/coursePublish.vue';
import coursePromote from '../coursePromote/coursePromote.vue';

export default {
    components: {
        courseInfo,
        courseTeaching,
        courseUpload,
        coursePublish,
        coursePromote,
    },
    data() {
        return {
            courseRoute: MyCourses,
        }
    },
    computed: {
        numberOfLecture() {
            return this.$store.getters.getNumberOfLecture
        },
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.courseForm {
    .courseLeftSide {
        max-width: 760px;
        min-width: 0;
        width: 100%;
        .courseTeachingWrapper {
            background: #fff;
            border-radius: 6px;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);

            .courseTeachingTitle {
                font-size: 20px;
                font-weight: 600;
                color: @global-purple;
            }
            .addLecture {
                font-size: 16px;
                color: #4c59ff;

                button {
                    outline: none;
                }
            }
        }
    }
    .courseRightSide {
        max-width: 296px;
        width: 100%;
        height: max-content;
        position: sticky;
        // top: 170px;
    }
}
</style>