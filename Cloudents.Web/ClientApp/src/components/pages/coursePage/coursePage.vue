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
            :color="snackObj.color"
            top
        >
            <div class="white--text">{{snackObj.text}}</div>
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
            loading: false,
            showSnackbar: false,
            snackObj: {
                color: '',
                text: ''
            },
            statusErrorCode: {
                date: this.$t('invalid_date'),
                studyRoomtext: this.$t('invalid_studyroom_text'),
                file: this.$t('invalid_file'),
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
                // validate for error or both empty
                if(documents === false && studyRooms === false) {
                    this.showSnackbar = true
                    this.snackObj.color = 'error'
                    return
                }
                this.$store.dispatch('updateCourseInfo', {documents, studyRooms}).then(() => {
                    this.$router.push({name: MyCourses})
                }).catch(ex => {
                    this.snackObj.text = this.statusErrorCode[ex.response.status]
                    this.snackObj.color = 'error'
                }).finally(() => {
                    this.showSnackbar = true
                    this.loading = false
                })
            }
        },
        documentValidate(files) {
            if(!files.length) return false

            let i, filesArr = []
            for (i = 0; i < files.length; i++) {
                const file = files[i];

                if(file.error) {
                    this.snackObj.text = this.statusErrorCode['file']
                    return false
                }

                filesArr.push({
                    blobName: file.blobName,
                    name: file.name,
                    visible: file.visible || false
                })
            }
            return filesArr
        },
        studyroomValidate(studyRoomList) {
            if(!studyRoomList.length) return false

            let i, studyRoomArr = []
            for (i = 0; i < studyRoomList.length; i++) {
                const studyRoom = studyRoomList[i];
                let userChooseDate =  this.$moment(`${studyRoom.date}T${studyRoom.hour}:00`);
                let isToday = userChooseDate.isSame(this.$moment(), 'day');
                if(isToday) {
                    let isValidDateToday = userChooseDate.isAfter(this.$moment().format())
                    if(!isValidDateToday) {
                        this.snackObj.text = this.statusErrorCode['date']
                        return false
                    }
                }

                if(!studyRoom.text) {
                    this.snackObj.text = this.statusErrorCode['studyRoomtext']
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
}
</style>