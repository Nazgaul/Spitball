<template>
  <div class="scheduledClasses">
    <unSupportedFeature v-if="isMobile"/>
    <div v-else class="scheduledWrapper">
      <div class="scheduled_title pb-10">{{$t('upcoming_classes')}}</div>
      <div class="calendarWrapper">
          <v-sheet height="64">
            <v-toolbar flat color="white">
              <v-btn outlined class="mr-4" color="grey darken-2" @click="setToday">{{$moment().calendar().split(' ')[0]}}</v-btn>
              <v-btn fab text small color="grey darken-2" @click="prev"><v-icon small v-text="isRtl?'sbf-arrow-right-carousel':'sbf-arrow-left-carousel'"/></v-btn>
              <v-btn fab text small color="grey darken-2" @click="next"><v-icon small v-text="isRtl?'sbf-arrow-left-carousel':'sbf-arrow-right-carousel'"/></v-btn>
              <v-toolbar-title v-if="$refs.scheduledCalendar">{{ $refs.scheduledCalendar.title }}</v-toolbar-title>
              <v-spacer></v-spacer>

              <v-menu bottom right>
                <template v-slot:activator="{ on, attrs }">
                  <v-btn outlined color="grey darken-2" v-bind="attrs" v-on="on">
                    <span>{{ typeToLabel[type] }}</span>
                    <v-icon right v-text="'sbf-arrow-down'"/>
                  </v-btn>
                </template>
                <v-list>
                  <v-list-item @click="type = 'day'" v-text="typeToLabel.day"/>
                  <v-list-item @click="type = 'week'" v-text="typeToLabel.week"/>
                  <v-list-item @click="type = 'month'" v-text="typeToLabel.month"/>
                  <v-list-item @click="type = '4day'" v-text="typeToLabel['4day']"/>
                </v-list>
              </v-menu>
            </v-toolbar>
          </v-sheet>

        <v-sheet height="600">
            <v-calendar
              ref="scheduledCalendar"
              color="primary"
              v-model="focus"
              :events="events"
              :event-color="getEventColor"
              :type="type"
              @click:event="showEvent"
              @click:more="viewDay"
              @click:date="viewDay"
              @change="updateRange"
              :locale="locale">

                <template #event="{ event }">
                  <div class="text-truncate px-1">
                    <span class="font-weight-bold">{{$moment(event.date).format('HH:mm')}}</span>
                    <span>{{event.name}}</span>
                  </div>
                </template>
                
              </v-calendar>

          <v-menu v-model="selectedOpen" :close-on-content-click="false" :activator="selectedElement" offset-x max-width="360px">
            <classCard :selectedClass="selectedClass" @closeClassCard="selectedOpen = false"></classCard>
          </v-menu>
        </v-sheet>
      </div>
    </div>
  </div>
</template>

<script>
  import classCard from './classCard.vue';
import unSupportedFeature from '../../coursePage/unSupportedFeature.vue';
  export default {
    name:'scheduledClasses',
    components:{classCard,unSupportedFeature},
    data() {
      return {
        locale: `${global.lang}-${global.country}`,
        isRtl:global.isRtl,
        focus: '',
        type: 'month',
        typeToLabel: {
          month: this.$t('month'),
          week: this.$t('week'),
          day: this.$t('day'),
          '4day': this.$t('4days'),
        },
        selectedClass: {},
        selectedElement: null,
        selectedOpen: false,
        events: [],
      }
    },
    watch: {
      scheduledClassesList(val){
        this.events = val;
        this.$refs.scheduledCalendar.checkChange();
      }
    },
    computed: {
      scheduledClassesList(){
        let classes = this.$store.getters.getScheduledClasses;
        let coursesIdxs = [...new Set(classes.map(c=>c.courseId))];
        let colors = this.getColors(coursesIdxs.length);
        return classes.map( c => {
          return {
            courseId: c.courseId,
            courseName: c.courseName,
            studentEnroll: c.studentEnroll,
            date: c.broadcastTime,
            name: c.studyRoomName || '',
            id: c.studyRoomId,
            start: this.$moment(c.broadcastTime).format('YYYY-MM-DD HH:mm'),
            end: this.$moment(c.broadcastTime).add(1, 'hours').format('YYYY-MM-DD HH:mm'),
            color: this.$moment(c.broadcastTime).isBefore()? 'grey' : colors[coursesIdxs.findIndex(i=> i===c.courseId)]
          }
        })
      },
      isMobile() {
        return this.$vuetify.breakpoint.xsOnly
      }
    },
    methods: {
      viewDay ({ date }) {
        this.focus = date
        this.type = 'day'
      },
      getEventColor (event) {
        return event.color
      },
      setToday () {
        this.focus = ''
      },
      prev () {
        this.$refs.scheduledCalendar.prev()
      },
      next () {
        this.$refs.scheduledCalendar.next()
      },
      showEvent ({ nativeEvent, event }) {
        const open = () => {
          this.selectedClass = event
          this.selectedElement = nativeEvent.target
          setTimeout(() => this.selectedOpen = true, 10)
        }

        if (this.selectedOpen) {
          this.selectedOpen = false
          setTimeout(open, 10)
        } else {
          open()
        }

        nativeEvent.stopPropagation()
      },
      updateRange () {
        this.events = this.scheduledClassesList
      },
      getColors(count){
        let colors = ['#4c59ff', '#41c4bc', '#4094ff', '#ff6f30', '#ebbc18', '#69687d', 
          '#1b2441','#5833cf', '#4daf50', '#995bea', '#074b8f', '#860941', '#757575', '#317ca0'] // 14;
        
        while (colors.length < count){
          let color = '#'+Math.floor(Math.random()*16777215).toString(16).padStart(6, '0');
          if(!colors.includes(color)){
              colors.push(color)
          }
        }
        return colors
      }
    },
    created() {
      if(!this.isMobile){
        this.$store.dispatch('updateScheduledClasses')
      }
    },
  }
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

.scheduledClasses {
  padding: 30px;
  max-width: 1334px;
  @media (max-width: @screen-xs) {
    padding: 30px 0;
    width: 100%;
    height: 100%;
  }
  .scheduledWrapper{
    padding: 16px 22px;
    border-radius: 8px;
    box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
    background-color: #ffffff;
    .scheduled_title {
      font-size: 22px;
      font-weight: 600;
    }
    .calendarWrapper{
       
    }
  }

}
</style>