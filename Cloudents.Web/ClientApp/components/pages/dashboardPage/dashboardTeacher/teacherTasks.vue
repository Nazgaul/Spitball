<template>
    <v-row class="teacherTasks mx-md-0 mb-2 mb-sm-4" dense>
        <v-col class="imageWrap flex-grow-0 pa-0">
            <userAvatar :size="'60'" :user-name="user.name" :user-id="user.id" :userImageUrl="user.image"/> 
        </v-col>
        <v-col class="taskCompleted  pl-4" align-self="center">
            <template v-if="taksNumberCompleted !== totalTasks">
              <div class="hasTask">
                <div class="mb-1 completedTitle">{{taksNumberCompleted}}/{{totalTasks}} {{$t('dashboardTeacher_task_completed')}}</div>
                <v-progress-linear
                  active
                  :background-opacity="0.3"
                  background-color="#dddddd"
                  height="6"
                  rounded
                  :value="value"
                  color="#5bbdb7"
                ></v-progress-linear>
              </div>
            </template>
            <template v-else>
              <div class="noTask">
                <div class="completedTitle">{{$t('dashboardTeacher_no_task')}}</div>
                <checkCircle />
              </div>
            </template>
        </v-col>
        <v-col cols="12" class="taskCol py-4 d-flex justify-space-between px-0 mt-3" v-if="!bookedSession">
            <div class="d-flex align-center">
              <assignmentIcon class="assignIcon" />
              <div class="taskText pl-3">
                {{$t('dashboardTeacher_book_session')}}
              </div>
            </div>
            <arrowRight class="arrowRight d-flex d-sm-none" @click="bookSession" />
            <v-btn class="taskAction d-none d-sm-flex" @click="bookSession" rounded outlined color="#4c59ff">{{$t('dashboardTeacher_book_btn')}}</v-btn>
        </v-col>
        <v-col cols="12" class="taskCol py-4 d-flex justify-space-between px-0" v-if="!calendarShared">
            <div class="d-flex align-center">
              <assignmentIcon class="assignIcon" />
              <div class="taskText pl-3">
                {{$t('dashboardTeacher_connect_calendar')}}
              </div>
            </div>
            <router-link :to="{name: 'myCalendar'}"><arrowRight class="arrowRight d-flex d-sm-none" /></router-link>
            <v-btn class="taskAction d-none d-sm-flex" :to="{name: 'myCalendar'}" :loading="btnLoading" rounded outlined color="#4c59ff">{{$t('dashboardTeacher_connect_btn')}}</v-btn>
        </v-col>
        <v-col cols="12" class="taskCol pb-0 py-4 d-flex justify-space-between px-0" v-if="!haveHours">
            <div class="d-flex align-center">
              <assignmentIcon class="assignIcon" />
                <div class="taskText pl-3">
                  {{$t('dashboardTeacher_work_hours')}}
                </div>
            </div>
            <router-link :to="{name: 'myCalendar'}"><arrowRight class="arrowRight d-flex d-sm-none" /></router-link>
            <v-btn class="taskAction d-none d-sm-flex" :to="{name: 'myCalendar'}" rounded outlined color="#4c59ff">{{$t('dashboardTeacher_works_btn')}}</v-btn>
        </v-col>
    </v-row>
</template>

<script>
import assignmentIcon from './images/assignment.svg';
import arrowRight from './images/arrow-right-copy-6.svg';
import checkCircle from './images/check-circle.svg';

export default {
  name: "teacherTasks",
  components: {
    assignmentIcon,
    arrowRight,
    checkCircle
  },
  data: () => ({
    totalTasks: 6,
    taksNumberCompleted: 3,
    value: 40,
    btnLoading: false,
    calendarShared: false,
    haveHours: false,
    bookedSession: false,
  }),
  computed: {
    user() {
      return this.$store.getters.accountUser ? this.$store.getters.accountUser : {}
    },
  },
  methods: {
    bookSession() {
      //TODO: fix convention
      let id, name;
      let country = global.country;
      if(country === "IL") {
        name = 'יניב-מדריך';
        id = "456373";
      } else if(country === "IN") {
        name = 'Bhaskar Patel';
        id = "461552";
      } else {
        id = "488449";
        name = 'David Hughes';
      }
      this.$router.push({name: 'profile', params: {id, name, openCalendar: true}})
    },
    getTutorActions() {
      let self = this;
      this.$store.dispatch('updateTutorActions').then(res => {
        this.calendarShared = res.calendarShared;
        this.haveHours = res.haveHours;
        this.bookedSession = res.bookedSession;
        this.updateTaksNumberCompleted()
      }).catch(ex => {
        self.$appInsights.trackException({exception: new Error(ex)});
      })
    },
    updateTaksNumberCompleted() {
      if(this.calendarShared) {
        this.taksNumberCompleted++;
        this.value += 20
      }
      if(this.haveHours) {
        this.taksNumberCompleted++;
        this.value += 20
      }
      if(this.bookedSession) {
        this.taksNumberCompleted++;
        this.value += 20
      }
      
    }
  },
  created() {
    this.getTutorActions()
  }
}
</script>
<style lang="less">
@import '../../../../styles/mixin.less';
@import '../../../../styles/colors.less';

.teacherTasks {
  padding: 16px;
  background: white;
  box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
  border-radius: 8px;

  @media (max-width: @screen-xs) {
    box-shadow: none;
    border-radius: 0;
  }
  .taskCompleted {
    .noTask {
      display: flex;
      align-items: center;
      justify-content: space-between;
    }
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
      transform: none /*rtl:scaleX(-1)*/;
      width: 10px;
    }
    &:last-child{
      border-bottom: none;
    }
  }
}
</style>
