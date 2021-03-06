<template>
  <v-layout class="calendar-container">
    <v-flex :class="{'sheet-loading':!isReady}">
      <div class="calendar-header">
        <h2 v-t="'calendar_header'"></h2>
        <h3 v-t="'calendar_title'"></h3>
        <div class="calendar-header-time">
          <Schedule />
          <div v-t="'calendar_time'"></div>
        </div>
      </div>
      <div class="navigation-btns-calendar">
        <v-btn
          sel="calendar_previous"
          :disabled="isGoPrev"
          small
          class="white--text elevation-0"
          color="#4452fc"
          @click="$refs.calendar.prev()"
        >
          <v-icon>sbf-arrow-left-carousel</v-icon>
        </v-btn>
        <span class="title-calendar">{{calendarMonth}}</span>
        <v-btn
          sel="calendar_next"
          :disabled="isGoNext"
          small
          class="white--text elevation-0"
          color="#4452fc"
          @click="$refs.calendar.next()"
        >
          <v-icon dark>sbf-arrow-right-carousel</v-icon>
        </v-btn>
      </div>
      <v-sheet>
        <v-card v-if="addEventDialog" class="addEventDialog" id="addEventDialog">
          <div
            class="event-dialog-title"
            :class="[{'event-dialog-title-send':!isEventSent,'event-dialog-title-done':isEventSent}]"
          >
            <span
              v-if="isNeedPayment && !isEventSent"
              v-t="'calendar_add_event_payment'"
            />
            <span
              v-if="!isEventSent && !isNeedPayment">{{$t('calendar_add_event_title',[tutorName])}}</span>
            <span
              v-if="isEventSent && !isNeedPayment"
              v-t="'calendar_add_event_thank'"
            />
          </div>

          <div
            :class="['event-dialog-body',
            {'event-dialog-body-payment':isNeedPayment,'event-dialog-body-send':!isEventSent && !isNeedPayment,'event-dialog-body-done':isEventSent && !isNeedPayment}]"
          >
            <img v-if="isNeedPayment && !isEventSent" src="./images/group-2.png" alt="group-2.png" />
            <div v-if="!isEventSent && !isNeedPayment">
              <p>{{formatDateString()}}</p>
              <p dir="ltr">{{formatTimeString()}}</p>
            </div>
            <span
              v-if="isEventSent && !isNeedPayment"
              v-t="'calendar_add_event_sent'"
            />
          </div>

          <div class="event-dialog-action">
            <v-btn
              :color="!isEventSent? 'white':'#4452fc'"
              @click="closeDialog"
              depressed
              rounded
              :class="[!isEventSent? 'cncl-btn': 'donebtn']"
            >
              <span>{{addEventText}}</span>
            </v-btn>

            <v-btn
              v-if="!isEventSent"
              @click="isNeedPayment? goPayment() : insertNewEvent()"
              :loading="isLoading"
              depressed
              rounded
              color="#4452fc"
              class="calbtn"
            >
              <span>{{needPaymentText}}</span>
            </v-btn>
          </div>
        </v-card>

        <v-calendar
          v-model="today"
          :start="today"
          type="week"
          :locale="calendarLocale"
          ref="calendar"
          :weekday-format="format"
          :first-interval="intervals.first"
          :interval-minutes="intervals.minutes"
          :interval-count="intervals.count"
          :interval-height="intervals.height"
        >
          <template v-slot:day-body="{ date, hour, timeToY, minutesToPixels }">
            <template v-for="event in eventsMap[date]">
              <div
                v-if="event.time"
                :key="event.time+2"
                class="my-event with-time"
                v-html="event.time"
                :style="{ top: timeToY(event.time) + 'px', height: minutesToPixels(60) + 'px' }"
              ></div>
            </template>
          </template>

          <template v-slot:interval="{date,time,past}">
            <div
              :class="['my-event',checkDateCell(past,date, time)? 'without-time-past':'without-time', {'cursor-none': isSelfTutor},{'selectedEvent': isSelected(date,time)}]"
            >
              <button @click="addEvent($event, date,time)" v-html="cellTime(date,time)"></button>
            </div>
          </template>
        </v-calendar>
      </v-sheet>
    </v-flex>
  </v-layout>
</template>

<script>
import { mapGetters, mapActions } from "vuex";
import Schedule from "./images/schedule.svg";
import * as componentConsts from '../pages/global/toasterInjection/componentConsts.js';
export default {
  components: {
    Schedule
  },
  data() {
    return {
      currentMonth: new Date().toLocaleString(
        `${global.lang}-${global.country}`,
        { month: "short" }
      ),
      isReady: false,
      isLoading: false,
      addEventDialog: false,
      isEventSent: false,
      selectedDate: "",
      selectedTime: "",
      today: new Date().FormatDateToString(),
      intervals: {
        first: this.getIntervalFirst || 8,
        minutes: 60,
        count: 16,
        height: 36
      }
    };
  },
  computed: {
    ...mapGetters([
      "getIntervalFirst",
      "getCalendarType",
      "getCalendarEvents",
      "getProfile",
      "accountUser",
      "getNeedPayment"
    ]),
    addEventText() {
      if(!this.isEventSent){
        return this.$t('calendar_add_event_cancel')
      }
      return this.$t('calendar_add_event_ok')
    },
    needPaymentText() {
      if(this.isNeedPayment){
        return this.$t('calendar_add_event_ok')
      }
      return this.$t('calendar_add_event_btn')
    },
    tutorName() {
      return this.getProfile.user.firstName.replace(/(?:^|\s)\S/g, function(a) {
        return a.toUpperCase();
      });
    },
    // calendarType() {
    //   return this.getCalendarType;
    // },
    calendarLocale() {
      return `${global.lang}-${global.country}`.toLowerCase();
    },
    calendarEvents() {
      return this.getCalendarEvents;
    },
    eventsMap() {
      const map = {};
      this.calendarEvents.forEach(e =>
        (map[e.date] = map[e.date] || []).push(e)
      );
      return map;
    },
    calendarMonth() {
      if (this.isReady && this.$refs.calendar) {
        let timeStamp = this.$refs.calendar.parsedStart;
        let startWeekDateMonth = this.$refs.calendar.getStartOfWeek(timeStamp);
        let endWeekDateMonth = this.$refs.calendar.getEndOfWeek(timeStamp);

        let date = new Date(startWeekDateMonth.date);
        let dateNext = new Date(endWeekDateMonth.date);
        let year = date.getFullYear();
        let month = date.toLocaleString(`${global.lang}-${global.country}`, {
          month: "short"
        });
        let nextMonth = dateNext.toLocaleString(
          `${global.lang}-${global.country}`,
          { month: "short" }
        );

        if (startWeekDateMonth.month !== endWeekDateMonth.month) {
          return `${month} - ${nextMonth} ${year}`;
        } else {
          return `${month} ${year}`;
        }
      } else {
        return false;
      }
    },
    isGoPrev() {
      let calendarMonth = Date.parse(new Date(this.today));

      let dd = String(new Date().getDate()).padStart(2, "0");
      let mm = String(new Date().getMonth() + 1).padStart(2, "0");
      let yyyy = new Date().getFullYear();
      let currentMonth = Date.parse(new Date(yyyy + "-" + mm + "-" + dd));

      return currentMonth >= calendarMonth;
    },
    isGoNext() {
      let lastDate = Object.keys(this.eventsMap).map(key => key);
      lastDate = lastDate[lastDate.length - 1];
      let lastDateStemp = new Date(`${lastDate}`).getTime();
      let currentDateStemp = new Date(`${this.today}`).getTime();
      return currentDateStemp >= lastDateStemp;
    },
    isRtl() {
      return this.$vuetify.rtl;
    },
    isMobile() {
      return this.$vuetify.breakpoint.xsOnly;
    },
    isNeedPayment() {
      // debugger
      // let price = this.getProfile.user.tutorData.price + ''
      // price.slice(1, this.getProfile.user.tutorData.price.length);
      // return this.getNeedPayment && Number(price) > 0;
      return this.getNeedPayment;
    },
    isSelfTutor() {
      return this.$route.name == "myCalendar" ||
              (!!this.getProfile &&
                      !!this.accountUser &&
                      this.getProfile.user.id == this.accountUser.id);

    }
  },
  methods: {
    ...mapActions([
      "btnClicked",
      "insertEvent",
      "updateNeedPayment"
    ]),
    format(dateFormat) {
      let date = new Date(
        dateFormat.year,
        dateFormat.month - 1,
        dateFormat.day,
        0,
        0,
        0
      );
      //let options = { weekday: this.isMobile? 'narrow':'short'};
      let v = this.isMobile ? "calendarMobile" : "calendarDesktop";
      return this._i18n.d(date, v);
      //return  date.toLocaleDateString('en-us', options);
    },
    checkDateCell(past, date, time) {
      if (past) {
        return true;
      }
      //debugger;
      let dateEvent = this.eventsMap[date];
      if (dateEvent) {
        const e = dateEvent.find(x => x.time === time);
        if (e != null) {
          return true;
        }
      }
      return false;

      // dateEvent.find
      // console.log(this.calendarEvents);
      // let lastDate = Object.keys(this.eventsMap).map((key)=>key)
      // lastDate = lastDate[lastDate.length-1]
      // let lastDateStemp = new Date(`${lastDate}`).getTime()

      // if(new Date(date).getTime() >= lastDateStemp){
      //   return true;
      // }
      // return false;
    },
    insertNewEvent() {
      this.isLoading = true;

      let fromISO =  this.$moment(this.selectedDate+"T"+this.selectedTime).format();
      let paramObj = {
        date: this.selectedDate,
        time: this.selectedTime
      };
      this.insertEvent(fromISO).then(
        () => {
          this.isEventSent = true;
          this.calendarEvents.push(paramObj);
          this.isLoading = false;
        },
        () => {
          this.addEventDialog = false;
          this.isLoading = false;
          this.$store.commit('addComponent',componentConsts.BOOK_FAILED);
        }
      );
    },
    addEvent(ev, date, time) {
      if (this.isSelfTutor) return;

      ev.stopImmediatePropagation();
      if (this.addEventDialog) return;
      this.selectedTime = time;
      this.selectedDate = date;
      this.addEventDialog = true;
    },
    cellTime(date, time) {
      return this.eventsMap[date] &&
        this.eventsMap[date].find(e => e.time === time)
        ? ""
        : time;
    },
    formatDateString() {
      let year = this.selectedDate.slice(0, 4);
      let month = this.selectedDate.slice(5, 7);
      let day = this.selectedDate.slice(8, 11);
      if (this.isRtl) {
        let options = { weekday: "long", month: "short", day: "numeric" };
        let dateStr = new Date(year, month - 1, day, 0, 0, 0, 0)
          .toLocaleDateString(`${global.lang}-${global.country}`, options)
          .split(" ");
        // let dateStr = new Date(this.selectedDate).toLocaleDateString(`${global.lang}-${global.country}`, options).split(' ')
        let dayNumber = new Date(year, month - 1, day, 0, 0, 0, 0).getDate();
        // let dayNumber = new Date(this.selectedDate).getDate()
        return `${dateStr[0]} ${dateStr[1]} ${dayNumber} ${dateStr[3]}`;
      } else {
        let dateStr = new Date(year, month - 1, day, 0, 0, 0, 0)
          .toDateString()
          .split(" ");
        // let dateStr = new Date(this.selectedDate).toDateString().split(' ');
        let dayNumber = new Date(year, month - 1, day, 0, 0, 0, 0).getDate();
        // let dayNumber = new Date(this.selectedDate).getDate()
        return `${dateStr[0]}, ${dateStr[1]} ${dayNumber}`;
      }
    },
    formatTimeString() {
      let hour = +this.selectedTime.split(":")[0];
      let dateHour = new Date(this.selectedDate);
      dateHour.setHours(hour);
      let endTime = dateHour.getHours() + 1;
      let ampm = endTime < 12 ? "am" : "pm";
      if (endTime < 10) {
        endTime = `0${endTime}:00`;
      } else {
        endTime = `${endTime}:00`;
      }
      return `${this.selectedTime} - ${endTime} ${this.isRtl ? "" : ampm}`;
    },
    closeDialog() {
      this.selectedTime = "";
      this.selectedDate = "";
      this.addEventDialog = false;
      this.isEventSent = false;
      this.isLoading = false;
    },
    goPayment() {
      this.$store.commit('addComponent',componentConsts.PAYMENT_DIALOG)
    },
    isSelected(date, time) {
      if (date === this.selectedDate && time === this.selectedTime) {
        return true;
      }
    }
  },
  mounted() {
    this.$refs.calendar.scrollToTime("06:00");
  },
  watch: {
    getCalendarEvents: function() {
      this.isReady = true;
    },
    isMobile: function(val) {
      if (val) {
        this.intervals.height = 56;
      } else {
        this.intervals.height = 36;
      }
    }
  },
  created() {
    if (this.isMobile) {
      this.intervals.height = 56;
    } else {
      this.intervals.height = 36;
    }
    this.updateNeedPayment(this.accountUser.needPayment);
    this.$nextTick(() => {
      if (this.getCalendarEvents) {
        this.isReady = true;
      }
    });
  }
};
</script>

<style lang="less">
@import "../../styles/mixin.less";
.calendar-container {
  // width:620px;
  max-width: 100%;
  width: 620px;
  margin: 0 auto;
  overflow: auto;
  .sheet-loading {
    opacity: 0.2;
  }
  .calendar-header {
    padding: 0 8px;
    text-align: center;
    h2 {
      color: #4452fc;
      margin-bottom: 10px;
      font-size: 20px;
    }
    h3 {
      color: #43425d;
      margin-bottom: 26px;
      font-size: 16px;
      font-weight: 600;
    }
    .calendar-header-time {
      margin-bottom: 22px;
      border-top: 1px solid #ddd;
      border-bottom: 1px solid #ddd;
      display: flex;
      justify-content: center;
      align-items: center;
      padding: 6px 0;
      div {
        margin-left: 8px;
      }
    }
  }
  .navigation-btns-calendar {
    display: flex;
    justify-content: space-between;
    align-items: baseline;
    margin-bottom: 40px;
    .title-calendar {
      font-size: 20px;
      color: @global-purple;

      @media (max-width: @screen-xs) {
        font-size: 16px;
      }
    }
    .v-btn {
      transform: none /*rtl:rotate(180deg)*/;
      min-width: auto !important;
      width: 40px;
      height: 30px;
      border-radius: 3px;
      @media (max-width: @screen-xs) {
        width: 34px;
        height: 28px;
        margin: 0;
      }
      .v-btn__content {
        .v-icon {
          font-size: 18px !important;
        }
      }
    }
  }

  .v-sheet {
    position: relative;
    @media (max-width: @screen-xs) {
      position: initial;
    }

    .addEventDialog {
      position: absolute;
      max-width: 334px;
      width: 100%;
      height: 262px;
      border-radius: 6px;
      box-shadow: 1px 6px 19px 0 rgba(0, 0, 0, 0.7);
      background-color: #ffffff;
      z-index: 10;
      top: 26%;
      left: 26%;

      display: flex;
      flex-direction: column;
      justify-content: space-evenly;

      @media (max-width: @screen-xs) {
        position: fixed;
        height: initial;
        margin: 0 auto;
        left: 0;
        right: 0;
        width: 88%;
        top: 34%;
      }
      .event-dialog-title {
        text-align: center;
        // padding: 10px;
        padding: 10px 20px;
        font-weight: 600;
        &.event-dialog-title-send {
          font-size: 18px;
          color: @global-purple;
        }

        &.event-dialog-title-done {
          color: @global-blue;
          font-size: 25px;
        }
      }
      .event-dialog-body {
        text-align: center;
        font-weight: 600;
        p {
          margin: 0;
        }
        &.event-dialog-body-payment {
          padding: 18px 0;
        }
        &.event-dialog-body-send {
          color: @global-blue;
          font-size: 20px;
          line-height: 1.6;
          padding: 28px 0 36px;
        }
        &.event-dialog-body-done {
          color: @global-purple;
          font-size: 18px;
          padding: 0 14px 72px 14px;
        }
      }
      .event-dialog-action {
        text-align: center;
        @media (max-width: @screen-xs) {
          margin-bottom: 10px;
        }

        .v-btn {
          min-width: 140px;
          height: 40px !important;
          padding: 0 32px !important;
          margin: 6px 8px;
        }
        .cncl-btn {
          color: @global-blue;
          border: 1px solid @global-blue !important;
        }
        .calbtn {
          color: white !important;
          text-transform: capitalize !important;
          font-size: 16px;
          font-weight: 600;
          letter-spacing: -0.3px;
        }
        .donebtn {
          color: white !important;
          text-transform: capitalize !important;
          font-size: 16px;
          font-weight: 600;
          letter-spacing: -0.3px;
          padding: 0 50px !important;
        }
      }
    }
  }
  .theme--light {
    .v-calendar-daily {
      border: none !important;
    }
  }

  .v-calendar {
    .v-calendar-daily__head {
      pointer-events: none;

      .v-calendar-daily__intervals-head {
        display: none;
      }
      .v-calendar-daily_head-day {
        border: none;
        border-bottom: 6px solid #dddddd;
        margin: 0 5px;
        @media (max-width: @screen-xs) {
          border-bottom: 4px solid #dddddd;
          margin: 0 2px;
        }
        .v-calendar-daily_head-weekday {
          font-size: 12px;
          font-weight: bold;
          padding: 0;
          @media (max-width: @screen-xs) {
            font-size: 12px;
            text-align: center;
          }
        }
        .v-calendar-daily_head-day-label {
          button {
            font-size: 24px;
            line-height: inherit;
            // padding: 0 0 6px 0;
            @media (max-width: @screen-xs) {
              font-size: 18px;
              text-align: center;
              line-height: 1.6;
              padding: 0;
            }
          }
        }
        &.v-present {
          border-bottom: 6px solid @global-blue;
          @media (max-width: @screen-xs) {
            border-bottom: 4px solid @global-blue;
          }
          .v-calendar-daily_head-weekday {
            color: @global-blue;
          }
          .v-calendar-daily_head-day-label {
            button {
              color: @global-blue;
            }
          }
        }
      }
    }
    .v-calendar-daily__intervals-body {
      display: none;
    }
    .v-calendar-daily__body {
      .v-calendar-daily__day-container {
        .v-calendar-daily__day {
          border: none;
          .v-calendar-daily__day-interval {
            border: none;
          }
        }
      }
      .v-calendar-daily__scroll-area {
        overflow-y: auto;
        .v-calendar-daily__pane {
          @media (max-width: @screen-xs) {
            height: 100% !important;
          }
        }
      }
    }
    .cursor-none {
      pointer-events: none;
    }
  }

  .selectedEvent {
    background-color: @global-blue;
    color: white !important;
  }
  .my-event {
    text-align: center;
    font-size: 12px;
    padding: 5px 5px 0;
    margin-right: 8px;
    position: relative;

    &.with-time {
      position: absolute;
      width: 100%;
      height: 40px;
      border: 2px solid #fff;
      font-size: 16px;
      font-weight: bold;

      text-align: center;
      color: #dddddd;
      @media (max-width: @screen-xs) {
        font-size: 12px;
        padding-top: 18px;
      }
      button {
        outline: none;
      }
    }
    &.without-time {
      position: absolute;
      width: 100%;
      height: 40px;
      border: 2px solid #fff;
      font-size: 16px;
      font-weight: bold;

      text-align: center;
      color: #5158af;
      @media (max-width: @screen-xs) {
        font-size: 12px;
        padding-top: 18px;
      }
      button {
        outline: none;
      }
    }

    &.without-time-past {
      pointer-events: none !important;
      position: absolute;
      width: 100%;
      height: 40px;
      border: 2px solid #fff;
      font-size: 16px;
      font-weight: bold;

      text-align: center;
      color: #dddddd;
      @media (max-width: @screen-xs) {
        font-size: 12px;
        padding-top: 18px;
      }
      button {
        outline: none;
      }
    }
  }
}
</style>
