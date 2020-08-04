<template>
    <div id="coursePage" class="coursePage ma-sm-8">
        <v-form ref="createCourse" v-if="!isMobile">
            <courseCreate @saveCourseInfo="saveCourseInfo" />
            <div class="d-flex">
                <div class="courseLeftSide">
                    <courseInfo />
                    <div class="courseTeachingWrapper mb-6">
                        <courseTeaching v-for="n in numberOfLecture" :key="n" :index="n" />
                    </div>
                    <courseUpload />
                </div>
                <div class="courseRightSide ms-6">
                    <coursePublish />
                    <coursePromote />
                </div>
            </div>
        </v-form>
        <unSupportedFeature v-else />
        <v-snackbar
            v-model="showSnackbar"
            :timeout="6000"
            color="error"
            top
        >
            <div class="white--text text-center">{{errorText}}</div>
        </v-snackbar>
    </div>
</template>

<script>
import { MyCourses } from '../../../routes/routeNames'

import createCourse from '../../../store/createCourse';
import storeService from '../../../services/store/storeService';

import courseCreate from './courseCreate/courseCreate.vue';
import courseInfo from './courseInfo/courseInfo.vue';
import courseTeaching from './courseTeaching/courseTeaching.vue';
import courseUpload from './courseUpload/courseUpload.vue';
import coursePublish from './coursePublish/coursePublish.vue';
import coursePromote from './coursePromote/coursePromote.vue';

import unSupportedFeature from './unSupportedFeature.vue';

export default {
    components: {
        courseCreate,
        courseInfo,
        courseTeaching,
        courseUpload,
        coursePublish,
        coursePromote,

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
    data() {
        return {
            courseRoute: MyCourses,
            loading: false,
            showSnackbar: false,
            errorText: '',
            statusErrorCode: {
                date: this.$t('invalid_date'), // when there is invalid date
                studyRoomText: this.$t('invalid_studyroom_text'), // when there is no text in one of the studyroom
                file: this.$t('invalid_file'), // when there error in 1 of file
                409: this.$t('invalid_409'),
            }
        }
    },
    methods: {
        saveCourseInfo() {
            if(this.$refs.createCourse.validate()) {
                this.loading = true
                let files = this.$store.getters.getFileData
                let studyRoom = this.$store.getters.getTeachLecture
                let documents = this.documentValidate(files)
                let studyRooms = this.studyroomValidate(studyRoom)
                
                if(documents === false || studyRooms === false) {
                    this.showSnackbar = true
                    this.loading = false
                    return
                }

                if(!documents.length && !studyRooms.length) {
                    this.errorText = this.$t('required_files_or_studyroom')
                    this.showSnackbar = true
                    this.loading = false
                    return 
                }
                
                let self = this
                this.$store.dispatch('updateCourseInfo', {documents, studyRooms}).then(() => {
                    self.$router.push({name: MyCourses})
                }).catch(ex => {
                    self.errorText = this.statusErrorCode[ex.response.status]
                }).finally(() => {
                    self.loading = false
                })
            }
        },
        documentValidate(files) {
            if(!files.length) return []

            let i, filesArr = []
            for (i = 0; i < files.length; i++) {
                const file = files[i];
                if(file.error) {
                    this.errorText = this.statusErrorCode['file']
                    return false
                }
                filesArr.push({
                    blobName: file.blobName,
                    name: file.name,
                    visible: file.visible === undefined ? true : file.visible
                })
            }
            return filesArr
        },
        studyroomValidate(studyRoomList) {
            if(studyRoomList.length === 1 && !studyRoomList[0].text) {
                return []
            }

            let i, studyRoomArr = []
            for (i = 0; i < studyRoomList.length; i++) {
                const studyRoom = studyRoomList[i];
                let userChooseDate =  this.$moment(`${studyRoom.date}T${studyRoom.hour}:00`);
                let isToday = userChooseDate.isSame(this.$moment(), 'day');
                if(isToday) {
                    let isValidDateToday = userChooseDate.isAfter(this.$moment().format())
                    if(!isValidDateToday) {
                        this.errorText = this.statusErrorCode['date']
                        return false
                    }
                }

                if(!studyRoom.text) {
                    this.errorText = this.statusErrorCode['studyRoomText']
                    return false
                } 

                studyRoomArr.push({
                    name: studyRoom.text,
                    date: userChooseDate
                })
            }
            return studyRoomArr
        }
    },
    beforeDestroy(){
        this.$store.commit('resetCreateCourse')
        this.$store.commit('resetUploadFiles')
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
        max-width: 760px;
        min-width: 0;
        width: 100%;
        .courseTeachingWrapper {
            background: #fff;
            border-radius: 6px;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
        }
    }
    .courseRightSide {
        max-width: 296px;
        width: 100%;
        height: max-content;
        position: sticky;
        top: 170px;
    }

    // Shiran design colors for border input's
    .v-textarea, .v-input {
        .v-input__slot {
            fieldset {
                border: 1px solid #b8c0d1;
            }
            .v-label {
                color: @global-purple;
            }
        }
        &.error--text {
            fieldset {
                border: 2px solid #ff5252;
            }
        }
    }
    .v-input--is-focused:not(.error--text) {
        fieldset {
            border: 2px solid #304FFE !important;
        }
    }
}
</style>