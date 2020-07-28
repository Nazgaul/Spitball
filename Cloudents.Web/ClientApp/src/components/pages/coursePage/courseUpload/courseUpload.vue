<template>
  <div class="courseUpload pa-5">
      <div class="courseUploadTitle mb-6" v-t="'add_content'"></div>
      <uploadMultipleFileStart />
        <template v-for="(file, index) in files">
            <fileCardError :singleFileIndex="index" :item="file" :key="file.id" />
        </template>
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
//         .uf-close {
//         position: absolute;
//         right: 10px;
//         top: 10px;
//         z-index: 99;
//         font-size: 12px;
//         cursor: pointer;
//         color: #adadba;
//         @media (max-width: @screen-xs) {
//             right: 14px;
//             top: 14px;
//         }
//     }

//     .uf-main {
//         height: 100%;
//         @media (max-width: @screen-xs) {
//             display: flex;
//             flex-direction: column;
//             justify-content: space-between;
//             align-items: center;
//         }
//    .uf-sEdit-bottm{
//         display: flex;
//         flex-direction: column;
//         align-items: center;

//         .v-btn{
//             min-width: 150px;
//             height: 40px !important;
//             margin-left: 0;
//             margin-right: 0;
//         }
//         .uf-sEdit-bottm-btn{
//             margin: 6px 8px;
//             color: white;
//             border: 1px solid @global-blue !important;
//             font-size: 14px;
//             font-weight: 600;
//             letter-spacing: -0.26px;
//         }
//        .uf-sEdit-terms{
//            @media (max-width: @screen-xs) {
//             padding: 0 40px;
//             text-align: center;
//             line-height: 1.83;
//            }
//             font-size: 12px;
//             font-weight: 600;
//             color: @global-purple;
//             .uf-sEdit-terms-link{
//                 color: @global-blue;
//                 cursor: pointer;
//             }
//        }
//    }
//         .uf-mStepper {
//             @media (max-width: @screen-xs) {
//                 height: 100%;
//             }
//             .uf-mHeader {
//                 display: flex;
//                 align-items: center;
//                 justify-content: flex-start;
//                 @media (max-width: @screen-xs) {
//                     margin: 0;
//                     display: block;
//                     text-align: center;
//                 }

//                 .uf-mImg {
//                     border-radius: 50%;
//                     width: 40px;
//                     height: 40px;
//                     object-fit: contain;
//                 }

//                 .uf-mTitle {
//                     font-size: 18px;
//                     font-weight: 600;
//                     color: @global-purple;
//                     @media (max-width: @screen-xs) {
//                         letter-spacing: 0.34px;
//                         font-size: 16px;
//                     }
//                 }
//             }

//             .uf-items {
//                 @media (max-width: @screen-xs) {
//                     height: calc(~"100% - 42px");
//                     min-height: calc(~"100% - 42px");
//                     max-height: calc(~"100% - 42px");
//                     margin-top: -30px;
//                 }
//                 .uf-mStepper-content {
//                     .v-stepper__wrapper {
//                         @media (max-width: @screen-xs) {
//                             height: 100%;
//                         }
//                     }
//                     @media (max-width: @screen-xs) {
//                         height: 100%;
//                     }
//                     padding: 0 !important;
//                 }
//                 .sbf-menu-down {
//                     margin-top: 8px; //global for all inputs arrow causing input custom height
//                     font-size: 30px;
//                 }
//             }
//         }
//     }
}
</style>