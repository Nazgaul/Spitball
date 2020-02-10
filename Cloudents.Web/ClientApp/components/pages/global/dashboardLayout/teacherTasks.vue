<template>
    <v-row class="teacherTasks mx-0 mb-2 mb-sm-4" dense>
        <v-col class="imageWrap flex-grow-0">
            <userAvatar :size="'60'" :user-name="user.name" :user-id="user.id" :userImageUrl="user.image"/> 
        </v-col>
        <v-col class="taskCompleted  pl-4" align-self="center">
            <div class="mb-1 completedTitle">{{taksNumberCompleted}} {{$t('dashboard_task_completed')}}</div>
            <v-progress-linear
              active
              :background-opacity="0.3"
              background-color="#dddddd"
              height="6"
              rounded
              :value="value"
              color="#5bbdb7"
            ></v-progress-linear>
        </v-col>
        <v-col cols="12" class="taskCol py-4 d-flex justify-space-between">
            <div class="d-flex align-center">
              <assignmentIcon class="assignIcon" />
              <div class="taskText pl-3">
                {{$t('dashboard_book_session')}}
              </div>
            </div>
            <arrowRight class="arrowRight d-flex d-sm-none" />
            <v-btn class="taskAction d-none d-sm-flex" rounded outlined color="#4c59ff">{{$t('dashboard_book_btn')}}</v-btn>
        </v-col>
        <v-col cols="12" class="taskCol py-4 d-flex justify-space-between">
            <div class="d-flex align-center">
              <assignmentIcon class="assignIcon" />
              <div class="taskText pl-3">
                {{$t('dashboard_connect_calendar')}}
              </div>
            </div>
            <arrowRight class="arrowRight d-flex d-sm-none" />
            <v-btn class="taskAction d-none d-sm-flex" rounded outlined color="#4c59ff">{{$t('dashboard_connect_btn')}}</v-btn>
        </v-col>
        <v-col cols="12" class="taskCol pb-0 py-4 d-flex justify-space-between">
            <div class="d-flex align-center">
              <assignmentIcon class="assignIcon" />
                <div class="taskText pl-3">
                  {{$t('dashboard_work_hours')}}
                </div>
            </div>
            <arrowRight class="arrowRight d-flex d-sm-none" />
            <v-btn class="taskAction d-none d-sm-flex" rounded outlined color="#4c59ff">{{$t('dashboard_works_btn')}}</v-btn>
        </v-col>
    </v-row>
</template>
<script>
import assignmentIcon from './images/assignment.svg';
import arrowRight from './images/arrow-right-copy-6.svg';

export default {
  name: "teacherTasks",
  components: {
    assignmentIcon,
    arrowRight
  },
  data: () => ({
    taksNumberCompleted: '3/6',
    value: 60
  }),
  computed: {
    user() {
      return this.$store.getters.accountUser ? this.$store.getters.accountUser : {}
    }
  }
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.teacherTasks {
  padding: 12px 16px;
  background: white;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;

  @media (max-width: @screen-xs) {
    box-shadow: none;
  }
  .imageWrap {
    .image {
      width: 60px;
      height: 60px;
      background: blue;
      border-radius: 50%;
    }
  }
  .taskCompleted {

    .completedTitle {
      color: @global-purple;
      font-weight: 600;
    }
  }
  .taskCol {
    border-bottom: 1px solid #dddddd;

    .taskText {
      color: @global-purple;
      font-weight: 600;
    }
    .taskAction {
      min-width: 120px;
      text-transform: initial;
      letter-spacing: normal;
      font-weight: 600;
    }
    .assignIcon {
      width: 20px;
    }
    .arrowRight {
      width: 10px;
    }
    &:last-child{
      border-bottom: none;
    }
  }
}
</style>
