<template>
    <v-row class="teacherTasks mx-0 mb-2 mb-sm-4" dense>
        <v-col class="imageWrap flex-grow-0">
            <div class="image"></div>
        </v-col>
        <v-col class="taskCompleted  pl-4" align-self="center">
            <div class="mb-1 completedTitle">{{taskCompleted}}</div>
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
                Book an unbording session
              </div>
            </div>
            <arrowRight class="arrowRight d-flex d-sm-none" />
            <v-btn class="taskAction d-none d-sm-flex" rounded outlined color="#4c59ff">book</v-btn>
        </v-col>
        <v-col cols="12" class="taskCol py-4 d-flex justify-space-between">
            <div class="d-flex align-center">
              <assignmentIcon class="assignIcon" />
              <div class="taskText pl-3">
                Connect your calendar
              </div>
            </div>
            <arrowRight class="arrowRight d-flex d-sm-none" />
            <v-btn class="taskAction d-none d-sm-flex" rounded outlined color="#4c59ff">Connect</v-btn>
        </v-col>
        <v-col cols="12" class="taskCol pb-0 py-4 d-flex justify-space-between">
            <div class="d-flex align-center">
              <assignmentIcon class="assignIcon" />
                <div class="taskText pl-3">
                  Set your work hours
                </div>
            </div>
            <arrowRight class="arrowRight d-flex d-sm-none" />
            <v-btn class="taskAction d-none d-sm-flex" rounded outlined color="#4c59ff">Update</v-btn>
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
    taskCompleted: '3/6 Task completed',
    value: 50
  })
}
</script>
<style lang="less">
@color: #43425d;

.teacherTasks {
  padding: 12px 16px;
  background: white;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;

  @media (max-width: 599px) {
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
      color: @color;
      font-weight: 600;
    }
  }
  .taskCol {
    border-bottom: 1px solid #dddddd;

    .taskText {
      color: @color;
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
