<template>
   <v-card class="classCard" color="white" width="360px" max-width="360px" min-width="320px" flat>
      <div :style="{'background':selectedClass.color}" class="cardHeader ps-6 pe-7 py-3">
         <v-icon size="14" @click="$emit('closeClassCard')" color="white" class="closeIcon" v-text="'sbf-close'"/>
         <div class="classCardTitle ">{{selectedClass.courseName}}</div>
      </div>
      <div class="cardWrapper px-6 py-3">
         <div class="classTitle pb-1">{{$t('live_title')}}</div>
         <div class="className">{{selectedClass.name}}</div>
         <div class="classInfoWrapper pt-5">
            <div class="classInfo d-flex align-center pb-5">
               <v-icon class="pe-4" size="18" color="#69687d" v-text="'sbf-classCalendar'"/>
               <div class="infoText">{{$moment(selectedClass.date).format('ddd, DD MMM')}}</div>
            </div>
            <div class="classInfo d-flex align-center pb-5">
               <v-icon class="pe-4" size="16" color="#69687d" v-text="'sbf-clock'"/>
               <div class="infoText">{{$moment(selectedClass.date).format('HH:mm')}}</div>
            </div>
            <div class="classInfo d-flex align-center pb-5">
               <v-icon class="pe-4" size="13" color="#69687d" v-text="'sbf-groupPersons'"/>
               <div class="infoText">{{$tc('students_count',selectedClass.studentsCount)}}</div>
            </div>
            <div class="classInfo d-flex align-baseline pb-2">
               <v-icon class="pe-4" size="10" color="#69687d" v-text="'sbf-link'"/>
               <div class="infoText">
                  <div class="linkTitle">{{$t('link_room')}}</div>
                  <a target="_blank" class="pt-1 linkUrl" :href="urlLink">{{urlLink}}</a>
               </div>
            </div>
         </div>

      </div>
   </v-card>
</template>

<script>
import * as routeNames from '../../../../routes/routeNames.js';

export default {
   name:'classCard',
   props:{
      selectedClass:{
         type: Object,
         required:true
      }
   },
   computed: {
      urlLink(){
         let path = this.$router.resolve({
            name: routeNames.StudyRoom,
            params:{
               id: this.selectedClass.id
            }
         })
         return `${global.location.origin}${path.href}`
      }
   },
}
</script>

<style lang="less">
@import "../../../../styles/mixin.less";

   .classCard{
      .cardHeader{
         border-top-left-radius: 6px;
         border-top-right-radius: 6px;
         position: relative;
         min-height: 50px;
         display: flex;
         align-items: center;
         .closeIcon{
            position: absolute;
            right: 10px;
            top: 10px;
         }
        .classCardTitle{
            color: #ffffff;
            font-size: 16px;
            font-weight: 600;
            .giveMeEllipsis(2,20);
         }   
      }
      .cardWrapper{
         font-size: 14px;

         .classTitle{
            color: #4c59ff;
            font-weight: bold;
         }
         .className{
            font-weight: 600;
            color: #43425d;
            line-height: 1.57;
         }
         .classInfoWrapper{
            .classInfo{
               .infoText{
                  overflow: auto;
                  color: #43425d;
                  font-size: 14px;
                  .linkTitle{
                     font-weight: 600;
                  }
                  .linkUrl{
                     font-size: 14px;
                     line-height: 1.5;
                     color: #3874e8;
                     text-decoration: underline;
                  }
               }
            }
         }
      }

   }
</style>