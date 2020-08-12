<template>
    <div id="coursePage" class="coursePage ma-sm-8">
        <v-stepper v-if="!isMobile" class="courseStepper elevation-0">
            <v-form ref="createCourse">
                <div class="courseActionsSticky">
                    <v-stepper-header class="courseStepHeader elevation-0">
                        <v-stepper-step class="stepStteper ps-8" :class="[step === 1 ? 'active' : 'noActive']" step="1">
                            {{$t('create_course')}}
                        </v-stepper-step>
                        <v-divider></v-divider>
                        <v-stepper-step class="stepStteper- ps-8" :class="[step === 2 ? 'active' : 'noActive']" step="2">
                            {{$t('eedit_page')}}
                        </v-stepper-step>
                        <v-divider></v-divider>
                        <v-stepper-step class="" :class="[{'stepStteper': courseVisible},step === 3 ? 'active' : 'noActive']" step="3">
                            {{$t('promote_course')}}
                        </v-stepper-step>
                    </v-stepper-header>
                    <courseCreate :courseRoute="courseRoute" @saveCourseInfo="saveCourseInfo" />
                </div>

                <v-stepper-items class="stepperItems">
                    <v-stepper-content :step="step" class="pa-0">
                        <component
                            :is="stepComponent"
                            ref="childComponent"
                            :currentCreatedCourseId="currentCreatedCourseId"
                        >
                        </component>
                    </v-stepper-content>
                </v-stepper-items>
            </v-form>
        </v-stepper>
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

import storeService from '../../../services/store/storeService';
import couponStore from '../../../store/couponStore';

import courseCreate from './courseCreate/courseCreate.vue';
import courseForm from './courseForm/courseForm.vue';
import courseShare from './courseShare/courseShare.vue';
import unSupportedFeature from './unSupportedFeature.vue';

export default {
    components: {
        courseCreate,
        courseForm,
        courseShare,
        unSupportedFeature
    },
    computed: {
        canCreateCourse() {
            return this.$store.getters.getIsCanCreateCourse
        },
        courseVisible() {
            return this.$store.getters.getCourseVisible
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        }
    },
    data() {
        return {
            step: 1,
            stepComponent: 'courseForm',
            saveMethodsName: {
                create: 'createCourseInfo',
                update: 'updateCourseInfo'
            },
            currentCreatedCourseId: null,
            courseRoute: MyCourses,
            loading: false,
            showSnackbar: false,
            errorText: '',
            statusErrorCode: {
                date: this.$t('invalid_date'), // when there is invalid date
                duplicateDate: this.$t('invalid_duplicate_date'),
                studyRoomText: this.$t('invalid_studyroom_text'), // when there is no text in one of the studyroom
                file: this.$t('invalid_file'), // when there error in 1 of file
                uploadFileNotFinished: this.$t('invalid_file_upload'), // when big file not finished upload chunks
                409: this.$t('invalid_409'),
            }
        }
    },
    methods: {
        saveCourseInfo() {
            if(this.step === 3) {
                this.$router.push({name: MyCourses})
                return
            }
            let form = this.$refs.createCourse
            if(form.validate()) {
                this.loading = true
                let files = this.$store.getters.getFileData
                let studyRoom = this.$store.getters.getTeachLecture
                let documentsValidation = this.documentValidate(files)
                let studyRooms = this.studyroomValidate(studyRoom)
                
                if(documentsValidation === 1 || studyRooms === 1) {
                    this.showSnackbar = true
                    this.loading = false
                    return
                }

                if(documentsValidation === 0 && studyRooms === 0) {
                    this.errorText = this.$t('required_files_or_studyroom')
                    this.showSnackbar = true
                    this.loading = false
                    this.goTo('courseUpload')
                    return 
                }

                studyRooms = studyRooms === 0 ? [] : studyRooms
                let documents = this.documentMap(files);

                let id = this.$route.params.id ? this.$route.params.id : undefined
                let methodName = id ? 'update' : 'create'
                let self = this
                this.$store.dispatch(this.saveMethodsName[methodName], {documents, studyRooms, id}).then(({data}) => {
                    if(self.courseVisible) {
                        self.currentCreatedCourseId = data?.id || id
                        self.goStep(3)
                        return
                    }
                    self.$router.push({name: MyCourses})
                }).catch(ex => {
                    if(ex.response) {
                        if(!ex.response.data) {
                            self.errorText = self.$t('profile_enroll_error')
                        }
                        if(ex.response.data) {
                            self.errorText = ex.response.data[Object.keys(ex.response.data)[0]][0]
                        }
                        if(ex.response.status === 409) {
                            self.errorText = this.statusErrorCode[ex.response.status]
                        }
                        self.showSnackbar = true
                    }
                }).finally(() => {
                    self.loading = false
                })
            } else {
                // debugger
                // let elemRef = this.$refs.createCourse
                // for (let i = 0; i < elemRef.inputs.length; i++) {
                //     const element = elemRef.inputs[i];
                //     if(element.errorBucket.length > 0) {
                //        this.$vuetify.goTo(element) 
                //     }
                // }
                this.goTo('courseInfo')
            }
        },
        documentValidate(files) {
            if(!files.length) return 0

            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                if(file.error) {
                    this.errorText = this.statusErrorCode['file']
                    return 1
                }
                if(file.progress < 100) {
                    this.errorText = this.statusErrorCode['uploadFileNotFinished']
                    return 1
                }
               
            }
            return 2;
        },
        documentMap(files) {
           let filesArr = []
            for (let i = 0; i < files.length; i++) {
                const file = files[i];
                filesArr.push({
                    blobName: file.blobName,
                    name: file.name,
                    visible: file.visible === undefined ? true : file.visible,
                    id: isNaN(file.id) ? undefined : file.id 
                })
            }
            return filesArr
        },
        studyroomValidate(studyRoomList) {
            if(studyRoomList.length === 1 && !studyRoomList[0].text) {
                return 0
            }

            let i, studyRoomArr = []
            for (i = 0; i < studyRoomList.length; i++) {
                const studyRoom = studyRoomList[i];
                let userChooseDate =  this.$moment(`${studyRoom.date}T${studyRoom.hour}:00`);
                if(!this.$route.params.id) {
                    let validateDuplicateSessionTime = studyRoomArr.filter((s) => {
                        return s.date.isSame(userChooseDate)
                    })
                    if(validateDuplicateSessionTime.length) {
                        this.errorText = this.statusErrorCode['duplicateDate']
                        return 1
                    }
                }
                let isToday = userChooseDate.isSame(this.$moment(), 'day');
                if(isToday) {
                    let isValidDateToday = userChooseDate.isAfter(this.$moment().format())
                    if(!isValidDateToday) {
                        this.errorText = this.statusErrorCode['date']
                        return 1
                    }
                }

                if(!studyRoom.text) {
                    this.errorText = this.statusErrorCode['studyRoomText']
                    return 1
                } 

                studyRoomArr.push({
                    name: studyRoom.text,
                    date: userChooseDate
                })
            }
            return studyRoomArr
        },
        goTo(ref) {
            let options = {
                offset: ref === 'courseInfo' ? '40': null,
            }
            this.$vuetify.goTo(this.$refs.childComponent, options)
        },
        goStep(step) {
            this.step = step
            if(step === 1) {
                this.stepComponent = 'courseForm'
            } else if(step === 3) {
                this.stepComponent = 'courseShare'
            }
        },
        showCourseNotVisible() {
            let isCourseVisible = this.canCreateCourse
            if(!isCourseVisible) {
                this.$store.commit('setShowCourse', false)
            }
        }
    },
    beforeDestroy(){
        this.$store.commit('resetCreateCourse')
        this.$store.commit('resetUploadFiles')
        this.$store.commit('resetCoupon')
        storeService.unregisterModule(this.$store, 'couponStore');
    },
    mounted() {
        this.showCourseNotVisible()
    },
    created() {
      storeService.registerModule(this.$store, 'couponStore', couponStore);
      let id = this.$route.params.id
      if(id) {
        this.$store.dispatch('getCourseInfo', id)
      }
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';

.coursePage {
    max-width: 1077px; // eidan request
    @media (max-width: @screen-xs) {
        height: 100%;
    }
    .courseStepper {
        overflow: visible; 
        background: inherit;
        .courseActionsSticky {
            position: sticky;
            z-index: 9;
            top: 70px;
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
            .courseStepHeader {
                background: #fff;
                .stepStteper {
                    &.noActive {
                        // cursor: pointer;
                        .v-stepper__step__step {
                            background: transparent !important;
                            border: 2px solid #4452fc !important;


                            font-weight: 600;
                            color: #4c59ff !important;
                        }
                            .v-stepper__label {
                                text-shadow: none;
                            }
                    }
                    &.active {
                        .v-stepper__step__step {
                            background: -webkit-linear-gradient(53deg, #4452fc 27%, #3892e4 115%) !important;
                            background: linear-gradient(53deg, #4452fc 27%, #3892e4 115%) !important;
                        }
                        .v-stepper__label {
                            text-shadow: 0 0 0 black;
                        }
                    }
                }
            }
        }
        .stepperItems, .v-stepper__wrapper {
            overflow: visible;
        }
    }
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