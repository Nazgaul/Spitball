<template>
    <div id="coursePage" class="coursePage ma-sm-8">
        <v-form ref="createCourse" @submit="saveCourseInfo" v-if="!isMobile">
            <courseCreate @saveCourseInfo="saveCourseInfo" />
            <div class="d-flex">
                <div class="courseLeftSide">
                    <courseInfo />
                    <div class="courseTeachingWrapper mb-6">
                        <courseTeaching v-for="n in numberOfLecture" :key="n" :index="n" />
                    </div>
                    <courseUpload />
                </div>
                <courseSticky />
            </div>
        </v-form>
        <unSupportedFeature v-else />
    </div>
</template>

<script>
import createCourse from '../../../store/createCourse';
import storeService from '../../../services/store/storeService';

import courseCreate from './courseCreate/courseCreate.vue';
import courseInfo from './courseInfo/courseInfo.vue';
import courseTeaching from './courseTeaching/courseTeaching.vue';
import courseUpload from './courseUpload/courseUpload.vue';
import courseSticky from './courseSticky/courseSticky.vue';

import unSupportedFeature from './unSupportedFeature.vue';

export default {
    components: {
      courseCreate,
      courseInfo,
      courseTeaching,
      courseUpload,
      courseSticky,

      unSupportedFeature
    },
    computed: {
        numberOfLecture() {
            return this.$store.getters.getNumberOfLecture
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        }
    },
    methods: {
        saveCourseInfo() {
            if(this.$refs.createCourse.validate()) {
                let files = this.$store.getters.getFileData
                let studyRoom = this.$store.getters.getTeachLecture

                // validate if tutor enter documents or studyroom
                if(!files.length && !studyRoom.length) return
                
                let documents = this.documentValidate(files)
                let studyRooms = this.documentValidate(studyRoom)
                
                this.$store.dispatch('updateCourseInfo', {documents, studyRooms}).then(res => {
                    console.log(res);
                }).catch(ex => {
                    console.error(ex);
                })
            }
        },
        documentValidate(files) {
            return files.map(file => {
                if(file.error) return Promise.reject('Error, file')

                return {
                    blobName: file.blobName,
                    name: file.name,
                    visible: file.visible || false
                }
            })
        },
        studyroomValidate(studyRoomList) {
            return studyRoomList.map(studyRoom => {
                let userChooseDate =  this.$moment(`${studyRoom.date}T${studyRoom.hour}:00`);         
                let isToday = userChooseDate.isSame(this.$moment(), 'day');
                if(isToday) {
                    let isValidDateToday = userChooseDate.isAfter(this.$moment().format())
                    if(!isValidDateToday) return Promise.reject('Error, date')
                }
                return {
                    name: studyRoom.text,
                    date: userChooseDate
                }
            })
        }
    },
    beforeDestroy(){
        storeService.unregisterModule(this.$store, 'createCourse');
    },
    created() {
        storeService.registerModule(this.$store, 'createCourse', createCourse);
    }
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';

.coursePage {
    @media (max-width: @screen-xs) {
        height: 100%;
    }    
    .courseLeftSide {
        min-width: 760px;
        max-width: 760px;

        .courseTeachingWrapper {
            background: #fff;
            border-radius: 6px;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        }
    }
    // .v-textarea, .v-input {
    //     .v-input__slot {
    //         fieldset {
    //             border: 1px solid #b8c0d1;
    //         }
    //         .v-label {
    //             color: @global-purple;
    //         }
    //     }
    //     &.error--text {
    //         fieldset {
    //             border: 2px solid #ff5252;
    //         }
    //     }
    // }
}
</style>