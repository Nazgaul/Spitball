<template>
    <div id="courseSessionSection" class="courseSession my-7" v-if="sessions.length">
        <div class="courseSessionTitle mb-3" v-t="'live_lecture'"></div>
        <div class="courseSessionWrapper">
            <div class="courseSessionRow mb-5 d-flex" v-for="(session, index) in sessions" :key="session.id">
                <div class="courseSessionLeft text-center white--text d-sm-block d-flex flex-column justify-center">
                    <div class="courseSessionDay">{{$moment(session.date).format('D')}}</div>
                    <div class="courseSessionMonth">{{$moment(session.date).format('MMM')}}</div>
                </div>
                <div class="courseSessionRight mx-4 my-auto py-2">
                    <div class="d-sm-flex mb-2">
                        <div class="courseSessionLecture flex-shrink-0">{{$t('live_le',[index+1])}}</div>
                        <div class="courseSessionDescription ms-sm-2">{{session.name}}</div>
                    </div>
                    <div class="d-flex">
                        <v-icon size="18">sbf-clockIcon</v-icon>
                        <div class="courseSessionTime ms-2">{{$moment(session.date).format('HH:mm')}}</div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'courseSession',
    computed: {
        sessions() {
            return this.$store.getters.getCourseSessionsPreview
        }
    }
}
</script>

<style lang="less">
@import '../../../styles/colors.less';
@import '../../../styles/mixin.less';
.courseSession {
    @media (max-width: @screen-xs) {
        margin: 0 16px !important;
    }
    .courseSessionTitle {
        font-size: 22px;
        font-weight: 600;
        color: @global-purple;
    }
    .courseSessionWrapper {
        max-width: 846px;
        .courseSessionRow {
            border: solid 1px #ddddd0;
            .courseSessionLeft {
                background: #41c4bc;
                padding: 8px 27px;
                @media (max-width: @screen-xs) {
                    padding: 6px 20px;
                }
                .courseSessionDay {
                    font-size: 29px;
                    font-weight: 600;
                }
                .courseSessionMonth {
                    font-size: 16px;
                    font-weight: 600;
                }
            }
            .courseSessionRight {
                .courseSessionLecture {
                    color: #41c4bc;
                    font-weight: 600;
                    font-size: 18px;
                }
                .courseSessionDescription {
                    font-size: 18px;
                    font-weight: 600;
                    color: #363637;
                }
            }
        }
    }
}
</style>