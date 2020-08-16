<template>
    <div class="profileCourses mt-4 mb-8">
        <draggable
            :list="courses"
            class="list-group"
            v-bind="dragOptions"
            :move="checkMove"
            @start="dragging = true"
            @end="handleEndMove"
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
    data() {
        return {
            oldIndex: 0,
            newIndex: 0
        }
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
        handleEndMove() {
            if(this.oldIndex !== this.newIndex) {
                let self = this
                this.$store.dispatch('updateProfileClassPosition', {
                    oldIndex: this.oldIndex,
                    newIndex: this.newIndex
                }).then(() => {
                    const movedItem = self.courses.slice(self.oldIndex, 1)[0]
                    self.courses.slice(self.newIndex, 0, movedItem)
                }).finally(() => {
                    self.oldIndex = 0
                    self.newIndex = 0
                })
            }
            this.dragging = false
        },
        checkMove(e) {
            this.oldIndex = e.draggedContext.index
            this.newIndex = e.draggedContext.futureIndex
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