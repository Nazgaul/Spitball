<template>
    <div id="coursePage" class="coursePage ma-sm-8">
        <courseCreate />
        <div class="d-flex">
            <div class="courseLeftSide">
                <courseInfo />
                <courseTeaching v-for="n in numberOfLecture" :key="n" :index="n" />
                <div class="mb-6">
                    <button v-t="'another_lecture'" @click="addLecture"></button>
                </div>
                <courseUpload />
            </div>
            <courseSticky />
        </div>
    </div>
</template>

<script>
import courseCreate from './courseCreate/courseCreate.vue';
import courseInfo from './courseInfo/courseInfo.vue';
import courseTeaching from './courseTeaching/courseTeaching.vue';
import courseUpload from './courseUpload/courseUpload.vue';
import courseSticky from './courseSticky/courseSticky.vue';

export default {
    components: {
      courseCreate,
      courseInfo,
      courseTeaching,
      courseUpload,
      courseSticky
    },
    computed: {
        numberOfLecture() {
            return this.$store.getters.getNumberOfLecture
        }
    },
    methods: {
        addLecture() {
            this.$store.commit('setNumberOfLecture', this.numberOfLecture + 1)
        }
    }
}
</script>

<style lang="less">
.coursePage {
    .courseLeftSide {
        min-width: 760px;
        max-width: 760px;
    }
}
</style>