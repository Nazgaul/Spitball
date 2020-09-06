<template>
   <div class="myCourses">
      <v-skeleton-loader v-if="skeleton" max-width="1366" type="table" />
      <v-data-table
         v-else
         :headers="headers"
         :items="$store.getters.getCoursesItems"
         :items-per-page="5"
         @pagination="handlePaginationIndexPosition"
         :mobile-breakpoint="0"
         class="myCourses_table"
      >
         <template v-slot:top>
            <div class="tableTop">
               <div class="myStudyRooms_title pb-3 pb-sm-0" v-t="'my_courses'"></div>
               <div class="text-end">
                  <v-btn
                     :to="{name: createCourseRoute}"
                     class="white--text"
                     depressed
                     width="180"
                     rounded
                     :block="$vuetify.breakpoint.xsOnly"
                     color="#5360FC"
                  >
                     <v-icon size="22" left>sbf-plus-circle</v-icon>
                     <span v-t="'dashboardPage_my_content_upload'"></span>
                  </v-btn>
               </div>
            </div>
         </template>

         <template v-slot:body="props">
            <draggable
               :list="props.items"
               :move="checkMove"
               :disabled="$vuetify.breakpoint.xsOnly"
               @start="dragging = true"
               @end="handleEndMove"
               tag="tbody"
            >
            
               <tr v-for="(item, index) in props.items" :key="item.id" @click.stop="handleRowClick(item)">
                  <td class="text-start">
                     <div class="tablePreview d-flex align-center">
                        <div class="tableIndex me-2">{{(page - 1) * itemsPerPage + index + 1}}.</div>
                        <v-skeleton-loader class="my-2" v-if="!isLoaded" height="80" width="127" type="image"></v-skeleton-loader>
                        <img v-show="isLoaded" @load="loaded" :src="$proccessImageUrl(item.image, {width:127, height:80})" class="tablePreview_img" width="127" height="80" />
                     </div>
                  </td>
                  <td class="text-start">
                     <div style="max-width: 200px">{{item.name}}</div>
                  </td>
                  <td class="text-start">
                     <div v-if="item.startOn">{{$moment(item.startOn).format('MMM D, YYYY')}}</div>
                  </td>
                  <td class="text-start">{{item.lessons}}</td>
                  <td class="text-start">{{item.documents}}</td>
                  <td class="text-start">
                     <v-tooltip top transition="fade-transition">
                        <template v-slot:activator="{on}">
                           <div class="d-flex" v-on="item.userNames && item.userNames.length ? on : null">
                              <v-icon size="14">sbf-groupPersons</v-icon>
                              <div class="ms-2">{{item.users}}</div>
                           </div>
                        </template>
                        <div v-for="(user, index) in item.userNames" :key="index">{{user}}</div>          
                     </v-tooltip>
                  </td>
                  <td class="text-start">
                     {{$price(item.price.amount, item.price.currency, true)}}
                  </td>
                  <td class="text-start">
                     {{item.isPublish ? $t('visible') : $t('notVisible')}}
                  </td>
                  <td class="text-start">
                     <v-tooltip top transition="fade-transition">
                        <template v-slot:activator="{on}">
                           <v-btn icon text v-on="on" @click.stop="goEdit(item)">
                              <v-icon size="18" color="#43425d">sbf-edit-icon</v-icon>
                           </v-btn>
                        </template>
                        <div v-t="'go_edit'"></div>          
                     </v-tooltip>
   
                     <!-- <v-menu offset-y>
                        <template v-slot:activator="{ on }">
                           <v-btn icon>
                              <v-icon size="18" v-on="on">sbf-3-dot</v-icon>
                           </v-btn>
                        </template>
                        <v-list>
                           <v-list-item :to="{name: uppdateCourseRoute, params: { id: item.id }}">
                              <v-list-item-title v-t="'go_edit'"></v-list-item-title>
                           </v-list-item>
                        </v-list>
                     </v-menu> -->
                  </td>
               </tr>
            </draggable>
         </template>
      </v-data-table>
      <v-snackbar
         v-model="showCourseNotVisible"
         :timeout="6000"
         color="error"
         top
      >
         <div class="white--text text-center">{{errorText}}</div>
      </v-snackbar>
   </div>
</template>

<script>
import { CourseCreate, CourseUpdate, CoursePage } from '../../../../routes/routeNames'

import draggable from 'vuedraggable'
export default {
   name:'myCourses',
   components: {
      draggable,
   },
   data() {
      return {
         errorText: '',
         showCourseNotVisible: false,
         skeleton: true,
         itemsPerPage: 5,
         page: 1,
         oldIndex: 0,
         newIndex: 0,
         isLoaded: false,
         uppdateCourseRoute: CourseUpdate,
         createCourseRoute: CourseCreate,
         headers: [
            { text: '', align:'', sortable: false, value:'preview' },
            { text: this.$t('dashboardPage_course_name'), align:'', sortable: false, value:'name' },
            { text: this.$t('dashboardPage_startOn'), align:'', sortable: false, value:'startOn' },
            { text: this.$t('dashboardPage_lecture'), align:'', sortable: false, value:'lessons' },
            { text: this.$t('dashboardPage_resource'), align:'', sortable: false, value:'documents' },
            { text: this.$t('dashboardPage_enroll'), align:'', sortable: false, value:'users' },
            { text:this.$t('dashboardPage_price'), align:'', sortable: false, value:'price.amount' },
            { text: this.$t('dashboardPage_status'), align:'', sortable: false, value:'isPublish' },
            { text: '', align:'', sortable: false, value:'action' }
         ]
      }
   },
   methods: {
      goEdit(item) {
         this.$router.push({name: this.uppdateCourseRoute, params: { id: item.id }})
      },
      handlePaginationIndexPosition(tablePage) {
         this.page = tablePage.page
         this.itemsPerPage = tablePage.itemsPerPage
      },
      checkMove(e) {
         this.oldIndex = e.draggedContext.index
         this.newIndex = e.draggedContext.futureIndex
      },
      handleEndMove() {
         if(this.oldIndex !== this.newIndex) {
            if(this.page > 1) {
               this.oldIndex = (this.page - 1) * this.itemsPerPage + this.oldIndex // this.oldIndex + this.itemsPerPage
               this.newIndex = (this.page - 1) * this.itemsPerPage + this.newIndex // this.newIndex + this.itemsPerPage
            }
            let self = this
            this.$store.dispatch('updateCoursePosition', {
               oldIndex: this.oldIndex,
               newIndex: this.newIndex
            }).finally(() => {
               self.oldIndex = 0
               self.newIndex = 0
            })
         }
         this.dragging = false
      },
      handleRowClick(item) {
         if(!item.isPublish) {
            this.errorText = this.$t('course_not_visible')
            this.showCourseNotVisible = true
            return
         }
         this.$router.push({name: CoursePage, params: { id: item.id }})
      },
      loaded() {
         this.isLoaded = true;
      },
   },
   beforeDestroy() {
      this.$store.commit('resetCourseItems')
   },
   created() {
      this.$store.dispatch('updateCoursesItems').then(() => {
         this.skeleton = false
      })
   }
}
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
@import "../../../../styles/colors.less";
.myCourses {
   margin: 30px;
   max-width: 1080px;
   box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.15);
   @media (max-width: @screen-xs) {
      margin: 8px 0;
      box-shadow: none;
   }
   .myCourses_table {
      .tableTop {
         padding: 20px;
         color: @global-purple;
         .myStudyRooms_title {
            font-size: 22px;
            font-weight: 600;
         }
      }
      thead {
         tr {
            th {
               color: @global-purple !important;
               font-weight: normal;
            }
         }
      }
      tbody {
         tr {
            &.sortable-chosen {
               background: #E2E8EE !important;
            }
            td {
               border-bottom: none !important;
               cursor: pointer;
            }
            .tablePreview {
               .tableIndex {
                  color: @global-purple;
                  font-weight: 600;
               }
               .tablePreview_img {
                  margin: 10px 0;
                  border: 1px solid #d8d8d8;
               }
            }
         }
         tr:nth-child(even) {
            background-color: #f5f5f5;
         }
      }
      .v-data-footer, .sbf-arrow-right-carousel, .sbf-arrow-left-carousel {
         color: @global-purple;
         font-size: 14px;
      }
   }
}
</style>