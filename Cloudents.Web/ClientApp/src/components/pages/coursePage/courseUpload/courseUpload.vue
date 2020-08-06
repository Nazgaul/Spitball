<template>
    <div class="courseUpload pa-5">
        <div class="courseUploadTitle mb-6" v-t="'add_content'"></div>
        <fileCardError v-for="(file, index) in fileErrorList" :singleFileIndex="index" :item="file" :key="file.id" />

        <template v-if="showFiles">
            <div class="uploadFilesTitle mb-6 mt-4" v-t="'upload_files'" v-show="files.length"></div>
                <draggable
                    :list="files"
                    class="list-group"
                    v-bind="dragOptions"
                    :move="checkMove"
                    @start="dragging = true"
                    @end="dragging = false"
                >
                    <transition-group type="transition" name="flip-list">
                        <uploadFiles v-for="(file, index) in files" :singleFileIndex="index" :item="file" :key="index" />
                    </transition-group>
                </draggable>
        </template>
        <uploadMultipleFileStart class="mt-4" />
    </div>
</template>

<script>
import uploadMultipleFileStart from './uploadMultipleFileStart.vue';
import fileCardError from './fileCardError.vue';
import uploadFiles from '../uploadFiles/uploadFiles.vue';
import draggable from "vuedraggable";

export default {
    name: 'courseUpload',
    components: {
        uploadMultipleFileStart,
        fileCardError,
        uploadFiles,
        draggable
    },
    date() {
        return {
            dragging: false
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
        showFiles: {
            get() {
                return this.$store.getters.getShowFiles
            },
            set(val) {
                this.$store.commit('setShowFiles', val)
            }
        },
        fileErrorList() {
            return this.files.filter(f => f.error)
        },
        files() {
            return this.$store.getters.getFileData
        }
    },
    methods: {
        checkMove: function(e) {
            let {futureIndex, index} = e.draggedContext
            const movedItem = this.files.slice(index, 1)[0]
            this.files.slice(futureIndex, 0, movedItem)
        }
    }
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.courseUpload {
    background: #fff;
    border-radius: 6px;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    max-width: 760px;

    .courseUploadTitle {
        font-size: 20px;
        font-weight: 600;
        color: @global-purple;
    }
    .uploadFilesTitle {
        font-size: 14px;
        font-weight: 600;
        color: @global-purple;
    }
    .list-group {
        cursor: move;
    }
}
</style>