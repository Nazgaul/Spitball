<template>
  <div class="courseUpload pa-5">
      <div class="courseUploadTitle mb-6" v-t="'add_content'"></div>
      <uploadMultipleFileStart />
        <template v-for="(file, index) in files">
            <fileCardError :singleFileIndex="index" :item="file" :key="file.id" />
        </template>
        <div class="uploadFilesTitle mb-6" v-t="'upload_files'" v-show="files.length"></div>
        <template v-for="(file, index) in files">
            <uploadFiles :singleFileIndex="index" :item="file" :key="index" />
        </template>
  </div>
</template>

<script>
import uploadMultipleFileStart from '../../../uploadFilesDialog/components/uploadMultipleFileStart.vue';
import fileCardError from '../../../uploadFilesDialog/components/fileCardError.vue';
import uploadFiles from '../uploadFiles/uploadFiles.vue';

export default {
    name: 'courseUpload',
    components: {
        uploadMultipleFileStart,
        fileCardError,
        uploadFiles,
    },
    data() {
        return {

        }
    },
    computed: {
        files() {
            return this.$store.getters.getFileData
        }
    },
    methods: {

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
        font-size: 20px;
        font-weight: 600;
        color: @global-purple;
    }
}
</style>