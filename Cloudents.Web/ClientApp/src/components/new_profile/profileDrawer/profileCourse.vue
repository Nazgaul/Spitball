<template>
    <div class="profileCourses mt-4 mb-8">
        <draggable
            :list="courses"
            class="list-group"
            v-bind="dragOptions"
            :move="checkMove"
            @start="dragging = true"
            @end="dragging = false"
        >
            <transition-group type="transition" name="flip-list">
                <div v-for="(course, index) in courses" :key="index" class="courseSessionsDragWrap d-flex align-center">
                    <img :src="$proccessImageUrl(course.image, 60, 60)" width="60" height="60" />
                    <div class="ms-2">{{course.name}}</div>
                </div>
            </transition-group>
        </draggable>
    </div>
</template>

<script>
import draggable from "vuedraggable";

export default {
    components: {
        draggable
    },
    computed: {
        dragOptions() {
            return {
                animation: 200,
                group: "description",
                disabled: false,
                ghostClass: "ghost"
            }
        },
        courses() {
            return this.$store.getters.getProfileCourses
        }
    },
    methods: {
        checkMove: function(e) {
            return
            let {futureIndex, index} = e.draggedContext
            const movedItem = this.files.slice(index, 1)[0]
            this.files.slice(futureIndex, 0, movedItem)
        }
    }
}
</script>

<style lang="less">
    .courseSessionsDragWrap {
        padding: 4px;
        margin: 4px;
        cursor: move;
        img {
            border: 1px solid #ddd;
        }
        &:hover {
            border: 2px dashed #d8d8df !important;
            padding: 2px;
        }
    }
</style>