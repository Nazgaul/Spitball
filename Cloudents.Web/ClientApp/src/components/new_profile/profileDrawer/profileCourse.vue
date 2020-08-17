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
                <div v-for="(course, index) in courses" :key="index" class="courseSessionsDragWrap mb-6">
                    <div class="mb-3">{{course.name}}</div>
                    <img :src="$proccessImageUrl(course.image, 200, 130)" width="200" height="130"/>
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